using Unity.Netcode;
using UnityEngine;

public class NetworkPlayer : NetworkBehaviour
{
  private GameObject m_leftHand;
  private GameObject m_rightHand;

  [SerializeField] private LayerMask m_layerMask;

  private GameObject m_leftHandGrabbedObject;
  private GameObject m_rightHandGrabbedObject;

  public void Initialize(GameObject leftHand, GameObject rightHand)
  {
    m_leftHand = leftHand;
    m_rightHand = rightHand;
  }

  public void LeftHandUpdate(Vector3 position, Quaternion rotation)
  {
    m_leftHand.transform.SetPositionAndRotation(position, rotation);
  }

  public void RightHandUpdate(Vector3 position, Quaternion rotation)
  {
    m_rightHand.transform.SetPositionAndRotation(position, rotation);
  }

  public void OnGrabAction(int hand)
  {
    if (hand == 0)
    {
      Collider[] hitColliders = Physics.OverlapSphere(m_leftHand.transform.position, 0.2f, m_layerMask);
      if (hitColliders.Length > 0)
      {
        m_leftHandGrabbedObject = hitColliders[0].gameObject;
        m_leftHandGrabbedObject.transform.parent = m_leftHand.transform;
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
        m_leftHandGrabbedObject.transform.parent = null;
        m_leftHandGrabbedObject = null;
      }
    }
    {
      if (m_rightHandGrabbedObject != null)
      {
        m_rightHandGrabbedObject.AddComponent<Rigidbody>();
        m_rightHandGrabbedObject.transform.parent = null;
        m_rightHandGrabbedObject = null;
      }
    }
  }
}
