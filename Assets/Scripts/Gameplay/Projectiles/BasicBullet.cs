using UnityEngine;
using Unity.Netcode;
using Monke.Gameplay.Character;
using Monke.Gameplay.Interfaces;
using System.Collections.Generic; 
using System.Collections;

namespace Monke.Projectiles
{
    public class BasicBullet : NetworkBehaviour
    {
        [SerializeField] float m_BulletSpeed;
        [SerializeField] float m_BulletForce;
        [SerializeField] int m_BulletDamage;
        [SerializeField] float m_BulletSize;
        [SerializeField] CircleCollider2D m_OurCollider;
        [SerializeField] Vector3 m_bulletGravity;
        [SerializeField] Vector3 m_initialVelocity;
        [SerializeField] GameObject m_OnHitParticlePrefab;
        [SerializeField]ParticleSystem m_ParticleSystem;
        [SerializeField] float k_GravityStrength = .05f;
        [SerializeField] TrailRenderer m_TrailRenderer;
        /// <summary>
        /// The character that created us. Can be 0 to signal that we were created generically by the server.
        /// </summary>
        ulong m_SpawnerId;

        const int k_MaxCollisions = 1;
        Collider2D[] m_CollisionCache = new Collider2D[k_MaxCollisions];
        bool m_IsInitialized;
        bool m_IsDead;
        bool m_isColliding;
        int m_CollisionMask;  //mask containing everything we test for while moving
        int m_BlockerMask;    //physics mask for things that block the arrow's flight.
        public float TimeStarted { get; set; }
        public float TimeRunning { get { return (Time.time - TimeStarted); } }
        
        List<GameObject> m_HitTargets = new List<GameObject>();

        public void Initialize(ulong spawnerId, float speed, float force, int damage, float size, Vector3 direction){
            m_SpawnerId = spawnerId;
            m_BulletSpeed = speed;
            m_BulletForce = force;
            m_BulletDamage = damage;
            m_BulletSize = size;
            m_initialVelocity = direction;
            m_bulletGravity = Vector3.zero;
            TimeStarted = Time.time;
        }

        override public void OnNetworkSpawn()
        {
            if(IsServer){
                m_IsInitialized = true;
                m_IsDead = false;
            }
                m_TrailRenderer = GetComponent<TrailRenderer>();    
                m_CollisionMask = LayerMask.GetMask(new[] { "Default", "Environment", "Platform", "Player" });
                m_BlockerMask = LayerMask.GetMask(new[] { "Default", "Environment", "Platform"});
                
            } 

        void Update(){
        }

        void FixedUpdate(){
            if(IsServer){
                if(!m_IsDead){
                    
                    
                    m_bulletGravity += new Vector3(0,-k_GravityStrength,0);
                    NetworkObject.transform.position += m_initialVelocity * m_BulletSpeed + m_bulletGravity;
                    DetectCollisions();
                }
            }
        }
        IEnumerator DespawnCoro(){
            
            yield return new WaitForSeconds(.5f);   
            this.NetworkObject.Despawn();
        } 
        void DetectCollisions()
        {

            var position = transform.localToWorldMatrix.MultiplyPoint(m_OurCollider.offset);
            var numCollisions = Physics2D.OverlapCircleNonAlloc(position, m_OurCollider.radius, m_CollisionCache, m_CollisionMask);
            for (int i = 0; i < numCollisions; i++)
            {
                int layerTest = 1 << m_CollisionCache[i].gameObject.layer;
                if ((layerTest & m_BlockerMask) != 0)
                {
                    //hit a wall
                    
                    m_BulletSpeed = 0f;
                    m_IsDead = true;
                    PlayCollisionParticlesClientRPC();
                    Debug.Log("Despawned bulled: blocked.");
                    StartCoroutine(DespawnCoro());
                    return;
                }

                if (m_CollisionCache[i].gameObject.layer == m_CollisionMask && !m_HitTargets.Contains(m_CollisionCache[i].gameObject))
                {
                    // check if colliding with player immediately after firing. if so, ignore.
                    if(!m_isColliding){
                        NetworkObject no;
                        if(m_CollisionCache[i].gameObject.TryGetComponent<NetworkObject>(out no)){
                            if(no.OwnerClientId == m_SpawnerId){
                                return;
                            }else if(TimeRunning <.3f){
                                return;
                            }
                            else if(i+1 == numCollisions){
                                m_isColliding = true;
                            }
                        }
                    }
                    
                    m_HitTargets.Add(m_CollisionCache[i].gameObject);

                    if (m_HitTargets.Count >= k_MaxCollisions)
                    {
                        // we've hit all the enemies we're allowed to! So we're done
                        m_IsDead = true;
                        PlayCollisionParticlesClientRPC();
                        StartCoroutine(DespawnCoro());
                    }

                    //all NPC layer entities should have one of these. 
                    var targetNetObj = m_CollisionCache[i].GetComponentInParent<NetworkObject>();
                    if (targetNetObj)
                    {
                        RecvHitEnemyClientRPC(targetNetObj.NetworkObjectId);

                        //retrieve the person that created us, if he's still around.
                        NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(m_SpawnerId, out var spawnerNet);
                        var spawnerObj = spawnerNet != null ? spawnerNet.GetComponent<ServerCharacter>() : null;
                        Debug.Log("Enemy Hit");
                        if (m_CollisionCache[i].TryGetComponent(out IDamageable damageable))
                        {
                            damageable.ReceiveHP(spawnerObj, -m_BulletDamage);
                        }
                    }

                    if (m_IsDead)
                    {
                        return; // don't keep examining collisions since we can't damage anybody else
                    }
                }
            }
        }
        [ClientRpc]
        private void PlayCollisionParticlesClientRPC(){
            this.GetComponent<MeshRenderer>().enabled = false;
            m_ParticleSystem.Play();
        }

        [ClientRpc]
        private void RecvHitEnemyClientRPC(ulong enemyId)
        {

            NetworkObject targetNetObject;
            if (NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(enemyId, out targetNetObject))
            {
                if (m_OnHitParticlePrefab)
                {
                    // show an impact graphic
                    Instantiate(m_OnHitParticlePrefab.gameObject, transform.position, transform.rotation);
                }
            }
        }
    }
}