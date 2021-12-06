using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    public class Movement : MonoBehaviour
    {
        [SerializeField, Range(0, 50)] private float maxSpeed = 10f;
        [SerializeField, Range(0, 50)] private float maxAcceleration = 10f;
        [SerializeField, Range(0, 90)] private float maxAngle;
        
        [Space(10)]
        
        [SerializeField, Range(0, 100)] private float dashMaxSpeed = 20f;
        [SerializeField, Range(0, 500)] private float dashAcceleration = 200f;
        [SerializeField, Range(0, 5)] private float dashCooldown = 0.75f;
        [SerializeField, Range(0, 2)] private float dashRequestBuffer = 0.5f;
        [SerializeField] private AnimationCurve dashCurve;

        private Rigidbody body;

        private Vector2 inputVector;
        private Vector2 lastInput;

        private Vector3 velocity;

        private float minGroundDot;
        private int contactCount;
        private Vector3 groundNormal;

        private bool dashRequested;
        private bool isDashing;
        private Vector3 dashDirection;
        private float dashRequestEnd;
        private float dashTime;
        private float nextDashTime;

        private bool IsGround => contactCount > 0;

        public float GetMaxAcceleration() => maxAcceleration;

        #region Input

        public void SetMove(InputAction.CallbackContext context)
        {
            inputVector = context.ReadValue<Vector2>();
        }

        public void RequestDash(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                dashRequested = true;
                dashRequestEnd = dashRequestBuffer + Time.time;
            }
        }

        #endregion

        private void OnValidate()
        {
            minGroundDot = Mathf.Cos(maxAngle * Mathf.Deg2Rad);
        }

        private void Awake()
        {
            OnValidate();
            body = GetComponent<Rigidbody>();
            
            if(dashCurve.length == 0)
                Debug.LogError(gameObject.name + " has no dash curve set.");
        }

        private void FixedUpdate()
        {
            UpdateState();

            if (isDashing)
                Dash();
            else
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

        private void Dash()
        {
            float curveEvaluation = dashCurve.Evaluate(dashTime);
            Vector3 desiredVelocity = dashDirection * Mathf.LerpUnclamped(maxSpeed, dashMaxSpeed, curveEvaluation);

            velocity = Vector3.MoveTowards(velocity, desiredVelocity, dashAcceleration * Time.deltaTime);
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

            if (dashCurve.length == 0)
                return;

            if (isDashing)
            {
                dashTime += Time.deltaTime;
            
                if (dashTime <= dashCurve[dashCurve.length - 1].time)
                    return;

                isDashing = false;
                nextDashTime = Time.time + dashCooldown;
            }
            
            if (dashRequested && !isDashing && Time.time >= nextDashTime)
            {
                isDashing = true;
                dashRequested = false;
                Vector2 direction = inputVector.sqrMagnitude != 0 ? inputVector : lastInput;
                dashDirection = ProjectDirectionGround(new Vector3(direction.x, 0, direction.y));
                dashTime = 0f;
            }
            else if (dashRequested && dashRequestEnd >= Time.time)
            {
                dashRequested = false;
            }
        }

        private void ClearState()
        {
            groundNormal = Vector3.zero;
            contactCount = 0;
            
            if (inputVector.sqrMagnitude != 0)
                lastInput = inputVector;
        }

        private void HandleCollision(Collision collision)
        {
            for (int i = 0; i < collision.contactCount; i++)
            {
                Vector3 normal = collision.GetContact(i).normal;

                float upDot = Vector3.Dot(Vector3.up, normal);
                if(upDot < minGroundDot)
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