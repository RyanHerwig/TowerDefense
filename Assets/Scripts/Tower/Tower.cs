using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class Tower : MonoBehaviour, IPointerClickHandler
{
    [Header("Damage Stats")]
    [Tooltip("Physical Damage Dealt per Attack")]
    public float AttackDamage;
    [Tooltip("Special Damage Dealt Per Attack")]
    public float SpecialDamage;

    [Tooltip("True Damage Dealt per Attack")]
    public float TrueDamage;

    [Header("Attack Speed Stats")]
    [Tooltip("The amount of times tower fires per seond")]
    public float FireRate;

    [Tooltip("The time it takes for the tower to start attacking after being placed")]
    public float PlacementDelay;

    [Header("Range Stats")]
    [Tooltip("The minimum range an enemy can be to be targetted")]
    public float MinRange;
    [Tooltip("The Maximum range an enemy can be to be targetted")]
    public float MaxRange;

    [Header("Tower Functionality")]
    [Tooltip("The layer that this tower affects")]
    public LayerMask targetLayer;

    [Tooltip("The piece that rotates towards its target")]
    [SerializeField] Transform towerPivot;

    [NonSerialized] public Enemy Target;

    [NonSerialized] public TargetPriority currentPriority;
    private float delay;

    TowerDamage towerDamage;
    TowerRangeMesh mesh;

    private bool clickPlace;
    void Start()
    {
        delay = 1 / FireRate;
        currentPriority = TargetPriority.First;
        towerDamage = GetComponent<TowerDamage>();
        mesh = GetComponentInChildren<TowerRangeMesh>();
        towerDamage.Init(AttackDamage, SpecialDamage, TrueDamage, FireRate, PlacementDelay);
        mesh.Init();

        clickPlace = false;
    }

    public void Tick()
    {
        // Ticks Tower Damage
        towerDamage.DamageTick(Target);

        if (Target)
        {
            // Calculate the direction to the target
            Vector3 direction = Target.transform.position - transform.position;

            // Set the y component to 0 to ignore vertical differences
            direction.y = 0;

            // Calculate the rotation
            //if (direction != Vector3.zero) // Ensure direction is not zero to avoid errors
            //{
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            towerPivot.transform.rotation = Quaternion.Euler(0, targetRotation.eulerAngles.y, 0);
            //}
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.white;

        if (Target != null)
            Gizmos.DrawWireCube(Target.transform.position, new Vector3(1, 1, 1));
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!clickPlace)
        {
            mesh.ShowTowerRange();
            clickPlace = true;
        }
        else
            mesh.ToggleTowerRange();
    }
}