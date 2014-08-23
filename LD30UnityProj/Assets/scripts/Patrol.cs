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
	public GameControllerJungle Controller;
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
		mySeeker = GetComponent<Seeker>();
		myAudio = GetComponent<AudioSource>();
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
		Vector2 player_position = new Vector2(PlayerTransform.position.x, PlayerTransform.position.y);
		if (!IsChasing && ((myRigidBody.position - player_position).magnitude < Controller.PatrolSightDistance))
		{
			mySeeker.StartPath (myRigidBody.position, PlayerTransform.position, OnPathComplete);
			IsChasing = true;
			IsReturning = false;
		}

		if (IsChasing)
		{
			myAudio.volume = Mathf.Lerp (myAudio.volume, 0.8f, Time.deltaTime * 2);
		}
		else
		{
			myAudio.volume = Mathf.Lerp (myAudio.volume, 0f, Time.deltaTime * 2);
		}
	}

	void FixedUpdate ()
	{
		if (IsPatrolling) {
			Vector2 movementVector = PatrolRoute [nextPatrolPoint] - myRigidBody.position;
			movementVector.Normalize ();
			movementVector *= Controller.PatrolSpeed;
			myRigidBody.velocity = movementVector;
			if ((PatrolRoute [nextPatrolPoint] - myRigidBody.position).magnitude < NextWayPointDistance)
				nextPatrolPoint++;
			if (nextPatrolPoint >= PatrolRoute.Count)
				nextPatrolPoint = 0;
		}
		if (!IsPatrolling)
		{
			Vector2 nextPoint = new Vector2(Path.vectorPath[nextChasePoint].x, Path.vectorPath[nextChasePoint].y);
			Vector2 movementVector = nextPoint - myRigidBody.position;
			movementVector.Normalize ();
			if (IsChasing)
			{
				movementVector *= Controller.PatrolChaseSpeed;
			} else if (IsReturning)
			{
				movementVector *= Controller.PatrolSpeed;
			}
			float new_x_vel = Mathf.Lerp (myRigidBody.velocity.x, movementVector.x, Time.fixedDeltaTime * 10);
			float new_y_vel = Mathf.Lerp (myRigidBody.velocity.y, movementVector.y, Time.fixedDeltaTime * 10);
			myRigidBody.velocity = new Vector2(new_x_vel, new_y_vel);
			if ((nextPoint - myRigidBody.position).magnitude < NextWayPointDistance)
				nextChasePoint++;
			if (nextChasePoint >= Path.vectorPath.Count)
			{
				if (IsChasing)
				{
					IsChasing = false;
					IsReturning = true;
					mySeeker.StartPath (myRigidBody.position, PatrolRoute [nextPatrolPoint], OnPathComplete);
				}
				else if (IsReturning)
				{
					IsReturning = false;
					IsPatrolling = true;
				}
			}
		}
	}

	public void AddPatrolPoint (Vector2 patrol_point)
	{
		PatrolRoute.Add (patrol_point);
	}

	void OnPathComplete(Path p)
	{
		if (!p.error)
		{
			Path = p;
			nextChasePoint = 0;
			IsPatrolling = false;
		}
	}

}
