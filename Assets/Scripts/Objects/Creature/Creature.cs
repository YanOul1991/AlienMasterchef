/*
  Blueprint class for creatures
  AlienMasterchef All Rights Reserved
*/

using UnityEngine;
using System;
using Unity.Netcode;

[RequireComponent(typeof(
  NetworkObject
))]
[Serializable]
public abstract class Creature<T> : NetworkBehaviour
{
  static public event Action<T> OnPanick;
  static public event Action<T> OnDeath;
  [SerializeField] public abstract float BaseMovementSpeed { get; protected set; }
  [SerializeField] public abstract float PanicMovementSpeed { get; protected set; }
  [SerializeField] public abstract GameObject ResultingFood { get; protected set; }

  protected abstract void Move();
  protected abstract void Panic();
  protected abstract void Death();
}
