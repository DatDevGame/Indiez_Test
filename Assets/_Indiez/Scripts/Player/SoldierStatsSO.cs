using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "SoldierStatsSO", menuName = "ZombieWar/Stats/SoldierStatsSO", order = 0)]
public class SoldierStatsSO : StatsSO
{
    [SerializeField, BoxGroup("Combat Stats")] public LayerMask TargetLayermask = -1;
}
