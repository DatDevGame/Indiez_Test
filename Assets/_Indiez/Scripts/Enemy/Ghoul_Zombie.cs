using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ghoul_Zombie : ZombiePrototype
{
    public override void Init(EnemyStatsSO statsSO = null)
    {
        base.Init(statsSO);
        m_HealthBarMesh.material = new Material(m_HealthBarSO.GhoulHealthBarMaterial);
    }
}
