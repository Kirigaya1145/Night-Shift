using UnityEngine;

public class PathNode
{
    public int gridX;
    public int gridZ;
    public bool isWall;
    public Vector3 worldPosition;

    public PathNode parent;
    public int gCost;
    public int hCost;
    public int fCost => gCost + hCost;

    public PathNode (bool isWall, Vector3 worldPositon, int gridX, int gridZ)
    {
        this.isWall = isWall;
        this.worldPosition = worldPosition;
        this.gridX = gridX;
        this.gridZ = gridZ;
    }
}
