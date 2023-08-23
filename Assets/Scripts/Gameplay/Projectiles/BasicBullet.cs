using UnityEngine;
using Unity.Netcode;
using Monke.Gameplay.Character;
using Monke.Gameplay.Interfaces;
using System.Collections.Generic;
using System.Collections;
using Monke.Gameplay.ClientPlayer;

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
        [SerializeField] ParticleSystem m_ParticleSystem;
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
        int m_BlockerMask;    //physics mask for things that block the bullet's flight.
        int m_DamagableLayer; //Layer containing anything that can take damage
        Vector3 m_PreviousFramePos;
        Vector3 m_Velocity; //movement since last frame.
        public float TimeStarted { get; set; }
        public float TimeRunning { get { return (Time.time - TimeStarted); } }

        List<GameObject> m_HitTargets = new List<GameObject>();

        public void Initialize(ulong spawnerId, float speed, float force, int damage, float size, Vector3 direction)
        {
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
            if (IsServer)
            {
                m_IsInitialized = true;
                m_IsDead = false;
            }
            this.GetComponent<MeshRenderer>().enabled = true;
            m_TrailRenderer = GetComponent<TrailRenderer>();
            m_CollisionMask = LayerMask.GetMask(new[] { "Player","Platform" });
            m_BlockerMask = LayerMask.GetMask(new[] {"Platform" });
            m_DamagableLayer = LayerMask.NameToLayer("Player");
            m_isColliding = false;
            m_HitTargets.Clear();
            m_Velocity = Vector3.zero;
            m_PreviousFramePos = this.transform.position;

        }

        void Update()
        {
        }

        void FixedUpdate()
        {
            if (IsServer)
            {
                if (!m_IsDead)
                {
                    m_PreviousFramePos = transform.position;
                    m_bulletGravity += new Vector3(0, -k_GravityStrength, 0);
                    NetworkObject.transform.position += m_initialVelocity * m_BulletSpeed + m_bulletGravity;
                    m_Velocity = transform.position - m_PreviousFramePos;
                    DetectCollisions();
                }
            }
        }
        IEnumerator DespawnCoro()
        {

            yield return new WaitForSeconds(.5f);
            this.NetworkObject.Despawn();
        }
        void DetectCollisions()
        {
            var position = transform.localToWorldMatrix.MultiplyPoint(m_OurCollider.offset);
            var numCollisions = Physics2D.OverlapCircleNonAlloc(position, m_OurCollider.radius, m_CollisionCache, m_CollisionMask);
            if(numCollisions == 0){
                m_isColliding = true;
            }
            for (int i = 0; i < numCollisions; i++)
            {
                // unless bullet has exited
                if (!m_isColliding)
                {
                    NetworkObject no;
                    if (m_CollisionCache[i].TryGetComponent<NetworkObject>(out no))
                    {
                        if (no.OwnerClientId == m_SpawnerId)
                        {
                            return;
                        }
                    }
                    m_isColliding = true;
                    
                }
                int layerTest = 1 << m_CollisionCache[i].gameObject.layer;
                if ((layerTest & m_BlockerMask) != 0)
                {
                    //hit a wall / player

                    m_BulletSpeed = 0f;
                    m_IsDead = true;
                    PlayCollisionParticlesClientRPC();
                    StartCoroutine(DespawnCoro());
                }
               if (m_CollisionCache[i].gameObject.layer == m_DamagableLayer && !m_HitTargets.Contains(m_CollisionCache[i].gameObject))
                {
                    m_HitTargets.Add(m_CollisionCache[i].gameObject);
                    if (m_HitTargets.Count >= k_MaxCollisions)
                    {
                        // we've hit all the enemies we're allowed to! So we're done
                        m_IsDead = true;
                        PlayCollisionParticlesClientRPC();
                        StartCoroutine(DespawnCoro());
                    }

                    //all Player layer entities should have one of these. 
                    var targetNetObj = m_CollisionCache[i].GetComponent<NetworkObject>();
                    if (targetNetObj)
                    {
                        RecvHitEnemyClientRPC(targetNetObj.NetworkObjectId,  m_Velocity.normalized * m_BulletForce);

                        //retrieve the person that created us, if he's still around.
                        NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(m_SpawnerId, out var spawnerNet);
                        var spawnerObj = spawnerNet != null ? spawnerNet.GetComponent<ServerCharacter>() : null;
                        if (m_CollisionCache[i].TryGetComponent(out IDamageable damageable))
                        {
                            Debug.Log("damage:" + m_BulletDamage);
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
        private void PlayCollisionParticlesClientRPC()
        {
            this.GetComponent<MeshRenderer>().enabled = false;
            m_ParticleSystem.Play();
        }

        [ClientRpc]
        private void RecvHitEnemyClientRPC(ulong enemyId, Vector3 force)
        {

            NetworkObject targetNetObject;
            if (NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(enemyId, out targetNetObject))
            {
                PlayerController enemyController = targetNetObject.GetComponent<PlayerController>();
                enemyController.AddForce(force);
            }
        }
    }
}