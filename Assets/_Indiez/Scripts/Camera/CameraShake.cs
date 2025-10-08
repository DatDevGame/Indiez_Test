using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using Sirenix.OdinInspector;

public class CameraShake : MonoBehaviour
{
    public static CameraShake Instance { get; private set; }

    [SerializeField] private CinemachineVirtualCamera m_VCam;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    [Button]
    public void Shake(float frequencyGainValue, float shakeCameraTime)
    {
        StartCoroutine(ShakeHandle(frequencyGainValue, shakeCameraTime));
    }

    private IEnumerator ShakeHandle(float frequencyGainValue, float shakeCameraTime)
    {
        var noise = m_VCam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        if (noise == null) yield break;
        noise.m_FrequencyGain = frequencyGainValue;
        yield return new WaitForSeconds(shakeCameraTime);
        noise.m_FrequencyGain = 0;
    }
}
