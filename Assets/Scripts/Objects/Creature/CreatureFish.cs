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
  [field: SerializeField] public override FoodStateTransform[] FoodStateTransforms { get; protected set; }
  [field: SerializeField] public override GameObject ResultingFood { get; protected set; }

  private static readonly WaitForSeconds s_DestinationCheckRate = new(0.01f);

  /* *******************
     * Components
  ******************* */
  private NavMeshAgent m_navAgent;
  private Rigidbody m_rigidbody;
  private Animator m_animator;

  public void Awake()
  {
    m_animator = GetComponent<Animator>();
    m_navAgent = GetComponent<NavMeshAgent>();
    m_rigidbody = GetComponent<Rigidbody>();
  }

  public override void OnNetworkSpawn()
  {
    base.OnNetworkSpawn();

    if (!IsServer)
    {
      m_navAgent.enabled = false;
    }

    Move();
  }

  private void OnTriggerEnter(Collider other)
  {
    if (other.gameObject.name == "Knife")
    {
      // GetComponent<NetworkObject>().Despawn(true);
      Death();
    }
  }

  private Vector3 GetRandomPoint()
  {
    Vector3 randomPoint = Random.insideUnitSphere * 5.0f;
    randomPoint += transform.position;

    if (NavMesh.SamplePosition(randomPoint, out NavMeshHit hit, 5.0f, NavMesh.AllAreas))
    {
      return hit.position;
    }

    return transform.position;
  }

  /* ----------------------------------
    --- Overrides | Methods
  ---------------------------------- */
  public override void InteractionListen(IInteractionTriggerer triggerer) { }

  protected override void Death() 
  {
    if (!IsServer) return;
    // SpawnResultingFood();
    
    NetworkServer.DeactivateGameObjectInNetwork(gameObject);
    GameObject _food = Instantiate(ResultingFood, transform.position, transform.rotation);
    NetworkServer.SpawnGameObjectInNetwork(_food);
    NetworkServer.DespawnGameObjectInNetwork(gameObject);
  }

  protected override void Move()
  {
    if (!IsServer) return;

    m_navAgent.destination = GetRandomPoint();
    StartCoroutine(CheckForDestination());
  }

  protected override void Panic() { }

  // private void SpawnResultingFood()
  // {
  //   GameObject instance = Instantiate(ResultingFood);
  //   instance.transform.position = transform.position;
  //   Destroy(gameObject);
  // }

  private IEnumerator CheckForDestination()
  {
    while (Vector3.Distance(m_navAgent.destination, transform.position) > 0.1f)
    {
      yield return null;
    }

    yield return new WaitForSeconds(Random.Range(0, 2.0f));
    Move();
    yield break;
  }
}