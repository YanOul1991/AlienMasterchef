using System;
using UnityEngine;

public class MetaController : MonoBehaviour
{
  [SerializeField] private Transform m_leftController;
  [SerializeField] private Transform m_rightController;

  [SerializeField] private SphereCollider m_leftControllerCollider;
  [SerializeField] private SphereCollider m_rightControllerCollider;

  [SerializeField] private LayerMask m_layerMask;

  private GameObject m_leftHandGrab;
  private GameObject m_rightHandGrab;

  private void Start()
  {
    m_leftControllerCollider.enabled = false;
    m_rightControllerCollider.enabled = false;
  }

  void Update()
  {
    m_leftControllerCollider.transform.position = m_leftController.position;
    m_rightControllerCollider.transform.position = m_rightController.position;
    m_leftControllerCollider.transform.rotation = m_leftController.rotation;
    m_rightControllerCollider.transform.rotation = m_rightController.rotation;

    // >>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>> RIGHT HAND GRAB
    if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger, OVRInput.Controller.RTouch))
    {
      Collider[] hitColliders = Physics.OverlapSphere(m_rightController.position, 0.2f, m_layerMask);

      if (hitColliders.Length > 0)
      {
        Debug.Log($"<color=cyan> GRABBED {hitColliders[0].gameObject.name}");
        m_rightHandGrab = hitColliders[0].gameObject;
        m_rightHandGrab.transform.parent = m_rightControllerCollider.transform;
        Destroy(m_rightHandGrab.GetComponent<Rigidbody>());
      }
    }

    // >>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>> LEFT HAND GRAB
    if (OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger, OVRInput.Controller.LTouch))
    {
      //m_leftControllerCollider.enabled = true;
    }

    // >>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>> LEFT HAND UNGRAB
    if (OVRInput.GetUp(OVRInput.Button.PrimaryIndexTrigger, OVRInput.Controller.LTouch))
    {
      //m_leftControllerCollider.enabled = false;
    }

    // >>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>> RIGHT HAND UNGRAB
    if (OVRInput.GetUp(OVRInput.Button.PrimaryIndexTrigger, OVRInput.Controller.RTouch))
    {
      if (m_rightHandGrab != null)
      {
        m_rightHandGrab.AddComponent<Rigidbody>();
        m_rightHandGrab.transform.parent = null;
        m_rightHandGrab = null;
      }
    }
  }
}
