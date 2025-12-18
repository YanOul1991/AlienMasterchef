using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;

[RequireComponent(typeof(NetworkObject), typeof(NetworkTransform))]
public class Knife : NetworkBehaviour
{
  private Vector3 initialPosition;
  private Quaternion initialRotation;

  private void Awake()
  {
    initialPosition = transform.position;
    initialRotation = transform.rotation;

    // NetworkServer.OnGameStarted += () => 
    // {
    //   gameObject.SetActive(true);
    //   GetComponent<NetworkObject>().Spawn();
    // };

    // gameObject.SetActive(false);
  } 

  public override void OnNetworkSpawn()
  {
    base.OnNetworkSpawn();
    Debug.Log("Knife has spawned");

    if(IsServer)
    {
      gameObject.AddComponent<Rigidbody>();
      GetComponent<Rigidbody>().collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
    }
  }

  private void FixedUpdate()
  {
    if (transform.position.y < -15.0f)
      transform.SetPositionAndRotation(initialPosition, initialRotation);
  }
}