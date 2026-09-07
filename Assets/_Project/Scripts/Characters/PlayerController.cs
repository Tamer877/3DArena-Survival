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

            // Fizik motoru kaynaklı devrilmeleri önlemek için rotasyon eksenlerini kilitliyoruz
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
            // Basit ve temiz girdi okuma (Legacy input veya yeni input wrapper bağlanabilir)
            float horizontal = Input.GetAxisRaw("Horizontal");
            float vertical = Input.GetAxisRaw("Vertical");

            // Çapraz yürürken hızın 1.41 katına çıkmaması için normalize ediyoruz
            _movementInput = new Vector3(horizontal, 0f, vertical).normalized;
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
            Vector3 targetPosition = _rb.position + _movementInput * (moveSpeed * Time.fixedDeltaTime);
            _rb.MovePosition(targetPosition);
        }

        private void ApplyRotation()
        {
            Vector3 lookDirection = _aimPoint - _rb.position;
            lookDirection.y = 0f; // Sadece yatay eksende dönme

            if (lookDirection.sqrMagnitude > 0.001f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(lookDirection);
                _rb.MoveRotation(targetRotation);
            }
        }
    }
}