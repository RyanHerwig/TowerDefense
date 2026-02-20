using UnityEngine;

public class Turret : TowerDamage
{
    public override void DamageTick(Enemy target)
    {
        // If target exists, do following
        if (target)
        {
            // If attack is on cooldown, decrease cooldown
            if (delay > 0f)
            {
                delay -= Time.deltaTime;
                return;
            }

            // If attack is off cooldown, attack and put attack on cooldown again.
            gameManager.EnqueueDamageData(new DamageData(target, AttackDamage, SpecialDamage, TrueDamage, target.Defense, target.Resistance));

            delay = 1 / FireRate;
        }
    }
}