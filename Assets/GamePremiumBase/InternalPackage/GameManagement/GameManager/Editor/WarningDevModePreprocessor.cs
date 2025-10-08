using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using HCore.Helpers;
using Premium.Logging;
using Premium.GameManagement;

public class WarningDevModePreprocessor : IPreprocessBuildWithReport
{
    public int callbackOrder => 0;

    public void OnPreprocessBuild(BuildReport report)
    {
        Premium.GameManagement.GlobalGameConfigSO globalGameConfigSO = Premium.GameManagement.GlobalGameConfigSO.Instance;
        if (globalGameConfigSO.isDevModeEnabled)
        {
            if (!EditorUtility.DisplayDialog("Warning Developer mode",
"Developer mode is enabled. Are you sure about that?",
"Definitely sure", "Disable it"))
            {
                Premium.Logging.GlobalLogConfigSO globalLogConfigSO = Premium.Logging.GlobalLogConfigSO.Instance;
                bool isLoggingInBuildEnabled = globalLogConfigSO.isLoggingInBuildEnabled;
                globalGameConfigSO.SetFieldValue("m_IsDevModeEnabled", false);
                globalGameConfigSO.InvokeMethod("OnValidate");
                EditorUtility.SetDirty(globalGameConfigSO);
                AssetDatabase.SaveAssetIfDirty(globalGameConfigSO);
                if (isLoggingInBuildEnabled)
                    throw new BuildFailedException("Failed to build because of recompiling script (remove custom define symbols). Please build again!!!");
            }
        }
    }
}