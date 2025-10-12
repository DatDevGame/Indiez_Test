using Sirenix.OdinInspector;
using UnityEngine;

public class SetSkyboxAtRuntime : MonoBehaviour
{
    [SerializeField] private Material m_SkyboxMaterial;

    private void Start()
    {
        if (m_SkyboxMaterial != null)
        {
            RenderSettings.skybox = m_SkyboxMaterial;
        }
    }

    [Button]
    public void SetSkybox()
    {
        if (m_SkyboxMaterial != null)
        {
            RenderSettings.skybox = m_SkyboxMaterial;
        }
    }
}
