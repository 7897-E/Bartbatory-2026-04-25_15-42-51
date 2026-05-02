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
                    Tile groundTile = GroundTiles[Random.Range(0, GroundTiles.Length)];
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

}
