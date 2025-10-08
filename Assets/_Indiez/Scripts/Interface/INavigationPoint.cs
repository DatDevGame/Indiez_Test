using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Indiez.Interface
{
    public enum PointType
    {
        UtilityPoint,
        CollectablePoint,
        NormalPoint,
        OpponentPoint
    }

    public interface INavigationPoint
    {
        public BaseSoldier GetSoldier();
        public bool IsAvailable();
        public PointType GetPointType();
        public Vector3 GetSelfPoint();
        public Vector3 GetTargetPoint();
    }
}

