using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileScript : MonoBehaviour
{
    public Point GridPosition { get; set; }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Setup(Point gridPos, Vector3 worldPos, Transform parent)
    {
        this.GridPosition = gridPos;
        transform.position = worldPos;
        transform.SetParent(parent);
        LevelManager.Instance.Tiles.Add(gridPos, this);
    }

    private void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(0))
        {
            PlaceTower();
        }
    }

    private void PlaceTower()
    {
        Vector3 newPos = transform.position + new Vector3(1.85f, 1.45f, 0);
        //Debug.Log(GridPosition.X + ", " + GridPosition.Y);
        Instantiate(GameManager.Instance.TowerPrefab, newPos, Quaternion.identity);
    }

}
