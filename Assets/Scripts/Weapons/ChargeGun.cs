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
    private float playerEnergyAtChargeStart;
    private float energyCostSoFar;

    private List<float> thresholds = new List<float> { .2f, .8f };
    private List<string> animationLevels = new List<string> { "SmallShotFired", "MediumShotFired", "LargeShotFired" };

    private Vector3 chargeShotRelativePosition = new Vector3(.18f, 1.082f, 0);

    public override float Click(Transform transform)
    {
        float chargeDuration = Time.time - chargeStartTime;
        if (!charging)
        {
            Player.PlayerEnergy.PauseRecharge();

            currentShot = stackPool.Pop();
            currentShot.transform.SetParent(playerTransform);
            currentShot.transform.localPosition = chargeShotRelativePosition;
            currentShot.transform.localRotation = Quaternion.Euler(0, 0, 90);

            charging = true;
            chargeStartTime = Time.time;
            playerEnergyAtChargeStart = Player.PlayerEnergy.energy;
            energyCostSoFar = 0;

            currentShot.SetActive(true);
            // Do not show the sprite until the animation has begun, since the sprite may have been on the final frame of the explosion animation when it was recycled.
            currentShot.GetComponent<SpriteRenderer>().enabled = false;
        }
        else if (chargeDuration >= thresholds[0] && chargeDuration < thresholds[1])
        {
            energyCostSoFar = (chargeDuration - thresholds[0]) * energyCostPerSecond;
            return energyCostSoFar - (playerEnergyAtChargeStart - Player.PlayerEnergy.energy);
        }

        return 0;
    }

    public override float Release(Transform transform)
    {
        if (CanFire())
        {
            Player.PlayerEnergy.UnpauseRecharge();

            nextFiringTime = Time.time + firingDelay;
            ChargeBlastProperties chargeBlastProperties = currentShot.GetComponent<ChargeBlastProperties>();

            charging = false;
            float chargeDuration = Time.time - chargeStartTime;
            int chargeLevel = getChargeLevel(chargeDuration);
            chargeBlastProperties.ChargeLevel = chargeLevel;
            chargeBlastProperties.Fired = true;

            currentShot.transform.SetParent(stackPool.transform);
            currentShot.GetComponent<Animator>().SetTrigger(getShotAnimationTrigger(chargeLevel));

            Vector2 direction = VectorUtil.DirectionToMousePointer(transform);

            currentShot.GetComponent<Rigidbody2D>().velocity = direction * projectileSpeed;

            return energyCostSoFar > minEnergyCost ? 0 : minEnergyCost - energyCostSoFar;
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

    public override float GetEnergyRequirement()
    {
        return minEnergyCost;
    }

    public override string GetName()
    {
        return "chargeGun";
    }
}
