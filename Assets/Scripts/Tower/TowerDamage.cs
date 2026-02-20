using System;
using UnityEngine;

public abstract class TowerDamage : MonoBehaviour
{
    protected GameManager gameManager;
    [NonSerialized] public float AttackDamage;
    [NonSerialized] public float SpecialDamage;
    [NonSerialized] public float TrueDamage;
    [NonSerialized] public float FireRate;
    protected float delay;
    protected void Start()
    {
        gameManager = GameManager.Instance;
    }

    public virtual void Init(float attackDamage, float specialDamage, float trueDamage, float fireRate, float startingDelay)
    {
        this.AttackDamage = attackDamage;
        this.SpecialDamage = specialDamage;
        this.TrueDamage = trueDamage;
        this.FireRate = fireRate;
        delay = startingDelay;
    }
    public abstract void DamageTick(Enemy target);
}