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

    [SerializeField]
    private int price;

    [SerializeField]
    private Text priceTxt;

    public GameObject TowerPrefab { get => towerPrefab; }
    public Sprite Sprite { get => sprite; }
    public int Price { get => price; }

    private void Start()
    {
        this.priceTxt.text = Price.ToString();
    }
}
