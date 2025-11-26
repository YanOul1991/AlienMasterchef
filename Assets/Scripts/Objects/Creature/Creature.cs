/*
  Blueprint class for creatures
  AlienMasterchef All Rights Reserved
*/

using UnityEngine;
using System;

public abstract class Creature<T> : MonoBehaviour, IInteractionListener
{
  static public event Action<T> OnPanick;
  static public event Action<T> OnDeath;

  protected float m_baseMoveSpeed;
  protected float m_basePanickSpeed;
  protected abstract void Move();
  protected abstract void Panic();
  protected abstract void Death();
  protected abstract void PlaySound();
  public abstract void InteractionListen(IInteractionTriggerer triggerer);
}
