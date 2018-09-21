using UnityEngine;

public class HeavyAttack : StateMachineBehaviour
{ 

    // This will be called when the animator first transitions to this state.
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetBool("HeavyAttack", false);
        animator.SetBool("Attacking", true);
    }

    // This will be called once the animator has transitioned out of the state.
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetBool("HeavyAttack", false);
    }
}
