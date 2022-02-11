﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenDamageCollider_ASB : StateMachineBehaviour
{
    StateManager states;

    public HandleDamageColliders.DamageType damageType;
    public HandleDamageColliders.DCtype dcType;
    public float delay;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (states == null)
        {
            states = animator.GetComponentInParent<StateManager>();
        }
        states.handleDC.OpenCollider(dcType, delay, damageType);
        
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    //override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (states == null)
        {
            states = animator.GetComponentInParent<StateManager>();
        }
        states.handleDC.CloseColliders();

    }

    // OnStateMove is called right after Animator.OnAnimatorMove()
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that processes and affects root motion
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}
}
