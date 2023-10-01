using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrosshairPos : MonoBehaviour
{
    Camera mainCamera;
    Ray ray;
    RaycastHit hit;

    public GameObject hitCollider;

    void Start()
    {
        mainCamera = Camera.main;
    }

    void Update()
    {
        ray.origin = mainCamera.transform.position;
        ray.direction = mainCamera.transform.forward;

        if (Physics.Raycast(ray, out hit))
        {
            transform.position = hit.point;
            hitCollider = hit.collider.gameObject;
        }
        else
        {
            transform.position = ray.origin + ray.direction * 1000.0f;
        }
    }

    private void OnDrawGizmos()
    {
        Debug.DrawRay(ray.origin, ray.direction, Color.blue);
    }
}