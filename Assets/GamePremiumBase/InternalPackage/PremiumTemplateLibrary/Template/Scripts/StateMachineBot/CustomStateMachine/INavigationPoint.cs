using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PointType
{
    UtilityPoint,
    CollectablePoint,
    NormalPoint,
    OpponentPoint
}

public interface INavigationPoint
{
    public bool IsAvailable();
    //Temp
    public BaseBoxer GetBoxer();
    public PointType GetPointType();
    public Vector3 GetSelfPoint();
    public Vector3 GetTargetPoint();
}