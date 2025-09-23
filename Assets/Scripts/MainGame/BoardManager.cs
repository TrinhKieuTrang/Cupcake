using UnityEngine;
using System.Collections.Generic;

public class BoardManager : MonoBehaviour
{
    public int cols = 8;
    public int rows = 8;
    public float cellSize = 1f;
    private Tray[,] grid;
    private bool[,] hasTile;
    private Vector2Int offset;
    private Dictionary<Vector2Int, SweetRay> sweetRays = new Dictionary<Vector2Int, SweetRay>();

    void Awake()
    {
        grid = new Tray[cols, rows];
        hasTile = new bool[cols, rows];
        offset = new Vector2Int(cols / 2, rows / 2);

        foreach (SweetRay ray in FindObjectsByType<SweetRay>(FindObjectsSortMode.None))
        {
            Vector2Int pos = WorldToGrid(ray.pickupPoint.position);
            sweetRays[pos] = ray;
        }

        foreach(Transform trans in transform.GetChild(0))
        {
            Vector2Int pos = WorldToGrid(trans.position);
            if (pos.x >= 0 && pos.y >= 0 && pos.x < cols && pos.y < rows)
            {
                hasTile[pos.x, pos.y] = true;
            }
        }
    }

    public Vector2Int WorldToGrid(Vector3 worldPos)
    {
        int x = Mathf.RoundToInt(worldPos.x / cellSize) + offset.x;
        int y = Mathf.RoundToInt(worldPos.z / cellSize) + offset.y;
        return new Vector2Int(x, y);
    }

    public Vector3 GridToWorld(Vector2Int gridPos)
    {
        int x = gridPos.x - offset.x;
        int y = gridPos.y - offset.y;
        return new Vector3(x * cellSize, 0, y * cellSize);
    }

    public bool CanMoveTo(Tray tray, Vector2Int startPos, Vector2Int gridPos, Vector2Int[] occupiedCells)
    {
        foreach (var cell in occupiedCells)
        {
            int xStart = startPos.x + cell.x;
            int yStart = startPos.y + cell.y;
            int xEnd = gridPos.x + cell.x;
            int yEnd = gridPos.y + cell.y;

            if (xEnd < 0 || yEnd < 0 || xEnd >= cols || yEnd >= rows || !hasTile[xEnd, yEnd])
                return false;

            int stepX = xEnd < xStart ? 1 : -1;
            for (int x = xEnd; x != xStart; x += stepX)
            {
                if (grid[x, yStart] != null && grid[x, yStart] != tray)
                    return false;
            }

            int stepY = yEnd < yStart ? 1 : -1;
            for (int y = yEnd; y != yStart; y += stepY)
            {
                if (grid[xEnd, y] != null && grid[xEnd, y] != tray)
                    return false;
            }
        }
        return true;
    }


    public void PlaceTray(Tray tray, Vector2Int gridPos, Vector2Int[] occupiedCells)
    {
        foreach (var cell in occupiedCells)
        {
            int x = gridPos.x + cell.x;
            int y = gridPos.y + cell.y;
            grid[x, y] = tray;
        }
    }

    public void ClearTray(Tray tray)
    {
        for (int i = 0; i < cols; i++)
            for (int j = 0; j < rows; j++)
                if (grid[i, j] == tray)
                {
                    grid[i, j] = null;
                    break;
                }
    }

    public SweetRay GetSweetRayAt(Vector2Int gridPos)
    {
        if (sweetRays.TryGetValue(gridPos, out SweetRay ray)) return ray;
        return null;
    }
}
