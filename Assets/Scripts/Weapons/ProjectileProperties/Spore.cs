using UnityEngine;
using System.Collections;

public class Spore : MonoBehaviour {
    [SerializeField]
    private int Damage;

	void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            Camera.main.GetComponent<SporeEffect>().ProcessSpore();
            other.GetComponent<PlayerHealth>().InflictDamage(Damage);
        }

        if (other.tag == "Player" || other.gameObject.layer == GameSettings.WallLayerNumber || other.tag == "Obstacle")
        {
            GetComponent<ProjectileDestroy>().Destroy();
        }
    }
}
