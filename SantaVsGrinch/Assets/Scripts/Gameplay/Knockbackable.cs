using UnityEngine;

public class Knockbackable : MonoBehaviour
{
    private Rigidbody body;

    private void Awake()
    {
        body = GetComponent<Rigidbody>();
    }

    public void Knockback(Vector3 force)
    {
        body.AddForce(force, ForceMode.Impulse);
    }
}
