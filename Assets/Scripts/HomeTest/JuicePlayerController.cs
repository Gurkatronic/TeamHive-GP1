using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class JuicePlayerController : MonoBehaviour
{
    CharacterController cController;
    float gravity = 5f;

    private void Awake() {
        cController = GetComponent<CharacterController>();
    }
}
