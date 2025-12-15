/*
  Blueprint class for creatures
  AlienMasterchef All Rights Reserved
*/

using UnityEngine;
using System;

[RequireComponent(
  typeof(Rigidbody))
 ]

[Serializable]
public abstract class Creature<T> : MonoBehaviour, IInteractionListener
{
  static public event Action<T> OnPanick;
  static public event Action<T> OnDeath;
  [SerializeField] public abstract float BaseMovementSpeed { get; protected set; }
  [SerializeField] public abstract float PanicMovementSpeed { get; protected set; }
  [SerializeField] public abstract FoodStateTransform[] FoodStateTransforms { get; protected set; }
  [SerializeField] public abstract GameObject ResultingFood { get; protected set; }

  protected abstract void Move();
  protected abstract void Panic();
  protected abstract void Death();
  protected abstract void PlaySound();
  public abstract void InteractionListen(IInteractionTriggerer triggerer);

  [Serializable]
  public struct FoodStateTransform
  {
    [SerializeField] public EToolType interactionTool;
    [SerializeField] public EBaseIngredient resultingFood;
  }
}
