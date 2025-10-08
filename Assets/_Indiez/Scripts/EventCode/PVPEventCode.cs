/// <summary>
/// Defines the event codes used in PvP gameplay events.
/// </summary>
public enum PVPEventCode
{
    /// <summary>
    /// Triggered when Start Level.
    ///
    /// <para>Parameters:</para>
    /// <list type="number">
    ///   <item>
    ///     <description><c>0</c> — <see cref="LevelDataSO"/>: This Current Level Data.</description>
    ///   </item>
    ///   <item>
    ///     <description><c>1</c> — <see cref="PlayerBoxer"/>: Player Info.</description>
    ///   </item>
    /// </list>
    /// </summary>    
    OnLevelStart,

    /// <summary>
    /// Triggered when Start Level.
    ///
    /// <para>Parameters:</para>
    /// <list type="number">
    ///   <item>
    ///     <description><c>0</c> — <see cref="LevelDataSO"/>: Data Current Level.</description>
    ///   </item>
    ///   <item>
    ///     <description><c>1</c> — <see cref="Bool"/>: IsVictory.</description>
    ///   </item>
    /// </list>
    /// </summary>    
    OnLevelEnd,

    /// <summary>
    /// Triggered when a character receives damage in PvP mode.
    ///
    /// <para>Parameters:</para>
    /// <list type="number">
    ///   <item>
    ///     <description><c>0</c> — <see cref="BaseBoxer"/>: The character that received the damage.</description>
    ///   </item>
    ///   <item>
    ///     <description><c>1</c> — <see cref="int"/>: The amount of damage received.</description>
    ///   </item>
    /// </list>
    /// </summary>    
    CharacterReceivedDamage,

    /// <summary>
    /// Triggered when a character Dead.
    ///
    /// <para>Parameters:</para>
    /// <list type="number">
    ///   <item>
    ///     <description><c>0</c> — <see cref="BaseBoxer"/>: The character Dead.</description>
    ///   </item>
    /// </list>
    /// </summary>    
    AnyCharacterDead
}
