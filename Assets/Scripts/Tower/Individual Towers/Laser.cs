using UnityEngine;

public class Laser : TowerDamage
{
    [SerializeField] Transform laserPivot;
    [SerializeField] LineRenderer laser;
    public override void DamageTick(Enemy target)
    {
        // If target exists, do following
        if (target)
        {
            //Set start of laser position to pivot
            laser.SetPosition(0, laserPivot.position);

            //Set end of laser to targetted enemy
            laser.SetPosition(1, target.body.position);

            //Show laser
            laser.enabled = true;
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
        else if (laser.enabled)
            laser.enabled = false; //Hides laser if there is no enemy
    }
}