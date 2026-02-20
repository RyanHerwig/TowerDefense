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
            EnemyEffect onFire = new(EnemyEffectName.Burning, false, 10f, attackDamage: parent.AttackDamage, 
                specialDamage: parent.SpecialDamage, trueDamage: parent.TrueDamage, effectRate: parent.FireRate);
            EnemyEffectData effectData = new(onFire, enemyManager.enemyTransformDict[other.transform]);
            gameManager.EnqueueAddEnemyEffects(effectData);
        }
    }
}