using System;
using Unity.Netcode;
using UnityEngine;

public class SpawnerManager : MonoBehaviour
{
  public GameObject m_spawningObject;
  public Transform m_spawnPosition;

  void OnCollisionEnter(Collision collision)
  {
    if (!NetworkServer.IsGameRunning) return;
    if (!NetworkServer.Singleton.IsServer) return;

    if (CompareTag(collision.gameObject.tag))
    {
      if (m_spawningObject.TryGetComponent(out NetworkObject network))
      {
        GameObject _instance = Instantiate(m_spawningObject, m_spawnPosition.position, Quaternion.identity);
        NetworkServer.Singleton.SpawnGameObjectInNetwork(_instance);
      }
    }

  }

  void OnTriggerEnter(Collider collision)
  {
    if (!NetworkServer.IsGameRunning) return;
    if (!NetworkServer.Singleton.IsServer) return;

    if (CompareTag(collision.gameObject.tag))
    {
      if (m_spawningObject.TryGetComponent(out NetworkObject network))
      {
        GameObject _instance = Instantiate(m_spawningObject, m_spawnPosition.position, Quaternion.identity);
        NetworkServer.Singleton.SpawnGameObjectInNetwork(_instance);
      }
    }
  }
}
