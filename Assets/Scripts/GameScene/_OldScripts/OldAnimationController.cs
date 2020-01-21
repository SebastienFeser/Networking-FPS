using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OldAnimationController : MonoBehaviour
{
    [SerializeField] private Animator animator;

    public void StayRifle()
    {
        animator.SetBool("Aiming", true);
        animator.SetFloat("Speed", 0f);
    }

    public void WalkRifle()
    {
        animator.SetBool("Aiming", true);
        animator.SetFloat("Speed", 0.5f);
    }

    public void RunRifle()
    {
        animator.SetBool("Aiming", true);
        animator.SetFloat("Speed", 1f);
    }

    public void ShootRifle()
    {
        animator.SetTrigger("Attack");
    }

    public void Death()
    {
        animator.SetTrigger("Death");
    }

    public void Mine()
    {

    }
}
