using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Audio
{
[CreateAssetMenu(fileName = "SoundDictionary", menuName = "ScriptableObjects/SoundDictionary", order = 2)]
public class SoundDictionary : ScriptableObject
{
    [SerializeField, TableView] private List<SoundData> sounds;
    
    public AudioClip GetClip(SoundKey soundType)
    {
        return sounds.First(sound => sound.soundType == soundType).audioClip;
    }
}

[Serializable]
public struct SoundData
{
    public SoundKey soundType;
    public AudioClip audioClip;
}
}
