using System;
using UnityEngine;
using System.Collections.Generic;
using Random = UnityEngine.Random;

public enum SingleAudioType {
    korvstånd,
    test2,
    test3,
    test4,
    test5
}

public enum RandomAudioType {
    random1,
    random2,
    random3
}

public class SoundManager : MonoBehaviour
{
    [Serializable]
    public class SingleAudio {
        [HideInInspector] public string name;
        public AudioClip audioClip;
    }

    [Serializable]
    public class RandomAudio {
        [HideInInspector] public string name;
        public AudioClip[] audioClips;
    }
    public static SoundManager Instance;
    [SerializeField] private List<SingleAudio> singleAudios = new();
    [SerializeField] private List<RandomAudio> randomAudios = new();


    /// <summary>
    /// Pick a random sound from the corrosponding audiotype.
    /// </summary>
    /// <param name="audioType"></param>
    /// <param name="lowPitch">Basic values of the randomized pitch</param>
    /// <param name="highPitch">Basic values of the randomized pitch</param>
    public void PlayRandomSound(RandomAudioType audioType, AudioSource source, float lowPitch = 0.8f, float highPitch = 1.2f) {
        int audioSelect = (int)audioType;
        float pitchRandom = Random.Range(lowPitch, highPitch);
        AudioClip[] audioClips = randomAudios[audioSelect].audioClips;
        if(audioClips.Length <= 0) {
            Debug.LogWarning("Audiotype is empty no clip to play");
            return;
        }

        AudioClip playClip = audioClips[Random.Range(0, audioClips.Length - 1)];
        source.pitch = pitchRandom;
        source.PlayOneShot(playClip);
    }

    public void OnValidate() {
        string[] singleAudioNames = Enum.GetNames(typeof(SingleAudioType));
        int singleAudioCount = singleAudios.Count;
        int singleNameCount = singleAudioNames.Length;
        int listCreate = singleNameCount - singleAudioCount;

        for(int i = 0; i < listCreate; i++) {
            SingleAudio temp = new();
            singleAudios.Add(temp);
        }

        for(int i = 0; i < singleAudios.Count; i++) {
            singleAudios[i].name = singleAudioNames[i];
        }
    }

    public void Awake() {
        if(Instance != this) {
            Instance = this;
        }
        else {
            Destroy(this.gameObject);
        }
    }


}
