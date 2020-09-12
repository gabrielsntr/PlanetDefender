using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AStarDebug : MonoBehaviour
{
    [SerializeField]
    private TileScript start, goal;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        ClickTile();
        if (Input.GetKeyDown(KeyCode.Space))
        {
            AStar.GetPath(start.GridPosition);
        }
    }

    private void ClickTile()
    {
        if (Input.GetMouseButtonDown(1))
        {
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
            if (hit.collider != null)
            {
                TileScript tmp = hit.collider.GetComponent<TileScript>();
                if (tmp != null && tmp.IsPath)
                {
                    if (start == null)
                    {
                        start = tmp;
                        start.SpriteRenderer.color = Color.green;
                    }
                    else if (goal == null)
                    {
                        goal = tmp;
                        goal.SpriteRenderer.color = Color.red;
                    }
                }
            }
        }
    }

    public void DebugPath(HashSet<Node> openList)
    {
        foreach (Node node in openList)
        {
            node.TileRef.SpriteRenderer.color = Color.cyan;
        }
    }
}
