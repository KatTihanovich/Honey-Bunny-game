using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    private static PlayerInput PlayerInput;
    public static Vector2 Movement;
    public static bool JumpWasPressed;
    public static bool AttackWasPressed;

    private InputAction moveAction;
    private InputAction jumpAction;
    private InputAction attackAction;

    private void Awake()
    {
        PlayerInput = GetComponent<PlayerInput>();

        moveAction = PlayerInput.actions["Move"];
        jumpAction = PlayerInput.actions["Jump"];
        attackAction = PlayerInput.actions["Attack"];
    }

    private void Update()
    {
        Movement = moveAction.ReadValue<Vector2>();
        JumpWasPressed = jumpAction.WasPressedThisFrame();
        AttackWasPressed = attackAction.WasPressedThisFrame();
    }
}