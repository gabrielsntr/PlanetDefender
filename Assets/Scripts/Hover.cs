using UnityEngine;

public class Hover : Singleton<Hover>
{

    private SpriteRenderer spriteRenderer;

    // Start is called before the first frame update
    void Start()
    {
        this.spriteRenderer = GetComponent<SpriteRenderer>();
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
    }

    public void Deactivate()
    {
        spriteRenderer.enabled = false;
        GameManager.Instance.ClickedBtn = null;
    }
}
