using System;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] float maxHealth;
    [NonSerialized] public float health;
    public float speed;
    public int id;
    public void Init()
    {
        health = maxHealth;
    }
}