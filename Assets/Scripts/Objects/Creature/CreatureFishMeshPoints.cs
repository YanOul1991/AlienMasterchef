using UnityEditor;
using UnityEngine;

public class CreatureFishMeshPoints : MonoBehaviour
{
  public Vector3 sizes;
  public float pointSpacing;
  public Vector3[] points;
  public float gizmosRadius;

  void OnDrawGizmosSelected()
  {
    Gizmos.color = Color.darkCyan;
    
    Gizmos.DrawWireCube(transform.position + (sizes / 2), sizes);

    foreach (Vector3 point in points)
      Gizmos.DrawSphere(point, gizmosRadius);
  }

  [CustomEditor(typeof(CreatureFishMeshPoints))]
  public class CreatureFishMeshPointsEditor : Editor
  {
    public override void OnInspectorGUI()
    {
      serializedObject.Update();
      DrawDefaultInspector();

      var script = (CreatureFishMeshPoints)target;

      int xCount = (int)(script.sizes.x / script.pointSpacing);
      int zCount = (int)(script.sizes.z / script.pointSpacing);
      
      script.points = new Vector3[xCount * zCount];

      int xRow = 0;
      int zRow = 0;

      for (int i = 0; i < script.points.Length; i++)
      {
        script.points[i].x = xRow * script.pointSpacing + script.transform.position.x;
        script.points[i].z = zRow * script.pointSpacing + script.transform.position.z;

        xRow ++; 

        if (xRow == xCount)
        {
          xRow = 0;
          zRow++;
        }
      }

      serializedObject.ApplyModifiedProperties();
    }
  }
}
