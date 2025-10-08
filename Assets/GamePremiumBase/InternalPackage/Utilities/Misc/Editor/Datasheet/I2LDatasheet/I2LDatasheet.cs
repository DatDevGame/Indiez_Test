using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using I2.Loc;
using Premium.Utils;
using UnityEngine.Networking;

[CreateAssetMenu(fileName = "I2LDatasheet", menuName = "Premium/Editor/Datasheet/I2LDatasheet")]
public class I2LDatasheet : Datasheet
{
    [SerializeField]
    protected MonoScript m_I2LTermEnumScript;

    public override void ExportData(string directoryPath)
    {
        // Do nothing
        Debug.LogError("Not support!");
    }

    public override void ImportData()
    {
        //RemoteDataDownloader.Download(remotePath, false, callback: OnSyncCompleted);

        void OnSyncCompleted(UnityWebRequestAsyncOperation asyncOperation, bool isSucceeded)
        {
            if (!isSucceeded)
            {
                //RemoteDataDownloader.DisplayFailDialog();
                return;
            }

            try
            {
                var absoluteFilePath = Path.Combine(Path.GetDirectoryName(Application.dataPath), localPath);
                var guids = AssetDatabase.FindAssets("t:LanguageSourceAsset");
                var assetPath = AssetDatabase.GUIDToAssetPath(guids[0]);
                var languageSourceAsset = AssetDatabase.LoadAssetAtPath<LanguageSourceAsset>(assetPath);
                // *NOTE: This api method not is provided by default (I2L plugin does not public it and we have to do it by ourself)
                LocalizationEditor.Import_Global_CSV(languageSourceAsset, absoluteFilePath, eSpreadsheetUpdateMode.Replace);
                var allTerms = new List<string>();
                foreach (var term in languageSourceAsset.SourceData.GetTermsList())
                {
                    allTerms.Add(term.Replace("-", "_"));
                }
                var i2LTermFilePath = AssetDatabase.GetAssetPath(m_I2LTermEnumScript);
                Debug.Log(i2LTermFilePath);
                EditorUtils.GenerateEnum(i2LTermFilePath, allTerms, "Define all terms in LanguageSource.", "I2LTerm");
                EditorUtility.SetDirty(languageSourceAsset);
                AssetDatabase.SaveAssetIfDirty(languageSourceAsset);
                isSucceeded = true;
            }
            catch (Exception exc)
            {
                isSucceeded = false;
                Debug.LogException(exc);
            }

            if (isSucceeded)
            {
                //RemoteDataDownloader.DisplaySuccessDialog();
            }
            else
            {
                //RemoteDataDownloader.DisplayFailDialog();
            }
        }
    }
}