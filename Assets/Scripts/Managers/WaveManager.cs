using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WaveManager 
{
    private int waveNumber;

    public int WaveNumber { get => waveNumber; private set => waveNumber = value; }

    private int[] monsters;

    public int[] Monsters { get => monsters; private set => monsters = value; }
    public int MonsterCount { get => monsterCount; private set => monsterCount = value; }

    private int monsterCount;

    private enum MonsterRange
    {
        BlueMonster = 1,
        GreenMonster = 2,
        PurpleMonster = 3,
        RedMonster = 4
    }

    public WaveManager(int waveNumber)
    {
        this.WaveNumber = waveNumber;
        MonsterCount = (WaveNumber) + Convert.ToInt32(Math.Round(WaveNumber * 0.5d, MidpointRounding.AwayFromZero));
        this.Monsters = new int[monsterCount];
        GenerateWave();
    }

    private void GenerateWave()
    {
        Dictionary<int, int> monstersAvailable = new Dictionary<int, int>();
        if (WaveNumber <= 5)
        {
            for (int i = 0; i < MonsterCount; i++)
            {
                Monsters[i] = 1;
            }
        }
        else if (WaveNumber <= 6)
        {
            monstersAvailable.Add(1, Convert.ToInt32(Math.Round(MonsterCount * 0.9d, 0)));
            monstersAvailable.Add(2, Convert.ToInt32(Math.Round(MonsterCount * 0.1d, 0)));
            for (int i = 0; i < MonsterCount; i++)
            {
                if (i < monstersAvailable.ElementAt(0).Value)
                {
                    Monsters[i] = monstersAvailable.ElementAt(0).Key;
                }
                else
                {
                    Monsters[i] = monstersAvailable.ElementAt(1).Key;
                }
            }
        }
        else if (WaveNumber <= 7)
        {
            monstersAvailable.Add(1, Convert.ToInt32(Math.Round(MonsterCount * 0.9d, 0)));
            monstersAvailable.Add(3, Convert.ToInt32(Math.Round(MonsterCount * 0.1d, 0)));
            for (int i = 0; i < MonsterCount; i++)
            {
                if (i < monstersAvailable.ElementAt(0).Value)
                {
                    Monsters[i] = monstersAvailable.ElementAt(0).Key;
                }
                else
                {
                    Monsters[i] = monstersAvailable.ElementAt(1).Key;
                }
            }
        }
        else if (WaveNumber <= 8)
        {
            monstersAvailable.Add(1, Convert.ToInt32(Math.Round(MonsterCount * 0.9d, 0)));
            monstersAvailable.Add(4, Convert.ToInt32(Math.Round(MonsterCount * 0.1d, 0)));
            for (int i = 0; i < MonsterCount; i++)
            {
                if (i < monstersAvailable.ElementAt(0).Value)
                {
                    Monsters[i] = monstersAvailable.ElementAt(0).Key;
                }
                else
                {
                    Monsters[i] = monstersAvailable.ElementAt(1).Key;
                }
            }
        }
        else
        {
            monstersAvailable.Add(1, Convert.ToInt32(Math.Round(MonsterCount * 0.5d, 0, MidpointRounding.AwayFromZero)));
            monstersAvailable.Add(2, Convert.ToInt32(Math.Round(MonsterCount * 0.2d, 0, MidpointRounding.AwayFromZero)));
            monstersAvailable.Add(3, Convert.ToInt32(Math.Round(MonsterCount * 0.2d, 0, MidpointRounding.AwayFromZero)));
            monstersAvailable.Add(4, Convert.ToInt32(Math.Round(MonsterCount * 0.1d, 0, MidpointRounding.AwayFromZero)));
            //Debug.Log("Count: " + MonsterCount);
            System.Random r = new System.Random();
            for (int i = 0; i < MonsterCount; i++)
            {
                int selectedMonster = monstersAvailable.Keys.OrderBy(x => r.Next()).Take(1).FirstOrDefault();
                /*Debug.Log("Selected monster: " + selectedMonster);
                foreach (var obj in monstersAvailable)
                {
                    Debug.Log("wave: " + WaveNumber + ", key: " + obj.Key + ", value: " + obj.Value);
                }*/
                if (selectedMonster == 0)
                {
                    Monsters[i] = 1;
                }
                else
                {
                    Monsters[i] = selectedMonster;
                    monstersAvailable[selectedMonster] -= 1;
                    if (monstersAvailable[selectedMonster] == 0)
                    {
                        monstersAvailable.Remove(selectedMonster);
                    }
                }
            }
        }
    }


    public string GetMonster(int monsterIndex)
    {
        return CastMonster(Monsters[monsterIndex]);
    }

    public string CastMonster(int monsterId)
    {
        return ((MonsterRange) monsterId).ToString();
    }

}

