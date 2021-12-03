using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    public class Movement : MonoBehaviour
    {
        [SerializeField, Range(0, 50)] private float maxSpeed = 10f;
        [SerializeField, Range(0, 50)] private float maxAcceleration = 10f;
        [SerializeField] private float maxAngle;

        private Rigidbody body;
        private Vector2 inputVector;

        private Vector3 velocity;

        private float minDot;
        private int contactCount;
        private Vector3 groundNormal;

        private bool IsGround => contactCount > 0;

        public void SetMove(InputAction.CallbackContext context)
        {
            inputVector = context.ReadValue<Vector2>();
        }

        private void OnValidate()
        {
            minDot = Mathf.Cos(maxAngle * Mathf.Deg2Rad);
        }

        private void Awake()
        {
            body = GetComponent<Rigidbody>();
            OnValidate();
        }

        private void FixedUpdate()
        {
            UpdateState();
            AdjustVelocity();

            body.velocity = velocity;
            ClearState();
        }

        private void AdjustVelocity()
        {
            Vector3 xAxis = ProjectDirectionGround(Vector3.right);
            Vector3 zAxis = ProjectDirectionGround(Vector3.forward);

            Vector2 adjustment;
            adjustment.x
                = inputVector.x * maxSpeed - Vector3.Dot(velocity, xAxis);
            adjustment.y
                = inputVector.y * maxSpeed - Vector3.Dot(velocity, zAxis);

            adjustment = Vector3.ClampMagnitude(adjustment, maxAcceleration * Time.deltaTime);

            velocity += xAxis * adjustment.x + zAxis * adjustment.y;
        }

        private void UpdateState()
        {
            velocity = body.velocity;
            
            if (IsGround)
            {
                if(contactCount > 1)
                    groundNormal.Normalize();
            }
            else
            {
                groundNormal = Vector3.up;
            }
        }

        private void ClearState()
        {
            groundNormal = Vector3.zero;
            contactCount = 0;
        }

        private void HandleCollision(Collision collision)
        {
            for (int i = 0; i < collision.contactCount; i++)
            {
                Vector3 normal = collision.GetContact(i).normal;

                float upDot = Vector3.Dot(Vector3.up, normal);
                if(upDot < minDot)
                    continue;

                groundNormal += normal;
                contactCount++;
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            HandleCollision(collision);
        }

        private void OnCollisionStay(Collision collision)
        {
            HandleCollision(collision);
        }

        Vector3 ProjectDirectionGround(Vector3 direction)
        {
            return (direction - groundNormal * Vector3.Dot(direction, groundNormal)).normalized;
        }
    }
}