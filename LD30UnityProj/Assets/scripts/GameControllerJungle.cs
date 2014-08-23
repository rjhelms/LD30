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
	public GameObject AudioControllerObject;

	private GameObject player;
	private Jeep player_Script;
	private Transform player_Transform;
	private SpriteRenderer player_Sprite;
	private Rigidbody2D player_Rigidbody2D;
	private List<Transform> checkpoint_Transforms;
	private List<GameObject> checkpoint_Objects;
	private int next_Checkpoint;
	private Compass compass;
	private bool level_End;
	private AudioController audio_Controller_Script;

	// Use this for initialization
	void Start ()
	{
		// initialize lists
		checkpoint_Transforms = new List<Transform> ();
		checkpoint_Objects = new List<GameObject> ();

		// build the level
		level_End = false;
		string[][] currentLevel = ReadLevel (LevelFile);
		BuildLevel (currentLevel);
		string[][] currentEntities = ReadLevel (EntityFile);
		BuildEntities (currentEntities);

		// get components and objects
		player_Script = player.GetComponent<Jeep> ();
		player_Script.Controller = this;
		player_Transform = player.GetComponent<Transform> ();
		player_Sprite = player.GetComponent<SpriteRenderer> ();
		player_Rigidbody2D = player.GetComponent<Rigidbody2D> ();

		compass = player_Transform.Find ("camera_main/compass").GetComponent<Compass> ();
		audio_Controller_Script = AudioControllerObject.GetComponent<AudioController>();

		// set the first checkpoint
		next_Checkpoint = 0;
		checkpoint_Objects [next_Checkpoint].SetActive (true);
	}

	void Update ()
	{
		PointCompass ();
	}

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

		newVelocity.Normalize ();
		player_Rigidbody2D.velocity = newVelocity * PlayerSpeed;
	}

	void PointCompass ()
	{
		if (!level_End) {
			Vector2 direction = (checkpoint_Transforms [next_Checkpoint].position - player_Transform.position).normalized;

			if (direction.x < -0.85f) {																// W
				compass.PointCompass (compass.SpriteW);
			} else if ((-0.85f < direction.x) && (direction.x < -0.35f) && (direction.y > 0)) {		// NW
				compass.PointCompass (compass.SpriteNW);
			} else if (direction.y > 0.85f) {														// N
				compass.PointCompass (compass.SpriteN);
			} else if ((0.35f < direction.x) && (direction.x < 0.85f) && (direction.y > 0)) {		// NE
				compass.PointCompass (compass.SpriteNE);
			} else if (direction.x > 0.85f) {														// E
				compass.PointCompass (compass.SpriteE);
			} else if ((0.35f < direction.x) && (direction.x < 0.85f) && (direction.y < 0)) {		// SE
				compass.PointCompass (compass.SpriteSE);
			} else if (direction.y < -0.85f) {														// S
				compass.PointCompass (compass.SpriteS);
			} else if ((-0.85f < direction.x) && (direction.x < -0.35f) && (direction.y < 0)) {		// SW
				compass.PointCompass (compass.SpriteSW);
			}
		}
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
						float ypos = level.Length - (i + 2);		// an off-by-two error?
						GameObject currentObject = Instantiate (LevelFileObjects [currentObjectIndex], 
						                                        new Vector3 (xpos, ypos), transform.rotation) as GameObject;
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
					Transform currentTransform = currentObject.GetComponent<Transform> ();
					if (currentObjectIndex == 0) {			// 0 - the player
						player = currentObject;
					} else if (currentObjectIndex == 1) {
						checkpoint_Transforms.Add (currentTransform);
						checkpoint_Objects.Add (currentObject);
						currentObject.SetActive (false);
					}
					currentTransform.parent = EntityTransform;
				}
			}
			
		}
	}

	public void HitCheckpoint (Transform currentCheckpoint)
	{
		if (next_Checkpoint < checkpoint_Transforms.Count) {
			if (checkpoint_Transforms [next_Checkpoint] == currentCheckpoint) {
				Debug.Log ("Groovy.");
				audio_Controller_Script.PlaySound(audio_Controller_Script.Checkpoint);
				checkpoint_Objects [next_Checkpoint].SetActive (false);
				next_Checkpoint++;
				if (next_Checkpoint == checkpoint_Transforms.Count) {
					Debug.Log ("Level end!");
					level_End = true;
				} else {
					checkpoint_Objects [next_Checkpoint].SetActive (true);
				}
			} else {
				Debug.Log ("Already been here.");
			}
		} else {
			Debug.Log ("Done level");
		}
	}
}