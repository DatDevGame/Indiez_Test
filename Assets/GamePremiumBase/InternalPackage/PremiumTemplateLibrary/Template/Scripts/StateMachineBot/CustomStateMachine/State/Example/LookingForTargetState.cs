using System;
using System.Text;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HCore.Helpers;
using Sirenix.OdinInspector;

[Serializable]
public class LookingForTargetState : AIBotState
{
    private List<AIBotController> m_OtherBotControllersIgnoreMyself;

    protected override void OnStateUpdate()
    {
        base.OnStateUpdate();
        if (TryFindTarget(out var target))
        {
            BotController.Target = target;
        }
        else
        {
            BotController.Target = null;
        }
    }

    public override void InitializeState(AIBotController botController)
    {
        base.InitializeState(botController);
    }

    private bool IsAbleToFocusAtPlayer()
    {
        return false;
    }

    private bool TryFindTarget(out INavigationPoint target)
    {
        target = null;

        if (BotController.AIProfile == null)
            return false;

        // Get all targets in range
        var targetsInRange = BotController.FindTargetsInRange();
        if (targetsInRange.Count == 0)
            return false;

        // Score targets based on type and distance
        var scoredTargets = new List<NavPointRngInfo>();

        foreach (var navPoint in targetsInRange)
        {
            float score = CalculateTargetScore(navPoint);
            scoredTargets.Add(new NavPointRngInfo(score, navPoint));
        }

        // Sort by score (highest first)
        scoredTargets.Sort((a, b) => b.Probability.CompareTo(a.Probability));

        // Select the best target
        if (scoredTargets.Count > 0)
        {
            target = scoredTargets[0].NavigationPoint;
            return true;
        }

        return false;
    }

    private float CalculateTargetScore(INavigationPoint navPoint)
    {
        // if (BotController.AIProfile == null)
        //     return 0f;

        // float baseScore = 0f;

        // Base score based on point type
        // switch (navPoint.GetPointType())
        // {
        //     case PointType.OpponentPoint:
        //         baseScore = BotController.AIProfile.PlayerTargetWeight;
        //         break;
        //     case PointType.CollectablePoint:
        //         baseScore = BotController.AIProfile.CollectableTargetWeight;
        //         break;
        //     case PointType.UtilityPoint:
        //         baseScore = BotController.AIProfile.UtilityTargetWeight;
        //         break;
        //     case PointType.NormalPoint:
        //         baseScore = 0.2f; // Lower priority for normal points
        //         break;
        // }

        // Distance factor (closer = higher score)
        // float distance = Vector3.Distance(BotController.BotTransform.position, navPoint.GetTargetPoint());
        // float distanceFactor = 1f - (distance / BotController.AIProfile.DetectionRange);

        // return baseScore * distanceFactor;
        return 0;
    }

    protected List<INavigationPoint> GetRandomNavPoints()
    {
        return null;
    }

    public class NavPointRngInfo : IRandomizable
    {
        public NavPointRngInfo(float probability, INavigationPoint navigationPoint)
        {
            this.probability = probability;
            this.navigationPoint = navigationPoint;
        }

        private float probability;
        private INavigationPoint navigationPoint;

        public float Probability
        {
            get => probability;
            set => probability = value;
        }
        public INavigationPoint NavigationPoint
        {
            get => navigationPoint;
            set => navigationPoint = value;
        }
    }
}

[Serializable]
public class LookingForTargetToChasingTargetTransition : AIBotStateTransition
{
    protected override bool Decide()
    {
        //Condition Transition In Here
        if (botController.Target != null)
        {
            return true;
        }
        return false;
    }
}