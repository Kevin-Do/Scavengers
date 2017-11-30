using System.Collections;
using System.Collections.Generic;
using NUnit.Framework.Constraints;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Player : MovingObject
{

	public int wallDamange = 1;

	public int pointsPerFood = 60;

	public int pointsPerSoda = 90;

	public float restartLevelDelay = 1;
	public Text foodText;

	private Animator _animator;

	private int food;
	public AudioClip moveSound1;
	public AudioClip moveSound2;
	public AudioClip eatSound1;
	public AudioClip eatSound2;
	public AudioClip drinkSound1;
	public AudioClip drinkSound2;
	public AudioClip gameOverSound;

	protected override void Start()
	{
		_animator = GetComponent<Animator>();
		food = GameManager.Instance.playerFoodPoints;
		foodText.text = "Food:" + food;
		base.Start();
		//Start from Moving Object!!
	}

	private void OnDisable()
	{
		GameManager.Instance.playerFoodPoints = food;
	}

	// Update is called once per frame
	void Update () {
		if (!GameManager.Instance.playerTurn)
		{
			return;
		}

		int horizontal = 0;
		int vertical = 0;

		horizontal = (int) Input.GetAxisRaw("Horizontal");
		vertical = (int) Input.GetAxisRaw("Vertical");

		if (horizontal != 0) //any left to right movement no diagonal movement
		{
			vertical = 0;
		}

		if (horizontal != 0 || vertical != 0)
		{
			foodText.color = Color.white;
			AttemptMove<Wall>(horizontal, vertical);
			//The generic T component is used here and we expect to be interacting with the wall
		}

	}

	private void OnTriggerEnter2D(Collider2D other)
	{
		if (other.tag == "Exit")
		{
			Invoke("Restart", restartLevelDelay);
			enabled = false;
		}
		else if (other.tag == "Food")
		{
			food += pointsPerFood;
			foodText.color = Color.green;
			foodText.text = "+ " + pointsPerFood + " Food:" + food;
			SoundManager.Instance.RandomizeSfx(eatSound1, eatSound2);
			other.gameObject.SetActive(false);
		}
		else if (other.tag == "Soda")
		{
			food += pointsPerSoda;
			foodText.color = Color.green;
			foodText.text = "+ " + pointsPerSoda + " Food:" + food;
			SoundManager.Instance.RandomizeSfx(drinkSound1, drinkSound2);
			other.gameObject.SetActive(false);
		}
	}

	protected override void AttemptMove<T>(int xDir, int yDir)
	{
		food -= 5; //moving costs food
		foodText.text = "Food:" + food;

		base.AttemptMove<T>(xDir, yDir);
		RaycastHit2D hit;
		if (Move(xDir, yDir, out hit))
		{
			SoundManager.Instance.RandomizeSfx(moveSound1, moveSound2);
		}
		CheckIfGameOver();
		GameManager.Instance.playerTurn = false;
		
	}

	protected override void OnCantMove<T>(T component)
	{
		Wall hitWall = component as Wall;
		hitWall.damageWall(wallDamange);
		//Wall Component function!!
		_animator.SetTrigger("playerChop");
	}

	private void Restart()
	{
		//Load the last scene that was loaded, we are restarting the main scene because we are just regenerating
		SceneManager.LoadScene(0);
	}

	public void LoseFood(int loss)
	{
		_animator.SetTrigger("playerHit");
		food -= loss;
		foodText.color = Color.red;
		foodText.text = "- " + loss + " Food:" + food;
		CheckIfGameOver();
	}
	
	private void CheckIfGameOver()
	{
		if (food <= 0)
		{
			SoundManager.Instance.PlaySingle(gameOverSound);
			SoundManager.Instance.musicSource.Stop();
			GameManager.Instance.GameOver();
		}
	}
}
