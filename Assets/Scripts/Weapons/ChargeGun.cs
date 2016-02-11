using UnityEngine;
using System.Collections.Generic;

public class ChargeGun : Weapon
{
    public Transform playerTransform;
    public float energyCostPerSecond;
    public float minEnergyCost;

    private GameObject currentShot;
    private bool charging = false;
    private float chargeStartTime;
    private float chargeDuration;

    private List<float> thresholds = new List<float> { .2f, .8f };
    private List<string> animationLevels = new List<string> { "SmallShotFired", "MediumShotFired", "LargeShotFired" };
    private List<int> damageLevels = new List<int> { 3, 5, 7 };
    private List<int> explosionDamageLevels = new List<int> { 1, 2, 3 };
    private List<float> explosionRadiusLevels = new List<float> { .6f, .8f, .9f };

    private Vector3 chargeShotRelativePosition = new Vector3(.18f, 1.082f, 0);

    public override float Click(Transform transform)
    {
        if (Player.PlayerEnergy.energy < minEnergyCost && !charging)
        {
            // Player can't fire.
            return 0;
        }

        if (Player.PlayerEnergy.energy <= 0 && currentShot)
        {
            charging = false;
            currentShot.GetComponent<Animator>().enabled = false;
            return 0;
        }

        chargeDuration = Time.time - chargeStartTime;

        if (!charging)
        {
            Player.PlayerEnergy.PauseRecharge();

            currentShot = StackPool.Pop();
            currentShot.transform.SetParent(playerTransform);
            currentShot.transform.localPosition = chargeShotRelativePosition;
            currentShot.transform.localRotation = Quaternion.Euler(0, 0, 90);

            charging = true;
            chargeStartTime = Time.time;

            currentShot.SetActive(true);
            // Do not show the sprite until the animation has begun, since the sprite may have been on the final frame of the explosion animation when it was recycled.
            currentShot.GetComponent<SpriteRenderer>().enabled = false;

            // If player fires before breaching the first threshold, the energy cost will simply be minEnergyCost. After breaching
            // the threshold, player will gradually start using energy to increase the charge size.
        }
        else if (chargeDuration >= thresholds[0] && chargeDuration < thresholds[1])
        {
            return Time.deltaTime * energyCostPerSecond;
        }

        return 0;
    }

    public override float Release(Transform transform)
    {
        float chargeDuration = Time.time - chargeStartTime;
        
        if (CanFire() && playerHasEnoughEnergy(chargeDuration) && currentShot)
        {
            Player.PlayerEnergy.UnpauseRecharge();

            nextFiringTime = Time.time + FiringDelay;
            ChargeBlastProperties chargeBlastProperties = currentShot.GetComponent<ChargeBlastProperties>();

            charging = false;
            int chargeLevel = getChargeLevel(chargeDuration);
            chargeBlastProperties.ChargeLevel = chargeLevel;
            chargeBlastProperties.Fired = true;
            chargeBlastProperties.Damage = damageLevels[chargeLevel];
            chargeBlastProperties.ExplosionDamage = explosionDamageLevels[chargeLevel];
            chargeBlastProperties.ExplosionRadius = explosionRadiusLevels[chargeLevel];

            currentShot.GetComponent<Animator>().enabled = true; // in case it was disabled due to player running out of energy
            currentShot.transform.SetParent(StackPool.transform);
            currentShot.GetComponent<Animator>().SetTrigger(getShotAnimationTrigger(chargeLevel));

            Vector2 direction = VectorUtil.DirectionToMousePointer(transform);

            currentShot.GetComponent<Rigidbody2D>().velocity = direction * ProjectileSpeed;
            currentShot = null;

            return chargeDuration >= thresholds[0] ? 0 : minEnergyCost;
        }

        return 0;
    }

    private string getShotAnimationTrigger(int chargeLevel)
    {
        return animationLevels[chargeLevel];
    }

    private int getChargeLevel(float chargeTime)
    {
        for (int x = 0; x < thresholds.Count; x++)
        {
            if (chargeTime < thresholds[x])
            {
                return x;
            }
        }

        return thresholds.Count;
    }

    private bool playerHasEnoughEnergy(float chargeDuration)
    {
        return chargeDuration >= thresholds[0] || Player.PlayerEnergy.energy > minEnergyCost;
    }

    public override float GetEnergyRequirement()
    {
        // This weird value basically means "always let the click logic run". Player energy can dip slightly below 0 when
        // charging (which is OK) but the click handler still must execute since it might have to pause the animation.
        return -5;
    }

    public override string GetName()
    {
        return "chargeGun";
    }
}
