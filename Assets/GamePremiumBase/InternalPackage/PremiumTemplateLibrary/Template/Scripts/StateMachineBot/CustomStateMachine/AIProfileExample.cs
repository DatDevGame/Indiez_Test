using UnityEngine;
using Sirenix.OdinInspector;

/// <summary>
/// Example script demonstrating how to create and use AIProfiles programmatically
/// </summary>
public class AIProfileExample : MonoBehaviour
{
    [Header("Example AI Profiles")]
    [SerializeField] private AIProfile m_AggressiveProfile;
    [SerializeField] private AIProfile m_DefensiveProfile;
    [SerializeField] private AIProfile m_BalancedProfile;
    
    [Header("Test AI Bot")]
    [SerializeField] private AIBotController m_TestAIBot;
    
    [Button("Create Aggressive AI Profile")]
    public void CreateAggressiveProfile()
    {
        m_AggressiveProfile = CreateAIProfile("AggressiveAI");
        
        // Configure aggressive behavior
        SetProfileValues(m_AggressiveProfile, 
            detectionRange: 8f,
            attackRange: 2f,
            moveSpeed: 4f,
            playerTargetWeight: 0.9f,
            collectableTargetWeight: 0.3f,
            utilityTargetWeight: 0.2f,
            attackCooldown: 0.5f,
            debugColor: Color.red);
            
        Debug.Log("Aggressive AI Profile created!");
    }
    
    [Button("Create Defensive AI Profile")]
    public void CreateDefensiveProfile()
    {
        m_DefensiveProfile = CreateAIProfile("DefensiveAI");
        
        // Configure defensive behavior
        SetProfileValues(m_DefensiveProfile,
            detectionRange: 6f,
            attackRange: 1.5f,
            moveSpeed: 2.5f,
            playerTargetWeight: 0.4f,
            collectableTargetWeight: 0.8f,
            utilityTargetWeight: 0.7f,
            attackCooldown: 1.5f,
            debugColor: Color.blue);
            
        Debug.Log("Defensive AI Profile created!");
    }
    
    [Button("Create Balanced AI Profile")]
    public void CreateBalancedProfile()
    {
        m_BalancedProfile = CreateAIProfile("BalancedAI");
        
        // Configure balanced behavior
        SetProfileValues(m_BalancedProfile,
            detectionRange: 5f,
            attackRange: 1.5f,
            moveSpeed: 3f,
            playerTargetWeight: 0.6f,
            collectableTargetWeight: 0.6f,
            utilityTargetWeight: 0.4f,
            attackCooldown: 1f,
            debugColor: Color.green);
            
        Debug.Log("Balanced AI Profile created!");
    }
    
    [Button("Apply Profile to AI Bot")]
    public void ApplyProfileToAIBot()
    {
        if (m_TestAIBot == null)
        {
            Debug.LogError("Test AI Bot is not assigned!");
            return;
        }
        
        // You would need to add a setter to AIBotController for this to work
        // For now, this is just a demonstration
        Debug.Log("Profile would be applied to AI Bot (requires setter implementation)");
    }
    
    [Button("Test AI Bot Setup")]
    public void TestAIBotSetup()
    {
        if (m_TestAIBot == null)
        {
            Debug.LogError("Test AI Bot is not assigned!");
            return;
        }
        
        // Initialize and start the AI Bot
        m_TestAIBot.Initialize(m_TestAIBot);
        m_TestAIBot.InitializeStateMachine();
        m_TestAIBot.StartStateMachine();
        
        Debug.Log("AI Bot setup and started!");
    }
    
    private AIProfile CreateAIProfile(string profileName)
    {
        // Create a new AIProfile instance
        var profile = ScriptableObject.CreateInstance<AIProfile>();
        profile.name = profileName;
        
        // Save the asset to the project
        #if UNITY_EDITOR
        string path = $"Assets/_DraftMaze/AIBotController/{profileName}.asset";
        UnityEditor.AssetDatabase.CreateAsset(profile, path);
        UnityEditor.AssetDatabase.SaveAssets();
        UnityEditor.AssetDatabase.Refresh();
        #endif
        
        return profile;
    }
    
    private void SetProfileValues(AIProfile profile, float detectionRange, float attackRange, 
        float moveSpeed, float playerTargetWeight, float collectableTargetWeight, 
        float utilityTargetWeight, float attackCooldown, Color debugColor)
    {
        if (profile == null) return;
        
        // Use reflection to set private fields (for demonstration purposes)
        var type = typeof(AIProfile);
        
        SetPrivateField(profile, type, "m_DetectionRange", detectionRange);
        SetPrivateField(profile, type, "m_AttackRange", attackRange);
        SetPrivateField(profile, type, "m_MoveSpeed", moveSpeed);
        SetPrivateField(profile, type, "m_PlayerTargetWeight", playerTargetWeight);
        SetPrivateField(profile, type, "m_CollectableTargetWeight", collectableTargetWeight);
        SetPrivateField(profile, type, "m_UtilityTargetWeight", utilityTargetWeight);
        SetPrivateField(profile, type, "m_AttackCooldown", attackCooldown);
        SetPrivateField(profile, type, "m_DebugColor", debugColor);
        
        // Mark as dirty for editor
        #if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(profile);
        #endif
    }
    
    private void SetPrivateField(object obj, System.Type type, string fieldName, object value)
    {
        var field = type.GetField(fieldName, 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        if (field != null)
        {
            field.SetValue(obj, value);
        }
    }
    
    private void OnValidate()
    {
        // Auto-assign test AI Bot if not set
        if (m_TestAIBot == null)
            m_TestAIBot = GetComponent<AIBotController>();
    }
} 