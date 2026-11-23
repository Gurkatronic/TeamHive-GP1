using UnityEngine;
using NaughtyAttributes;
using UnityEngine.InputSystem;

[CreateAssetMenu(fileName = "PlayableCharacterScriptableObject", menuName = "Player/PlayableCharacterScriptableObject")]
public class PlayableCharacterScriptableObject : ScriptableObject
{
    //Should be able to change color in start menu
    //What character model that should be used
    //What audio pack this player will use
    [HideInInspector] public Color outlineColor;
    public GameObject characterPrefab;
    public PlayerSounds playerSounds;
}
