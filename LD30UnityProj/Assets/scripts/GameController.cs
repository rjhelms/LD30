using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using Pathfinding;

public class GameController : MonoBehaviour
{
	#region Public Fields
	public Transform LevelTransform;
	public Transform EntityTransform;
	public float PlayerSpeed = 1.0f;
	public float PlayerBoostSpeed = 5.0f;
	public float PatrolSpeed = 1.0f;
	public float PatrolChaseSpeed = 2.0f;
	public float PatrolSightDistance = 2.0f;
	public TextAsset LevelFile;
	public TextAsset EntityFile;
	public TextAsset PatrolFile;
	public GameObject[] LevelFileObjects;
	public GameObject[] EntityFileObjects;
	public GameObject AudioControllerObject;
	public GameObject Astar;
	public int CheckpointScore;
	public int LoseScore;
	public int WinLevel;
	public int LoseLevel;
	public int BoostCost;
	public float FadeSpeed;
	#endregion

	#region Private Fields
	private GameObject player;
	private Jeep player_Script;
	private Transform player_Transform;
	private SpriteRenderer player_Sprite;
	private Rigidbody2D player_Rigidbody2D;
	private List<Transform> checkpoint_Transforms;
	private List<GameObject> checkpoint_Objects;
	private List<Transform> patrol_Transforms;
	private List<Patrol> patrol_Scripts;
	private int next_Checkpoint;
	private Compass compass;
	private bool level_End;
	private AudioController audio_Controller_Script;
	private AstarPath a_Star_Path;
	private List<GUIText> score_Texts;
	private List<GUIText> remaining_Texts;
	private SceneFadeInOut scene_Fader;
	private string score_String;
	private string remaining_String;
	private bool is_Chased;
	private bool has_Lost = false;
	private bool has_Won = false;
	#endregion

	// Use this for initialization
	void Start ()
	{


		// initialize lists
		checkpoint_Transforms = new List<Transform> ();
		checkpoint_Objects = new List<GameObject> ();
		patrol_Transforms = new List<Transform> ();
		patrol_Scripts = new List<Patrol> ();
		score_Texts = new List<GUIText> ();
		remaining_Texts = new List<GUIText> ();

		// build the level
		level_End = false;
		string[][] currentLevel = ReadLevel (LevelFile);
		BuildLevel (currentLevel);
		string[][] currentEntities = ReadLevel (EntityFile);
		BuildEntities (currentEntities);

		// select our checkpoints
		ChooseCheckpoints ();
		Debug.Log (checkpoint_Objects.Count);

		// get components and objects

		compass = player_Transform.Find ("camera_main/compass").GetComponent<Compass> ();
		audio_Controller_Script = AudioControllerObject.GetComponent<AudioController> ();
		audio_Controller_Script.FadeSpeed = FadeSpeed;
		scene_Fader = FindObjectOfType<GUITexture> ().GetComponent<SceneFadeInOut> ();
		scene_Fader.FadeSpeed = FadeSpeed;

		// initialize the pathfinder

		a_Star_Path = Astar.GetComponent<AstarPath> ();
		a_Star_Path.UpdateGraphs (new Pathfinding.GraphUpdateObject ());
		a_Star_Path.Scan ();

		// build the patrollers - this needs to be done after the pathfinder is up

		string[][] currentPatrols = ReadLevel (PatrolFile);
		BuildPatrols (currentPatrols);
		foreach (Patrol current_patrol in patrol_Scripts) {
			current_patrol.FadeSpeed = FadeSpeed;
		}

		// set the first checkpoint
		next_Checkpoint = 0;
		checkpoint_Objects [next_Checkpoint].SetActive (true);

		// setup GUI

		foreach (GUIText text in FindObjectsOfType<GUIText>()) {
			if (text.name.Contains ("text_score"))
				score_Texts.Add (text);
			if (text.name.Contains ("text_remaining"))
				remaining_Texts.Add (text);
		}

		RenderText ();
	}

	void Update ()
	{
		is_Chased = false;
		foreach (Patrol this_patrol in patrol_Scripts) {
			if (this_patrol.IsChasing)
				is_Chased = true;
		}
		PointCompass ();
		RenderText ();
		if (has_Lost) {
			scene_Fader.EndScene (LoseLevel);
			audio_Controller_Script.FadeOut ();
			foreach (Patrol current_patrol in patrol_Scripts) {
				current_patrol.FadeOut ();
			}
		} else if (has_Won) {
			scene_Fader.EndScene (WinLevel);
			audio_Controller_Script.FadeOut ();
			foreach (Patrol current_patrol in patrol_Scripts) {
				current_patrol.FadeOut ();
			}
		}
	}

	void FixedUpdate ()
	{
		PlayerMove ();
	}

	#region Level Initialization Methods
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

	void BuildLevel (string[][] level_array)
	{
		for (int i = 0; i < level_array.Length; i++) {
			for (int j = 0; j < level_array[i].Length; j++) {
				string currentObjectIndexString = level_array [i] [j];
				if (!string.IsNullOrEmpty (currentObjectIndexString)) {
					int currentObjectIndex = Convert.ToInt32 (currentObjectIndexString);
					if (LevelFileObjects [currentObjectIndex] != null) {
						float xpos = j;
						float ypos = level_array.Length - (i + 2);		// an off-by-two error?
						GameObject currentObject = Instantiate (LevelFileObjects [currentObjectIndex], 
						                                        new Vector3 (xpos, ypos), transform.rotation) 
							as GameObject;
						Transform currentTransform = currentObject.GetComponent<Transform> ();
						currentTransform.parent = LevelTransform;
					}
				}
			}
		}
	}

	void BuildEntities (string[][] entity_array)
	{
		for (int i = 0; i < entity_array.Length; i++) {
			string currentObjectIndexString = entity_array [i] [0];
			if (!string.IsNullOrEmpty (currentObjectIndexString)) {
				int currentObjectIndex = Convert.ToInt32 (currentObjectIndexString);
				if (EntityFileObjects [currentObjectIndex] != null) {
					GameObject currentObject = Instantiate (EntityFileObjects [currentObjectIndex], 
					                                        new Vector3 (Convert.ToSingle (entity_array [i] [1]), 
					             Convert.ToSingle (entity_array [i] [2]), 0), 
					                                        transform.rotation) as GameObject;
					Transform currentTransform = currentObject.GetComponent<Transform> ();
					if (currentObjectIndex == 0) {			// 0 - the player
						player = currentObject;
						player_Script = player.GetComponent<Jeep> ();
						player_Script.Controller = this;
						player_Transform = player.GetComponent<Transform> ();
						player_Sprite = player.GetComponent<SpriteRenderer> ();
						player_Rigidbody2D = player.GetComponent<Rigidbody2D> ();
					} else if (currentObjectIndex == 1) {	// 1 - a checkpoint
						checkpoint_Transforms.Add (currentTransform);
						checkpoint_Objects.Add (currentObject);
						currentObject.SetActive (false);
					} else if (currentObjectIndex == 2) {	// 2 - a patrol
						Patrol currentPatrol = currentObject.GetComponent<Patrol> ();
						if (patrol_Transforms.Count < ScoreManager.Instance.Patrols) {
							currentPatrol.Controller = this;
							currentPatrol.PlayerTransform = player_Transform;
							patrol_Transforms.Add (currentTransform);
							patrol_Scripts.Add (currentPatrol);
						} else {
							Destroy (currentPatrol.gameObject);
						}
					}
					currentTransform.parent = EntityTransform;
				}
			}
			
		}
	}

	void BuildPatrols (string[][] patrol_array)
	{
		for (int i = 0; i < patrol_array.Length; i++) {
			string currentObjectIndexString = patrol_array [i] [0];
			if (!string.IsNullOrEmpty (currentObjectIndexString)) {
				int patrol_index = Convert.ToInt32 (patrol_array [i] [0]);
				if (patrol_index < ScoreManager.Instance.Patrols) {
					Vector2 next_patrol_vector = new Vector2 (Convert.ToSingle (patrol_array [i] [1]), 
				                                          Convert.ToSingle (patrol_array [i] [2]));
					Patrol currentPatrol = patrol_Scripts [patrol_index];
					currentPatrol.AddPatrolPoint (next_patrol_vector);
				}
			}
		}
	}

	void ChooseCheckpoints ()
	{
		List<int> random_checkpoint_indices = new List<int> ();
		
		List<Transform> random_checkpoint_transforms = new List<Transform> ();
		List<GameObject> random_checkpoint_objects = new List<GameObject> ();
		
		int next_number;
		int target = ScoreManager.Instance.Checkpoints;
		
		if (target >= checkpoint_Transforms.Count) {
			Debug.Log ("Not enough checkpoints, reducing target.");
			target = checkpoint_Transforms.Count - 1;	// don't get stuck in an infinite loop.
		}
		
		for (int i = 0; i < target; i++) {
			do {
				next_number = UnityEngine.Random.Range (0, checkpoint_Transforms.Count - 1);
			} while (random_checkpoint_indices.Contains(next_number));
			random_checkpoint_indices.Add (next_number);
		}
		
		foreach (var i in random_checkpoint_indices) {
			random_checkpoint_transforms.Add (checkpoint_Transforms [i]);
			random_checkpoint_objects.Add (checkpoint_Objects [i]);
		}
		
		random_checkpoint_transforms.Add (checkpoint_Transforms [checkpoint_Transforms.Count - 1]);
		random_checkpoint_objects.Add (checkpoint_Objects [checkpoint_Objects.Count - 1]);
		
		checkpoint_Transforms = random_checkpoint_transforms;
		checkpoint_Objects = random_checkpoint_objects;
	}
	#endregion

	void PlayerMove ()
	{
		Vector2 newVelocity = new Vector2 (0, 0);
		if (!has_Lost) {									// you can't move if you're dead
			// zero out velocity
			player_Rigidbody2D.velocity = Vector2.zero;
			newVelocity = new Vector2 (0, 0);

			// process input
			if (Input.GetKey (KeyCode.W) || Input.GetKey (KeyCode.Z) || Input.GetKey (KeyCode.UpArrow)) {
				newVelocity += Vector2.up;
			}
			if (Input.GetKey (KeyCode.S) || Input.GetKey (KeyCode.DownArrow)) {
				newVelocity -= Vector2.up;
			}
			if (Input.GetKey (KeyCode.A) || Input.GetKey (KeyCode.Q) || Input.GetKey (KeyCode.LeftArrow)) {
				newVelocity -= Vector2.right;
			}
			if (Input.GetKey (KeyCode.D) || Input.GetKey (KeyCode.RightArrow)) {
				newVelocity += Vector2.right;
			}

			// set the player velocity
			newVelocity.Normalize ();
			if ((Input.GetKey (KeyCode.Space) || Input.GetKey (KeyCode.LeftShift)) 
				&& ScoreManager.Instance.Money > 0) { // can't boost when you're broke
				newVelocity *= PlayerBoostSpeed;
				ScoreManager.Instance.Money -= Convert.ToInt32 (Math.Floor ((BoostCost * Time.fixedDeltaTime)));
				if (ScoreManager.Instance.Money < 0)	// don't put money into negative by boosting
					ScoreManager.Instance.Money = 0;
			} else {
				newVelocity *= PlayerSpeed;
			}
			player_Rigidbody2D.velocity = newVelocity;
		}

		// adjust the pitch of the engine
		float pitch = Mathf.Lerp (audio_Controller_Script.EnginePitch, newVelocity.magnitude / 3.6f + 0.33f, 0.2f);
		audio_Controller_Script.EnginePitch = pitch;
	}

	void PointCompass ()
	{
		if (!level_End) {
			Vector2 direction = (checkpoint_Transforms [next_Checkpoint].position - 
				player_Transform.position).normalized;

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



	public void HitCheckpoint (Transform currentCheckpoint)
	{
		if (!has_Lost && !has_Won) {
			if (next_Checkpoint < checkpoint_Transforms.Count) {
				if (checkpoint_Transforms [next_Checkpoint] == currentCheckpoint) {
					audio_Controller_Script.PlaySound (audio_Controller_Script.Checkpoint);
					checkpoint_Objects [next_Checkpoint].SetActive (false);
					next_Checkpoint++;
					if (next_Checkpoint == checkpoint_Transforms.Count) {
						level_End = true;
						Win ();
					} else {
						checkpoint_Objects [next_Checkpoint].SetActive (true);
						ScoreManager.Instance.Money += CheckpointScore;
					}
				}
			}
		}
	}

	public void Lose ()
	{
		if (!has_Lost && !has_Won) {
			audio_Controller_Script.PlaySound (audio_Controller_Script.Lose);
			ScoreManager.Instance.Money += LoseScore;
			has_Lost = true;
		}
	}

	public void Win ()
	{
		if (!has_Lost && !has_Won) {
			if (Application.loadedLevel % 2 != 0) {
				ScoreManager.Instance.Checkpoints++;
			}
		}
		has_Won = true;
	}

	void RenderText ()
	{
		score_String = "$" + ScoreManager.Instance.Money;
		foreach (GUIText text in score_Texts) {
			text.text = score_String;
		}
		int remaining_checkpoints = checkpoint_Transforms.Count - next_Checkpoint - 1;
		remaining_String = "";
		if (is_Chased) {
			remaining_String += "Lose the cops!\r\n";
		} else if (remaining_checkpoints > 0) {
			if (Application.loadedLevel % 2 != 0)
				remaining_String += "Drop off the drugs!\r\n";
			else
				remaining_String += "Pick up the drugs!\r\n";
		}
		if (remaining_checkpoints > 0)
			remaining_String += "Remaining: " + remaining_checkpoints;
		else
			remaining_String += "Get to the airport!";
		foreach (GUIText text in remaining_Texts) {
			text.text = remaining_String;
		}
	}


}