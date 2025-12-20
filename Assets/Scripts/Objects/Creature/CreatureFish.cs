/*;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
 
    +++ CreatureFish.cs
        Script des creatures de type Fish.
  
    +++ Par: 
        Yanis Oulmane

;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;*/

using System.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.AI;

public class CreatureFish : Creature<CreatureFish>
{
  /* ----------------------------------
    --- Overrides | Fields
  ---------------------------------- */
  [field: SerializeField] public override float BaseMovementSpeed { get; protected set; }
  [field: SerializeField] public override float PanicMovementSpeed { get; protected set; }
  [field: SerializeField] public override GameObject ResultingFood { get; protected set; }
  [field: SerializeField] private AudioClip m_deathSound;

  /* *******************
     * Components
  ******************* */
  private NavMeshAgent m_navAgent;

  public void Awake()
  {

    if (TryGetComponent(out NavMeshAgent navMeshAgent)) m_navAgent = navMeshAgent;
    else m_navAgent = (NavMeshAgent)gameObject.AddComponent(typeof(NavMeshAgent));
  }

  public override void OnNetworkSpawn()
  {
    base.OnNetworkSpawn();
    if (!IsServer) m_navAgent.enabled = false;
    Move();
  }

  private void OnTriggerEnter(Collider other)
  {
    if (other.gameObject.name == "Knife") Death();
  }

  private Vector3 GetRandomPoint()
  {
    Vector3 randomPoint = Random.insideUnitSphere * 5.0f;
    randomPoint += transform.position;

    if (NavMesh.SamplePosition(randomPoint, out NavMeshHit hit, 5.0f, NavMesh.AllAreas))
      return hit.position;

    return transform.position;
  }

  /* ----------------------------------
    --- Overrides | Methods
  ---------------------------------- */

  protected override void Death() 
  {
    if (!IsServer) return;

    PlayDeathSoundRpc();

    NetworkServer.Singleton.DeactivateGameObjectInNetwork(gameObject);
    GameObject _food = Instantiate(ResultingFood, transform.position, transform.rotation);
    NetworkServer.Singleton.SpawnGameObjectInNetwork(_food);
    NetworkServer.Singleton.DespawnGameObjectInNetwork(gameObject);
  }

  [Rpc(SendTo.ClientsAndHost)]
  private void PlayDeathSoundRpc()
  {
    if (TryGetComponent(out AudioSource audioSource))
    {
      audioSource.PlayOneShot(m_deathSound);
    }
    else
    {
      audioSource = gameObject.AddComponent<AudioSource>();
      audioSource.PlayOneShot(m_deathSound);
    }
  }

  protected override void Move()
  {
    if (!IsServer) return;

    m_navAgent.destination = GetRandomPoint();
    StartCoroutine(CheckForDestination());
  }

  protected override void Panic() { }

  private IEnumerator CheckForDestination()
  {
    while (Vector3.Distance(m_navAgent.destination, transform.position) > 0.1f)
      yield return null;
    yield return new WaitForSeconds(Random.Range(0, 2.0f));
    Move();
    yield break;
  }
}