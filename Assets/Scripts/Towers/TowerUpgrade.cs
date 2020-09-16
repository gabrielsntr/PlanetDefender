using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TowerUpgrade : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GameManager.Instance.Changed += new CurrencyChanged(PriceCheck);
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void PriceCheck()
    {
        if (GameManager.Instance.SelectedTower != null)
        {
            if (GameManager.Instance.SelectedTower.Price <= GameManager.Instance.Currency)
            {
                transform.GetComponent<Image>().color = Color.white;
                transform.Find("UpgradeTxt").GetComponent<Text>().color = Color.white;
            }
            else
            {
                GetComponent<Image>().color = Color.grey;
                transform.Find("UpgradeTxt").GetComponent<Text>().color = Color.grey;
            }
        }
    }

}
