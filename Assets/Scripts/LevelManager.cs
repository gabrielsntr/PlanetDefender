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

    private Point redSpawn;
    
    [SerializeField]
    private GameObject redPortalPrefab;

    [SerializeField]
    private Transform map;

    private Vector3 maxTile;

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

        Vector3 maxTile = Vector3.zero;

        Vector3 worldStart = Camera.main.ScreenToWorldPoint(new Vector3(0, Screen.height));
        for (int y = 0; y < mapY; y++)
        {
            char[] newTiles = mapData[y].ToCharArray();
            for (int x = 0; x < mapX; x++)
            {
                PlaceTile(newTiles[x].ToString(), x, y, worldStart);
            }
        }
        Point portalPosition = new Point(-1, -1);
        for (int y = 0; y < mapY; y++)
        {
            char[] newTiles = mapData[y].ToCharArray();
            for (int x = 0; x < mapX; x++)
            {
                if (newTiles[x].ToString().Equals("4"))
                {
                    Debug.Log(x + ", " + y);
                    portalPosition = new Point(x, y);
                    break;
                }
            }
            if (portalPosition.X != -1 && portalPosition.Y != -1)
            {
                break;
            }
        }

        maxTile = Tiles[new Point(mapX-1, mapY-1)].transform.position;
        this.maxTile = maxTile;
        
        SpawnPortal(portalPosition.X, portalPosition.Y, worldStart);

    }

    private void PlaceTile(string tileType, int x, int y, Vector3 worldStart)
    {
        int tileIndex = int.Parse(tileType);
        TileScript newTile = Instantiate(tilePrefabs[tileIndex]).GetComponent<TileScript>();

        newTile.Setup(new Point(x, y), new Vector3(worldStart.x + (TileSize * x), worldStart.y - (TileSize * y), 0), map);

        
        
    }

    private string[] ReadLevelText()
    {
        TextAsset bindData = Resources.Load("Level") as TextAsset;
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
        Instantiate(redPortalPrefab, new Vector3(worldStart.x + (TileSize * x), worldStart.y - (TileSize * y), 0), Quaternion.identity);
    }

}
