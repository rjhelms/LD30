using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Pathfinding;

public class Patrol : MonoBehaviour
{

	public Sprite SpriteN;
	public Sprite SpriteE;
	public Sprite SpriteW;
	public Sprite SpriteS;
	public GameController Controller;
	public List<Vector2> PatrolRoute;
	public bool IsPatrolling;
	public bool IsChasing;
	public bool IsReturning;
	public float NextWayPointDistance = 0.0625f;
	public Transform PlayerTransform;
	public Path Path;
	private int nextPatrolPoint;
	private int nextChasePoint;
	private AudioSource myAudio;
	private SpriteRenderer mySprite;
	private Rigidbody2D myRigidBody;
	private Seeker mySeeker;

	// Use this for initialization
	void Start ()
	{
		mySprite = GetComponent<SpriteRenderer> ();
		myRigidBody = GetComponent<Rigidbody2D> ();
		mySeeker = GetComponent<Seeker> ();
		myAudio = GetComponent<AudioSource> ();
		nextPatrolPoint = 0;
		IsPatrolling = true;
	}
	
	// Update is called once per frame
	void Update ()
	{
		float x = myRigidBody.velocity.x;
		float y = myRigidBody.velocity.y;
		
		if (Mathf.Abs (x) > Mathf.Abs (y)) {
			if (x > 0) {
				mySprite.sprite = SpriteE;
			} else {
				mySprite.sprite = SpriteW;
			}
			
		}
		if (Mathf.Abs (y) > Mathf.Abs (x)) {
			if (y > 0) {
				mySprite.sprite = SpriteN;
			} else {
				mySprite.sprite = SpriteS;
			}
		}
		Vector2 player_position = new Vector2 (PlayerTransform.position.x, PlayerTransform.position.y);
		if (IsChasing) {
			Debug.DrawRay (myRigidBody.position, 
			              (new Vector3 (PlayerTransform.position.x, PlayerTransform.position.y) - 
				new Vector3 (myRigidBody.position.x, myRigidBody.position.y)).normalized * 
				Controller.PatrolSightDistance * 2, Color.red);
		} else {
			Debug.DrawRay (myRigidBody.position, 
			               (new Vector3 (PlayerTransform.position.x, PlayerTransform.position.y) - 
				new Vector3 (myRigidBody.position.x, myRigidBody.position.y)).normalized * 
				Controller.PatrolSightDistance);
		}
		if (!IsChasing && ((myRigidBody.position - player_position).magnitude < Controller.PatrolSightDistance)) {
			mySeeker.StartPath (myRigidBody.position, PlayerTransform.position, OnPathComplete);
			IsChasing = true;
			IsReturning = false;
		}

		if (IsChasing) {
			myAudio.volume = Mathf.Lerp (myAudio.volume, 0.6f, Time.deltaTime * 2);
			if (!myAudio.isPlaying) {
				myAudio.pitch = Random.Range (0.95f, 1.05f);
				myAudio.loop = true;
				myAudio.Play ();
			}
		} else {
			myAudio.volume = Mathf.Lerp (myAudio.volume, 0f, Time.deltaTime * 2);
			if (myAudio.isPlaying && myAudio.volume < 0.025f) {
				myAudio.loop = false;
				;
			}
		}
	}

	void FixedUpdate ()
	{
		// a sanity check - if not in any state or pathfinding, patrolling

		if (!(IsPatrolling || IsChasing || IsReturning) && mySeeker.IsDone ())
			IsPatrolling = true;
		if (IsPatrolling) {
			DoPatrol ();
		} else {
			if (nextChasePoint < Path.vectorPath.Count) {
				DoPath ();
			} else {
				EndPath ();
			}
		}
	}

	void DoPatrol ()
	{
		Vector2 movementVector = PatrolRoute [nextPatrolPoint] - myRigidBody.position;
		movementVector.Normalize ();
		movementVector *= Controller.PatrolSpeed;
		myRigidBody.velocity = movementVector;
		if ((PatrolRoute [nextPatrolPoint] - myRigidBody.position).magnitude < NextWayPointDistance)
			nextPatrolPoint++;
		if (nextPatrolPoint >= PatrolRoute.Count)
			nextPatrolPoint = 0;
	}

	void DoPath ()
	{
		Vector2 nextPoint = new Vector2 (Path.vectorPath [nextChasePoint].x, Path.vectorPath [nextChasePoint].y);
		// if we're too close to the point, iterate through the path until we get to one worth going to.
		while ((nextPoint - myRigidBody.position).magnitude < NextWayPointDistance) {
			nextChasePoint++;
			if (nextChasePoint >= Path.vectorPath.Count) {
				EndPath ();
				return;
			} else {
				nextPoint = new Vector2 (Path.vectorPath [nextChasePoint].x, Path.vectorPath [nextChasePoint].y);
			}
		}
		Vector2 movementVector = nextPoint - myRigidBody.position;
		movementVector.Normalize ();
		if (IsChasing) {
			movementVector *= Controller.PatrolChaseSpeed;
		} else
			if (IsReturning) {
			movementVector *= Controller.PatrolSpeed;
		}
		myRigidBody.velocity = movementVector;
	}

	void EndPath ()
	{
		if (IsChasing && mySeeker.IsDone ()) {

			// keep moving in the same direction
			myRigidBody.velocity = myRigidBody.velocity.normalized * Controller.PatrolChaseSpeed;

			// look for the player and chase him if found
			Vector2 player_position = new Vector2 (PlayerTransform.position.x, PlayerTransform.position.y);
			if ((myRigidBody.position - player_position).magnitude < (Controller.PatrolSightDistance * 2)) {
				Debug.Log ("Spotted again, generating a new path.");
				mySeeker.StartPath (myRigidBody.position, PlayerTransform.position, OnPathComplete);
				IsChasing = true;
				IsReturning = false;
			} else {
				Debug.Log ("Lost him, generating a path home.");
				IsChasing = false;
				IsReturning = true;
				mySeeker.StartPath (myRigidBody.position, PatrolRoute [nextPatrolPoint], OnPathComplete);
			}
		} else {
			if (IsReturning && mySeeker.IsDone ()) {
				IsReturning = false;
				IsPatrolling = true;
				Debug.Log ("Back on patrol.");
			}
		}
	}

	public void AddPatrolPoint (Vector2 patrol_point)
	{
		PatrolRoute.Add (patrol_point);
	}

	void OnPathComplete (Path p)
	{
		if (!p.error) {
			Path = p;
			nextChasePoint = 0;
			IsPatrolling = false;
		}
	}

}
