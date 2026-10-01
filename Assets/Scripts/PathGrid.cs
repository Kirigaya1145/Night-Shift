using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathGrid : MonoBehaviour
{
    public LayerMask obstacleMask;
    public Vector2 gridWorldSize = new Vector2(40, 40);
    public float nodeRAdius = 0.5f;
    public float checkBottom = 0.1f;
    public float checkHeight = 2.2f;

    PathNode[,] grid;
    float nodeDiameter;
    Vector3 bottomLeft;
    int gridSizeX, gridSizeZ;
    readonly Dictionary<EnemyFSM, List<Vector3>> debugPaths = new Dictionary<EnemyFSM, List<Vector3>>();

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
        bottomLeft = transform.position - Vector3.right * gridWorldSize.x / 2f - Vector3.forward * gridWorldSize.y / 2f;
        Vector3 halfEx = new Vector3(nodeRAdius * 0.9f, checkHeight / 2f, nodeRAdius * 0.9f);
        for (int x = 0; x < gridSizeX; x++)
        {
            for (int z = 0; z < gridSizeZ; z++)
            {
                Vector3 worldPoint = bottomLeft + Vector3.right * (x * nodeDiameter + nodeRAdius) + Vector3.forward * (z * nodeDiameter + nodeRAdius);
                Vector3 boxCenter = worldPoint + Vector3.up * (checkBottom + checkHeight / 2f);
                bool isWall = Physics.CheckBox(boxCenter, halfEx, Quaternion.identity, obstacleMask, QueryTriggerInteraction.Ignore);
                grid[x, z] = new PathNode(isWall, worldPoint, x, z);
            }
        }
    }
    public PathNode NodeFromWorldPoint(Vector3 worldPos)
    {
        int x = Mathf.Clamp(Mathf.FloorToInt((worldPos.x - bottomLeft.x) / nodeDiameter), 0, gridSizeX - 1);
        int z = Mathf.Clamp(Mathf.FloorToInt((worldPos.z - bottomLeft.z) / nodeDiameter), 0, gridSizeZ - 1);
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
                if (checkX < 0 || checkX >= gridSizeX || checkZ < 0 || checkZ >= gridSizeZ) continue;

                if (x != 0 && z != 0)
                {
                    bool sideXBlocked = grid[node.gridX + x, node.gridZ].isWall;
                    bool sideZBlocked = grid[node.gridX, node.gridZ + z].isWall;
                    if (sideXBlocked || sideZBlocked) continue;
                }
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
    public bool IsBlockedAt(Vector3 worldPos)
    {
        if (grid == null) return false;
        if (Mathf.Abs(worldPos.x - transform.position.x) > gridWorldSize.x / 2f ||
            Mathf.Abs(worldPos.z - transform.position.z) > gridWorldSize.y / 2f)
            return true;
        return NodeFromWorldPoint(worldPos).isWall;
    }
    public float FreeDistance(Vector3 from, Vector3 dir, float maxDist)
    {
        float step = nodeDiameter * 0.5f;
        for (float d = step; d <= maxDist; d += step)
        {
            if (IsBlockedAt(from + dir * d)) return d - step;
        }
        return maxDist;
    }
    public void ReportPath(EnemyFSM owner, List<Vector3> path)
    {
        if (path == null || path.Count == 0) debugPaths.Remove(owner);
        else debugPaths[owner] = path;
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireCube(transform.position, new Vector3(gridWorldSize.x, 1f, gridWorldSize.y));
        if (grid == null) return;

        HashSet<PathNode> pathSet = new HashSet<PathNode>();
        HashSet<PathNode> startSet = new HashSet<PathNode>();
        HashSet<PathNode> targetSet = new HashSet<PathNode>();

        foreach (var kv in debugPaths)
        {
            if (kv.Key == null) continue;
            startSet.Add(NodeFromWorldPoint(kv.Key.transform.position));
            foreach (Vector3 p in kv.Value) pathSet.Add(NodeFromWorldPoint(p));
            targetSet.Add(NodeFromWorldPoint(kv.Value[kv.Value.Count - 1]));
        }

        foreach (PathNode n in grid)
        {
            Gizmos.color = n.isWall ? Color.red : new Color(1, 1, 1, 0.2f);
            if (pathSet.Contains(n)) Gizmos.color = Color.black;   // path A*
            if (startSet.Contains(n)) Gizmos.color = Color.green;   // posisi enemy
            if (targetSet.Contains(n)) Gizmos.color = Color.cyan;    // tujuan (pemain)
            Gizmos.DrawCube(n.worldPosition, new Vector3(nodeDiameter - 0.1f, 0.1f, nodeDiameter - 0.1f));
        }
    }
}
