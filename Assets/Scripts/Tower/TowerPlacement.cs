using System;
using UnityEngine;

public class TowerPlacement : MonoBehaviour
{
    #region Singleton
    private static TowerPlacement instance;

    public static TowerPlacement Instance
    {
        get
        {
            if (instance == null)
                instance = FindAnyObjectByType<TowerPlacement>();
            return instance;
        }
    }
    #endregion
    GameObject currentTower;
    [SerializeField] private int startingHealth;
    [NonSerialized] public int Health;

    [SerializeField] private int startingMoney;
    [NonSerialized] public float Money;
    [SerializeField] LayerMask placementLayers;
    [SerializeField] LayerMask towerCollide;
    [SerializeField] Camera cam;
    [SerializeField] Transform towerFolder;

    GameManager gameManager;
    bool canPlace;
    UIManager uIManager;

    void Start()
    {
        gameManager = GameManager.Instance;
        uIManager = UIManager.Instance;
        Health = startingHealth;
        Money = startingMoney;
        canPlace = true;
        uIManager.UpdateHealth(Health);
        uIManager.UpdateMoney(Money);
    }

    void Update()
    {
        if (currentTower != null && canPlace)
        {
            Ray cameraRay = cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hitInfo;

            if (Physics.Raycast(cameraRay, out hitInfo, 100f, towerCollide))
            {
                currentTower.transform.position = hitInfo.point;
            }

            //Cancels placing tower
            if (Input.GetKeyDown(KeyCode.Q))
            {
                CancelTower();
                return;
            }

            if (Input.GetMouseButtonDown(0) && hitInfo.collider.gameObject != null)
            {
                if (!hitInfo.collider.gameObject.CompareTag("Invalid Tower Placement"))
                {
                    BoxCollider collider = currentTower.gameObject.GetComponent<BoxCollider>();
                    collider.isTrigger = true;
                    Vector3 center = currentTower.gameObject.transform.position + collider.center;
                    Vector3 halfExtents = collider.size / 2;
                    if (!Physics.CheckBox(center, halfExtents, Quaternion.identity, placementLayers, QueryTriggerInteraction.Ignore))
                    {
                        gameManager.towersInGame.Add(currentTower.GetComponent<Tower>());
                        currentTower = null;
                        collider.isTrigger = false;
                    }
                }
            }
        }
    }

    public void PlaceTower(Tower tower)
    {
        float cost = tower.cost;
        if (Money >= cost)
        {
            currentTower = Instantiate(tower.gameObject, Vector3.zero, Quaternion.identity, towerFolder);
            UpdateMoney(-cost);
        }
    }

    public void UpdateMoney(float moneyToAdd)
    {
        Money += moneyToAdd;
        uIManager.UpdateMoney(Money);
    }

    public void UpdateHealth(int healthToAdd)
    {
        Health += healthToAdd;
        uIManager.UpdateHealth(Health);
    }

    public void CancelTower()
    {
        if (currentTower != null)
        {
            Destroy(currentTower);
            currentTower = null;
        }
    }

    //Cannot place tower when hovering a button
    public void OnMouseEnterAnyButton()
    {
        canPlace = false;
    }

    public void OnMouseExitAnyButton()
    {
        canPlace = true;
    }
}