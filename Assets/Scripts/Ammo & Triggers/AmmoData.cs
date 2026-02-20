using UnityEngine;

[CreateAssetMenu(fileName ="New Ammo Data", menuName = "Create Ammo Data")]
public class AmmoData : ScriptableObject
{
    public GameObject AmmoPrefab;
    public AmmoType AmmoId;
}

public enum AmmoType
{
    CannonBall = 1
}