using System;
using UnityEngine;

public abstract class TowerDamage : MonoBehaviour
{
    protected GameManager gameManager;
    [NonSerialized] public float attackDamage;
    [NonSerialized] public float specialDamage;
    [NonSerialized] public float trueDamage;
    protected float fireRate;
    protected float delay;
    protected void Start()
    {
        gameManager = GameManager.Instance;
    }

    public virtual void Init(float attackDamage, float specialDamage, float trueDamage, float fireRate, float startingDelay)
    {
        this.attackDamage = attackDamage;
        this.specialDamage = specialDamage;
        this.trueDamage = trueDamage;
        this.fireRate = fireRate;
        delay = startingDelay;
    }
    public abstract void DamageTick(Enemy target);
}