using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Premium
{
[CreateAssetMenu(fileName = "audio", menuName = "Premium/ScriptableObject/ReferentialSoundClip")]
public class ReferentialSoundClip : ScriptableObject
{
    [SerializeField] private AudioClip clip = null;
    public AudioClip Clip => clip;
    public AudioClip[] clips;
}
}