using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Photon.Deterministic;
using System.Linq;
using System.Runtime.Serialization;

namespace Quantum;

public unsafe partial class Graph
{
    public Node[] nodes;
    
    public void AddNode(int id, FPVector3 position)
    {
        if (id < 0 || id >= nodes.Length)
        {
            throw new ArgumentOutOfRangeException(nameof(id));
        }

        Node newNode = new Node
        {
            Id = id,
            Position = position, // Assuming Node has a Position property
            NeighborCount = 0
        };

        nodes[id] = newNode;
    }

    public void PrintGraph()
    {
        foreach (var node in nodes)
        {
            Log.Debug($"Node ID: {node.Id}, Position: {node.Position}, Neighbor Count: {node.NeighborCount}");

            for (int j = 0; j < node.NeighborCount; j++)
            {
                int neighborId = node.Neighbors[j];
                Log.Debug($"\tNeighbor ID: {neighborId}");
            }
        }
    }
    
    public void RemoveNode(int id)
    {
        if (id < 0 || id >= nodes.Length)
        {
            throw new ArgumentOutOfRangeException(nameof(id));
        }

        nodes[id] = new Node(); // Reset the node

        for (int j = 0; j < nodes.Length; j++)
        {
            Node updatedNode = nodes[j];
            int count = 0;
            for (int i = 0; i < updatedNode.NeighborCount; i++)
            {
                if (updatedNode.Neighbors[i] != id)
                {
                    updatedNode.Neighbors[count++] = updatedNode.Neighbors[i];
                }
            }
            updatedNode.NeighborCount = count;
            nodes[j] = updatedNode;
        }
    }
    
    public void RemoveEdge(int from, int to)
    {
        if (from < 0 || from >= nodes.Length || to < 0 || to >= nodes.Length)
        {
            throw new ArgumentOutOfRangeException();
        }

        Node updatedFromNode = nodes[from];
        int count = 0;
        for (int i = 0; i < updatedFromNode.NeighborCount; i++)
        {
            if (updatedFromNode.Neighbors[i] != to)
            {
                updatedFromNode.Neighbors[count++] = updatedFromNode.Neighbors[i];
            }
        }
        updatedFromNode.NeighborCount = count;
        nodes[from] = updatedFromNode;
    }
    
    public Node GetNode(int id)
    {
        if (id < 0 || id >= nodes.Length)
        {
            throw new ArgumentOutOfRangeException(nameof(id));
        }

        return nodes[id];
    }

    
    public void AddEdge(int from, int to)
    {
        if (from < 0 || from >= nodes.Length || to < 0 || to >= nodes.Length)
        {
            throw new ArgumentOutOfRangeException();
        }

        if (nodes[from].NeighborCount >= 16)
        {
            throw new InvalidOperationException("Neighbor list is full");
        }

        nodes[from].Neighbors[nodes[from].NeighborCount++] = to;
    }


    public void Clear()
    {
        if (nodes == null)
        {
            // nodes is not initialized, so there's nothing to clear
            return;
        }

        for (int i = 0; i < nodes.Length; i++)
        {
            nodes[i] = new Node();
        }
    }

}