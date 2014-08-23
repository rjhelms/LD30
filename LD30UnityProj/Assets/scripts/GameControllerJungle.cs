using UnityEngine;
using System;
using System.Collections;
using System.IO;
using System.Text.RegularExpressions;

public class GameControllerJungle : MonoBehaviour
{
	public Transform LevelTransform;
	public float PlayerSpeed = 1.0f;
	public TextAsset LevelFile;
	public TextAsset EntityFile;

	public GameObject[] LevelFileObjects;
	public GameObject[] EntityFileObjects;

	private GameObject player;
	private Transform player_Transform;
	private SpriteRenderer player_Sprite;
	private Rigidbody2D player_Rigidbody2D;
	
	// Use this for initialization
	void Start ()
	{


		string[][] currentLevel = ReadLevel (LevelFile);
		BuildLevel (currentLevel);
		string[][] currentEntities = ReadLevel (EntityFile);
		BuildEntities (currentEntities);

		player_Transform = player.GetComponent<Transform> ();
		player_Sprite = player.GetComponent<SpriteRenderer> ();
		player_Rigidbody2D = player.GetComponent<Rigidbody2D> ();
	}
	
	// Update is called once per frame
	void FixedUpdate ()
	{
		player_Rigidbody2D.velocity = Vector2.zero;

		if (Input.GetKey (KeyCode.W)) {
			player_Rigidbody2D.velocity = (player_Rigidbody2D.velocity + new Vector2 (0, 1)).normalized * PlayerSpeed;
		} else if (Input.GetKey (KeyCode.S)) {
			player_Rigidbody2D.velocity = (player_Rigidbody2D.velocity + new Vector2 (0, -1)).normalized * PlayerSpeed;
		}

		if (Input.GetKey (KeyCode.A)) {
			player_Rigidbody2D.velocity = (player_Rigidbody2D.velocity + new Vector2 (-1, 0)).normalized * PlayerSpeed;
		} else if (Input.GetKey (KeyCode.D)) {
			player_Rigidbody2D.velocity = (player_Rigidbody2D.velocity + new Vector2 (1, 0)).normalized * PlayerSpeed;
		}
	
	}

	string[][] ReadLevel (TextAsset file)
	{
		string text = file.text;
		string[] lines = Regex.Split (text, "\r\n");
		int rows = lines.Length;

		string[][] levelBase = new string[rows][];
		for (int i =0; i< lines.Length; i++) {
			string[] stringsOfLine = Regex.Split (lines [i], ",");
			levelBase [i] = stringsOfLine;
		}
		return levelBase;
	}

	void BuildLevel (string[][] level)
	{
		for (int i =0; i<level.Length; i++) {
			for (int j =0; j < level[i].Length; j++) {
				string currentObjectIndexString = level [i] [j];
				if (!string.IsNullOrEmpty (currentObjectIndexString)) {
					int currentObjectIndex = Convert.ToInt32 (currentObjectIndexString);
					if (LevelFileObjects [currentObjectIndex] != null) {
						GameObject currentObject = Instantiate (LevelFileObjects [currentObjectIndex], 
						                                        new Vector3 (j, (level.Length - i), 10), transform.rotation) as GameObject;
						Transform currentTransform = currentObject.GetComponent<Transform>();
						currentTransform.parent = LevelTransform;
					}
				}
			}
		}
	}

	void BuildEntities (string[][] entities)
	{
		for (int i = 0; i < entities.Length; i++)
		{
			Debug.Log (i.ToString());
			string currentObjectIndexString = entities[i][0];
			if (!string.IsNullOrEmpty (currentObjectIndexString))
			{
				int currentObjectIndex = Convert.ToInt32 (currentObjectIndexString);
				if (EntityFileObjects [currentObjectIndex] != null) 
				{
					GameObject currentObject = Instantiate (EntityFileObjects [currentObjectIndex], 
					                                        new Vector3 (Convert.ToInt32 (entities[i][1]), Convert.ToInt32 (entities[i][2]), 0), transform.rotation) as GameObject;

					if (currentObjectIndex == 0)			// being a jackass and just assuming 0 is always the player
					{
						player = currentObject;
					}
				}
			}
			
		}
	}
}