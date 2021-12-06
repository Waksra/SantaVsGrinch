using UnityEngine;

public class Knockbackable : MonoBehaviour
{
    private Rigidbody body;
    private Damageable damageable;

    private void Awake()
    {
        body = GetComponent<Rigidbody>();
        damageable = GetComponent<Damageable>();
    }

    public void Knockback(Vector3 force)
    {
        float knockback = damageable.Health < 100f ? 1f : damageable.Health * 0.01f;
        body.AddForce(force * knockback, ForceMode.Impulse);
    }

    public void KnockbackConstant(Vector3 force)
    {
        float knockback = damageable.Health < 100f ? 1f : damageable.Health * 0.01f;
        body.AddForce(force * knockback, ForceMode.Acceleration);
    }
}
