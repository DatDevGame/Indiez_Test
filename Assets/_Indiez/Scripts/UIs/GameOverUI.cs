using System.Collections;
using System.Collections.Generic;
using HCore.Events;
using Premium;
using Premium.GameManagement;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

public class GameOverUI : MonoBehaviour
{
    [SerializeField, BoxGroup("Reference")] private TMP_Text m_Title;
    [SerializeField, BoxGroup("Reference")] private MultiImageButton m_ContinueButton;
    [SerializeField, BoxGroup("Reference")] private CanvasGroupVisibility m_CanvasGroupVisibility;

    private void Awake()
    {
        GameEventHandler.AddActionEvent(PVEEventCode.OnLevelStart, OnLevelStart);
        GameEventHandler.AddActionEvent(PVEEventCode.OnLevelEnd, OnLevelEnd);
        m_ContinueButton.onClick.AddListener(OnContinue);
    }

    private void OnDestroy()
    {
        GameEventHandler.RemoveActionEvent(PVEEventCode.OnLevelStart, OnLevelStart);
        GameEventHandler.RemoveActionEvent(PVEEventCode.OnLevelEnd, OnLevelEnd);
        m_ContinueButton.onClick.RemoveListener(OnContinue);
    }

    private void OnLevelStart()
    {

    }

    private void OnLevelEnd(params object[] parameters)
    {
        if (parameters[0] == null || parameters.Length <= 0)
            return;
        bool isVictory = (bool)parameters[0];
        string title = isVictory ? "Victory" : "Lose";
        m_Title.SetText($"{title}");
        m_CanvasGroupVisibility.Show();
    }

    private void OnContinue()
    {
        LoadingScreenUI.Load(SceneManager.LoadSceneAsync(SceneName.MainScene, isPushToStack: false));
    }
}
