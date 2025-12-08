/*;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
 
    +++ CreatureFish.cs
        Script des creatures de type Fish.
  
    +++ Par: 
        Yanis Oulmane

;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;*/

using UnityEngine;
using UnityEngine.AI;

[RequireComponent(
  typeof(Rigidbody),
  typeof(Animator),
  typeof(NavMeshAgent)
)]
public class CreatureFish : Creature<CreatureFish>
{
#if UNITY_EDITOR
  [Header("DEBUGGING")]
  public bool __DEBUG__DebugMode;
  public Transform[] __DEBUG__MovementTargetsTest;
  public int __DEBUG__MovementIndex;
#endif

  /* ----------------------------------
    --- Overrides | Fields
  ---------------------------------- */
  [field: SerializeField] public override float BaseMovementSpeed { get; protected set; }
  [field: SerializeField] public override float PanicMovementSpeed { get; protected set; }
  [field: SerializeField] public override FoodStateTransform[] FoodStateTransforms { get; protected set; }
  [field: SerializeField] public override GameObject ResultingFood { get; protected set; }

  /* *******************
     * Components
  ******************* */
  private NavMeshAgent m_navAgent;
  private Rigidbody m_rigidbody;
  private Animator m_animator;

  public void Start()
  {
    m_animator = GetComponent<Animator>();
    m_navAgent = GetComponent<NavMeshAgent>();
    m_rigidbody = GetComponent<Rigidbody>();

#if UNITY_EDITOR
    if (__DEBUG__DebugMode) {
      __DEBUG__MovementIndex = 0;
      m_navAgent.destination = __DEBUG__MovementTargetsTest[__DEBUG__MovementIndex].position;
    }
#endif // UNITY_EDITOR
  }

  private void Update()
  {
#if UNITY_EDITOR
    if (__DEBUG__DebugMode)
    {
      if (Vector3.Distance(m_navAgent.destination, transform.position) < 1.0f)
      {
        Debug.Log("[--- DEBUG ---] Fish arrived at destination.");
        __DEBUG__MovementIndex = __DEBUG__MovementIndex == __DEBUG__MovementTargetsTest.Length - 1 ? 0 : __DEBUG__MovementIndex + 1;
        m_navAgent.destination = __DEBUG__MovementTargetsTest[__DEBUG__MovementIndex].position;
      }
    }
#endif // UNITY_EDITOR
  }

  private void OnCollisionEnter(Collision collision)
  {
    if (collision.gameObject.name == "Knife")
    {


      GetComponent<CapsuleCollider>().enabled = false;
      m_rigidbody.isKinematic = true;
      m_navAgent.speed = 0;
      Death();
    }
  }

  /* ----------------------------------
  --- Overrides | Methods
---------------------------------- */
  public override void InteractionListen(IInteractionTriggerer triggerer) { }
  protected override void Death() 
  {
    m_animator.SetTrigger("Death");
    Invoke(nameof(ChangeToFood), 3.0f);
  }
  protected override void Move() { }
  protected override void Panic() { }
  protected override void PlaySound() { }

  private void ChangeToFood()
  {
    GameObject instance = Instantiate(ResultingFood);
    instance.transform.position = transform.position;
    Destroy(gameObject);
  }
}
