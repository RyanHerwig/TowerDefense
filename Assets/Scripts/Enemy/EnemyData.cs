using UnityEngine;

[CreateAssetMenu(fileName ="New Enemy Data", menuName = "Create Enemy Data")]
public class EnemyData : ScriptableObject
{
    public GameObject EnemyPrefab;
    public EnemyType EnemyID;
}

public enum EnemyType
{
    Basic = 1,
    Fast = 2,
    Slow = 3,
    NullPhysical = 4,
    NullPhysicalBuffer = 5,
    NullSpecial = 6,
    NullSpecialBuffer = 7
}