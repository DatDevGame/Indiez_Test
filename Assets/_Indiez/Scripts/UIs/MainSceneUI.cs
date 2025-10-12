using System.Collections;
using System.Collections.Generic;
using Premium.GameManagement;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

public class MainSceneUI : MonoBehaviour
{
    [SerializeField, BoxGroup("Reference")] private MultiImageButton m_PlayButton;

    private void Awake()
    {
        m_PlayButton.onClick.AddListener(PlayButton);
    }

    private void OnDestroy()
    {
        m_PlayButton.onClick.RemoveListener(PlayButton);
    }

    private void PlayButton()
    {
        LoadingScreenUI.Load(SceneManager.LoadSceneAsync(SceneName.PlayScene, isPushToStack: false));
    }
}
