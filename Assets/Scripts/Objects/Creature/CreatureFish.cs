/* ;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
 
    +++ CreatureFish.cs
        Script for Fish creatures

    +++ Yanis Oulmane
 
 ;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;; */

using UnityEngine;
using UnityEngine.AI;

public class CreatureFish : Creature<CreatureFish>
{
#if UNITY_EDITOR
  [Header("DEBUGGING")]
  public bool __DEBUG__DebugMode;
  public Transform[] __DEBUG__MovementTargetsTest;
  public int __DEBUG__MovementIndex;
#endif

  [field: SerializeField] public override float BaseMovementSpeed { get; protected set; }
  [field: SerializeField] public override float PanicMovementSpeed { get; protected set; }
  [field: SerializeField] public override FoodStateTransform[] FoodStateTransforms { get; protected set; }
  private NavMeshAgent m_navAgent;


  public void Start()
  {
    m_navAgent = GetComponent<NavMeshAgent>();

#if UNITY_EDITOR
    if (__DEBUG__DebugMode)
    {
      __DEBUG__MovementIndex = 0;
      m_navAgent.destination = __DEBUG__MovementTargetsTest[__DEBUG__MovementIndex].position;
    }
#endif
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
#endif
  }

  private void OnCollisionEnter(Collision collision)
  {
    if (collision.gameObject.name == "Knife")
    {
      Death();
    }
  }

  public override void InteractionListen(IInteractionTriggerer triggerer) { }
  protected override void Death() 
  {
    GetComponent<Rigidbody>().isKinematic = true;
    Animator animator = GetComponent<Animator>();
    m_navAgent.enabled = false;
    animator.SetTrigger("Death");
  }
  protected override void Move() { }
  protected override void Panic() { }
  protected override void PlaySound() { }
}
