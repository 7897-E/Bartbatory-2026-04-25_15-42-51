using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapGeneration : MonoBehaviour
{
    private List<Vector2Int> m_EmptyCellsList;
    private Tilemap m_Tilemap;
    private Grid m_Grid;
    public int Width = 50;
    public int Height = 50;
    public Tile[] GroundTiles;
    public Tile[] WallTiles;
    public Tilemap tilemap { get { return m_Tilemap; } }
    public Tile[] wallTiles { get { return WallTiles; } }
    public Tile[] ObstacleTiles;
    public float[] GroundTileWeights;
    public void Init()
    {
        m_Tilemap = GetComponentInChildren<Tilemap>();
        m_Grid = GetComponent<Grid>();
        m_EmptyCellsList = new List<Vector2Int>();

        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                if (x == 0 || x == Width - 1 || y == 0 || y == Height - 1)
                {
                    Tile wallTile = WallTiles.Length > 0 ? WallTiles[Random.Range(0, WallTiles.Length)] : null;
                    m_Tilemap.SetTile(new Vector3Int(x, y, 0), wallTile);
                }
                else
                {
                    Tile groundTile = GetRandomWeightedTile(GroundTiles, GroundTileWeights);
                    m_Tilemap.SetTile(new Vector3Int(x, y, 0), groundTile);
                    m_EmptyCellsList.Add(new Vector2Int(x, y));
                }
            }
        }

        m_EmptyCellsList.Remove(new Vector2Int(1, 1));
    }

    void Start()
    {
        
    }

    void Update()
    {
        
    }
    public Vector3 CellToWorld(Vector2Int cellIndex)
    {
        return m_Grid.GetCellCenterWorld((Vector3Int)cellIndex);
    }

    public Vector3 GetBorderSpawnPosition(int margin = 1)
    {
        if (m_Grid == null)
            return Vector3.zero;

        if (Width <= margin * 2 || Height <= margin * 2)
            return CellToWorld(new Vector2Int(0, 0));

        int x = 0;
        int y = 0;
        int side = Random.Range(0, 4);

        switch (side)
        {
            case 0: // left
                x = margin;
                y = Random.Range(margin, Height - margin);
                break;
            case 1: // right
                x = Width - margin - 1;
                y = Random.Range(margin, Height - margin);
                break;
            case 2: // bottom
                x = Random.Range(margin, Width - margin);
                y = margin;
                break;
            case 3: // top
                x = Random.Range(margin, Width - margin);
                y = Height - margin - 1;
                break;
        }

        return CellToWorld(new Vector2Int(x, y));
    }

    public Vector3 GetOffscreenSpawnPosition(Camera cam, float margin = 2f)
    {
        if (cam == null || m_Grid == null)
            return Vector3.zero;

        Vector3 bottomLeft = cam.ViewportToWorldPoint(new Vector3(0f, 0f, cam.nearClipPlane));
        Vector3 topRight = cam.ViewportToWorldPoint(new Vector3(1f, 1f, cam.nearClipPlane));

        float camMinX = bottomLeft.x;
        float camMaxX = topRight.x;
        float camMinY = bottomLeft.y;
        float camMaxY = topRight.y;

        Vector3 mapMin = CellToWorld(new Vector2Int(0, 0));
        Vector3 mapMax = CellToWorld(new Vector2Int(Width - 1, Height - 1));

        float mapMinX = Mathf.Min(mapMin.x, mapMax.x) - 0.5f;
        float mapMaxX = Mathf.Max(mapMin.x, mapMax.x) + 0.5f;
        float mapMinY = Mathf.Min(mapMin.y, mapMax.y) - 0.5f;
        float mapMaxY = Mathf.Max(mapMin.y, mapMax.y) + 0.5f;

        var validSides = new System.Collections.Generic.List<int>();

        if (camMinX - margin >= mapMinX && camMinX - margin <= mapMaxX)
            validSides.Add(0);
        if (camMaxX + margin >= mapMinX && camMaxX + margin <= mapMaxX)
            validSides.Add(1);
        if (camMinY - margin >= mapMinY && camMinY - margin <= mapMaxY)
            validSides.Add(2);
        if (camMaxY + margin >= mapMinY && camMaxY + margin <= mapMaxY)
            validSides.Add(3);

        if (validSides.Count == 0)
            return GetBorderSpawnPosition(margin: 1);

        int side = validSides[Random.Range(0, validSides.Count)];
        float x = 0f;
        float y = 0f;

        switch (side)
        {
            case 0: // left
                x = camMinX - margin;
                y = Random.Range(Mathf.Max(camMinY, mapMinY), Mathf.Min(camMaxY, mapMaxY));
                break;
            case 1: // right
                x = camMaxX + margin;
                y = Random.Range(Mathf.Max(camMinY, mapMinY), Mathf.Min(camMaxY, mapMaxY));
                break;
            case 2: // bottom
                x = Random.Range(Mathf.Max(camMinX, mapMinX), Mathf.Min(camMaxX, mapMaxX));
                y = camMinY - margin;
                break;
            case 3: // top
                x = Random.Range(Mathf.Max(camMinX, mapMinX), Mathf.Min(camMaxX, mapMaxX));
                y = camMaxY + margin;
                break;
        }

        x = Mathf.Clamp(x, mapMinX, mapMaxX);
        y = Mathf.Clamp(y, mapMinY, mapMaxY);
        return new Vector3(x, y, 0f);
    }
    private Tile GetRandomWeightedTile(Tile[] tiles, float[] weights)
    {
        if (tiles == null || tiles.Length == 0)
            return null;

        if (weights == null || weights.Length != tiles.Length)
            return tiles[Random.Range(0, tiles.Length)];

        float total = 0f;
        for (int i = 0; i < weights.Length; i++)
        {
            float w = Mathf.Max(0f, weights[i]); 
            total += w;
        }

        if (total <= 0f)
        {
            return tiles[Random.Range(0, tiles.Length)];
        }

        float r = Random.value * total;
        float running = 0f;

        for (int i = 0; i < tiles.Length; i++)
        {
            running += Mathf.Max(0f, weights[i]);
            if (r <= running)
                return tiles[i];
        }

        return tiles[tiles.Length - 1];
    }
}
