using UnityEngine;

[CreateAssetMenu(fileName ="New Enemy Data", menuName = "Create Enemy Data")]
public class EnemyData : ScriptableObject
{
    public GameObject EnemyPrefab;
    public int EnemyID;
}
