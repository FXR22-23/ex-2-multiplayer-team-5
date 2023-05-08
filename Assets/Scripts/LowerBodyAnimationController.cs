using System;
using UnityEngine;

public class LowerBodyAnimationController : MonoBehaviour
{
    [SerializeField] private Animator _animator;
    
    [SerializeField] [Range(0, 1)]
    private float leftFootPositionWeight;
    [SerializeField] [Range(0, 1)]
    private float rightFootPositionWeight;
    
    [SerializeField] [Range(0, 1)]
    private float leftFootRotationWeight;
    [SerializeField] [Range(0, 1)]
    private float rightFootRotationWeight;

    [SerializeField] private Vector3 footOffset;

    [SerializeField] private Vector3 raycastLeftOffset;
    [SerializeField] private Vector3 raycastRightOffset;

    private void OnAnimatorIK(int layerIndex)
    {
        Vector3 leftFootPosition = _animator.GetIKPosition(AvatarIKGoal.LeftFoot);
        Vector3 rightFootPosition = _animator.GetIKPosition(AvatarIKGoal.RightFoot);

        RaycastHit hitLeftFoot;
        RaycastHit hitRightFoot;

        bool isLeftFootDown = Physics.Raycast(leftFootPosition+raycastLeftOffset, Vector3.down, out hitLeftFoot);
        bool isRightFootDown = Physics.Raycast(rightFootPosition+raycastRightOffset, Vector3.down, out hitRightFoot);
        
        CalculateFoot(isLeftFootDown,hitLeftFoot, AvatarIKGoal.LeftFoot, leftFootPositionWeight,leftFootRotationWeight);
        CalculateFoot(isRightFootDown,hitRightFoot, AvatarIKGoal.RightFoot, rightFootPositionWeight,rightFootRotationWeight);

    }

    private void CalculateFoot(bool isFootDown, RaycastHit hitFoot, AvatarIKGoal avatarIKGoal, 
        float footPositionWeight, float footRotationWeight)
    {
        if (isFootDown)
        {
            _animator.SetIKPositionWeight(avatarIKGoal,footPositionWeight);
            _animator.SetIKPosition(avatarIKGoal,hitFoot.point + footOffset);

            Quaternion footRotation =
                Quaternion.LookRotation(Vector3.ProjectOnPlane(transform.forward, hitFoot.normal),
                    hitFoot.normal);
            _animator.SetIKRotationWeight(avatarIKGoal,footRotationWeight);
            _animator.SetIKRotation(avatarIKGoal,footRotation);
        }

        else
        {
            _animator.SetIKPositionWeight(avatarIKGoal, 0);
        }
    }
    
    
}
