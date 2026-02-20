using System;
using UnityEngine;

public class Cannon : TowerDamage
{
    AmmoManager ammoManager;

    public ParticleSystem Megumin;
    [SerializeField] float baseExplosionRadius;
    [SerializeField] GameObject ball;
    [SerializeField] Transform cannonBallSpawnLocation;
    [NonSerialized] public float ExplosionRadius;

    public override void Init(float attackDamage, float specialDamage, float trueDamage, float fireRate, float startingDelay)
    {
        ammoManager = AmmoManager.Instance;
        AttackDamage = attackDamage;
        SpecialDamage = specialDamage;
        TrueDamage = trueDamage;
        FireRate = fireRate;
        delay = startingDelay;

        ExplosionRadius = baseExplosionRadius;
    }

    public override void DamageTick(Enemy target)
    {
        // If target exists, do following
        if (target)
        {
            // If attack is on cooldown, decrease cooldown
            if (delay > 0f)
            {
                delay -= Time.deltaTime;
                return;
            }

            // If attack is off cooldown, attack and put attack on cooldown again.
            ammoManager.SpawnAmmo(AmmoType.CannonBall, this, target, cannonBallSpawnLocation);

            delay = 1 / FireRate;
        }
    }
}