using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LevelManager : Singleton<LevelManager>
{
    [SerializeField]
    private GameObject[] tilePrefabs;
    public float TileSize 
    {
        get { return tilePrefabs[0].GetComponent<SpriteRenderer>().sprite.bounds.size.x; }
    }

    [SerializeField]
    private CameraMovement cameraMovement;

    public Point StartPath { get; set; }
    public Point EndPath { get; set; }

    private Stack<Node> path;
    
    public Stack<Node> Path 
    {
        get
        {
            if(path == null)
            {
                GeneratePath();
            }
            return new Stack<Node>(new Stack<Node>(path));
        }
    }

    [SerializeField]
    private GameObject redPortalPrefab;

    public Portal RedPortal { get; set; }

    [SerializeField]
    private Transform map;

    private Vector3 maxTile;

    private Point mapSize;

    public Dictionary<Point, TileScript> Tiles { get; set; }

    // Start is called before the first frame update
    void Start()
    {
        Point p = new Point(0, 0);
        CreateLevel();
        cameraMovement.SetLimits(new Vector3(this.maxTile.x + TileSize, this.maxTile.y - TileSize));

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void CreateLevel()
    {
        Tiles = new Dictionary<Point, TileScript>();

        string[] mapData = ReadLevelText();

        int mapX = mapData[0].ToCharArray().Length;
        int mapY = mapData.Length;

        mapSize = new Point(mapData[0].ToCharArray().Length, mapData.Length);

        Vector3 maxTile = Vector3.zero;
        Vector3 worldStart = Camera.main.ScreenToWorldPoint(new Vector3(0, Screen.height));
        Point portalPosition = new Point(-1, -1);
        for (int y = 0; y < mapY; y++)
        {
            char[] newTiles = mapData[y].ToCharArray();
            for (int x = 0; x < mapX; x++)
            {
                
                //Debug.Log("Tile: " + newTiles[x].ToString() + ", x: " + x + ", y: " + y);
                string tileType = newTiles[x].ToString();
                PlaceTile(tileType, x, y, worldStart);
                if (tileType == "6")
                {
                    portalPosition = new Point(x, y);
                }
                if (tileType == "7")
                {
                    EndPath = new Point(x, y);
                }
            }
        }

        maxTile = Tiles[new Point(mapX-1, mapY-1)].transform.position;
        this.maxTile = maxTile;
        
        SpawnPortal(portalPosition.X, portalPosition.Y, worldStart);
        StartPath = new Point(portalPosition.X, portalPosition.Y);

    }

    private void PlaceTile(string tileType, int x, int y, Vector3 worldStart)
    {
        int tileIndex = int.Parse(tileType);
        TileScript newTile = Instantiate(tilePrefabs[tileIndex]).GetComponent<TileScript>();

        bool isPath = false;
        if (tileType == "4" || tileType == "5" || tileType == "6" || tileType == "7")
        {
            isPath = true;
        }
        newTile.Setup(new Point(x, y), new Vector3(worldStart.x + (TileSize * x), worldStart.y - (TileSize * y), 0), map, isPath);
    }

    private string[] ReadLevelText()
    {
        TextAsset bindData = Resources.Load("Level" + LevelSelected.Level.ToString()) as TextAsset;
        string data = bindData.text.Replace(Environment.NewLine, string.Empty);
        return data.Split('-');
    }

    private void SpawnPortal(int x, int y, Vector3 worldStart)
    {
        if (x == -1 || y == -1)
        {
            x = 0;
            y = 0;
        }        
        GameObject tmp = (GameObject) Instantiate(redPortalPrefab, new Vector3(worldStart.x + (TileSize * x), worldStart.y - (TileSize * y) + (0.2f), 0), Quaternion.identity);
        RedPortal = tmp.GetComponent<Portal>();
        RedPortal.name = "RedPortal";
    }

    public bool InBounds(Point position)
    {
        return position.X >= 0 && position.Y >= 0 && position.X < mapSize.X && position.Y < mapSize.Y;
    }

    public void GeneratePath()
    {
        path = AStar.GetPath(StartPath, EndPath);
    }

}
