using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{

    private PlayerInput playerInput;
    public int GetPlayerId() { return playerInput.playerIndex; }
    
    [SerializeField] private GameObject projectilePrefab = default;
    [Tooltip("Fire rate in shots per second. \nExample: \n  '3' would fire three times per second while \n  '0.25' would fire once every four seconds.")]
    [SerializeField] private float fireRate = 2f;
    [SerializeField] private float firePower = 100f;
    
    private Vector3 aimInput;
    private bool isFiring;

    private float timeLastFired;
    
    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
    }

    #region Input

    public void SetFireInput(InputAction.CallbackContext context)
    {
        isFiring = context.performed;
    }
    
    #endregion

    private void Update()
    {
        Fire();
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
}
