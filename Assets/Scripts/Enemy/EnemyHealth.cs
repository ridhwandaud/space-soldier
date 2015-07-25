using UnityEngine;
using System.Collections;

public class EnemyHealth : MonoBehaviour {

    public int health = 20;

    public void InflictDamage(int damagePoints)
    {
        health -= damagePoints;

        if (health <= 0)
        {
            Destroy(gameObject);
            Application.LoadLevel(Application.loadedLevel);
        }
    }
}
