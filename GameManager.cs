using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
	public static GameManager Instance = null;
	//This means you can access the public variables funcitons from anywhere
	public BoardManager BoardScript;
	public int playerFoodPoints = 100;
	public float turnDelay = .05f;
	public float levelDelay = 2f;

	private Text LevelText;
	private GameObject LevelImage;
	private List<Enemy> enemies;
	private bool enemiesMoving;
	private bool doingSetup;
	[HideInInspector] public bool playerTurn = true;
	
	private int Level = 0;

	void Awake()
	{
		if (Instance == null)
		{
			Instance = this;
		}
		else if (Instance != this)
		{
			Destroy(gameObject);
		}
		DontDestroyOnLoad((gameObject));
		enemies = new List<Enemy>();
		BoardScript = GetComponent<BoardManager>();
	}

	void InitGame()
	{
		doingSetup = true;
		//Showing title card

		LevelImage = GameObject.Find("Level Image");
		LevelText = GameObject.Find("LevelText").GetComponent<Text>();
		LevelText.text = "Day " + Level;
		LevelImage.SetActive(true);
		Invoke("HideLevelImage", levelDelay);
		enemies.Clear();
		//Clear out enemies from last level!!
		BoardScript.SetupScene(Level);
	}

	private void HideLevelImage()
	{
		LevelImage.SetActive(false);
		doingSetup = false;
	}

	public void GameOver ()
	{
		//Game over screen
		LevelText.color = Color.cyan;
		LevelText.text = "You survived " + Level + " days before starving to death.";
		LevelImage.SetActive(true);
		enabled = false;
	}

	IEnumerator MoveEnemies()
	{
		enemiesMoving = true;
		yield return new WaitForSeconds(turnDelay);
		if (enemies.Count == 0)
		{
			yield return new WaitForSeconds(turnDelay);
		}
		for (int i = 0; i < enemies.Count; i++)
		{
			enemies[i].MoveEnemy();
			yield return new WaitForSeconds(enemies[i].moveTime);
		}
		playerTurn = true;
		enemiesMoving = false;
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (playerTurn || enemiesMoving || doingSetup)
		{
			return;
		}
		StartCoroutine(MoveEnemies());
	}

	public void AddEnemyToList(Enemy script)
	{
		enemies.Add(script);
	}

	private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
	{
		//called everytime a scene is loaded
		Level++;
		InitGame();
	}
	
		
	void OnEnable() {
		SceneManager.sceneLoaded += OnSceneLoaded;
	}
	
	void OnDisable() {
		SceneManager.sceneLoaded -= OnSceneLoaded;
	}
 
}
