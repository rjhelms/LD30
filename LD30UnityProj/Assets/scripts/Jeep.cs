using UnityEngine;
using System.Collections;

public class Jeep : MonoBehaviour
{
	public Sprite SpriteN;
	public Sprite SpriteE;
	public Sprite SpriteW;
	public Sprite SpriteS;
	private SpriteRenderer mySprite;
	private Rigidbody2D myRigidBody;
	private float angle;

	// Use this for initialization
	void Start ()
	{
		mySprite = GetComponent<SpriteRenderer> ();
		myRigidBody = GetComponent<Rigidbody2D> ();
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
}
