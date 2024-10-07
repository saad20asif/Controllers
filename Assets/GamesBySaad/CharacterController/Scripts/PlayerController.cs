using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FablockGaming.FinalCharacterController
{
    public class PlayerController : MonoBehaviour
    {
        #region Class Variables
        [Header("Componenets")]
        [SerializeField] private Camera PlayerCamera;
        [SerializeField] private CharacterController CharacterController;

        [Header("Base Movement")]
        public float RunAcceleration = 0.25f;
        public float RunSpeed = 4f;
        public float SprintAcceleration = 0.5f;
        public float SprintSpeed = 7f;
        public float Drag = 0.1f;
        public float MovingThreshold = 0.01f;
        public float Gravity = 25f;
        public float JumpSpeed = 25f;

        [Header("Camera Settings")]
        public float LookSenseH = 0.1f;
        public float LookSenseV = 0.1f;
        public float LookLimitV = 89f;

        private PlayerLocomotionInput _playerLocomotionInput;

        private PlayerState _playerState;   

        private Vector2 _cameraRotation = Vector2.zero;
        private Vector2 _playerTargetRotation = Vector2.zero;

        private float _verticalvelocity = 0f;
        #endregion

        #region Startup Methods

        private void Awake()
        {
            _playerLocomotionInput = GetComponent<PlayerLocomotionInput>();
            _playerState = GetComponent<PlayerState>();
        }
        #endregion

        #region Update Logic
        private void Update()
        {
            UpdateMovementState();
            HandleVerticalMovement();
            HandleLateralMovement();
        }

        private void UpdateMovementState()
        {
            bool _isMovementInput = _playerLocomotionInput.MovementInput != Vector2.zero;
            bool _isMovementLaterally = IsMovingLaterally();
            bool _isSprinting = _playerLocomotionInput.SprintToggledOn && _isMovementLaterally;
            bool _isGrounded = IsGrounded();


            PlayerMovementState _lateralState = _isSprinting?PlayerMovementState.Sprinting :
                                                _isMovementLaterally || _isMovementInput? PlayerMovementState.Running : PlayerMovementState.Idling;
            _playerState.SetPlayerMovement(_lateralState);

            if(!_isGrounded && CharacterController.velocity.y >=0)
            {
                _playerState.SetPlayerMovement(PlayerMovementState.Jumping);
            }
            else if(!_isGrounded && CharacterController.velocity.y <0f)
            {
                _playerState.SetPlayerMovement(PlayerMovementState.Falling);
            }
        }
        private void HandleVerticalMovement()
        {
            bool _isGrounded = _playerState.InGroundedState();
            if (_isGrounded && _verticalvelocity < 0)
                _verticalvelocity = 0;
            _verticalvelocity -= Gravity*Time.deltaTime; // it means player is in the air so apply gravity 

            if(_playerLocomotionInput.JumpPressed && _isGrounded)
            {
                _verticalvelocity += Mathf.Sqrt(JumpSpeed * 3 * Gravity);
                print("Applying vertical velocity : " + _verticalvelocity);
            }
        }
        private void HandleLateralMovement()
        {
            
            bool _isSprinting = _playerState.CurrentPlayerMovementState == PlayerMovementState.Sprinting;
            bool _isGrounded = _playerState.InGroundedState();

            float _lateralAcceleration = _isSprinting ? SprintAcceleration : RunAcceleration; // State dependent acc and speed
            float _clampLateralMagitude = _isSprinting ? SprintSpeed : RunSpeed;

            Vector3 _cameraForwardXZ = new Vector3(PlayerCamera.transform.forward.x, 0f, PlayerCamera.transform.forward.z).normalized;//Represents the forward direction of the camera. This is used for forward/backward movement relative to the camera.
            Vector3 _cameraRightXZ = new Vector3(PlayerCamera.transform.right.x, 0f, PlayerCamera.transform.right.z).normalized;//Represents the right direction of the camera. This is used for left/right movement relative to the camera
            Vector3 _movementDirection = _cameraRightXZ * _playerLocomotionInput.MovementInput.x + _cameraForwardXZ * _playerLocomotionInput.MovementInput.y;

            Vector3 _movementDelta = _movementDirection * _lateralAcceleration * Time.deltaTime;

            Vector3 _newVelocity = CharacterController.velocity + _movementDelta;
            Vector3 _currentDrag = _newVelocity.normalized * Drag * Time.deltaTime;
            _newVelocity = (_newVelocity.magnitude > Drag * Time.deltaTime) ? _newVelocity - _currentDrag : Vector3.zero; // If the player's speed is greater than the drag effect, the velocity is reduced by drag. If not, the velocity is set to zero (player stops completely).
            _newVelocity = Vector3.ClampMagnitude(_newVelocity, _clampLateralMagitude);
            _newVelocity.y += _verticalvelocity;
            
            CharacterController.Move(_newVelocity * Time.deltaTime);
        }

        #endregion

        #region State Checks
        private bool IsMovingLaterally()
        {
            Vector3 _lateralVelocity = new Vector3(CharacterController.velocity.x, 0, CharacterController.velocity.z);
            return _lateralVelocity.magnitude > MovingThreshold;
        }
        private bool IsGrounded()
        {
            return CharacterController.isGrounded;
        }
        #endregion

        #region LateUpdate Logic
        private void LateUpdate()
        {
            _cameraRotation.x += LookSenseH * _playerLocomotionInput.LookInput.x;
            _cameraRotation.y = Mathf.Clamp(_cameraRotation.y - LookSenseV * _playerLocomotionInput.LookInput.y, -LookLimitV, LookLimitV);

            _playerTargetRotation.x += transform.eulerAngles.x + LookSenseH * _playerLocomotionInput.LookInput.x;
            transform.rotation = Quaternion.Euler(0f, _playerTargetRotation.x, 0f);

            PlayerCamera.transform.rotation = Quaternion.Euler(_cameraRotation.y, _cameraRotation.x, 0f);

        }
        #endregion;
    }
}
