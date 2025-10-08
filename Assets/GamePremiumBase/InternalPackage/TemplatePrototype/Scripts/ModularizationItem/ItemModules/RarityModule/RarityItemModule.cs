using System;
using System.Collections;
using System.Collections.Generic;
using HCore.Helpers;
using UnityEngine;

public enum RarityType
{
    Common,
    Uncommon,
    Rare,
    Epic,
    Legendary,
    Mythic,
}
[Serializable, CustomInspectorName("RarityModule")]
public class RarityItemModule : ItemModule
{
    public event Action<RarityItemModule> onRarityChanged;

    [SerializeField]
    protected RarityType m_RarityType;

    public virtual RarityType rarityType
    {
        get => m_RarityType;
        set
        {
            m_RarityType = value;
            onRarityChanged?.Invoke(this);
        }
    }
}