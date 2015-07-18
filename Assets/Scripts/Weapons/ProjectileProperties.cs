using UnityEngine;
using System.Collections;

public abstract class ProjectileProperties : MonoBehaviour {
    public int Damage;

    // Impact effect = explosions or other events that should take place on impact.
    public abstract void ImpactEffect();
}
