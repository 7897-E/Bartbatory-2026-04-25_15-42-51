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
    public Tile[] ObstacleTiles;

    [Header("Room Generation Settings")]
    public int numRooms = 5;
    public int minRoomSize = 3;
    public int maxRoomSize = 10;
    public int roomMargin = 3;

    private struct Room
    {
        public int x, y, width, height;
        public Room(int x, int y, int width, int height)
        {
            this.x = x;
            this.y = y;
            this.width = width;
            this.height = height;
        }
    }

    public void Init()
    {
        m_Tilemap = GetComponentInChildren<Tilemap>();
        m_Grid = GetComponent<Grid>();
        m_EmptyCellsList = new List<Vector2Int>();

        List<Room> rooms = GenerateRooms(numRooms, minRoomSize, maxRoomSize, roomMargin);

        foreach (Room room in rooms)
        {
            for (int x = room.x - 1; x <= room.x + room.width; x++)
            {
                for (int y = room.y - 1; y <= room.y + room.height; y++)
                {
                    if (x >= 0 && x < Width && y >= 0 && y < Height)
                    {
                        if (x == room.x - 1 || x == room.x + room.width || y == room.y - 1 || y == room.y + room.height)
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
            }
        }

        for (int i = 0; i < rooms.Count - 1; i++)
        {
            CreateCorridor(rooms[i], rooms[i + 1]);
        }

        // Fill empty spaces with walls
        FillEmptySpaces();

        bool spawnInRoom = false;
        foreach (Room room in rooms)
        {
            if (room.x <= 1 && 1 < room.x + room.width && room.y <= 1 && 1 < room.y + room.height)
            {
                spawnInRoom = true;
                break;
            }
        }
        if (!spawnInRoom)
        {
            Room spawnRoom = new Room(0, 0, 5, 5);
            rooms.Add(spawnRoom);
            for (int x = spawnRoom.x - 1; x <= spawnRoom.x + spawnRoom.width; x++)
            {
                for (int y = spawnRoom.y - 1; y <= spawnRoom.y + spawnRoom.height; y++)
                {
                    if (x >= 0 && x < Width && y >= 0 && y < Height)
                    {
                        if (x == spawnRoom.x - 1 || x == spawnRoom.x + spawnRoom.width || y == spawnRoom.y - 1 || y == spawnRoom.y + spawnRoom.height)
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
            }
        }

        m_EmptyCellsList.Remove(new Vector2Int(1, 1));
    }

    private void CreateCorridor(Room roomA, Room roomB)
    {
        Vector2Int centerA = new Vector2Int(roomA.x + roomA.width / 2, roomA.y + roomA.height / 2);
        Vector2Int centerB = new Vector2Int(roomB.x + roomB.width / 2, roomB.y + roomB.height / 2);

        // Horizontal then vertical
        int x1 = Mathf.Min(centerA.x, centerB.x);
        int x2 = Mathf.Max(centerA.x, centerB.x);
        int y1 = centerA.y;
        int y2 = centerB.y;

        // Create horizontal corridor with walls (3 tiles wide)
        for (int x = x1; x <= x2; x++)
        {
            PlaceFloorTile(x, y1);
            PlaceFloorTile(x, y1 - 1);
            PlaceFloorTile(x, y1 + 1);
            PlaceWallTile(x, y1 - 2);
            PlaceWallTile(x, y1 + 2);
        }

        // Create vertical corridor with walls (3 tiles wide)
        for (int y = Mathf.Min(y1, y2); y <= Mathf.Max(y1, y2); y++)
        {
            PlaceFloorTile(x2, y);
            PlaceFloorTile(x2 - 1, y);
            PlaceFloorTile(x2 + 1, y);
            PlaceWallTile(x2 - 2, y);
            PlaceWallTile(x2 + 2, y);
        }
    }

    private void PlaceFloorTile(int x, int y)
    {
        if (x >= 0 && x < Width && y >= 0 && y < Height)
        {
            Tile groundTile = GroundTiles[Random.Range(0, GroundTiles.Length)];
            m_Tilemap.SetTile(new Vector3Int(x, y, 0), groundTile);
            if (!m_EmptyCellsList.Contains(new Vector2Int(x, y)))
            {
                m_EmptyCellsList.Add(new Vector2Int(x, y));
            }
        }
    }

    private void PlaceWallTile(int x, int y)
    {
        if (x >= 0 && x < Width && y >= 0 && y < Height)
        {
            Tile wallTile = WallTiles.Length > 0 ? WallTiles[Random.Range(0, WallTiles.Length)] : null;
            m_Tilemap.SetTile(new Vector3Int(x, y, 0), wallTile);
        }
    }

    private void FillEmptySpaces()
    {
        for (int y = 0; y < Height; y++)
        {
            for (int x = 0; x < Width; x++)
            {
                TileBase tile = m_Tilemap.GetTile(new Vector3Int(x, y, 0));
                if (tile == null)
                {
                    PlaceWallTile(x, y);
                }
            }
        }
    }

    private List<Room> GenerateRooms(int numRooms, int minSize, int maxSize, int margin)
    {
        List<Room> rooms = new List<Room>();
        for (int i = 0; i < numRooms; i++)
        {
            int width = Random.Range(minSize, maxSize + 1);
            int height = Random.Range(minSize, maxSize + 1);
            int x = Random.Range(margin, Width - width - margin);
            int y = Random.Range(margin, Height - height - margin);

            Room newRoom = new Room(x, y, width, height);

            bool overlaps = false;
            foreach (Room room in rooms)
            {
                if (newRoom.x < room.x + room.width + margin && newRoom.x + newRoom.width + margin > room.x &&
                    newRoom.y < room.y + room.height + margin && newRoom.y + newRoom.height + margin > room.y)
                {
                    overlaps = true;
                    break;
                }
            }

            if (!overlaps)
            {
                rooms.Add(newRoom);
            }
        }
        return rooms;
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
