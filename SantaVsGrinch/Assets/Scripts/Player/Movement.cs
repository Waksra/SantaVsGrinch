using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    public class Movement : MonoBehaviour
    {
        [SerializeField, Range(0, 50)] private float maxSpeed = 10f;
        [SerializeField, Range(0, 50)] private float maxDeepSnowSpeed = 5f;
        [SerializeField, Range(0, 50)] private float maxAcceleration = 10f;
        [SerializeField, Range(0, 50)] private float maxDeepSnowAcceleration = 15f;
        [SerializeField, Range(0, 50)] private float maxIceAcceleration = 5f;
        [SerializeField, Range(0, 90)] private float maxAngle;
        [SerializeField] private LayerMask groundLayer = -1;
        [SerializeField] private LayerMask iceLayer = -1;
        [SerializeField] private LayerMask deepSnowLayer = 0;

        [SerializeField] private LayerMask probeMask = -1;
        
        [SerializeField, Range(0, 100)] private float maxSnapSpeed = 35f;
        [SerializeField, Range(0, 10)] private float probeDistance = 1f;

        [Space(10)]
        
        [SerializeField, Range(0, 100)] private float dashMaxSpeed = 20f;
        [SerializeField, Range(0, 500)] private float dashAcceleration = 200f;
        [SerializeField, Range(0, 5)] private float dashCooldown = 0.75f;
        [SerializeField, Range(0, 2)] private float dashRequestBuffer = 0.5f;
        [SerializeField] private AnimationCurve dashCurve;

        private AnimatorHelper animatorHelper;
        
        private Rigidbody body;

        private Vector2 inputVector;
        private Vector2 lastInput;

        private Vector3 velocity;

        private float minGroundDot;
        private int contactCount;
        private Vector3 groundNormal;
        private bool onIce;
        private bool inDeepSnow;

        private int stepsSinceLastGrounded;

        private bool dashRequested;
        private bool isDashing;
        private Vector3 dashDirection;
        private float dashRequestEnd;
        private float dashTime;
        private float nextDashTime;

        private bool OnGround => contactCount > 0;

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
            animatorHelper = GetComponent<AnimatorHelper>();
            
            if(dashCurve.length == 0)
                Debug.LogError(gameObject.name + " has no dash curve set.");
        }

        private void Update()
        {
            float speed = velocity.magnitude / maxSpeed;
            animatorHelper.SetAnimatorFloat("Speed", speed);
        }

        private void FixedUpdate()
        {
            UpdateState();

            Debug.Log(onIce);
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

            float speed = maxSpeed;
            float acceleration = maxAcceleration;
            if (inDeepSnow)
            {
                speed = maxDeepSnowSpeed;
                acceleration = maxDeepSnowAcceleration;
            }
            else if (onIce)
                acceleration = maxIceAcceleration;

            Vector2 adjustment;
            adjustment.x
                = inputVector.x * speed - Vector3.Dot(velocity, xAxis);
            adjustment.y
                = inputVector.y * speed - Vector3.Dot(velocity, zAxis);

            adjustment = Vector3.ClampMagnitude(adjustment, acceleration * Time.deltaTime);

            velocity += xAxis * adjustment.x + zAxis * adjustment.y;
        }

        private void Dash()
        {
            float curveEvaluation = dashCurve.Evaluate(dashTime);
            Vector3 desiredVelocity = ProjectDirectionGround(dashDirection) * Mathf.LerpUnclamped(maxSpeed, dashMaxSpeed, curveEvaluation);

            velocity = Vector3.MoveTowards(velocity, desiredVelocity, dashAcceleration * Time.deltaTime);
        }

        private void UpdateState()
        {
            velocity = body.velocity;
            stepsSinceLastGrounded += 1;
            
            if (OnGround || SnapToGround())
            {
                stepsSinceLastGrounded = 0;
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
            onIce = false;
            inDeepSnow = false;
            
            if (inputVector.sqrMagnitude != 0)
                lastInput = inputVector;
        }

        private bool SnapToGround()
        {
            if (stepsSinceLastGrounded > 1)
                return false;
            
            float speed = velocity.magnitude;
            if (speed > maxSnapSpeed)
                return false;
            
            if (!Physics.Raycast(body.position, Vector3.down, out RaycastHit hit, probeDistance, probeMask))
                return false;
            
            if (hit.normal.y < minGroundDot)
                return false;

            contactCount = 1;
            groundNormal = hit.normal;
            float dot = Vector3.Dot(velocity, hit.normal);
            if(dot > 0f)
                velocity = (velocity - hit.normal * dot).normalized * speed;
            return true;
        }

        private void HandleCollision(Collision collision)
        {
            int layer = collision.gameObject.layer;
            if((groundLayer & (1 << layer)) == 0)
                return;
            
            for (int i = 0; i < collision.contactCount; i++)
            {
                Vector3 normal = collision.GetContact(i).normal;

                float upDot = Vector3.Dot(Vector3.up, normal);
                if(upDot < minGroundDot)
                    continue;

                if ((deepSnowLayer & (1 << layer)) != 0)
                    inDeepSnow = true;
                else if ((iceLayer & (1 << layer)) != 0)
                    onIce = true;
                
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

        private void OnTriggerStay(Collider other)
        {
            if ((deepSnowLayer & (1 << other.gameObject.layer)) != 0)
                inDeepSnow = true;
        }

        Vector3 ProjectDirectionGround(Vector3 direction)
        {
            return (direction - groundNormal * Vector3.Dot(direction, groundNormal)).normalized;
        }
    }
}