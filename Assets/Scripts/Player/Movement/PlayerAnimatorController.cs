using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimatorController : MonoBehaviour
{
	private Animator animator;
	private float moveSpeedMultiplier;
	private Rigidbody rb;

    public void Setup(Animator animator, Rigidbody rb, float moveSpeedMultiplier)
    {
		this.animator = animator;
		this.moveSpeedMultiplier = moveSpeedMultiplier;
		this.rb = rb;
    }

	public void OnAnimatorMove()
	{
		if (Time.deltaTime > 0)
		{
			Vector3 velocity = (animator.deltaPosition * moveSpeedMultiplier) / Time.deltaTime;

			velocity.y = rb.velocity.y;
			rb.velocity = velocity;
			rb.rotation *= animator.deltaRotation;
		}
	}
}
