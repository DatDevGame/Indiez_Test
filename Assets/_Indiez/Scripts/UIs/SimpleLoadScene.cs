using System;
using System.Collections;
using System.Collections.Generic;
using Premium.GameManagement;
using UnityEngine;
public class SimpleLoadScene : MonoBehaviour
{
    void Start()
    {
        LoadingScreenUI.Load(SceneManager.LoadSceneAsync(SceneName.MainScene, isPushToStack: false));
    }
}
