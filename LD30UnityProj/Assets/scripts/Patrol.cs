using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Patrol : MonoBehaviour
{

	public Sprite SpriteN;
	public Sprite SpriteE;
	public Sprite SpriteW;
	public Sprite SpriteS;
	public GameControllerJungle Controller;
	public List<Vector2> PatrolRoute;
	public bool isPatrolling;

	private int nextPatrolPoint;
	private SpriteRenderer mySprite;
	private Rigidbody2D myRigidBody;
	private Vector2 lastVector;
	// Use this for initialization
	void Start ()
	{
		mySprite = GetComponent<SpriteRenderer> ();
		myRigidBody = GetComponent<Rigidbody2D> ();
		nextPatrolPoint = 0;
		isPatrolling = true;
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
		
	}

	void FixedUpdate ()
	{

		Vector2 movementVector = PatrolRoute [nextPatrolPoint] - myRigidBody.position;
		movementVector.Normalize ();
		movementVector *= Controller.PatrolSpeed;
		myRigidBody.velocity = movementVector;
		if (movementVector.normalized == -lastVector.normalized)
		{
			nextPatrolPoint++;
		}
		if (nextPatrolPoint == PatrolRoute.Count)
			nextPatrolPoint = 0;
		lastVector = movementVector;
	}

	public void AddPatrolPoint (Vector2 patrol_point)
	{
		PatrolRoute.Add (patrol_point);
	}
}
