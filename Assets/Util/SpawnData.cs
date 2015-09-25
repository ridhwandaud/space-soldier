using UnityEngine;
using System.Collections;
using System;

public class SpawnData {

    public int min;
    public int max;
    public GameObject prefab;
    public bool isEnemy;

    public SpawnData(int min, int max, GameObject prefab, bool isEnemy = true)
    {
        this.min = min;
        this.max = max;
        this.prefab = prefab;
        this.isEnemy = isEnemy;
    }
}
