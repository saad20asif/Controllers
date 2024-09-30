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

        private static int inputXHash = Animator.StringToHash("inputX");
        private static int inputYHash = Animator.StringToHash("inputY");

        private Vector3 _currentBlendInput = Vector3.zero;

        private void Awake()
        {
            _playerLocomotionInput = GetComponent<PlayerLocomotionInput>(); 
        }

        private void Update()
        {
            UpdateAnimationState();
        }

        private void UpdateAnimationState()
        {
            Vector2 _inputTarget = _playerLocomotionInput.MovementInput;
            _currentBlendInput = Vector3.Lerp(_currentBlendInput, _inputTarget, LocomotionBlendSpeed * Time.deltaTime);
            print("_inputTarget : " + _inputTarget);
            Animator.SetFloat(inputXHash, _currentBlendInput.x);
            Animator.SetFloat(inputYHash, _currentBlendInput.y);
        }
    }
}
