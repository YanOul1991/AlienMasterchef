using UnityEngine;

public class CreatureButterfly : Creature<CreatureButterfly>
{
  [field: SerializeField] public override float BaseMovementSpeed { get; protected set; }
  [field: SerializeField] public override float PanicMovementSpeed { get; protected set; }
  [field: SerializeField] public override FoodStateTransform[] FoodStateTransforms { get; protected set; }
  public override void InteractionListen(IInteractionTriggerer triggerer){ }
  protected override void Death(){ }
  protected override void Move(){ }
  protected override void Panic(){ }
  protected override void PlaySound(){ }
}
