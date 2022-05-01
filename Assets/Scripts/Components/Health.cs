using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class Health : IECSComponent
{
    public double maxHealth;
    public double currentHealth;
    public bool isImmortal;

    public double getCurrentHealthPercent()
    {
        return (currentHealth / maxHealth) * 100;
    }
}
