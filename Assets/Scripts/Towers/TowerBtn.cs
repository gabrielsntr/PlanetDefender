using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TowerBtn : MonoBehaviour
{

    [SerializeField]
    private GameObject towerPrefab;

    [SerializeField]
    private Sprite sprite;

    private int price;

    [SerializeField]
    private Text priceTxt;

    public bool IsActive { get; set; }

    public GameObject TowerPrefab { get => towerPrefab; }
    public Sprite Sprite { get => sprite; }
    public int Price { get => price; }

    private void Start()
    {
        this.price = towerPrefab.GetComponent<Tower>().Price;
        this.priceTxt.text = towerPrefab.GetComponent<Tower>().Price.ToString();
        GameManager.Instance.Changed += new CurrencyChanged(PriceCheck);
    }

    private void PriceCheck()
    {
        if (Price <= GameManager.Instance.Currency)
        {
            GetComponent<Image>().color = Color.white;
            priceTxt.color = Color.white;
        }
        else
        {
            GetComponent<Image>().color = Color.grey;
            priceTxt.color = Color.grey;
        }
    }

    public void ShowInfo()
    {
        GameManager.Instance.SetToolTipText(this.TowerPrefab.GetComponent<Tower>());
        GameManager.Instance.ShowStatsHover();
    }
}
