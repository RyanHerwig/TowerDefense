using UnityEngine;

public class FireTriggerCollisionDetector : MonoBehaviour
{
    [SerializeField] private Flamethrower parent;

    GameManager gameManager;
    private EnemyManager enemyManager;
    private void Start()
    {
        gameManager = GameManager.Instance;
        enemyManager = EnemyManager.Instance;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            Enemy enemy = enemyManager.enemyTransformDict[other.transform];
            BurnEffect burn = new(parent.SpecialDamage, parent.Duration, parent.FireRate);
            EnemyEffectData effectData = new(burn, enemy);
            gameManager.EnqueueAddEnemyEffects(effectData);

            ElementEffect fire = new(ElementName.Fire, 3f);
            ElementEffectData elementData = new(fire, enemy, parent);
            gameManager.EnqueueAddElement(elementData);
        }
    }
}