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
    private GameObject menuBtn, speedBtn, pauseBtn, towersBtn, waveBtn, 
        wavePanel, currencyPanel, healthPanel, upgradePanel, statsPanel, 
        pauseMenuPanel, gameOverUI, optionsMenuPanel, sellMenu, exitMenu, 
        restartMenu, victoryPanel, canvasUi;

    private List<Monster> activeMonsters = new List<Monster>();

    private Tower selectedTower;

    private WaveManager waveManager;

    private float speed;

    private int wave, lives, health, currency;

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
        set
        {
            Time.timeScale = value;
            if (value == 0)
            {
                this.pauseBtn.GetComponent<Image>().sprite = pauseBtnActive;
                this.speedBtn.GetComponent<Image>().sprite = speedBtnInactive;
            }
            else
            {
                if (value == 1)
                {
                    this.pauseBtn.GetComponent<Image>().sprite = pauseBtnInactive;
                    this.speedBtn.GetComponent<Image>().sprite = speedBtnInactive;
                }
                else
                {
                    this.speedBtn.GetComponent<Image>().sprite = speedBtnActive;
                }
            }
        }
    }
    public Tower SelectedTower { get => selectedTower; set => selectedTower = value; }

    public int MonstersKilled { get; set; }

    private void Awake()
    {
        Pool = GetComponent<ObjectPool>();
    }

    void Start()
    {
        FirstTimePlaying();
        upgradePanel.SetActive(false);
        MonstersKilled = 0;
        wave = 1;
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
            if (Currency < SelectedTower.NextLevelTower.GetComponent<Tower>().Price)
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
            SoundManager.Instance.PlayEffect("menubutton");
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
        SoundManager.Instance.PlayEffect("tower_emplacement");
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
            if (Currency >= SelectedTower.NextLevelTower.GetComponent<Tower>().Price)
            {
                SoundManager.Instance.PlayEffect("menubutton");
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
            }
            else if (canvasUi.transform.Find("Lore").gameObject.activeSelf)
            {
                FirstTimePlaying();
            }
            else
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
        SoundManager.Instance.PlayEffect("menubutton");
        WaveOver = false;
        GameSpeed = 1f;
        wave++;
        waveText.text = string.Format("Onda: {0}/40", wave);
        StartCoroutine(SpawnWave());
        pauseBtn.SetActive(true);
        speedBtn.SetActive(true);
        waveBtn.SetActive(false);
        towersBtn.SetActive(false);
        upgradePanel.SetActive(false);
        TotalMonsters = waveManager.MonsterCount;
        HideStats();
    }

    private IEnumerator SpawnWave()
    {
        waveManager = new WaveManager(wave);
        for (int i = 0; i < waveManager.MonsterCount; i++)
        {
            LevelManager.Instance.GeneratePath();
            Monster monster = Pool.GetObject(waveManager.GetMonster(i)).GetComponent<Monster>();
            this.health = monster.Health;
            this.speed = monster.Speed;
            if (wave % 3 == 0)
            {
                //this.speed += speed * 0.1f;
                this.health += Convert.ToInt32(health * 0.1f);
            }
            monster.Spawn(health, speed);
            activeMonsters.Add(monster);
            yield return new WaitForSeconds(UnityEngine.Random.Range(1f, 2f));
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
        if (wave == 40)
        {
            GameWon();
        }
        WaveOver = true;
        waveBtn.SetActive(true);
        speedBtn.SetActive(false);
        pauseBtn.SetActive(false);
        towersBtn.SetActive(true);
        GameSpeed = 1;
        gameAccelerated = false;
        //this.Currency += wave*5;
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

    public void GameWon()
    {
            SoundManager.Instance.PlayEffect("victory");
            waveBtn.SetActive(false);
            pauseBtn.SetActive(false);
            speedBtn.SetActive(false);
            towersBtn.SetActive(false);
            victoryPanel.transform.Find("WaveTxt").GetComponent<Text>().text = wave.ToString();
            victoryPanel.transform.Find("MonstersKilledTxt").GetComponent<Text>().text = MonstersKilled.ToString();
            victoryPanel.SetActive(true);
            GameSpeed = 0;
    }

    public void ContinuePlaying()
    {
        SoundManager.Instance.PlayEffect("menubutton");
        waveBtn.SetActive(true);
        pauseBtn.SetActive(true);
        speedBtn.SetActive(true);
        towersBtn.SetActive(true);
        victoryPanel.SetActive(false);
        GameSpeed = 1;
    }

    public void Restart()
    {
        SoundManager.Instance.PlayEffect("menubutton");
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        RestartWindow();
    }

    public void RestartWindow()
    {
        SoundManager.Instance.PlayEffect("menubutton");
        restartMenu.SetActive(!restartMenu.activeSelf);
    }

    public void QuitWindow()
    {
        SoundManager.Instance.PlayEffect("menubutton");
        exitMenu.SetActive(!exitMenu.activeSelf);
    }

    public void QuitGame()
    {
        SoundManager.Instance.PlayEffect("menubutton");
        Time.timeScale = 1;
        SceneManager.LoadScene(0);
    }

    public void PauseGame()
    {
        SoundManager.Instance.PlayEffect("menubutton");
        if (GameSpeed > 0f)
        {
            gameAccelerated = false;
            GameSpeed = 0;
        }
        else
        {
            GameSpeed = 1;  
        }
    }

    public void SpeedGame()
    {
        SoundManager.Instance.PlayEffect("menubutton");
        if (GameSpeed <= 1 && !gameAccelerated)
        {
            gameAccelerated = true;
            GameSpeed = gameSpeedAcc;
        } else
        {
            GameSpeed = 1;
            gameAccelerated = false;
        }
    }

    public void SellWindow()
    {
        SoundManager.Instance.PlayEffect("menubutton");
        sellMenu.SetActive(!sellMenu.activeSelf);
    }

    public void SellTower()
    {
        SoundManager.Instance.PlayEffect("menubutton");
        if (SelectedTower != null)
        {
            Currency += Convert.ToInt32(SelectedTower.Price / 2);
            SelectedTower.GetComponentInParent<TileScript>().IsEmpty = true;
            Destroy(SelectedTower.transform.gameObject);
            DeselectTower();
            SellWindow();
        }
    }

    public void CancelSelling()
    {
        SoundManager.Instance.PlayEffect("menubutton");
        sellMenu.SetActive(!sellMenu.activeSelf);
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
        SoundManager.Instance.PlayEffect("menubutton");
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
            }
        }
    }

    public void ShowOptions()
    {
        SoundManager.Instance.PlayEffect("menubutton");
        optionsMenuPanel.SetActive(!optionsMenuPanel.activeSelf);
    }

    public void FirstTimePlaying()
    {
        canvasUi.transform.Find("Lore").gameObject.SetActive(!canvasUi.transform.Find("Lore").gameObject.activeSelf);
        wavePanel.SetActive(!wavePanel.activeSelf);
        healthPanel.SetActive(!healthPanel.activeSelf);
        currencyPanel.SetActive(!currencyPanel.activeSelf);
        towersBtn.SetActive(!towersBtn.activeSelf);
        waveBtn.SetActive(!waveBtn.activeSelf);
        menuBtn.SetActive(!menuBtn.activeSelf);
        currencyPanel.SetActive(!currencyPanel.activeSelf);
        wavePanel.SetActive(!wavePanel.activeSelf);
    }

}
