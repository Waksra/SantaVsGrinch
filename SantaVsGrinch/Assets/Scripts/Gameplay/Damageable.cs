using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class Damageable : MonoBehaviour
{
    [SerializeField] public bool smashMode = false;
    [SerializeField] private bool isPlayer = false;

    [SerializeField] private UnityEvent<float> damageEvent;
    [SerializeField] private UnityEvent deathEvent;
    
    [SerializeField] private float maxHealth = 1f;
    private float health;
    public float Health
    {
        get { return health; }
        private set { health = value; }
    }

    private void Start()
    {
        Respawn();
    }
    
    public void TakeDamage(float damage)
    {
        if (Health <= 0f) return;
        
        damageEvent?.Invoke(damage);
        
        CameraController.AddTrauma(damage * 0.01f);
        
        if (smashMode)
        {
            Health += damage;
        }
        else
        {
            Health -= damage;
            if (Health <= 0f)
            {
                Health = 0f;
                Die();
            }
        }
    }

    public void Respawn()
    {
        if (smashMode)
            Health = 0;
        else
            Health = maxHealth;
    }

    public void Die()
    {
        deathEvent?.Invoke();
        if (isPlayer)
            FindObjectOfType<GameMode>().AddDeath(GetComponent<PlayerInput>().playerIndex);
    }
}
