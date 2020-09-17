using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TileScript : MonoBehaviour
{
    public Point GridPosition { get; set; }

    private Color32 fullColor = new Color32(255, 118, 118, 255);

    private Color32 emptyColor = new Color32(96, 255, 90, 255);

    public bool IsEmpty { get; set; }
    public bool IsPath { get; set; }

    public Vector3 WorldPosition { get; set; }

    public SpriteRenderer SpriteRenderer { get => spriteRenderer; set => spriteRenderer = value; }

    public SpriteRenderer spriteRenderer;

    private Tower myTower;


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
        this.WorldPosition = worldPos;
        transform.position = worldPos;
        transform.SetParent(parent);
        this.IsPath = isPath;
        this.IsEmpty = true;
        LevelManager.Instance.Tiles.Add(gridPos, this);
    }

    private void OnMouseOver()
    {
        if (!EventSystem.current.IsPointerOverGameObject() && GameManager.Instance.ClickedBtn != null)
        {
            if (IsPath || !IsEmpty)
            {
                ColorTile(fullColor);
            }
            else
            {
                ColorTile(emptyColor);
                if (Input.GetMouseButtonDown(0))
                {
                    PlaceTower(GameManager.Instance.ClickedBtn.TowerPrefab);
                }
            }
        }
        else if (!EventSystem.current.IsPointerOverGameObject() && GameManager.Instance.ClickedBtn == null && Input.GetMouseButton(0))
        {
            if (Input.GetMouseButtonDown(0) && myTower != null) 
            {
                GameManager.Instance.SelectTower(myTower);

            }
            if (Input.GetMouseButtonDown(0) && myTower == null)
            {
                GameManager.Instance.DeselectTower();
            }
        }
    }

    private void OnMouseExit()
    {
        ColorTile(Color.white);
    }

    private void PlaceTower(GameObject towerPrefab)
    {
        if (!IsEmpty)
        {
            Destroy(transform.GetChild(0).gameObject);
        }
        Vector3 newPos = transform.position + new Vector3(this.spriteRenderer.sprite.bounds.size.x / 2, -this.spriteRenderer.sprite.bounds.size.y / 2, 0);
        GameObject tower = (GameObject)Instantiate(towerPrefab, newPos, Quaternion.identity);
        
        tower.transform.SetParent(transform);
        this.myTower = tower.transform.GetComponent<Tower>();
        StartCoroutine(DisableRange(tower));
        GameManager.Instance.BuyTower(myTower.Price);
        GameManager.Instance.ClickedBtn = null;
        IsEmpty = false;
        ColorTile(Color.white);
    }

    public void UpgradeTower()
    {
        PlaceTower(GameManager.Instance.SelectedTower.NextLevelTower);
        GameManager.Instance.DeselectTower();
        GameManager.Instance.SelectTower(myTower);
        SoundManager.Instance.PlayEffect("tower_upgrade");
    }

    private void ColorTile(Color newColor)
    {
        SpriteRenderer.color = newColor;
    }

    private IEnumerator DisableRange(GameObject tower)
    {
        if (tower.transform.GetChild(2) != null)
        {
            Color towerColor = tower.transform.GetChild(2).GetComponent<SpriteRenderer>().color;
            float alpha = tower.transform.GetChild(2).GetComponent<SpriteRenderer>().color.a;
            for (float t = 0.0f; t < 1.0f; t += Time.deltaTime / 1.5f)
            {
                Color newColor = new Color(towerColor.r, towerColor.g, towerColor.b, Mathf.Lerp(alpha, 0.0f, t));
                try
                {
                    tower.transform.GetChild(2).GetComponent<SpriteRenderer>().color = newColor;
                } 
                finally
                {
                    
                }
                yield return null;
            }
            tower.transform.GetChild(2).GetComponent<SpriteRenderer>().enabled = false;
            tower.transform.GetChild(2).GetComponent<SpriteRenderer>().color = towerColor;

        }
    }

}
