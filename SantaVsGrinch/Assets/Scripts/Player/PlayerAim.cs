using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    public class PlayerAim : MonoBehaviour
    {
        [SerializeField, Range(0, 50)] private float maxRotationSpeed = 50f;
        [SerializeField] private LayerMask aimLayer;
        
        private new Camera camera;
        private Rigidbody body;
        
        private Vector3 aimInput;
        private Vector3 mouseWorldPosition;
        private bool isMouseAim;

        private void Awake()
        {
            camera = Camera.main;
            body = GetComponent<Rigidbody>();
        }

        private void FixedUpdate()
        {
            if(isMouseAim)
                CalculateAimFromMouse();
            
            HandleRotation();
        }

        public void SetAimInput(InputAction.CallbackContext context)
        {
            if (context.control.device.description.deviceClass.Equals("Mouse"))
            {
                Vector3 mouseInput = context.ReadValue<Vector2>();
                if (!Physics.Raycast(camera.ScreenPointToRay(mouseInput), out RaycastHit hit, Mathf.Infinity, aimLayer))
                    return;

                mouseWorldPosition = hit.point;
                isMouseAim = true;
                CalculateAimFromMouse();
            }
            else
            {
                Vector2 joystickInput = context.ReadValue<Vector2>();
                aimInput = new Vector3(joystickInput.x, 0f, joystickInput.y).normalized;
                isMouseAim = false;
            }
        }
        
        private void HandleRotation()
        {
            if (aimInput == Vector3.zero) return;
            Vector3 targetDirection = aimInput;
            Quaternion targetRotation = Quaternion.LookRotation(targetDirection, Vector3.up);
            targetRotation = Quaternion.RotateTowards(transform.rotation, targetRotation, maxRotationSpeed);
            body.MoveRotation(targetRotation);
        }
        
        private void CalculateAimFromMouse()
        {
            aimInput = mouseWorldPosition - transform.position;
            aimInput.y = 0f;
            aimInput.Normalize();
        }
    }
}