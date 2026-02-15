using System;
using System.Collections;
using UnityEngine;

public abstract class Ammo : MonoBehaviour
{
    public float Speed;
    [SerializeField] protected LayerMask EffectLayer;

    [NonSerialized] public float AttackDamage;
    [NonSerialized] public float SpecialDamage;
    [NonSerialized] public float TrueDamage;
    [NonSerialized] public AmmoType Id;

    protected GameManager gameManager;
    protected EnemyManager enemyManager;
    protected AmmoManager ammoManager;

    protected Enemy target;
    protected Transform targetTransform;
    protected float timer;

    protected TowerDamage towerOrigin;

    public virtual void Init(TowerDamage source, Enemy enemy, Transform spawnPoint)
    {
        gameObject.SetActive(true);
        towerOrigin = source;
        AttackDamage = source.attackDamage;
        SpecialDamage = source.specialDamage;
        TrueDamage = source.trueDamage;
        transform.position = spawnPoint.position;

        gameManager = GameManager.Instance;
        enemyManager = EnemyManager.Instance;
        ammoManager = AmmoManager.Instance;
        target = enemy;
        targetTransform = enemy.transform;
        timer = 0;

        StartCoroutine(Fire());
    }

    public abstract IEnumerator Fire();
}