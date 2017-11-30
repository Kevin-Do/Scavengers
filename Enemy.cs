using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MovingObject
{

	public int playerDamage;

	private Animator _animator;

	private Transform target;

	public bool skipMove;

	public AudioClip enemyAttack1;
	public AudioClip enemyAttack2;

	// Use this for initialization
	protected override void Start ()
	{
		GameManager.Instance.AddEnemyToList(this);
		_animator = GetComponent<Animator>();
		target = GameObject.FindGameObjectWithTag("Player").transform;
		base.Start();
	}

	protected override void AttemptMove<T>(int xDir, int yDir)
	{
		if (skipMove)
		{
			skipMove = false;
			return;
		}
		
		base.AttemptMove<T>(xDir, yDir);
	}

	public void MoveEnemy()
	{
		int xDir = 0;
		int yDir = 0;
		
		//figure out direction
		if (Math.Abs(target.transform.position.x - transform.position.x) > float.Epsilon)
		{
			yDir = target.transform.position.y > transform.position.y ? 1 : -1; //1 go up, target is above us
		}
		else
		{
			xDir = target.transform.position.x > transform.position.x ? 1 : -1; // try to match x values
		}
		AttemptMove<Player>(xDir, yDir);
	}

	protected override void OnCantMove<T>(T component)
	{
		Player hitPlayer = component as Player;
		_animator.SetTrigger("enemyAttack");
		SoundManager.Instance.RandomizeSfx(enemyAttack1, enemyAttack2);
		hitPlayer.LoseFood(playerDamage);
	}
}
