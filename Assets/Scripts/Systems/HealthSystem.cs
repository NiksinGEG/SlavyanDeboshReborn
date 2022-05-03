using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthSystem : IECSSystem
{
    public HealthSystem(ECSService service) : base(service) { }

    public override void Run()
    {
        
    }

    public void DoDamage(Health health, double damage)
    {
        if (!health.isImmortal)
        {
            health.currentHealth -= damage;
            if (health.currentHealth <= 0)
            {
                // чел умирает 
            }
        }
        else
        {
            return;
        }
    }

    public void Heal(Health health,double hpToHeal)
    {
        health.currentHealth += hpToHeal;

        if (health.currentHealth > health.maxHealth)
        {
            health.currentHealth = health.maxHealth;
        }
    }

    public void MakeImmortal(Health health)
    {
        health.isImmortal = true;
    }
}
