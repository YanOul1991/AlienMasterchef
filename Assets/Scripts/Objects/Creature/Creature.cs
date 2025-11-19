using UnityEngine;
using System;

public abstract class Creature : MonoBehaviour, IInteractionListener
{
  static public event Action OnPanick;
  static public event Action OnDeath;
  protected float m_baseMoveSpeed;
  protected float m_basePanickSpeed;
  protected abstract void Move();
  protected abstract void Panic();
  protected abstract void Death();
  protected abstract void PlaySound();

  // Interface
  public abstract void InteractionListen(IInteractionTriggerer triggerer);
}
