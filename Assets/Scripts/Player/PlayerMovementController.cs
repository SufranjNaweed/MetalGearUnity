
using UnityEngine;

namespace StealthGame
{
    [SelectionBase]
    public class PlayerMovementController : MonoBehaviour
    {
        #region Show in inspector

        [Header("Controllers")]

        [SerializeField] private PlayerInputController _inputController;

        [Header("Movement parameters")]
        [SerializeField] private float _walkSpeed;
        [SerializeField] private float _runSpeed;
        [SerializeField] private float _sneakSpeed;
        [SerializeField] private float _turnSpeed;

        #endregion


        #region Init

        private void Awake()
        {
            _transform = GetComponent<Transform>();
            _rigidbody = GetComponent<Rigidbody>();
            _cameraTransform = Camera.main.transform;
        }


        #endregion


        #region Update

        private void Update()
        {
            _movementDirection = _transform.TransformDirection(_inputController.MovementInput);
            _movementDirection.y = 0;

            SetSpeed();
        }

        private void SetSpeed()
        {
            if (_inputController.SneakInput.IsActive)
            {
                _currentSpeed = _sneakSpeed;
            }
            else if (_inputController.RunInput.IsActive)
            {
                _currentSpeed = _runSpeed;
            }
            else
            {
                _currentSpeed = _walkSpeed;
            }
        }

        private void FixedUpdate()
        {
            // Déplace le joueur selon son référentiel
            Move();

            // Tourne le joueur vers l'orientation de la caméra
            RotateTowardsCameraForward();
        }

        private void Move()
        {
            Vector3 velocity = _movementDirection * _currentSpeed;
            _rigidbody.velocity = velocity;
        }

        private void RotateTowardsCameraForward()
        {
            if (_inputController.HasMovementInput)
            {
                Vector3 lookDirection = _cameraTransform.forward;
                lookDirection.y = 0;
                Quaternion rotation = Quaternion.LookRotation(lookDirection);
                rotation = Quaternion.RotateTowards(_rigidbody.rotation, rotation, _turnSpeed * Time.fixedDeltaTime);
                _rigidbody.MoveRotation(rotation);
            }
        }

        #endregion


        #region Private

        private Transform _transform;
        private Transform _cameraTransform;
        private Rigidbody _rigidbody;

        private Vector3 _movementDirection;
        private float _currentSpeed;

        #endregion
    }
}