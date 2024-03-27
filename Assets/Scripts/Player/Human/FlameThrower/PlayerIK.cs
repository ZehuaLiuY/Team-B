using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(Animator))]
public class PlayerIK : MonoBehaviour
{
    [SerializeField]
    private Transform LeftHandIKTarget;
    [SerializeField]
    private Transform RightHandIKTarget;
    [SerializeField]
    private Transform LeftElbowIKTarget;
    [SerializeField]
    private Transform RightElbowIKTarget;

    private Animator Animator;
    public bool IKActive = true; // 控制IK是否激活的变量

    private void Awake()
    {
        Animator = GetComponent<Animator>();
    }

    private void OnAnimatorIK(int layerIndex)
    {
        if (!IKActive) // 如果IKActive为false，则不执行IK设置
            return;

        Animator.SetIKHintPositionWeight(AvatarIKHint.RightElbow, 1);
        Animator.SetIKHintPositionWeight(AvatarIKHint.LeftElbow, 1);
        Animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1);
        Animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 1);
        
        Animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 1);
        Animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 1);

        Animator.SetIKPosition(AvatarIKGoal.LeftHand, LeftHandIKTarget.position);
        Animator.SetIKPosition(AvatarIKGoal.RightHand, RightHandIKTarget.position);
        Animator.SetIKHintPosition(AvatarIKHint.LeftElbow, LeftElbowIKTarget.position);
        Animator.SetIKHintPosition(AvatarIKHint.RightElbow, RightElbowIKTarget.position);

        Animator.SetIKRotation(AvatarIKGoal.LeftHand, LeftHandIKTarget.rotation);
        Animator.SetIKRotation(AvatarIKGoal.RightHand, RightHandIKTarget.rotation);
    }
    
    public void EnableIK(bool enable)
    {
        IKActive = enable;
    }
}
