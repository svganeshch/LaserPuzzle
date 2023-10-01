using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.VisualScripting;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

public class PlayerController : MonoBehaviour
{
    [Header("Mouse")]
    public float mouseSensitivity = 1f;
    public bool invertX = false;
    public bool invertY = false;

    private Vector2 mousePos;

    [Header("Player")]
    public float moveSpeed = 5f;
    public float jumpSpeed = 2.5f;
    private float gravity = 9.8f;
    private float velocitySpeed = 0;
    private CharacterController playerController;
    private Vector3 playerDirection;

    [Header("Laser")]
    public Transform laserHoldPoint;
    private LaserGun laserGun;
    public bool laserHold = false;

    private MirrorHandler mirror;
    public Transform holdPoint;

    CrosshairPos crosshair;

    private PlayerInput playerInput;
    private InputAction mouseLookAction;
    private InputAction playerMoveAction;
    private InputAction playerJumpAction;

    private MouseToRotation MouseToRotation;

    private void Awake()
    {
        MouseToRotation = new MouseToRotation();
        crosshair = GetComponentInChildren<CrosshairPos>();
        playerController = GetComponent<CharacterController>();
        laserGun = GetComponentInChildren<LaserGun>();

        playerInput = GetComponent<PlayerInput>();
    }

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;

        mouseLookAction = playerInput.actions["Look"];
        mouseLookAction.started += ctx => MouseLook(ctx.ReadValue<Vector2>());

        playerMoveAction = playerInput.actions["Move"];
        playerJumpAction = playerInput.actions["Jump"];
    }

    void Update()
    {
        if (playerController.isGrounded)
        {
            velocitySpeed = 0;
            if (playerJumpAction.IsPressed())
            {
                velocitySpeed = jumpSpeed;
            }
        }
        velocitySpeed -= gravity * Time.deltaTime;

        PlayerMove(playerMoveAction.ReadValue<Vector2>(), velocitySpeed);
    }

    public void PlayerMove(Vector2 value, float vSpeed)
    {
        playerDirection = (transform.right * value.x + transform.forward * value.y).normalized;
        playerDirection.y = vSpeed;
        playerController.Move(moveSpeed * Time.deltaTime * playerDirection);
    }

    public void MouseLook(Vector2 value)
    {
        mousePos = new Vector2(value.x * mouseSensitivity * ((invertX) ? -1 : 1), value.y * mouseSensitivity * ((invertY) ? -1 : 1));

        Quaternion rotval = MouseToRotation.GetRotation(mousePos);
        transform.localRotation = rotval;
    }

    public void OnPickUp(InputValue value)
    {
        if (mirror != null)
        {
            if (!laserHold)
                laserGun.gameObject.SetActive(!laserGun.gameObject.activeSelf);

            mirror.Release();
            mirror = null;

            return;
        }

        if (crosshair.hitCollider.TryGetComponent<MirrorHandler>(out mirror))
        {
            if (!laserHold)
                laserGun.gameObject.SetActive(false);

            mirror.Hold();
        }

        Debug.Log("triggered");
    }

    public void OnLaserHold()
    {
        if (!laserHold)
        {
            laserGun.gameObject.transform.parent = null;
            laserHold = true;
        }
        else
        {
            laserGun.gameObject.transform.parent = laserHoldPoint;
            laserGun.gameObject.transform.SetLocalPositionAndRotation(new Vector3(0.8f, -0.5f, 0.5f), laserHoldPoint.localRotation);
            laserHold = false;
        }
    }
}
