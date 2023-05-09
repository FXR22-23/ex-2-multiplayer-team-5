using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class AnimationController : MonoBehaviour
{
    [SerializeField] private InputActionReference move;
    [SerializeField] private Animator _animator;

    private void OnEnable()
    {
        move.action.started += AnimationLegs;
        move.action.canceled += StopAnimation;
    }

    private void OnDisable()
    {
        move.action.started -= AnimationLegs;
        move.action.canceled -= StopAnimation;
    }

    private void AnimationLegs(InputAction.CallbackContext obj)
    {
        bool isMovingForward = move.action.ReadValue<Vector2>().y > 0;
        if (isMovingForward)
        {
            _animator.SetBool("isWalking",true);
            _animator.SetFloat("animSpeed", 1);
        }   
        else
        {
            _animator.SetBool("isWalking",true);
            _animator.SetFloat("animSpeed", -1);
        }
    }

    private void StopAnimation(InputAction.CallbackContext obj)
    {
        _animator.SetBool("isWalking",false);
        _animator.SetFloat("animSpeed", 0);

    }
}
