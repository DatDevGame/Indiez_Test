using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "WeaponSO", menuName = "ZombieWar/Weapon/WeaponSO")]
public class WeaponSO : ItemSO
{
    [BoxGroup("IK Data")] public IKData IK_Idle;
    [BoxGroup("IK Data")] public IKData IK_Aim;
    [BoxGroup("PointFire Config")] public Vector3 PointFirePos;
    [BoxGroup("PointFire Config")] public Vector3 PointFireEur;
    [BoxGroup("Animation Key Data")] public string IdleAnimationKey, AimAnimationKey;
}

[Serializable]
public class IKData
{
    public Vector3 RevolverLocalPosition, RevolverLocalRotation;
    public Vector3 LeftHandIkHintLocalPosion, LeftHandIkHintLocalRotation;
}
