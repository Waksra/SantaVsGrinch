using Sirenix.OdinInspector;
using UnityEngine;

public class ForceField : MonoBehaviour
{
    [SerializeField] private bool onAwake = true;
    [SerializeField, Range(-50f, 50f)] private float strength = 1f;
    [SerializeField, Range(0f, 360f)] private float angle = 0f;
    [SerializeField] private ForceFieldType forceFieldType = ForceFieldType.Sphere;
    
    [ShowIfGroup("Sphere", Condition = "forceFieldType", Value = ForceFieldType.Sphere)]
    [SerializeField] private float radius = 5f;
    [ShowIfGroup("Box", Condition = "forceFieldType", Value = ForceFieldType.Box)]
    [SerializeField] private Vector3 box = Vector3.one * 10f;
    
    [SerializeField] private AnimationCurve forceStrengthCurve = AnimationCurve.Linear(0f, 1f, 1f, 0f);
    [SerializeField] private LayerMask layerMask = default;

    private bool isActive;
    public void SetIsActive(bool value) => isActive = value;

    private void Awake()
    {
        if (onAwake)
            isActive = true;
    }

    private void FixedUpdate()
    {
        if (isActive)
            AddForce();
    }

    private void AddForce()
    {
        Collider[] colliders;
        if (forceFieldType == ForceFieldType.Sphere)
            colliders = Physics.OverlapSphere(transform.position, radius, layerMask);
        else
            colliders = Physics.OverlapBox(transform.position, box * 0.5f, transform.rotation, layerMask);

        if (colliders.Length <= 0) return;

        foreach (var collider in colliders)
        {
            Vector3 vector = collider.transform.position - transform.position;
            float knockbackMagnitude = strength * forceStrengthCurve.Evaluate(vector.magnitude / radius);
            Vector3 dir = new Vector3(vector.x, 0f, vector.z).normalized;
            collider.GetComponent<Knockbackable>().KnockbackConstant(dir * knockbackMagnitude);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        if (forceFieldType == ForceFieldType.Sphere)
        {
            Gizmos.DrawWireSphere(transform.position, radius);
        }
        else
        {
            Gizmos.DrawWireCube(transform.position, box);
        }
    }
}

public enum ForceFieldType
{
    Sphere,
    Box
}
