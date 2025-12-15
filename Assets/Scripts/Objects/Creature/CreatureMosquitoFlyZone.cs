using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class CreatureMosquitoFlyZone : MonoBehaviour
{
  public float X;
  public float Y;
  public float Z;
  public Vector3 Opposite { get; set; }

#if UNITY_EDITOR
  [Header("DEBUG")]
  [SerializeField] private bool displayGizmos;
#endif

  private void Awake()
  {
    Opposite = transform.position + new Vector3(X, Y, Z);
    CreatureMosquito.SetFlyZoneData(this);
  }

#if UNITY_EDITOR
  void OnDrawGizmos()
  {
    if (!displayGizmos) 
      return;

    Vector3 _opp = new(X, Y, Z);

    Gizmos.color = new Color(1.0f , 0.0f, 1.0f, 0.25f);
    Gizmos.DrawCube(transform.position + (_opp / 2), _opp);

    Gizmos.color = new Color(1.0f , 0.0f, 1.0f, 0.75f);
    Gizmos.DrawWireCube(transform.position + (_opp / 2), _opp);
  }

  [CustomEditor(typeof(CreatureMosquitoFlyZone))]
  public class CreatureMosquitoFlyZoneEditor : Editor
  {
    // SerializedProperty opposite;
    SerializedProperty x;
    SerializedProperty y;
    SerializedProperty z;

    void OnEnable()
    {
      // opposite = serializedObject.FindProperty("Opposite");
      x = serializedObject.FindProperty("X");
      y = serializedObject.FindProperty("Y");
      z = serializedObject.FindProperty("Z");
    }

    private void OnSceneGUI()
    {
      CreatureMosquitoFlyZone zone = (CreatureMosquitoFlyZone)target;
    }

    public override void OnInspectorGUI()
    {
      serializedObject.Update();

      CreatureMosquitoFlyZone zone = (CreatureMosquitoFlyZone)target;
      
      EditorGUILayout.Slider(x, 0, 50.0f, new GUIContent("X Size"));
      EditorGUILayout.Slider(y, 0, 50.0f, new GUIContent("Y Size"));
      EditorGUILayout.Slider(z, 0, 50.0f, new GUIContent("Z Size"));

      zone.displayGizmos = EditorGUILayout.Toggle(zone.displayGizmos);

      serializedObject.ApplyModifiedProperties();
    }
  }
  #endif
}

