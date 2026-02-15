using System;
using UnityEngine;
using UnityEngine.Serialization;

public class Enemy : MonoBehaviour
{
    [Header("Basic Stats")]

    [Tooltip("Total amount of starting HP the enemy has")]
    [SerializeField] float maxHealth;

    [Tooltip("How fast the enemy moves")]
    public float Speed;

    [Header("Resistances")]
    [Tooltip("How Effective Physical Damage is")]
    public float Defense;

    [Tooltip("How Effective Special Damage is")]
    public float Resistance;

    [Header("Immunities")]
    [Tooltip("Check if enemy takes 0 damage from physical attacks")]
    public bool ImmuneToPhysical;

    [Tooltip("Check if enemy takes 0 damage from special attacks")]
    public bool ImmuneToSpecial;

    [NonSerialized] public bool isAlive;
    [NonSerialized] public float Health;
    [NonSerialized] public int NodeIndex;
    [NonSerialized] public EnemyType Id;
    [NonSerialized] public Transform body;
    public void Init()
    {
        Health = maxHealth;
        NodeIndex = 0;
        transform.position = GameManager.Instance.enemySpawnNode;
        body = transform;
        isAlive = true;
    }
}