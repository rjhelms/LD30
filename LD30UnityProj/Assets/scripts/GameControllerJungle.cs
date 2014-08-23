using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

public class GameControllerJungle : MonoBehaviour
{
	public Transform LevelTransform;
	public Transform EntityTransform;
	public float PlayerSpeed = 1.0f;
	public TextAsset LevelFile;
	public TextAsset EntityFile;
	public GameObject[] LevelFileObjects;
	public GameObject[] EntityFileObjects;

	private GameObject player;
	private Jeep player_Script;
	private Transform player_Transform;
	private SpriteRenderer player_Sprite;
	private Rigidbody2D player_Rigidbody2D;

	private List<GameObject> checkpoints;
	private int nextCheckpoint;
	
	// Use this for initialization
	void Start ()
	{
		checkpoints = new List<GameObject>();

		string[][] currentLevel = ReadLevel (LevelFile);
		BuildLevel (currentLevel);
		string[][] currentEntities = ReadLevel (EntityFile);
		BuildEntities (currentEntities);

		player_Script = player.GetComponent<Jeep>();
		player_Script.Controller = this;
		player_Transform = player.GetComponent<Transform> ();
		player_Sprite = player.GetComponent<SpriteRenderer> ();
		player_Rigidbody2D = player.GetComponent<Rigidbody2D> ();

		nextCheckpoint = 0;
	}
	
	// Update is called once per frame
	void FixedUpdate ()
	{
		PlayerMove ();
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

	void PlayerMove ()
	{
		player_Rigidbody2D.velocity = Vector2.zero;
		Vector2 newVelocity = new Vector2 (0, 0);

		if (Input.GetKey (KeyCode.W)) {
			newVelocity += Vector2.up;
		}
		if (Input.GetKey (KeyCode.S)) {
			newVelocity -= Vector2.up;
		}
		if (Input.GetKey (KeyCode.A)) {
			newVelocity -= Vector2.right;
		}
		if (Input.GetKey (KeyCode.D)) {
			newVelocity += Vector2.right;
		}

		newVelocity.Normalize();
		player_Rigidbody2D.velocity = newVelocity * PlayerSpeed;
	}

	void BuildLevel (string[][] level)
	{
		for (int i =0; i<level.Length; i++) {
			for (int j =0; j < level[i].Length; j++) {
				string currentObjectIndexString = level [i] [j];
				if (!string.IsNullOrEmpty (currentObjectIndexString)) {
					int currentObjectIndex = Convert.ToInt32 (currentObjectIndexString);
					if (LevelFileObjects [currentObjectIndex] != null) {
						float xpos = j;
						float ypos = level.Length - (i+2);		// an off-by-two error?
						GameObject currentObject = Instantiate (LevelFileObjects [currentObjectIndex], 
						                                        new Vector3 (xpos,ypos), transform.rotation) as GameObject;
						Transform currentTransform = currentObject.GetComponent<Transform> ();
						currentTransform.parent = LevelTransform;
					}
				}
			}
		}
	}

	void BuildEntities (string[][] entities)
	{
		for (int i = 0; i < entities.Length; i++) {
			Debug.Log (i.ToString ());
			string currentObjectIndexString = entities [i] [0];
			if (!string.IsNullOrEmpty (currentObjectIndexString)) {
				int currentObjectIndex = Convert.ToInt32 (currentObjectIndexString);
				if (EntityFileObjects [currentObjectIndex] != null) {
					GameObject currentObject = Instantiate (EntityFileObjects [currentObjectIndex], 
					                                        new Vector3 (Convert.ToInt32 (entities [i] [1]), Convert.ToInt32 (entities [i] [2]), 0), 
					                                        transform.rotation) as GameObject;

					if (currentObjectIndex == 0) {			// 0 - the player
						player = currentObject;
					}
					else if (currentObjectIndex == 1)
					{
						Debug.Log(currentObject.ToString());
						checkpoints.Add (currentObject);
					}
					Transform currentTransform = currentObject.GetComponent<Transform>();
					currentTransform.parent = EntityTransform;
				}
			}
			
		}
	}

	public void HitCheckpoint(GameObject currentCheckpoint)
	{
		if (nextCheckpoint < checkpoints.Count)
		{
			if (checkpoints[nextCheckpoint] == currentCheckpoint)
			{
				Debug.Log("Groovy.");
				nextCheckpoint++;
			}
			else
			{
				Debug.Log ("Already been here.");
			}
		}
		else
		{
			Debug.Log ("Already been here.");
		}
	}
}