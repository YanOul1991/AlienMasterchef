/*;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
 
    +++ CreatureButterfly.cs
        Script des creatures de type Butterfly.
  
    +++ Par: 
        Yanis Oulmane

;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;*/

using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public sealed class CreatureMosquito : Creature<CreatureMosquito>
{
  private static CreatureMosquitoFlyZone flyZoneData;
  private Vector3 m_targetDestination;
  private Vector3 m_velocity;
  private AudioClip deathSound;

  /* ----------------------------------
    --- Overrides
  ---------------------------------- */
  [field: SerializeField] public override float BaseMovementSpeed { get; protected set; }
  [field: SerializeField] public override float PanicMovementSpeed { get; protected set; }
  [field: SerializeField] public override GameObject ResultingFood { get; protected set; }
  public static void SetFlyZoneData(CreatureMosquitoFlyZone zone) => flyZoneData = zone;


  /* >>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
    >>> UNITY API FUNCTIONS
  <<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<< */
  public override void OnNetworkSpawn()
  {
    base.OnNetworkSpawn();
    // Only Server applies 
    // movement logic to creature
    if (IsServer)
    {
      transform.position = GetRandomPosition();
      Move();
    }
  }
  private void FixedUpdate()
  {
    transform.position += m_velocity * Time.fixedDeltaTime;
  }

  /* >>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>
    >>> DEFINED FUNCTIONS
  <<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<< */
  private static Vector3 GetRandomPosition()
  {
    return new Vector3(
      UnityEngine.Random.Range(flyZoneData.transform.position.x, flyZoneData.Opposite.x),
      UnityEngine.Random.Range(flyZoneData.transform.position.y, flyZoneData.Opposite.y),
      UnityEngine.Random.Range(flyZoneData.transform.position.z, flyZoneData.Opposite.z)
    );
  }

  private IEnumerator OnArrivedToDestination()
  {
    while (Vector3.Distance(transform.position, m_targetDestination) > 0.5f)
    {
      yield return null;
    }

    m_velocity = Vector3.zero;
    yield return new WaitForSeconds(UnityEngine.Random.Range(0, 2.0f));
    Move();
    yield break;
  }

  /* ----------------------------------
      --- Overrides
  ---------------------------------- */

  void OnTriggerEnter(Collider other)
  {
    if (other.gameObject.name == "Knife") Death();
  }
  protected override void Death()
  {
    if (!IsServer) return;

    PlayDeathSoundRpc();
    NetworkServer.Singleton.DeactivateGameObjectInNetwork(gameObject);
    GameObject _food = Instantiate(ResultingFood, transform.position, transform.rotation);
    NetworkServer.Singleton.SpawnGameObjectInNetwork(_food);
    NetworkServer.Singleton.DespawnGameObjectInNetwork(gameObject);
  }

  protected override void Move()
  {
    m_targetDestination = GetRandomPosition();
    transform.LookAt(m_targetDestination);
    m_velocity = transform.forward.normalized * BaseMovementSpeed;
    StartCoroutine(OnArrivedToDestination());
  }

  protected override void Panic(){ }
  
  [Rpc(SendTo.ClientsAndHost)]
  private void PlayDeathSoundRpc()
  {
    if (deathSound != null)
    {
      GetComponent<AudioSource>().PlayOneShot(deathSound);
    }
  }
}
