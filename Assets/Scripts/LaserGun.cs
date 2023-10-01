using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using UnityEngine.InputSystem;

public class LaserGun : MonoBehaviour
{
    public CrosshairPos crosshairPos;
    public Transform laserOriginPoint;
    public LineRenderer lineRenderer;
    public int mirrorCount = 3;
    private int defaultMirrorCount = 1;

    Ray ray;
    RaycastHit hit;

    private PlayerInput playerInput;
    private InputAction laserHoldAction;
    private bool laserHold = false;
    private Vector3 laserHoldPoint;

    public Camera mirrorCam;
    private InputAction switchMirrorCam;
    private int minCamPosition = 2;
    private int camLookAt;
    private int maxCamPosition;
    private int currentCamPosition;

    private void Awake()
    {
        playerInput = FindObjectOfType<PlayerInput>();
    }

    void Start()
    {
        lineRenderer.enabled = true;
        lineRenderer.useWorldSpace = true;

        laserHoldAction = playerInput.actions["LaserHold"];
        laserHoldAction.started += _ => setLaserHold();

        switchMirrorCam = playerInput.actions["SwitchMirrorCam"];
        switchMirrorCam.performed += _ => SwitchMirrorCam();

        //currentCamPosition = minCamPosition;
    }

    void Update()
    {
        lineRenderer.positionCount = 1;
        lineRenderer.SetPosition(0, laserOriginPoint.position);

        if (laserHold)
        {
            ray.origin = laserOriginPoint.position;
            ray.direction = laserHoldPoint - ray.origin;
        }
        else
        {
            ray.origin = Camera.main.transform.position;
            ray.direction = crosshairPos.transform.position - ray.origin;
        }

        for (int i = 0; i < mirrorCount; i++)
        {
            if (Physics.Raycast(ray, out hit))
            {
                lineRenderer.positionCount++;
                lineRenderer.SetPosition(lineRenderer.positionCount - 1, hit.point);

                ray = new Ray(hit.point, Vector3.Reflect(ray.direction, hit.normal));

                if (!hit.collider.gameObject.CompareTag("Mirror"))
                    break;
            }
            //else
            //{
                lineRenderer.positionCount++;
                lineRenderer.SetPosition(lineRenderer.positionCount - 1, ray.origin + ray.direction);
            //}
        }

        if (currentCamPosition !=null && camLookAt != null)
        {
            mirrorCam.transform.position = lineRenderer.GetPosition(currentCamPosition);
            mirrorCam.transform.LookAt(lineRenderer.GetPosition(camLookAt));
        }
    }

    private void setLaserHold()
    {
        if (!laserHold)
        {
            laserHoldPoint = Camera.main.transform.position;
            laserHold = true;
        }
        else
            laserHold = false;
    }

    private void SwitchMirrorCam()
    {
        maxCamPosition = lineRenderer.positionCount;

        if (currentCamPosition > maxCamPosition + 1)
        {
            currentCamPosition = (currentCamPosition + 1) % (maxCamPosition + 1);

            
            camLookAt = currentCamPosition + 1;
        }
        Debug.Log("current cam pos : " + currentCamPosition);
    }

    private void OnDrawGizmos()
    {
        Debug.DrawRay(ray.origin, ray.direction, Color.red);
    }
}
