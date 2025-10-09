using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AnimationKeySO", menuName = "BoxingArena/Animation/AnimationKeySO")]
public class AnimationKeySO : SerializableScriptableObject
{
    public string Idle = "Idle";
    public string Walking = "Chasing";
    public string Attack = "HeadAttack";
    public string Hit_1 = "Hit_1";
    public string Hit_2 = "Hit_2";
    public string Dead = "Dead";
    public string DeadTrigger = "DeadTrigger";
}
