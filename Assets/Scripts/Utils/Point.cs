using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Point
{
    public int Y { get; set; }
    public int X { get; set; }

    public Point(int x, int y)
    {
        this.X = x;
        this.Y = y;
    }

    public static bool operator ==(Point a, Point b)
    {
        return a.X == b.X && a.Y == b.Y;
    }
    public static bool operator !=(Point a, Point b)
    {
        return a.X != b.X || a.Y != b.Y;
    }
}
