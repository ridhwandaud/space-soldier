using UnityEngine;
using System.Collections;

public class Spore : MonoBehaviour {

	void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            Camera.main.GetComponent<SporeEffect>().ProcessSpore();
        }

        if (other.tag == "Player" || other.tag == "Wall" || other.tag == "Obstacle")
        {
            GetComponent<ProjectileDestroy>().Destroy();
        }
    }
}
