using UnityEngine;
using System;
using System.Collections;

[RequireComponent(typeof(Animator))]

public class IKController : MonoBehaviour
{

    //动画控制
    protected Animator animator;
    //是否开始IK动画
    public bool ikActive = true;

    public bool isGirl = true;

    public GameObject leftHandObj;
    public GameObject rightHandObj;

    void Start()
    {
        //得到动画控制对象
        animator = GetComponent<Animator>();
    }

    //a callback for calculating IK
    //它是回调访法。
    //前提是在Unity导航菜单栏中打开Window->Animator打开动画控制器窗口，在这里必须勾选IK Pass！！！
    void OnAnimatorIK()
    {
        if (animator)
        {

            //if the IK is active, set the position and rotation directly to the goal. 
            //即或IK动画后开始让右手节点寻找参考目标。 
            if (ikActive)
            {
                leftHandObj = GameObject.Find(isGirl ? "WomanIKPointLH" : "ManIKPointLH");
                if (leftHandObj != null)
                {
                    animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1f);
                    animator.SetIKPosition(AvatarIKGoal.LeftHand, leftHandObj.transform.position);
                    //animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 1f);
                    //animator.SetIKRotation(AvatarIKGoal.LeftHand, leftHandObj.transform.rotation);
                }
                else
                {
                    animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 0f);
                    animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 0f);
                }

                rightHandObj = GameObject.Find(isGirl ? "WomanIKPointRH" : "ManIKPointRH");
                if (rightHandObj != null)
                {
                    animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 1f);
                    animator.SetIKPosition(AvatarIKGoal.RightHand, rightHandObj.transform.position);
                    //animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 1f);
                    //animator.SetIKRotation(AvatarIKGoal.LeftHand, rightHandObj.transform.rotation);
                }
                else
                {
                    animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 0f);
                    animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 0f);
                }
            }

            //if the IK is not active, set the position and rotation of the hand back to the original position
            //如果取消IK动画，哪么重置骨骼的坐标。
            else
            {
                animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 0f);
                animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 0f);
                animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 0f);
                animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 0f);
            }
        }
    }


}