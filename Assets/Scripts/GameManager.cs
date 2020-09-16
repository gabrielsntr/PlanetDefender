﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public delegate void CurrencyChanged();

public class GameManager : Singleton<GameManager>
{

    public event CurrencyChanged Changed;

    [SerializeField]
    private Sprite speedBtnActive, speedBtnInactive, pauseBtnActive, pauseBtnInactive;

    [SerializeField]
    private int gameSpeedAcc;

    private float speed;

    private int wave = 0, lives, health;

    private bool gameAccelerated;

    [SerializeField]
    private Text livesText;

    public int Lives
    {
        get => lives;
        set
        {
            this.lives = value;
            this.livesText.text = lives.ToString();
            if (lives <= 0)
            {
                this.lives = 0;
                GameOver();
            }
        }
    }

    private int currency;
    
    [SerializeField]
    private Text currencyText;
    public int Currency
    {
        get => currency;
        set
        {
            this.currency = value;
            this.currencyText.text = value.ToString();
            OnCurrencyChanged();
        }
    }

    [SerializeField]
    private Text waveText;

    [SerializeField]
    private GameObject speedBtn, pauseBtn, towersBtn, waveBtn, wavePanel, currencyPanel, healthPanel, upgradePanel, statsPanel;

    [SerializeField]
    private Text sellText;

    private List<Monster> activeMonsters = new List<Monster>();

    public bool WaveOver { get; set; }

    public TowerBtn ClickedBtn 
    { 
        get; 
        set; 
    }

    public ObjectPool Pool { get; set; }

    private bool gameOver = false;

    [SerializeField]
    private GameObject gameOverUI;

    public int TotalMonsters { get; set; }
    public float GameSpeed 
    { 
        get => Time.timeScale; 
        set => Time.timeScale = value; 
    }

    private Tower selectedTower;

    private void Awake()
    {
        Pool = GetComponent<ObjectPool>();
    }

    void Start()
    {
        Lives = 10;
        Currency = 100;
        WaveOver = true;
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
            HideStats();
            DeselectTower();
            ClickedBtn = towerBtn;
            Hover.Instance.Activate(towerBtn.Sprite);
            wavePanel.SetActive(false);
            healthPanel.SetActive(false);
            currencyPanel.SetActive(false);
            towersBtn.SetActive(false);
            waveBtn.SetActive(false);
            upgradePanel.SetActive(false);
        }
    }

    public void BuyTower()
    {
        Currency -= ClickedBtn.Price;
        Hover.Instance.Deactivate();
        ClickedBtn = null;
        wavePanel.SetActive(true);
        healthPanel.SetActive(true);
        currencyPanel.SetActive(true);
        towersBtn.SetActive(true);
        waveBtn.SetActive(true);
    }

    public void OnCurrencyChanged()
    {
        if (Changed != null)
        {
            Changed();
        }
    }

    private void HandleEscape()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Hover.Instance.Deactivate();
            ClickedBtn = null;
            waveBtn.SetActive(true);
            wavePanel.SetActive(true);
            healthPanel.SetActive(true);
            currencyPanel.SetActive(true);
            towersBtn.SetActive(true);
        }
        if (Input.GetMouseButton(1))
        {
            Hover.Instance.Deactivate();
            ClickedBtn = null;
            waveBtn.SetActive(true);
            wavePanel.SetActive(true);
            healthPanel.SetActive(true);
            currencyPanel.SetActive(true);
            towersBtn.SetActive(true);
        }
    }

    public void SelectTower(Tower tower)
    {
        DeselectTower();
        selectedTower = tower;
        SetToolTipText(selectedTower);
        ShowStats();
        selectedTower.Select();
        sellText.text = "+" + Convert.ToInt32(tower.Price/2);
        if (WaveOver)
        {
            upgradePanel.SetActive(true);
        }
    }

    public void DeselectTower()
    {
        if (selectedTower != null)
        {
            //Calls select to deselect it
            selectedTower.Deselect();
        }
        HideStats();
        upgradePanel.SetActive(false);
        //Remove the reference to the tower
        selectedTower = null;
    }

    public void StartWave()
    {
        WaveOver = false;
        GameSpeed = 1f;
        wave++;
        waveText.text = string.Format("Wave: {0}", wave);
        StartCoroutine(SpawnWave());
        pauseBtn.SetActive(true);
        speedBtn.SetActive(true);
        waveBtn.SetActive(false);
        towersBtn.SetActive(false);
        upgradePanel.SetActive(false);
        TotalMonsters = wave*3;
    }

    private IEnumerator SpawnWave()
    {
        for (int i = 0; i < wave*3; i++)
        {
            LevelManager.Instance.GeneratePath();
            int monsterIndex = UnityEngine.Random.Range(0, 4);
            string type = string.Empty;

            switch (monsterIndex)
            {
                case 0:
                    type = "BlueMonster";
                    break;
                case 1:
                    type = "RedMonster";
                    break;
                case 2:
                    type = "GreenMonster";
                    break;
                case 3:
                    type = "PurpleMonster";
                    break;
                default:
                    break;
            }

            Monster monster = Pool.GetObject(type).GetComponent<Monster>();
            this.health = monster.Health;
            this.speed = monster.Speed;
            //Debug.Log("Wave: " + wave + ", type: " + type + ", health: " + health + ", speed: " + speed);
            if (wave % 2 == 0)
            {
                this.speed += speed * 0.1f;
                this.health += Convert.ToInt32(health * 0.5);
            }
            monster.Spawn(health, speed);
            activeMonsters.Add(monster);
            yield return new WaitForSeconds(1.5f);
        }
    }


    public void RemoveMonster(Monster monster)
    {
        
        activeMonsters.Remove(monster);
        TotalMonsters--;
        if (TotalMonsters <= 0) {
            EndWave();
        }
    }

    public void EndWave()
    {
        WaveOver = true;
        waveBtn.SetActive(true);
        speedBtn.SetActive(false);
        pauseBtn.SetActive(false);
        towersBtn.SetActive(true);
        GameSpeed = 1;
        gameAccelerated = false;
        this.speedBtn.GetComponent<Image>().sprite = speedBtnInactive;
        this.Currency += wave*5; 
    }

    public void GameOver()
    {
        if (!gameOver)
        {
            gameOver = true;
            waveBtn.SetActive(false);
            pauseBtn.SetActive(false);
            speedBtn.SetActive(false);
            towersBtn.SetActive(false);
            wavePanel.SetActive(false);
            healthPanel.SetActive(false);
            currencyPanel.SetActive(false);
            gameOverUI.SetActive(true);
            GameSpeed = 0;
        }
    }

    public void Restart()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void PauseGame()
    {
        if (GameSpeed > 0f)
        {
            gameAccelerated = false;
            GameSpeed = 0;
            this.pauseBtn.GetComponent<Image>().sprite = pauseBtnActive;
            this.speedBtn.GetComponent<Image>().sprite = speedBtnInactive;
        }
        else
        {
            GameSpeed = 1;  
            this.pauseBtn.GetComponent<Image>().sprite = pauseBtnInactive;
        }
    }

    public void SpeedGame()
    {
        if (GameSpeed <= 1 && !gameAccelerated)
        {
            this.speedBtn.GetComponent<Image>().sprite = speedBtnActive;
            this.pauseBtn.GetComponent<Image>().sprite = pauseBtnInactive;
            gameAccelerated = true;
            GameSpeed = gameSpeedAcc;
        } else
        {
            GameSpeed = 1;
            gameAccelerated = false;
            this.speedBtn.GetComponent<Image>().sprite = speedBtnInactive;
        }
    }

    public void SellTower()
    {
        if (selectedTower != null)
        {
            Currency += Convert.ToInt32(selectedTower.Price / 2);
            selectedTower.GetComponentInParent<TileScript>().IsEmpty = true;
            Destroy(selectedTower.transform.gameObject);
            DeselectTower();
        }
    }

    public void ShowStatsHover()
    {
        statsPanel.SetActive(!statsPanel.activeSelf);
    }

    public void ShowStats()
    {
        statsPanel.SetActive(true);
    }

    public void HideStats()
    {
        statsPanel.SetActive(false);
    }

    public void SetToolTipText(Tower selected)
    {
        GameObject child = statsPanel.transform.GetChild(0).gameObject;
        TowerRange values = selected.transform.Find("Range").GetComponent<TowerRange>();
        child.transform.Find("DamageTxt").GetComponent<Text>().text = values.Damage.ToString();
        child.transform.Find("AttackSpeedTxt").GetComponent<Text>().text = values.ProjectileSpeed.ToString();
        child.transform.Find("CooldownTxt").GetComponent<Text>().text = values.AttackCooldown.ToString();
        child.transform.Find("PriceTxt").GetComponent<Text>().text = selected.Price.ToString();
        child.transform.Find("ProjectileTxt").GetComponent<Text>().text = values.ProjectileType;
        child.transform.Find("NameLbl").GetComponent<Text>().text = selected.transform.GetComponent<Tower>().TurretName;
    }
}
