using System.Collections;
using UnityEngine;

public class AnimateDissolve : MonoBehaviour
{
    public void PlayDissolve(Material mat, float duration)
    {
        if (mat == null)
        {
            Debug.LogWarning("AnimateDissolve: Material is null!");
            return;
        }

        if (!mat.HasProperty("_DissolveValue"))
        {
            Debug.LogWarning($"AnimateDissolve: Material '{mat.name}' doesn't have '_DissolveValue' property.");
            return;
        }

        StartCoroutine(DissolveEffect(mat, duration));
    }

    private IEnumerator DissolveEffect(Material mat, float duration)
    {
        float time = 0f;
        while (time < duration)
        {
            time += Time.deltaTime;
            mat.SetFloat("_DissolveValue", time / duration);
            yield return null;
        }
        mat.SetFloat("_DissolveValue", 1f);
    }
}
