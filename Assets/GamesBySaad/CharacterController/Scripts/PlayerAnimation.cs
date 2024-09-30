using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FablockGaming.FinalCharacterController
{
    public class PlayerAnimation : MonoBehaviour
    {
        [SerializeField] private Animator Animator;
        private PlayerLocomotionInput _playerLocomotionInput;

        private static int inputXHash = Animator.StringToHash("inputX");
        private static int inputYHash = Animator.StringToHash("inputY");

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
            print("_inputTarget : " + _inputTarget);
            Animator.SetFloat(inputXHash, _inputTarget.x);
            Animator.SetFloat(inputYHash, _inputTarget.y);
        }
    }
}
