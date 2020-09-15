using UnityEngine;

public class Hover : Singleton<Hover>
{
    
    private SpriteRenderer spriteRenderer;
    private SpriteRenderer spriteRange;

    // Start is called before the first frame update
    void Start()
    {
        this.spriteRenderer = GetComponent<SpriteRenderer>();
        this.spriteRange = transform.GetChild(0).GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        FollowMouse();
    }

    private void FollowMouse()
    {
        if (spriteRenderer.enabled)
        {
            transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            transform.position = new Vector3(transform.position.x, transform.position.y, 20);
        }
    }

    public void Activate(Sprite sprite)
    {
        spriteRenderer.enabled = true;
        this.spriteRenderer.sprite = sprite;
        spriteRange.enabled = true; 
    }

    public void Deactivate()
    {
        spriteRenderer.enabled = false;
        GameManager.Instance.ClickedBtn = null;
        spriteRange.enabled = false;
    }
}
