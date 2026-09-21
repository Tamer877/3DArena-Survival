using UnityEngine;

namespace ArenaSurvival.Characters
{
    [RequireComponent(typeof(Rigidbody))]
    public class PlayerController : MonoBehaviour
    {
        [Header("Movement")]
        [SerializeField] private float moveSpeed = 8f;

        [Header("Aiming")]
        [SerializeField] private LayerMask groundLayer;

        private Rigidbody _rb;
        private Camera _mainCamera;
        private Vector3 _movementInput;
        private Vector3 _aimPoint;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody>();
            _mainCamera = Camera.main;

            // rotasyonu fizik motoru değil biz yöneteceğiz
            _rb.freezeRotation = true;
        }

        private void Update()
        {
            ReadMovementInput();
            CalculateAimPoint();
        }

        private void FixedUpdate()
        {
            ApplyMovement();
            ApplyRotation();
        }

        private void ReadMovementInput()
        {
            float horizontal = Input.GetAxisRaw("Horizontal");
            float vertical = Input.GetAxisRaw("Vertical");
            _movementInput = new Vector3(horizontal, 0f, vertical).normalized; // normalize: çapraz gidince hız artmasın
        }

        private void CalculateAimPoint()
        {
            Ray ray = _mainCamera.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hitInfo, 100f, groundLayer))
            {
                _aimPoint = hitInfo.point;
            }
        }

        private void ApplyMovement()
        {
            // MovePosition çarpışmaları korur, transform.position dogrudan atamak korumaz
            Vector3 targetPosition = _rb.position + _movementInput * (moveSpeed * Time.fixedDeltaTime);
            _rb.MovePosition(targetPosition);
        }

        private void ApplyRotation()
        {
            Vector3 lookDirection = _aimPoint - _rb.position;
            lookDirection.y = 0f;

            if (lookDirection.sqrMagnitude > 0.001f) // sıfır vektörde LookRotation NaN üretir
            {
                Quaternion targetRotation = Quaternion.LookRotation(lookDirection);
                _rb.MoveRotation(targetRotation);
            }
        }
    }
}