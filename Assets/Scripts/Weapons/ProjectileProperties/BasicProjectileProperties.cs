using UnityEngine;
using System.Collections;

// Basic projectile has no effect other than damage.
public class BasicProjectileProperties : ProjectileProperties
{
	public override void ImpactEffect () {
	    // Do nothing.
	}
}
