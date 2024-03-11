using System.Collections;
using System.Collections.Generic;
using Quantum;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class S_SpawnPointsGridTool : MonoBehaviour
{
    public int rows = 5;
    public int columns = 5;
    public float spacing = 1.0f;
    public GameObject childPrefab;
    
    public void SetNumberOfChildren()
    {
        while (transform.childCount > 0) 
        {
            foreach (Transform child in transform)
            {
                DestroyImmediate(child.gameObject);
            }
        }
        
        // Create new children
        for (int i = 0; i < rows * columns; i++)
        {
            GameObject child = Instantiate(childPrefab, transform);
            child.tag = "SpawnPoint"; // Tagging the child
        }

        // Layout the new children
        LayoutChildrenInGrid();
    }

    public void LayoutChildrenInGrid()
    {
        Vector3 startPosition = transform.position - new Vector3(columns / 2.0f * spacing, 0, rows / 2.0f * spacing) + new Vector3(spacing / 2.0f, 0, spacing / 2.0f);

        for (int x = 0; x < columns; x++)
        {
            for (int z = 0; z < rows; z++)
            {
                int childIndex = x * rows + z;
                if (childIndex < transform.childCount)
                {
                    Vector3 position = startPosition + new Vector3(x * spacing, 0, z * spacing);
                    Transform child = transform.GetChild(childIndex);
                    child.position = position;
                    child.tag = "SpawnPoint"; // Tagging the child
                }
            }
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Vector3 startPosition = transform.position - new Vector3(columns / 2.0f * spacing, 0, rows / 2.0f * spacing);

        for (int x = 0; x < columns; x++)
        {
            for (int z = 0; z < rows; z++)
            {
                Vector3 position = startPosition + new Vector3(x * spacing, 0, z * spacing);
                Gizmos.DrawWireCube(position, new Vector3(0.1f, 0.1f, 0.1f));
            }
        }
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(S_SpawnPointsGridTool))]
public class S_SpawnPointsGridToolEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        S_SpawnPointsGridTool script = (S_SpawnPointsGridTool)target;

        if (GUILayout.Button("Set Number Of Children"))
        {
            script.SetNumberOfChildren();
        }

        if (GUILayout.Button("Layout Children In Grid"))
        {
            script.LayoutChildrenInGrid();
        }
    }
}
#endif
