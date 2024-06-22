using UnityEngine;
using Monke.Gameplay.Character;
using Unity.Netcode;
public class CharacterSpawner : NetworkBehaviour
{
    public GameObject m_ClientCharacterPrefab;

    public void SpawnClientCharacter(ServerCharacter character, ulong clientId)
    {
        if(character.m_ClientCharacter != null) return;

        GameObject clientCharacter = Instantiate(m_ClientCharacterPrefab, character.transform);
        clientCharacter.GetComponent<NetworkObject>().IncludeTransformWhenSpawning(clientId);
        clientCharacter.GetComponent<NetworkObject>().Spawn();

        
        character.InitializeClientCharacter(clientCharacter.GetComponent<ClientCharacter>());
    }
    public void DespawnClientCharacter(ServerCharacter character)
    {
        if (character.m_ClientCharacter == null) return;
        character.CleanUpClientCharacter();
        Destroy(character.m_ClientCharacter.gameObject);
    }
}