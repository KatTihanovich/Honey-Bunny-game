// Файл: SoundData.cs
using UnityEngine;

namespace Game.Audio
{
    [CreateAssetMenu(fileName = "SoundData", menuName = "Audio/SoundData", order = 1)]
    public class SoundData : ScriptableObject
    {
        [System.Serializable]
        public class SoundEntry
        {
            public string Name; // Строковый идентификатор звука
            public AudioClip Clip;
            [Range(0f, 1f)] public float Volume = 1f;
            [Range(0.1f, 3f)] public float Pitch = 1f;
        }

        [SerializeField] private SoundEntry[] _sounds;

        public bool TryGetSound(string soundName, out AudioClip clip, out float volume, out float pitch)
        {
            foreach (var sound in _sounds)
            {
                if (sound.Name == soundName)
                {
                    clip = sound.Clip;
                    volume = sound.Volume;
                    pitch = sound.Pitch;
                    return clip != null;
                }
            }
            clip = null;
            volume = 1f;
            pitch = 1f;
            return false;
        }
    }
}