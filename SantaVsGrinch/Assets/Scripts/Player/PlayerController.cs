using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private Rigidbody body;
    private Camera cam;

    private PlayerInput playerInput;
    public int GetPlayerId() { return playerInput.playerIndex; }
    
    [SerializeField] private float maxRotationSpeed = 10f;
    [SerializeField] private LayerMask groundLayerMask = default;
    [SerializeField] private GameObject projectilePrefab = default;
    [Tooltip("Fire rate in shots per second. \nExample: \n  '3' would fire three times per second while \n  '0.25' would fire once every four seconds.")]
    [SerializeField] private float fireRate = 2f;
    [SerializeField] private float firePower = 100f;
    
    private Vector3 aimInput;
    private bool isFiring;

    private float timeLastFired;

    private Vector3 mouseWorldPosition;
    
    private void Awake()
    {
        body = GetComponent<Rigidbody>();
        cam = Camera.main;
        playerInput = GetComponent<PlayerInput>();
    }

    #region Input

    public void SetAimInput(InputAction.CallbackContext context)
    {
        if (context.control.device.description.deviceClass.Equals("Mouse"))
        {
            Vector3 mouseInput = context.ReadValue<Vector2>();
            if (Physics.Raycast(cam.ScreenPointToRay(mouseInput), out RaycastHit hit, Mathf.Infinity, groundLayerMask.value))
            {
                mouseWorldPosition = hit.point;
            }
            aimInput = mouseWorldPosition - transform.position;
            aimInput.y = 0f;
            aimInput.Normalize();
        }
        else
        {
            Vector2 joystickInput = context.ReadValue<Vector2>();
            aimInput = new Vector3(joystickInput.x, 0f, joystickInput.y).normalized;
        }
    }

    public void SetFireInput(InputAction.CallbackContext context)
    {
        isFiring = context.performed;
    }
    
    #endregion

    private void Update()
    {
        Fire();
        Rotate();
    }

    private void Rotate()
    {
        if (mouseWorldPosition == Vector3.zero) return;
        
        aimInput = mouseWorldPosition - transform.position;
        aimInput.y = 0f;
        aimInput.Normalize();
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
            go.GetComponent<Projectile>().SetInstigator(playerInput.playerIndex);
            go.GetComponent<Projectile>().Fire(transform.forward * firePower);
            
            float selfKnockback = 1f;
            GetComponent<Knockbackable>().Knockback(-transform.forward * selfKnockback);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(mouseWorldPosition, 1f);
    }
}