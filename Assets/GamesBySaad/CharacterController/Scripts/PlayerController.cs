using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FablockGaming.FinalCharacterController
{
    public class PlayerController : MonoBehaviour
    {
        [Header("Componenets")]
        [SerializeField] private Camera PlayerCamera;
        [SerializeField] private CharacterController CharacterController;

        [Header("Base Movement")]
        public float RunAcceleration = 0.25f;
        public float RunSpeed = 4f;
        public float Drag = 0.1f;

        [Header("Camera Settings")]
        public float LookSenseH = 0.1f;
        public float LookSenseV = 0.1f;
        public float LookLimitV = 89f;

        private PlayerLocomotionInput _playerLocomotionInput;

        private Vector2 _cameraRotation = Vector2.zero;
        private Vector2 _playerTargetRotation = Vector2.zero;

        private void Awake()
        {
            _playerLocomotionInput = GetComponent<PlayerLocomotionInput>();
        }
        private void Update()
        {
            Vector3 _cameraForwardXZ = new Vector3(PlayerCamera.transform.forward.x, 0f, PlayerCamera.transform.forward.z).normalized;//Represents the forward direction of the camera. This is used for forward/backward movement relative to the camera.
            Vector3 _cameraRightXZ = new Vector3(PlayerCamera.transform.right.x, 0f, PlayerCamera.transform.right.z).normalized;//Represents the right direction of the camera. This is used for left/right movement relative to the camera
            Vector3 _movementDirection = _cameraRightXZ * _playerLocomotionInput.MovementInput.x + _cameraForwardXZ * _playerLocomotionInput.MovementInput.y;
            
            Vector3 _movementDelta = _movementDirection*RunAcceleration*Time.deltaTime;

            Vector3 _newVelocity = CharacterController.velocity + _movementDelta;
            Vector3 _currentDrag = _newVelocity.normalized * Drag * Time.deltaTime;
            _newVelocity = (_newVelocity.magnitude > Drag * Time.deltaTime) ? _newVelocity - _currentDrag : Vector3.zero; // If the player's speed is greater than the drag effect, the velocity is reduced by drag. If not, the velocity is set to zero (player stops completely).
            _newVelocity = Vector3.ClampMagnitude(_newVelocity, RunSpeed);

            CharacterController.Move(_newVelocity*Time.deltaTime);
        }
        private void LateUpdate()
        {
            _cameraRotation.x += LookSenseH * _playerLocomotionInput.LookInput.x;
            _cameraRotation.y = Mathf.Clamp(_cameraRotation.y - LookSenseV * _playerLocomotionInput.LookInput.y, -LookLimitV, LookLimitV);

            _playerTargetRotation.x += transform.eulerAngles.x + LookSenseH * _playerLocomotionInput.LookInput.x;
            transform.rotation = Quaternion.Euler(0f, _playerTargetRotation.x, 0f);

            PlayerCamera.transform.rotation = Quaternion.Euler(_cameraRotation.y, _cameraRotation.x, 0f);

        }
    }
}
