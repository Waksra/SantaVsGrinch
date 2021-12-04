using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private Rigidbody body;
    private Camera cam;
    
    [SerializeField] private float maxRotationSpeed = 10f;
    [SerializeField] private LayerMask groundLayerMask = default;
    [SerializeField] private GameObject projectilePrefab = default;
    [Tooltip("Fire rate in shots per second. \nExample: \n  '3' would fire three times per second while \n  '0.25' would fire once every four seconds.")]
    [SerializeField] private float fireRate = 2f;
    [SerializeField] private float firePower = 100f;
    
    private Vector3 aimInput;
    private bool isFiring;

    private float timeLastFired;

    private void Awake()
    {
        body = GetComponent<Rigidbody>();
        cam = Camera.main;
    }

    #region Input

    public void SetAimInput(InputAction.CallbackContext context)
    {
        //TODO: Figure out which device is being used and set aimInput accordingly

        Vector3 mouseInput = context.ReadValue<Vector2>();
        if (Physics.Raycast(cam.ScreenPointToRay(mouseInput), out RaycastHit hit, Mathf.Infinity, groundLayerMask))
        {
            mouseInput.z = hit.distance;
        }
        aimInput = cam.ScreenToWorldPoint(mouseInput) - transform.position;
        aimInput.y = 0f;
        aimInput.Normalize();
    }

    public void SetFireInput(InputAction.CallbackContext context)
    {
        isFiring = context.performed;
    }
    
    #endregion

    private void Update()
    {
        Fire();
    }

    private void FixedUpdate()
    {
        if (aimInput == Vector3.zero) return;
        Vector3 targetDirection = aimInput;
        Quaternion targetRotation = Quaternion.LookRotation(targetDirection, Vector3.up);
        targetRotation = Quaternion.RotateTowards(transform.rotation, targetRotation, maxRotationSpeed);
        body.MoveRotation(targetRotation);
    }

    private void Fire()
    {
        if (!isFiring) return;
        if (timeLastFired + 1f / fireRate <= Time.time)
        {
            timeLastFired = Time.time;
            GameObject go = Instantiate(projectilePrefab, transform.position + aimInput, Quaternion.Euler(transform.forward));
            go.GetComponent<Projectile>().Fire(transform.forward * firePower);
        }
    }
}
