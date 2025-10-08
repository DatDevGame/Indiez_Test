using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CustomInspectorName("WeaponModule")]
public class WeaponInfoModule : ItemModule
{
    public float Damage;
    public float FireRate;
    public float Range;
    public int Ammo;
    public int MaxAmmo;
}
