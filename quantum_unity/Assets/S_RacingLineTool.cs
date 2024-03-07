using System.Collections.Generic;
using Photon.Deterministic;
using UnityEngine;
using UnityEngine.AI;
using Quantum;
using NavMesh = UnityEngine.AI.NavMesh; // Assuming Quantum is a necessary namespace
#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteInEditMode]
public unsafe class S_RacingLineTool : MonoBehaviour
{
    [SerializeField] private GameObject startPoint;
    [SerializeField] private GameObject endPoint;
    private MapData mapData;
    private MapCustomDataAsset customData;
    private Graph graphAsset;

    [InspectorButton("BakeNavigationGraph")] [SerializeField]
    private bool _bakeNavigationGraph;

    [InspectorButton("ClearNavigationGraph")] [SerializeField]
    private bool _clearNavigationGraph;

    [InspectorButton("DebugGraphContents")] [SerializeField]
    private bool _debugGraphContents;

    private void InitializeData()
    {
        mapData = FindObjectOfType<MapData>();
        customData = UnityDB.FindAsset<MapCustomDataAsset>(mapData.Asset.Settings.UserAsset.Id);
        graphAsset = UnityDB.FindAsset<GraphAsset>(customData.Settings.AINavigationGraph.Id).Settings;
    }

    private void ClearNavigationGraph()
    {
        if (graphAsset != null)
        {
            graphAsset.Clear();
        }

        MarkDirtyAndDebug();
    }

    private void BakeNavigationGraph()
    {
        AddPathNodes();
        MarkDirtyAndDebug();
    }

    private void MarkDirtyAndDebug()
    {
#if UNITY_EDITOR
        if (customData != null)
            EditorUtility.SetDirty(UnityDB.FindAsset<GraphAsset>(customData.Settings.AINavigationGraph.Id));
#endif
        DebugGraphContents();
    }

    private void DebugGraphContents()
    {
        InitializeData();
        graphAsset.PrintGraph();
        //
        // if (graphAsset == null)
        //     return;
        // Debug.Log("Graph Contents:");
        // foreach (var node in graphAsset.SafeNodes)
        // {
        //     string nodeInfo = $"Node {node.Position.ToUnityVector3()} has neighbors: ";
        //     for (int j = 0; j < 16; j++)
        //     {
        //         int neighborId = node.Neighbors[j];
        //         if (neighborId == default)
        //             continue; // Skip default values in fixed-size array
        //         var neighborNode = graphAsset.GetNode(neighborId);
        //         if (neighborNode.HasValue)
        //         {
        //             nodeInfo += $"(Neighbor: {neighborNode.Value.Position.ToUnityVector3()}, Cost: 1) ";
        //         }
        //     }
        //
        //     Debug.Log(nodeInfo);
        // }
    }

    private void AddPathNodes()
    {
        InitializeData();
        graphAsset.Clear();
        var pathNodes = CalculateNavMeshPathAndCreateNodes(startPoint.transform.position, endPoint.transform.position);
        int lastNodeId = -1;
        graphAsset.nodes = new Node[pathNodes.Count];
        
        for (int i = 0; i < pathNodes.Count; i++)
        {
            int nodeId = i; // Example ID, assign as needed
            graphAsset.AddNode(nodeId, pathNodes[i]);
            if (lastNodeId != -1)
            {
                graphAsset.AddEdge(lastNodeId, nodeId);
            }

            lastNodeId = nodeId;
        }
    }

    private List<FPVector3> CalculateNavMeshPathAndCreateNodes(Vector3 start, Vector3 end)
    {
        NavMeshPath path = new NavMeshPath();
        NavMesh.CalculatePath(start, end, NavMesh.AllAreas, path);
        List<FPVector3> pathNodes = new List<FPVector3>();
        foreach (var point in path.corners)
        {
            pathNodes.Add(point.ToFPVector3());
        }

        return pathNodes;
    }

    private void OnDrawGizmos()
    {
        InitializeData();
        if (graphAsset == null)
            return;

        // Set a color for the nodes
        Gizmos.color = Color.yellow;

        // Draw each node as a small sphere
        foreach (var node in graphAsset.nodes)
        {
            Gizmos.DrawSphere(node.Position.ToUnityVector3(), 50f); // Adjust the size as needed

            // Draw edges
            for (int j = 0; j < 16; j++)
            {
                int neighborId = node.Neighbors[j];
                if (neighborId == default)
                    continue; // Skip default values
                var neighborNode = graphAsset.GetNode(neighborId);

                    // Set a color for the edges
                    Gizmos.color = Color.green;

                    // Draw a line from this node to its neighbor
                    Gizmos.DrawLine(node.Position.ToUnityVector3(), neighborNode.Position.ToUnityVector3());
                
            }
        }
    }
}