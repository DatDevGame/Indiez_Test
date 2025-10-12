public enum PVEEventCode
{
    /// <summary>
    /// Triggered when Start Level.
    ///
    /// <para>Parameters:</para>
    /// <list type="number">
    ///   <item>
    ///     <description><c>0</c> — <see cref="LevelSO"/>: This Current Level Data.</description>
    ///   </item>
    ///   <item>
    ///     <description><c>1</c> — <see cref="BaseSoldier"/>: Player Info.</description>
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
    ///     <description><c>0</c> — <see cref="LevelSO"/>: Data Current Level.</description>
    ///   </item>
    ///   <item>
    ///     <description><c>1</c> — <see cref="bool"/>: IsVictory.</description>
    ///   </item>
    /// </list>
    /// </summary>    
    OnLevelEnd,
}