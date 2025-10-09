using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "ZombieAIProfile", menuName = "ZombieWar/StateMachine/ZombieAIProfile")]
public class ZombieAIProfile : AIProfile
{
    [Title("Looking State Config", "", TitleAlignments.Centered)]
    [MinMaxSlider(0f, 5f, true)]
    [SerializeField]
    protected Vector2 m_TimeRandomChasingTarget;
    public float TimeRandomChasingTarget => Random.Range(m_TimeRandomChasingTarget.x, m_TimeRandomChasingTarget.y);

    [Title("Chasing State Config", "", TitleAlignments.Centered)]
    public float RotationSpeed = 2f;
    public float ReachThreshold = 0.1f;
}
