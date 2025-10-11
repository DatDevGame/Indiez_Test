
public enum PlayerEventCode
{
    /// <summary>
    /// Call When Take Damage
    /// <para>Param: <b>float amount</b> — Damage.</para>
    /// </summary>
    TakeDamage,

    /// <summary>
    /// Call When Equip Weapon
    /// <para>Param: <b>WeaponSO weaponSO</b> — Data Weapon.</para>
    /// </summary>
    EquipWeapon,

    /// <summary>
    /// Call When click button Throw Grenade
    /// </summary>
    ThrowGrenadeTrigger,
}
