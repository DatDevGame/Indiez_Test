using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss_Ghoul_Zombie : ZombiePrototype
{
    public override void Init(EnemyStatsSO statsSO = null)
    {
        base.Init(statsSO);
        m_HealthBarMesh.material = new Material(m_HealthBarSO.BossGhoulHealthBarMaterial);
    }
}
