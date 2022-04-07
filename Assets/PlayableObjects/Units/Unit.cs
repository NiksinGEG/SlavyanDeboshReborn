using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum UnitType { trooper, melee, sniper, builder }
public struct UnitAttributes
{
    int team;
    public int Team { get; set; }

    int damage;
    public int Damage { get; set; }
    int cur_health;
    public int CurHealth { get; set; }
    int max_health;
    public int MaxHealth { get; set; }
    int rad_move;
    public int RadMove { get; set; }
    int rad_attack;
    public int RadAttack { get; set; }

    int max_act_num;
    public int MaxActNum { get; set;}
    int cur_act_num;
    public int CurActNum { get; set; }

    int fov;
    public int Fov { get; set; }

    UnitType unitType;
    public UnitType UnitType { get { return unitType; } set { } }
};

public class Unit : MonoBehaviour
{
    PrefabManager prefabManager;
    public UnitAttributes unitAttributes;
    public Unit(int team)
    {
        this.unitAttributes.Team = team;
    }


}

public class Trooper : Unit
{
    Trooper(int team) : base(team)
    {
        this.unitAttributes.UnitType = UnitType.trooper;

        this.unitAttributes.Fov = 4;
        this.unitAttributes.Damage = 5;
        
        this.unitAttributes.CurHealth = 20;
        this.unitAttributes.MaxHealth = 20;

        this.unitAttributes.RadMove = 2;
    }
}