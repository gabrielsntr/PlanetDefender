using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TileScript : MonoBehaviour
{
    public Point GridPosition { get; set; }

    private Color32 fullColor = new Color32(255, 118, 118, 255);

    private Color32 emptyColor = new Color32(96, 255, 90, 255);

    public bool IsEmpty { get; private set; }
    public bool IsPath { get; set; }

    public SpriteRenderer SpriteRenderer { get => spriteRenderer; set => spriteRenderer = value; }

    public SpriteRenderer spriteRenderer;


    // Start is called before the first frame update
    void Start()
    {
        SpriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Setup(Point gridPos, Vector3 worldPos, Transform parent, bool isPath)
    {
        this.GridPosition = gridPos;
        transform.position = worldPos;
        transform.SetParent(parent);
        this.IsPath = isPath;
        this.IsEmpty = false;
        LevelManager.Instance.Tiles.Add(gridPos, this);
    }

    private void OnMouseOver()
    {
        //!EventSystem.current.IsPointerOverGameObject() && 
        if (GameManager.Instance.ClickedBtn != null)
        {
            if (IsPath || IsEmpty)
            {
                ColorTile(fullColor);
            }
            else
            {
                ColorTile(emptyColor);
                if (Input.GetMouseButtonDown(0))
                {
                    PlaceTower();
                }
            }
        }
    }

    private void OnMouseExit()
    {
        ColorTile(Color.white);
    }

    private void PlaceTower()
    {
        Vector3 newPos = transform.position + new Vector3(1.85f, 1.45f, 0);
        GameObject tower = (GameObject)Instantiate(GameManager.Instance.ClickedBtn.TowerPrefab, newPos, Quaternion.identity);
        tower.transform.SetParent(transform);
        GameManager.Instance.BuyTower();
        IsEmpty = true;
        ColorTile(Color.white);
    }

    private void ColorTile(Color newColor)
    {
        SpriteRenderer.color = newColor;
    }

}
