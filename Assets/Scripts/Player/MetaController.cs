using Unity.Netcode;
using UnityEngine;

public class MetaController : MonoBehaviour
{
  [Header("Meta Controls")]
  [SerializeField] private Transform m_leftController;
  [SerializeField] private Transform m_rightController;
  [SerializeField] private Transform m_playerRoot;
  
  [Header("GameObjects")]
  [SerializeField] private GameObject m_leftHandObject;
  [SerializeField] private GameObject _rightHandObject;
  [SerializeField] private LayerMask m_layerMask;

  private bool updateToNetwork;

  private void Start()
  {
    updateToNetwork = false;
    NetworkServer.OnGameStarted += () =>
    {
      Debug.Log("[ --- MetaController --- ] GameHasStarted");
      updateToNetwork = true;
    };
  }


  void Update()
  {
    if (updateToNetwork)
    {
      NetworkServer.Singleton.OnClientUpdateServerRpc(
      new NetworkPlayerDataUpdate 
      {
        rootPosition        = m_playerRoot.position,
        rootRotation        =Quaternion.Euler(0, Camera.main.transform.rotation.y, 0),
        leftHandPosition    = m_leftController.position,
        leftHandRotation    = m_leftController.rotation,
        rightHandPosition   = m_rightController.position,
        rightHandRotation   = m_rightController.rotation,
      });

      // >>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>> LEFT HAND GRAB
      if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger, OVRInput.Controller.LTouch))
        NetworkServer.Singleton.OnClientGrabActionServerRpc(0);

      // >>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>> RIGHT HAND GRAB
      if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger, OVRInput.Controller.RTouch))
        NetworkServer.Singleton.OnClientGrabActionServerRpc(1);

      // >>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>> LEFT HAND UNGRAB
      if (OVRInput.GetUp(OVRInput.Button.PrimaryIndexTrigger, OVRInput.Controller.LTouch))
        NetworkServer.Singleton.OnClientUngrabActionServerRpc(0);

      // >>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>> RIGHT HAND UNGRAB
      if (OVRInput.GetUp(OVRInput.Button.PrimaryIndexTrigger, OVRInput.Controller.RTouch))
        NetworkServer.Singleton.OnClientUngrabActionServerRpc(1);
    }
  }
}

public struct NetworkPlayerDataUpdate : INetworkSerializable
{
  public Vector3 rootPosition;
  public Quaternion rootRotation;
  public Vector3 leftHandPosition;
  public Quaternion leftHandRotation;
  public Vector3 rightHandPosition;
  public Quaternion rightHandRotation;

  public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
  {
    serializer.SerializeValue(ref rootPosition);
    serializer.SerializeValue(ref rootRotation);
    serializer.SerializeValue(ref leftHandPosition);
    serializer.SerializeValue(ref leftHandRotation);
    serializer.SerializeValue(ref rightHandPosition);
    serializer.SerializeValue(ref rightHandRotation);
  }
}