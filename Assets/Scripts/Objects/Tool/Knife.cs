using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;

[RequireComponent(
  typeof(NetworkObject), 
  typeof(NetworkTransform))
]
[RequireComponent(typeof(AudioSource))]
public class Knife : NetworkBehaviour
{
  public AudioClip m_creatureDeathSound;

  private Vector3 initialPosition;
  private Quaternion initialRotation;


  private void Awake()
  {
    initialPosition = transform.position;
    initialRotation = transform.rotation;
  } 
  
  public override void OnNetworkSpawn()
  {
    base.OnNetworkSpawn();

    if(IsServer)
    {
      if (TryGetComponent(out Rigidbody _rb))
      {
        _rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
      }
      else
      {
        _rb = gameObject.AddComponent<Rigidbody>();
        _rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
      }
    }
  }

  private void FixedUpdate()
  {
    if (transform.position.y < -15.0f)
      transform.SetPositionAndRotation(initialPosition, initialRotation);
  }

  void OnCollisionEnter(Collision collision)
  {
    if (collision.gameObject.TryGetComponent(out CreatureFish creatureFish))
      PlayDeathSoundRpc();
      
    if (collision.gameObject.TryGetComponent(out CreatureMosquito creatureMosquito))
      PlayDeathSoundRpc();
  }

  void OnTriggerEnter(Collider collision)
  {
    if (collision.gameObject.TryGetComponent(out CreatureFish creatureFish))
      PlayDeathSoundRpc();
      
    if (collision.gameObject.TryGetComponent(out CreatureMosquito creatureMosquito))
      PlayDeathSoundRpc();
  }

  [Rpc(SendTo.Everyone)]
  private void PlayDeathSoundRpc()
  {
    GetComponent<AudioSource>().PlayOneShot(m_creatureDeathSound);
  }
}