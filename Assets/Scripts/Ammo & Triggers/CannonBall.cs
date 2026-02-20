using System.Collections;
using UnityEngine;

public class CannonBall : Ammo
{
    ParticleSystem Megumin;
    float explosionRadius;

    public override IEnumerator Fire()
    {
        Megumin = ((Cannon) towerOrigin).Megumin;
        explosionRadius = ((Cannon)towerOrigin).ExplosionRadius;
        while (timer <= 2)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetTransform.position, Speed);
            timer += Time.deltaTime;
            yield return new WaitForFixedUpdate();
        }
        timer = 0;

        ammoManager.RemoveAmmo(this);
        yield return null;
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            Megumin.transform.position = transform.position;
            Megumin.transform.localScale = new Vector3(explosionRadius, explosionRadius, explosionRadius);
            Megumin.Play();

            Collider[] enemiesInRadius = Physics.OverlapSphere(transform.position, explosionRadius, EffectLayer);

            int enemiesInRadiusCount = enemiesInRadius.Length;
            for (int j = 0; j < enemiesInRadiusCount; j++)
            {
                Enemy enemyToDamage = enemyManager.enemyTransformDict[enemiesInRadius[j].transform];
                DamageData damageToApply = new DamageData(enemyToDamage, AttackDamage, SpecialDamage, TrueDamage, target.Defense, target.Resistance);
                gameManager.EnqueueDamageData(damageToApply);
            }

            ammoManager.RemoveAmmo(this);
        }
    }
}