using System;
using UnityEngine;
using System.Collections.Generic;
using Random = UnityEngine.Random;
using Unity.Mathematics;

public enum SingleAudioType {
    korv,
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
    [Header("Lists fills based on the amount of types. Just attach audiofiles.")]
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
            Debug.LogWarning($"Audiotype: {audioType.ToString()} is empty no clip to play");
            return;
        }

        AudioClip playClip = audioClips[Random.Range(0, audioClips.Length - 1)];
        source.pitch = pitchRandom;
        source.PlayOneShot(playClip);
    }

    /// <summary>
    /// Playing a single audio
    /// </summary>
    /// <param name="audioType">Single audio type</param>
    /// <param name="source">Where the sound should come from</param>
    public void PlaySingleSound(SingleAudioType audioType, AudioSource source) {
        int audioSelect = (int)audioType;
        AudioClip playClip = singleAudios[audioSelect].audioClip;
        if(playClip == null) {
            Debug.LogWarning($"Audiotype: {audioType.ToString()} is empty no clip to play");
            return;
        }
    }

    //This just handles creating list at the same length as the enum, sets the name based on enum names.
    //This is nasty dont LOOK dont TOUCH
    public void OnValidate() { 
        string[] singleAudioNames = Enum.GetNames(typeof(SingleAudioType));
        string[] randomAudioNames = Enum.GetNames(typeof(RandomAudioType));
        int singleAudioCount = singleAudios.Count;
        int randomAudioCount = randomAudios.Count;
        int singleNameCount = singleAudioNames.Length;
        int randomNameCount = randomAudioNames.Length;
        int listCreateSingle = singleNameCount - singleAudioCount;
        int listCreateRandom = randomNameCount - randomAudioCount;

        for(int i = 0; i < listCreateSingle; i++) {
            SingleAudio temp = new();
            singleAudios.Add(temp);
        }

        for(int i = 0; i < listCreateRandom; i++) {
            RandomAudio temp = new();
            randomAudios.Add(temp);
        }

        for(int i = 0; i < singleAudios.Count; i++) {
            singleAudios[i].name = singleAudioNames[i];
        }
        
        for(int i = 0; i < randomAudios.Count; i++) {
            randomAudios[i].name = randomAudioNames[i];
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
