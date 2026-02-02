using System;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] float maxHealth;
    [NonSerialized] public float health;
    [NonSerialized] public int nodeIndex;
    public float speed;
    public int id;
    public void Init()
    {
        health = maxHealth;
        nodeIndex = 0;
        transform.position = GameManager.Instance.enemySpawnNode;
    }
}