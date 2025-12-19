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
 
 private Vector3 m_validPosition;

  private void Awake()
  {
  if (TryGetComponent(out Rigidbody rb))
   rb.isKinematic = true;

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
   Debug.Log($"<color=orange>[---- FOOD ----]Collided with Tool</color>");
   TransformFoodRpc(tool.GetToolType());
  }
  }

  void FixedUpdate()
  {
    if (transform.position.y < -15)
   transform.position = m_validPosition;
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
    currentIngredient.SetState(transform.resultingState);
    Debug.Log($"Transformed to: {currentIngredient}");
    return;
   }
  }
  Debug.Log($"No valid transformation for {toolType} on {currentIngredient}");
 }

 [System.Serializable]
 private class FoodStateTransform
 {
  public EToolType toolType;
  public EIngredientState requiredState;
  public EIngredientState resultingState;
 }
}