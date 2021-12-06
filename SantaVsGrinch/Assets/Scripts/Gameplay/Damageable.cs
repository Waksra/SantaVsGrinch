using UnityEngine;
using UnityEngine.Events;

public class Damageable : MonoBehaviour
{
    [SerializeField] private bool smashMode = false;
    
    [SerializeField] private UnityEvent<int> deathEvent;
    
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
        if (smashMode)
        {
            Health += damage;
        }
        else
        {
            Health -= damage;
            if (Health <= 0f)
            {
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
        Debug.Log($"{gameObject.name} died.");
        deathEvent.Invoke(GetComponent<PlayerController>().GetPlayerId());
        GameObject.FindObjectOfType<GameMode>().LoseLife(GetComponent<PlayerController>().GetPlayerId());
    }
}
