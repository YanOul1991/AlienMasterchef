using Unity.Netcode;
using UnityEngine;
using System.Linq;
using System;

public class Trash : MonoBehaviour
{
  void OnCollisionEnter(Collision collision)
  {
    if (!NetworkServer.IsGameRunning || !NetworkServer.Singleton.IsServer)
      return;

    // >>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
    if (collision.gameObject.TryGetComponent(out Plate plate))
    {
      Debug.Log($"[---- Trash ----] COLLISION ENTER | Collision of object {plate}");
      NetworkObject[] networkObjects = collision.gameObject.GetComponentsInChildren<NetworkObject>(true);
      foreach (var obj in networkObjects)
      {
        // NetworkServer.Singleton.DeactivateGameObjectInNetwork(obj.gameObject);
        NetworkServer.Singleton.DespawnGameObjectInNetwork(obj.gameObject);
      }
      return;
    }
    // >>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
    if (collision.gameObject.TryGetComponent(out Tool tool))
    {
      Debug.Log($"[---- Trash ----] COLLISION ENTER | Collision of object {tool}");
      NetworkObject[] networkObjects = collision.gameObject.GetComponentsInChildren<NetworkObject>(true);
      foreach (var obj in networkObjects)
      {
        // NetworkServer.Singleton.DeactivateGameObjectInNetwork(obj.gameObject);
        NetworkServer.Singleton.DespawnGameObjectInNetwork(obj.gameObject);
      }
      return;
    }
    // >>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
    if (collision.gameObject.TryGetComponent(out Food food))
    {
      Debug.Log($"[---- Trash ----] COLLISION ENTER | Collision of object {food}");
      NetworkObject[] networkObjects = collision.gameObject.GetComponentsInChildren<NetworkObject>(true);
      foreach (var obj in networkObjects)
      {
        // NetworkServer.Singleton.DeactivateGameObjectInNetwork(obj.gameObject);
        NetworkServer.Singleton.DespawnGameObjectInNetwork(obj.gameObject);
      }
      return;
    }
  }

  void OnTriggerEnter(Collider collision)
  {
    if (!NetworkServer.IsGameRunning || !NetworkServer.Singleton.IsServer)
      return;

    // >>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
    if (collision.gameObject.TryGetComponent(out Plate plate))
    {
      Debug.Log($"[---- Trash ----] TRIGGER ENTER | Collision of object {plate}");
      NetworkObject[] networkObjects = collision.gameObject.GetComponentsInChildren<NetworkObject>(true);
      foreach (var obj in networkObjects)
      {
        // NetworkServer.Singleton.DeactivateGameObjectInNetwork(obj.gameObject);
        NetworkServer.Singleton.DespawnGameObjectInNetwork(obj.gameObject);
      }
      return;
    }
    // >>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
    if (collision.gameObject.TryGetComponent(out Tool tool))
    {
      Debug.Log($"[---- Trash ----] TRIGGER ENTER | Collision of object {tool}");
      NetworkObject[] networkObjects = collision.gameObject.GetComponentsInChildren<NetworkObject>(true);
      foreach (var obj in networkObjects)
      {
        // NetworkServer.Singleton.DeactivateGameObjectInNetwork(obj.gameObject);
        NetworkServer.Singleton.DespawnGameObjectInNetwork(obj.gameObject);
      }
      return;
    }
    // >>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
    if (collision.gameObject.TryGetComponent(out Food food))
    {
      Debug.Log($"[---- Trash ----] TRIGGER ENTER | Collision of object {food}");
      NetworkObject[] networkObjects = collision.gameObject.GetComponentsInChildren<NetworkObject>(true);
      foreach (var obj in networkObjects)
      {
        // NetworkServer.Singleton.DeactivateGameObjectInNetwork(obj.gameObject);
        NetworkServer.Singleton.DespawnGameObjectInNetwork(obj.gameObject);
      }
      return;
    }
  }
}
