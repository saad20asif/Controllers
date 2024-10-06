using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FablockGaming.FinalCharacterController
{
    public class PlayerAnimation : MonoBehaviour
    {
        [SerializeField] private Animator Animator;
        [SerializeField] private float LocomotionBlendSpeed = 0.2f;

        private PlayerLocomotionInput _playerLocomotionInput;
        private PlayerState _playerState;

        private static int inputXHash = Animator.StringToHash("inputX");
        private static int inputYHash = Animator.StringToHash("inputY");
        private static int inputMagnitudeHash = Animator.StringToHash("inputMagnitude");

        private Vector3 _currentBlendInput = Vector3.zero;

        private void Awake()
        {
            _playerLocomotionInput = GetComponent<PlayerLocomotionInput>(); 
            _playerState = GetComponent<PlayerState>();
        }

        private void Update()
        {
            UpdateAnimationState();
        }

        private void UpdateAnimationState()
        {
            bool _isSprinting = _playerState.CurrentPlayerMovementState == PlayerMovementState.Sprinting;

            Vector2 _inputTarget = _isSprinting? _playerLocomotionInput.MovementInput*1.5f : _playerLocomotionInput.MovementInput;
            _currentBlendInput = Vector3.Lerp(_currentBlendInput, _inputTarget, LocomotionBlendSpeed * Time.deltaTime);

            Animator.SetFloat(inputXHash, _currentBlendInput.x);
            Animator.SetFloat(inputYHash, _currentBlendInput.y);
            Animator.SetFloat(inputMagnitudeHash, _currentBlendInput.magnitude);
        }
    }
}
