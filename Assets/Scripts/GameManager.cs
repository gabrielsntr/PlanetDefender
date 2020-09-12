using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : Singleton<GameManager>
{

    private int currency;
    
    [SerializeField]
    private Text currencyText;

    public TowerBtn ClickedBtn 
    { 
        get; 
        set; 
    }
    public int Currency 
    { 
        get => currency;
        set
        {
            this.currency = value;
            this.currencyText.text = value.ToString();
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        Currency = 5;
    }

    // Update is called once per frame
    void Update()
    {
        HandleEscape();
    }

    public void PickTower(TowerBtn towerBtn)
    {
        if (Currency >= towerBtn.Price)
        {
            ClickedBtn = towerBtn;
            Hover.Instance.Activate(towerBtn.Sprite);
        }
    }

    public void BuyTower()
    {
        Currency -= ClickedBtn.Price;
        Hover.Instance.Deactivate();
    }

    private void HandleEscape()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Hover.Instance.Deactivate();
        }
    }

}
