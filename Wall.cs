using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : MonoBehaviour
{

	public Sprite DmgSprite;

	public int hp = 4;

	public AudioClip chopSound1;
	public AudioClip chopSound2;

	public SpriteRenderer SpriteRenderer;
	// Use this for initialization
	void Awake ()
	{
		SpriteRenderer = GetComponent<SpriteRenderer>();
	}

	public void damageWall(int loss)
	{
		SpriteRenderer.sprite = DmgSprite;
		hp -= loss;
		SoundManager.Instance.RandomizeSfx(chopSound1, chopSound2);

		if (hp <= 0)
		{
			gameObject.SetActive(false);
		}
	}
	

}
