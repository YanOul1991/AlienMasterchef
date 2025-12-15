/*;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
 
    +++ CreatureButterfly.cs
        Script des creatures de type Butterfly.
  
    +++ Par: 
        Yanis Oulmane

;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;*/

using System.Collections;
using UnityEngine;

public sealed class CreatureMosquito : Creature<CreatureMosquito>
{
  private static CreatureMosquitoFlyZone flyZoneData;

  private Vector3 m_targetDestination;
  private Vector3 m_velocity;

  /* ----------------------------------
    --- Overrides | Fields
  ---------------------------------- */
  [field: SerializeField] public override float BaseMovementSpeed { get; protected set; }
  [field: SerializeField] public override float PanicMovementSpeed { get; protected set; }
  [field: SerializeField] public override FoodStateTransform[] FoodStateTransforms { get; protected set; }
  [field: SerializeField] public override GameObject ResultingFood { get; protected set; }

  public static void SetFlyZoneData(CreatureMosquitoFlyZone zone) => flyZoneData = zone;

  private void OnEnable()
  {
    transform.position = GetRandomPosition();
    Move();
  }

  private static Vector3 GetRandomPosition()
  {
    return new Vector3(
      Random.Range(flyZoneData.transform.position.x, flyZoneData.Opposite.x),
      Random.Range(flyZoneData.transform.position.y, flyZoneData.Opposite.y),
      Random.Range(flyZoneData.transform.position.z, flyZoneData.Opposite.z)
    );
  }

  private void FixedUpdate()
  {
    transform.position += m_velocity * Time.fixedDeltaTime;

    // if (Vector3.Distance(transform.position, m_targetDestination) < 1.0f)
    //   StartCoroutine(OnArrivedToDestination());
  }

  private IEnumerator OnArrivedToDestination()
  {
    while (Vector3.Distance(transform.position, m_targetDestination) > 0.5f)
    {
      yield return null;
    }

    m_velocity = Vector3.zero;
    yield return new WaitForSeconds(Random.Range(0, 2.0f));
    Move();
    yield break;
  }

  /* ----------------------------------
      --- Override | Methods
  ---------------------------------- */
  public override void InteractionListen(IInteractionTriggerer triggerer){ }
  protected override void Death(){ }

  protected override void Move()
  {
    m_targetDestination = GetRandomPosition();
    transform.LookAt(m_targetDestination);
    m_velocity = transform.forward.normalized * BaseMovementSpeed;
    StartCoroutine(OnArrivedToDestination());
  }

  protected override void Panic(){ }
  protected override void PlaySound(){ }

  void OnDrawGizmos()
  {
    Gizmos.color = Color.orange;

    Gizmos.DrawLine(transform.position, m_targetDestination);
  }
}
