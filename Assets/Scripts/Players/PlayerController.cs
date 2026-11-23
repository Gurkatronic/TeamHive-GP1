using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    float Speed = 20f;
    [SerializeField]Rigidbody rb;
    Vector3 force;
    Vector2 direction;
    InputAction moveAction;
    PlayerLogic playerLogic;

    private void Start()
    {
        moveAction = GetComponent<PlayerInput>().actions["Move"];
        playerLogic = GetComponent<PlayerLogic>();
    }
    private void Update()
    {
        direction = moveAction.ReadValue<Vector2>();
        if (playerLogic.stunned)
        {
            direction = Vector2.zero;
        }
        force = new Vector3(direction.x, 0, direction.y);
    }
    private void FixedUpdate()
    {
        rb.AddForce(Speed * playerLogic.relicSpeedMultiplier * force); 
        if(direction != Vector2.zero)
        {
            transform.rotation = Quaternion.LookRotation(force);
        }
    }


}