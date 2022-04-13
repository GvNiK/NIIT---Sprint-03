using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GuardEmote : MonoBehaviour
{
    public Sprite alertedEmote;
    public Sprite lostPlayerEmote;
    public Sprite suspiciousEmote;

	public float emoteDisplayTime = 3.0f;

	private Animator animator;
    private float currentEmoteDisplayTime;
	private SpriteRenderer spriteRenderer;

	private void Update()
	{
		currentEmoteDisplayTime -= Time.deltaTime;
		if (currentEmoteDisplayTime <= 0)
		{
			animator.SetBool("Visible", false);
		}
	}
	private void OnEnable()
	{
		animator = GetComponent<Animator>();
		spriteRenderer = GetComponentInChildren<SpriteRenderer>();
	}

	public void ShowAlertedEmote()
	{
		spriteRenderer.sprite = alertedEmote;
		DisplayEmote();
	}

	public void ShowLostPlayerEmote()
	{
		spriteRenderer.sprite = lostPlayerEmote;
		DisplayEmote();
	}

	public void ShowSuspiciousEmote()
	{
		spriteRenderer.sprite = suspiciousEmote;
		DisplayEmote();

	}

	private void DisplayEmote()
	{
		animator.SetBool("Visible", true);
		currentEmoteDisplayTime = emoteDisplayTime;
	}

	public void HideEmote()
	{
		animator.SetBool("Visible", false);
		currentEmoteDisplayTime = 0;
	}
}
