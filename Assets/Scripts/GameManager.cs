using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : Singleton<GameManager>
{

    

    private int wave = 0;


    private int lives;

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
        }
    }

    [SerializeField]
    private Text waveText;

    [SerializeField]
    private GameObject waveBtn;

    [SerializeField]
    private GameObject towersBtn;

    [SerializeField]
    private GameObject speedBtn;

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

    private void Awake()
    {
        Pool = GetComponent<ObjectPool>();
    }

    void Start()
    {
        Lives = 10;
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

    public void StartWave()
    {
        wave++;
        waveText.text = string.Format("Wave: {0}", wave);
        StartCoroutine(SpawnWave());
        waveBtn.SetActive(false);
        towersBtn.SetActive(false);
        speedBtn.SetActive(true);
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
            monster.Spawn();
            activeMonsters.Add(monster);
            yield return new WaitForSeconds(1.5f);
        }
    }


    public void RemoveMonster(Monster monster)
    {
        
        activeMonsters.Remove(monster);
        TotalMonsters--;
        if (activeMonsters.Count <= 0 && TotalMonsters <= 0) {
            EndWave();
        }
    }

    public void EndWave()
    {
        waveBtn.SetActive(true);
        towersBtn.SetActive(true);
        speedBtn.SetActive(false);
    }

    public void GameOver()
    {
        if (!gameOver)
        {
            gameOver = true;
            waveBtn.SetActive(false);
            gameOverUI.SetActive(true);
            Time.timeScale = 0;
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

    public void SpeedGame()
    {
        if (Time.timeScale == 1)
        {
            Time.timeScale = 3;
        } else
        {
            Time.timeScale = 1;
        }
        
    }
}
