using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "WeaponSO", menuName = "ZombieWar/Weapon/WeaponSO")]
public class WeaponSO : ItemSO
{
    [BoxGroup("IK Data")] public Vector3 RevolverLocalPosition, RevolverLocalRotation;
    [BoxGroup("IK Data")] public Vector3 LeftHandIkHintLocalPosion, LeftHandIkHintLocalRotation;
}
