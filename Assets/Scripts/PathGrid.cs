using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathGrid : MonoBehaviour
{
    public LayerMask obstacleMask;
    public Vector2 gridWorldSize = new Vector2(40, 40);
    public float nodeRAdius = 0.5f;

    PathNode[,] grid;
    float nodeDiameter;
    int gridSizeX, gridSizeZ;

    void Awake()
    {
        nodeDiameter = nodeRAdius * 2f;
        gridSizeX = Mathf.RoundToInt(gridWorldSize.x / nodeDiameter);
        gridSizeZ = Mathf.RoundToInt(gridWorldSize.y / nodeDiameter);
        CreateGrid();
    }
    void CreateGrid()
    {
        grid = new PathNode[gridSizeX, gridSizeZ];
        Vector3 bottomLeft = transform.position - Vector3.right * gridWorldSize.x / 2f - Vector3.forward * gridWorldSize.y / 2f;
        for (int x = 0; x < gridSizeX; x++)
        {
            for (int z = 0; z < gridSizeZ; z++)
            {
                Vector3 worldPoint = bottomLeft + Vector3.right * (x * nodeDiameter + nodeRAdius) + Vector3.forward * (z * nodeDiameter + nodeRAdius);
                bool isWall = Physics.CheckSphere(worldPoint, nodeRAdius * 0.9f, obstacleMask);
                grid[x, z] = new PathNode(isWall, worldPoint, x, z);
            }
        }
    }
    public PathNode NodeFromWorldPoint(Vector3 worldPos)
    {
        float percentX = (worldPos.x - transform.position.x + gridWorldSize.x / 2f) / gridWorldSize.x;
        float percentZ = (worldPos.z - transform.position.z + gridWorldSize.y / 2f) / gridWorldSize.y;
        percentX = Mathf.Clamp01(percentX);
        percentZ = Mathf.Clamp01(percentZ);
        int x = Mathf.Clamp(Mathf.RoundToInt((gridSizeX - 1) * percentX), 0, gridSizeX - 1);
        int z = Mathf.Clamp(Mathf.RoundToInt((gridSizeZ - 1) * percentZ), 0, gridSizeZ - 1);
        return grid[x, z];
    }
    public List<PathNode> GetNeighbors(PathNode node)
    {
        List<PathNode> neighbors = new List<PathNode>();
        for (int x = -1;  x <= 1; x++)
        {
            for (int z = -1; z <= 1; z++)
            {
                if (x == 0 && z == 0) continue;
                int checkX = node.gridX + x;
                int checkZ = node.gridZ + z;
                if (checkX >= 0 && checkX < gridSizeX && checkZ >= 0 && checkZ < gridSizeZ)
                    neighbors.Add(grid[checkX, checkZ]);
            }
        }
        return neighbors;
    }
    public IEnumerable<PathNode> AllNodes()
    {
        foreach (PathNode n in grid)
            yield return n;
    }
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position, new Vector3(gridWorldSize.x, 1f, gridSizeZ));
        if(grid == null) return;
        foreach (PathNode n in grid)
        {
            Gizmos.color = n.isWall ? Color.red : new Color(1, 1, 1, 0.2f);
            Gizmos.DrawCube(n.worldPosition, new Vector3(nodeDiameter - 0.1f, 0.1f, nodeDiameter - 0.1f));
        }
    }
}
