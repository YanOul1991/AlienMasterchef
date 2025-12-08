/*;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;
 
    +++ CreatureButterfly.cs
        Script des creatures de type Butterfly.
  
    +++ Par: 
        Yanis Oulmane

;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;*/


using UnityEngine;

public class CreatureButterfly : Creature<CreatureButterfly>
{
  /* ----------------------------------
    --- Overrides | Fields
  ---------------------------------- */
  [field: SerializeField] public override float BaseMovementSpeed { get; protected set; }
  [field: SerializeField] public override float PanicMovementSpeed { get; protected set; }
  [field: SerializeField] public override FoodStateTransform[] FoodStateTransforms { get; protected set; }
  [field: SerializeField] public override GameObject ResultingFood { get; protected set; }


  /* ----------------------------------
      --- Override | Methods
  ---------------------------------- */
  public override void InteractionListen(IInteractionTriggerer triggerer){ }
  protected override void Death(){ }
  protected override void Move(){ }
  protected override void Panic(){ }
  protected override void PlaySound(){ }
}
