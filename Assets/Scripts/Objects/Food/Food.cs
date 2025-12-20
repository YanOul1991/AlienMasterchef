using System.Collections.Generic;
using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;

[RequireComponent(
 typeof(NetworkObject),
 typeof(NetworkTransform)
)]
public class Food : NetworkBehaviour
{
  [SerializeField] private FoodStateTransform[] foodStates;
  [SerializeField] private Ingredient currentIngredient;

  [SerializeField] private AudioClip m_audioKnife;
  [SerializeField] private AudioClip m_audioBlender;
  [SerializeField] private AudioClip m_audioStove;
  [SerializeField] private AudioClip m_audioFrier;

  private Dictionary<EToolType, AudioClip> m_clips;

  private Vector3 m_validPosition;

  private void Awake()
  {
    if (TryGetComponent(out Rigidbody rb))
      rb.isKinematic = true;

    m_clips = new Dictionary<EToolType, AudioClip>()
    { { EToolType.Knife, m_audioKnife },
      { EToolType.Blender, m_audioBlender },
      { EToolType.Stove, m_audioStove },
      { EToolType.Frier, m_audioFrier }
    };

    NetworkServer.OnGameStarted += () =>
    {
      if (TryGetComponent(out Rigidbody rigidbody))
      {
        if (!IsServer) Destroy(rigidbody);
        else rigidbody.isKinematic = false;
      }

      m_validPosition = transform.position;
    };
  }

  private void Start()
  {
    currentIngredient ??= new Ingredient(EBaseIngredient.Fish, EIngredientState.Original);
  }

  public override void OnNetworkSpawn()
  {
    base.OnNetworkSpawn();
    if (TryGetComponent(out Rigidbody rigidbody))
    {
      if (!IsServer) Destroy(rigidbody);
      else rigidbody.isKinematic = false;
    }
    m_validPosition = transform.position;
  }

  public Ingredient GetIngredient()
  {
    return currentIngredient;
  }

  public void SetIngredient(Ingredient ingredient)
  {
    currentIngredient = ingredient;
  }

  void OnTriggerEnter(Collider other)
  {
    if (other.gameObject.TryGetComponent(out Tool tool))
    {
      Debug.Log($"<color=orange>[---- FOOD ----] TRIGGER with Tool</color>");
      TransformFoodRpc(tool.GetToolType());
    }
  }

  void OnCollisionEnter(Collision collision)
  {
    if (collision.gameObject.TryGetComponent(out Tool tool))
    {
      Debug.Log($"<color=orange>[---- FOOD ----] COLLISION with Tool</color>");
      TransformFoodRpc(tool.GetToolType());
    }
  }

  void FixedUpdate()
  {
    if (transform.position.y < -15)
    {
      if (TryGetComponent(out Rigidbody rb))
      {
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        transform.position = m_validPosition;
      }
    }
  }
  
  [Rpc(SendTo.ClientsAndHost)]
  private void TransformFoodRpc(EToolType toolType)
  {
    m_validPosition = transform.position;
    foreach (var transform in foodStates)
    {
      if (transform.toolType == toolType &&
        transform.requiredState == currentIngredient.state)
      {
        PlayToolSoundRpc(toolType);
        Debug.Log($"Transformed to: {currentIngredient}");
        currentIngredient.SetState(transform.resultingState);
        return;
      }
    }
    Debug.Log($"No valid transformation for {toolType} on {currentIngredient}");
  }

  [Rpc(SendTo.ClientsAndHost)]
  private void PlayToolSoundRpc(EToolType toolType)
  {
    if (TryGetComponent(out AudioSource audioSource))
    {
      audioSource.PlayOneShot(m_clips[toolType]);
    }
    else
    {
      audioSource = gameObject.AddComponent<AudioSource>();
      audioSource.PlayOneShot(m_clips[toolType]);
    }
  }

  [System.Serializable]
  private class FoodStateTransform
  {
    public EToolType toolType;
    public EIngredientState requiredState;
    public EIngredientState resultingState;
  }
}