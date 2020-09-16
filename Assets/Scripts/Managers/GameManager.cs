using System;
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

    [SerializeField]
    private Text waveText, sellText, upgradeText, currencyText, livesText;

    [SerializeField]
    private GameObject menuBtn, speedBtn, pauseBtn, towersBtn, waveBtn, wavePanel, currencyPanel, healthPanel, upgradePanel, statsPanel, pauseMenuPanel, gameOverUI, optionsMenuPanel;

    private List<Monster> activeMonsters = new List<Monster>();

    private Tower selectedTower;

    private float speed;

    private int wave = 0, lives, health, currency;

    private bool gameAccelerated, gameOver = false;

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

    public bool WaveOver { get; set; }

    public TowerBtn ClickedBtn 
    { 
        get; 
        set; 
    }

    public ObjectPool Pool { get; set; }

    public int TotalMonsters { get; set; }
    public float GameSpeed 
    { 
        get => Time.timeScale; 
        set => Time.timeScale = value; 
    }
    public Tower SelectedTower { get => selectedTower; set => selectedTower = value; }

    public int MonstersKilled { get; set; }

    private void Awake()
    {
        Pool = GetComponent<ObjectPool>();
    }

    void Start()
    {
        upgradePanel.SetActive(false);
        MonstersKilled = 0;
        Lives = 10;
        Currency = 100;
        WaveOver = true;
    }

    // Update is called once per frame
    void Update()
    {
        HandleEscape();
    }

    public void SelectTower(Tower tower)
    {
        DeselectTower();
        SelectedTower = tower;
        SetToolTipText(SelectedTower);
        
        SelectedTower.Select();
        sellText.text = Convert.ToInt32(tower.Price / 2).ToString();
        if (SelectedTower.NextLevelTower.gameObject == null)
        {
            upgradePanel.transform.Find("UpgradeBtn").gameObject.SetActive(false);
        }
        else
        {
            if (Currency < tower.Price)
            {
                upgradePanel.transform.Find("UpgradeBtn").GetComponent<Image>().color = Color.grey;
                upgradePanel.transform.Find("UpgradeBtn").transform.Find("UpgradeTxt").GetComponent<Text>().color = Color.grey;
            }
            else
            {
                upgradePanel.transform.Find("UpgradeBtn").GetComponent<Image>().color = Color.white;
                upgradePanel.transform.Find("UpgradeBtn").transform.Find("UpgradeTxt").GetComponent<Text>().color = Color.white;
            }
            upgradePanel.transform.Find("UpgradeBtn").gameObject.SetActive(true);
            upgradeText.text = SelectedTower.NextLevelTower.GetComponent<Tower>().Price.ToString();
        }

        if (WaveOver)
        {
            ShowStats();
            upgradePanel.SetActive(true);
        }
    }

    public void DeselectTower()
    {
        if (SelectedTower != null)
        {
            //Calls select to deselect it
            SelectedTower.Deselect();
        }
        HideStats();
        upgradePanel.SetActive(false);
        //Remove the reference to the tower
        SelectedTower = null;
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
            menuBtn.SetActive(false);
        }
    }

    public void BuyTower(int price)
    {
        Currency -= price;
        Hover.Instance.Deactivate();
        ClickedBtn = null;
        wavePanel.SetActive(true);
        healthPanel.SetActive(true);
        currencyPanel.SetActive(true);
        towersBtn.SetActive(true);
        waveBtn.SetActive(true);
        menuBtn.SetActive(true);
    }

    public void UpgradeTower()
    {
        if (SelectedTower != null)
        {
            if (Currency >= SelectedTower.Price)
            {
                SelectedTower.GetComponentInParent<TileScript>().UpgradeTower();
            }
        }
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
            if (ClickedBtn != null)
            {
                Hover.Instance.Deactivate();
                ClickedBtn = null;
                waveBtn.SetActive(true);
                wavePanel.SetActive(true);
                healthPanel.SetActive(true);
                currencyPanel.SetActive(true);
                towersBtn.SetActive(true);
            } else
            {
                PauseMenu();
            }
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
        HideStats();
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
            gameOverUI.transform.Find("WaveTxt").GetComponent<Text>().text = wave.ToString();
            gameOverUI.transform.Find("MonstersKilledTxt").GetComponent<Text>().text = MonstersKilled.ToString();
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
        if (SelectedTower != null)
        {
            Currency += Convert.ToInt32(SelectedTower.Price / 2);
            SelectedTower.GetComponentInParent<TileScript>().IsEmpty = true;
            Destroy(SelectedTower.transform.gameObject);
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

    public void ShowUpgradedStatsHover()
    {
        SetToolTipText(SelectedTower.NextLevelTower.GetComponent<Tower>());
    }

    public void HideUpgradedStatsHover()
    {
        SetToolTipText(SelectedTower);
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

    public void PauseMenu()
    {
        if (!pauseMenuPanel.gameObject.activeSelf)
        {
            GameSpeed = 0;
            pauseMenuPanel.SetActive(true);
            menuBtn.SetActive(false);
            waveBtn.SetActive(false);
            pauseBtn.SetActive(false);
            speedBtn.SetActive(false);
            towersBtn.SetActive(false);
        } 
        else {
            GameSpeed = 1;
            menuBtn.SetActive(true);
            pauseMenuPanel.SetActive(false);
            if (WaveOver)
            {
                waveBtn.SetActive(true);
                towersBtn.SetActive(true);
            }
            else
            {
                pauseBtn.SetActive(true);
                speedBtn.SetActive(true);
                this.pauseBtn.GetComponent<Image>().sprite = pauseBtnInactive;
                this.speedBtn.GetComponent<Image>().sprite = speedBtnInactive;
            }
        }
    }

    public void ShowOptions()
    {

        optionsMenuPanel.SetActive(!optionsMenuPanel.activeSelf);

    }

}
