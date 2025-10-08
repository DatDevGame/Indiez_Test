using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEngine;
using Premium.GameManagement;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Premium.Logging
{
    [Flags]
    public enum StackTraceLogType
    {
        //
        // Summary:
        //     LogType used for Errors.
        Error = 1 << 0,
        //
        // Summary:
        //     LogType used for Asserts. (These could also indicate an error inside Unity itself.)
        Assert = 1 << 1,
        //
        // Summary:
        //     LogType used for Warnings.
        Warning = 1 << 2,
        //
        // Summary:
        //     LogType used for regular log messages.
        Log = 1 << 3,
        //
        // Summary:
        //     LogType used for Exceptions.
        Exception = 1 << 4
    }
    [GlobalConfig("Assets/Resources")]
    [WindowMenuItem("GlobalConfig", "Logging", "Assets/Resources")]
    public class GlobalLogConfigSO : GlobalConfig<GlobalLogConfigSO>
    {
        public const string k_PremiumDebugDefineSymbol = "PREMIUM_DEBUG";
        public const string k_UnityEditorDefineSymbol = "UNITY_EDITOR";
        public const string k_DefaultTag = "Default";

        [Title("Debug Log")]
        [SerializeField]
        private bool m_IsLoggingInEditorEnabled = true;
        [SerializeField, ShowIf("isDevModeEnabled")]
        private bool m_IsLoggingInBuildEnabled;

        [Title("Save Log")]
        [SerializeField, ShowIf("isDevModeEnabled")]
        private bool m_SaveLogToFile;
        // 16KB char buffer to prevent performance hiccups when Flush data from char buffer to file stream
        [SerializeField, ShowIf("isDevModeEnabled"), LabelText("Max Buffer Size (Byte)")]
        private int m_MaxBufferSize = 1024 * 16;
        [SerializeField, ShowIf("isDevModeEnabled")]
        private StackTraceLogType m_SaveStackTraceLogType = StackTraceLogType.Error | StackTraceLogType.Warning | StackTraceLogType.Exception;

        public bool isDevModeEnabled => GlobalGameConfigSO.Instance.isDevModeEnabled;
        public bool isLoggingInEditorEnabled => m_IsLoggingInEditorEnabled;
        public bool isLoggingInBuildEnabled => isDevModeEnabled && m_IsLoggingInBuildEnabled;
        public bool saveLogToFile => isDevModeEnabled && m_SaveLogToFile;
        public int maxBufferSize => m_MaxBufferSize;
        public StackTraceLogType saveStackTraceLogType => m_SaveStackTraceLogType;

#if UNITY_EDITOR
        private void OnValidate()
        {
            var buildTargetGroup = EditorUserBuildSettings.selectedBuildTargetGroup;
            if (buildTargetGroup == BuildTargetGroup.Unknown)
            {
                var propertyInfo = typeof(EditorUserBuildSettings).GetProperty("activeBuildTargetGroup", BindingFlags.Static | BindingFlags.NonPublic);
                if (propertyInfo != null)
                    buildTargetGroup = (BuildTargetGroup)propertyInfo.GetValue(null, null);
            }

            var previousProjectDefines = PlayerSettings.GetScriptingDefineSymbolsForGroup(buildTargetGroup);
            var projectDefines = previousProjectDefines.Split(';').ToList();

            if (isLoggingInBuildEnabled)
            {
                if (!projectDefines.Contains(k_PremiumDebugDefineSymbol))
                {
                    // Prevent reload assemblies
                    EditorApplication.LockReloadAssemblies();
                    projectDefines.Add(k_PremiumDebugDefineSymbol);
                    PlayerSettings.SetScriptingDefineSymbolsForGroup(buildTargetGroup, string.Join(";", projectDefines.ToArray()));
                    EditorApplication.UnlockReloadAssemblies();
                }
            }
            else
            {
                if (projectDefines.Contains(k_PremiumDebugDefineSymbol))
                {
                    // Prevent reload assemblies
                    EditorApplication.LockReloadAssemblies();
                    projectDefines.Remove(k_PremiumDebugDefineSymbol);
                    PlayerSettings.SetScriptingDefineSymbolsForGroup(buildTargetGroup, string.Join(";", projectDefines.ToArray()));
                    EditorApplication.UnlockReloadAssemblies();
                }
            }
        }
#endif
    }
}