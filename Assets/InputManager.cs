using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    private static PlayerInput PlayerInput;
    public static Vector2 Movement;
    public static bool JumpWasPressed;

    private InputAction moveAction;
    private InputAction jumpAction;

    private void Awake()
    {
        PlayerInput = GetComponent<PlayerInput>();

        moveAction = PlayerInput.actions["Move"];
        jumpAction = PlayerInput.actions["Jump"];
    }

    // Update is called once per frame
    private void Update()
    {
        Movement = moveAction.ReadValue<Vector2>();
        JumpWasPressed = jumpAction.WasPressedThisFrame();
    }
}