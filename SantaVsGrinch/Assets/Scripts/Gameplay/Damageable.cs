using UnityEngine;
using UnityEngine.Events;

public class Damageable : MonoBehaviour
{
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
        Health = maxHealth;
    }
    
    public void TakeDamage(float damage)
    {
        Health -= damage;
        if (Health <= 0f)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log($"{gameObject.name} died.");
        deathEvent.Invoke(GetComponent<PlayerController>().GetPlayerId());
    }
}
