using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tower : MonoBehaviour
{
    private SpriteRenderer baseSpriteRenderer, turretSpriteRenderer, rangeSpriteRenderer;
    // Start is called before the first frame update
    void Start()
    {
        baseSpriteRenderer = transform.GetChild(0).GetComponent<SpriteRenderer>();
        turretSpriteRenderer = transform.GetChild(1).GetComponent<SpriteRenderer>();
        rangeSpriteRenderer = transform.GetChild(2).GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void Select()
    {
        rangeSpriteRenderer.enabled = !rangeSpriteRenderer.enabled;
    }

}
