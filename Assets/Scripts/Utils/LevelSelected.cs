using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class LevelSelected
{
    private static int level = 0;

    public static int Level { get => level; set => level = value; }
}
