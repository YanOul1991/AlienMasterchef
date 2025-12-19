using Unity.Netcode;
using UnityEngine;

public class NetworkPlayer : NetworkBehaviour
{
  [SerializeField] private GameObject m_body;
  [SerializeField] private GameObject m_leftHand;
  [SerializeField] private GameObject m_rightHand;
  [SerializeField] private GameObject m_head;
  [SerializeField] private LayerMask m_layerMask;
  private GameObject m_leftHandGrabbedObject;
  private GameObject m_rightHandGrabbedObject;

  private Vector3 m_bodyOffset;
  private Vector3 m_headOffset;

  [Rpc(SendTo.Everyone)]
  public void InitializeNetworkPlayerRpc()
  {
    m_bodyOffset = m_body.transform.localPosition * 0.5f;
    m_headOffset = m_head.transform.localPosition * 0.5f;
  }

  [Rpc(SendTo.Everyone)]
  public void UpdateNetworkPlayerDataRpc(NetworkPlayerDataUpdate data)
  {
    m_body.transform.position = data.rootPosition + m_bodyOffset;
    m_body.transform.rotation = Quaternion.identity;

    m_head.transform.position = data.rootPosition + m_headOffset;
    m_head.transform.rotation = data.rootRotation;

    m_leftHand.transform.position = data.leftHandPosition;
    m_leftHand.transform.rotation = data.leftHandRotation;

    m_rightHand.transform.position = data.rightHandPosition;
    m_rightHand.transform.position = data.rightHandPosition;

  }

  // public void RootUpdate(Vector3 position, Quaternion rotation) 
  //   => m_root.transform.SetPositionAndRotation(position, rotation);

  // public void LeftHandUpdate(Vector3 position, Quaternion rotation) 
  //   => m_leftHand.transform.SetPositionAndRotation(position, rotation);

  // public void RightHandUpdate(Vector3 position, Quaternion rotation) 
  //   => m_rightHand.transform.SetPositionAndRotation(position, rotation);

  public void OnGrabAction(int hand)
  {
    if (hand == 0)
    {
      Collider[] hitColliders = Physics.OverlapSphere(m_leftHand.transform.position, 0.2f, m_layerMask);
      if (hitColliders.Length > 0)
      {
        m_leftHandGrabbedObject = hitColliders[0].gameObject;
        m_leftHandGrabbedObject.transform.parent = m_leftHand.transform;
        // m_leftHandGrabbedObject.GetComponent<Collider>().isTrigger = true;
        Destroy(m_leftHandGrabbedObject.GetComponent<Rigidbody>());
      }
    }
    else
    {
      Collider[] hitColliders = Physics.OverlapSphere(m_rightHand.transform.position, 0.2f, m_layerMask);
      if (hitColliders.Length > 0)
      {
        m_rightHandGrabbedObject = hitColliders[0].gameObject;
        m_rightHandGrabbedObject.transform.parent = m_rightHand.transform;
        // m_rightHandGrabbedObject.GetComponent<Collider>().isTrigger = true;
        Destroy(m_rightHandGrabbedObject.GetComponent<Rigidbody>());
      }
    }
  }

  public void OnUngrabAction(int hand)
  {
    if (hand == 0)
    {
      if (m_leftHandGrabbedObject != null)
      {
        m_leftHandGrabbedObject.AddComponent<Rigidbody>();
        // m_leftHandGrabbedObject.GetComponent<Collider>().isTrigger = false;
        m_leftHandGrabbedObject.transform.parent = null;
        m_leftHandGrabbedObject = null;
      }
    }
    {
      if (m_rightHandGrabbedObject != null)
      {
        m_rightHandGrabbedObject.AddComponent<Rigidbody>();
        // m_rightHandGrabbedObject.GetComponent<Collider>().isTrigger = false;
        m_rightHandGrabbedObject.transform.parent = null;
        m_rightHandGrabbedObject = null;
      }
    }
  }
}
