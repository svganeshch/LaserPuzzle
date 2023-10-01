using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Composites;

public class MirrorHandler : MonoBehaviour
{
    private bool isHolding = false;

    private InputAction mirrorMoveAction;
    private InputAction mirrorRotateAction;

    private PlayerInput playerInput;
    private MouseToRotation MouseToRotation;

    private Vector2 moveDirection;
    private float mirrorManipulateSpeed = 3f;

    private Ray checkRayRight;
    private Ray checkRayLeft;
    private Ray checkRayUp;
    private Ray checkRayDown;
    private float checkRayDistance = 0.2f;
    private float checkRayRightDistance;
    private float checkRayLeftDistance;
    private float checkRayUpDistance;
    private float checkRayDownDistance;
    private MeshRenderer meshRenderer;

    private void Awake()
    {
        MouseToRotation = new MouseToRotation();

        playerInput = FindObjectOfType<PlayerInput>();
        meshRenderer = GetComponent<MeshRenderer>();
    }

    void Start()
    {
        mirrorMoveAction = playerInput.actions["MirrorMove"];
        mirrorRotateAction = playerInput.actions["MirrorRotate"];
    }

    private void Update()
    {
        if (isHolding)
        {
            CheckColliders();

            if (mirrorMoveAction.IsPressed())
            {
                var val = mirrorMoveAction.ReadValue<Vector2>();
                moveDirection = (transform.right * -val.x + transform.up * val.y).normalized;

                if (!mirrorRotateAction.IsPressed())
                {
                    if (moveDirection.x > 0)
                    {
                        if (checkRayRightDistance < checkRayDistance) return;
                    }
                    else if (moveDirection.x < 0)
                    {
                        if (checkRayLeftDistance < checkRayDistance) return;
                    }
                    else if (moveDirection.y > 0)
                    {
                        if (checkRayUpDistance < checkRayDistance) return;
                    }
                    else if (moveDirection.y < 0)
                    {
                        if (checkRayDownDistance < checkRayDistance) return;
                    }

                    transform.localPosition += mirrorManipulateSpeed * Time.deltaTime * (Vector3)moveDirection;
                }
                else
                {
                    transform.localRotation = MouseToRotation.GetMirrorRotation(mirrorManipulateSpeed * Time.deltaTime * new Vector2(-val.x, val.y), 10);
                }
            }
        }
    }

    public void Hold()
    {
        isHolding = true;
    }

    public void Release()
    {
        isHolding = false;
    }

    private void CheckColliders()
    {
        Vector3 center = meshRenderer.bounds.center;
        float scaleX = transform.localScale.x / 2;
        float scaleY = transform.localScale.y / 2;

        checkRayRight.origin = center + new Vector3(scaleX, 0, 0);
        checkRayRight.direction = transform.right;

        checkRayLeft.origin = center + new Vector3(-scaleX, 0, 0);
        checkRayLeft.direction = -transform.right;

        checkRayUp.origin = center + new Vector3(0, scaleY, 0);
        checkRayUp.direction = transform.up;

        checkRayDown.origin = center + new Vector3(0, -scaleY, 0);
        checkRayDown.direction = -transform.up;

        Physics.Raycast(checkRayRight, out RaycastHit hitInfoRight);
        Physics.Raycast(checkRayLeft, out RaycastHit hitInfoLeft);
        Physics.Raycast(checkRayUp, out RaycastHit hitInfoUp);
        Physics.Raycast(checkRayDown, out RaycastHit hitInfoDown);

        checkRayRightDistance = hitInfoRight.distance;
        checkRayLeftDistance = hitInfoLeft.distance;
        checkRayUpDistance = hitInfoUp.distance;
        checkRayDownDistance = hitInfoDown.distance;
    }

    private void OnDrawGizmos()
    {
        Debug.DrawRay(checkRayRight.origin, checkRayRight.direction * Mathf.Infinity, Color.cyan);
        Debug.DrawRay(checkRayLeft.origin, checkRayLeft.direction * Mathf.Infinity, Color.cyan);
        Debug.DrawRay(checkRayUp.origin, checkRayUp.direction * Mathf.Infinity, Color.cyan);
        Debug.DrawRay(checkRayDown.origin, checkRayDown.direction * Mathf.Infinity, Color.cyan);
    }
}
