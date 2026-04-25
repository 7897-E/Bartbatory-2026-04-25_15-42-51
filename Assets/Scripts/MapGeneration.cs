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
    public int Width;
    public int Height;
    public Tile[] GroundTiles;
    public Tile[] WallTiles;
    public Tile[] ObstacleTiles;
    public void Init()
    {
        m_Tilemap = GetComponentInChildren<Tilemap>();
        m_Grid = GetComponent<Grid>();
        m_EmptyCellsList = new List<Vector2Int>();
        for(int y = 0; y <Height; ++y)
        {
            for(int x =0; x<Width; ++x)
            {
                Tile tile = GroundTiles[Random.Range(0, GroundTiles.Length)];
                m_EmptyCellsList.Add(new Vector2Int(x, y));
                m_Tilemap.SetTile(new Vector3Int(x, y, 0), tile);
            }
        }
        m_EmptyCellsList.Remove(new Vector2Int(1, 1));
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public Vector3 CellToWorld(Vector2Int cellIndex)
    {
        return m_Grid.GetCellCenterWorld((Vector3Int)cellIndex);
    }

}
