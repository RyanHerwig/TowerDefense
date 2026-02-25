using System;
using UnityEngine;

public class Flamethrower : TowerDamage
{
    public float Duration;
    [SerializeField] private Collider fireTrigger;
    [SerializeField] private ParticleSystem fireEffect;
    [NonSerialized] public float damage;
    public override void Init(float attackDamage, float specialDamage, float trueDamage, float fireRate, float startingDelay)
    {
        AttackDamage = attackDamage;
        SpecialDamage = specialDamage;
        TrueDamage = trueDamage;
        FireRate = fireRate;
        delay = startingDelay;
    }

    public override void DamageTick(Enemy target)
    {
        fireTrigger.enabled = target != null;

        if(target)
        {
            if (!fireEffect.isPlaying) fireEffect.Play();
            return;
        }

        fireEffect.Stop();
    }
}