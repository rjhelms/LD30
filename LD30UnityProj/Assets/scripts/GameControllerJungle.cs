using UnityEngine;
using System.Collections;

public class GameControllerJungle : MonoBehaviour
{

	public GameObject Player;
	public float PlayerSpeed = 1.0f;
	private Transform player_Transform;
	private SpriteRenderer player_Sprite;
	private Rigidbody2D player_Rigidbody2D;

	// Use this for initialization
	void Start ()
	{
		player_Transform = Player.GetComponent<Transform> ();
		player_Sprite = Player.GetComponent<SpriteRenderer> ();
		player_Rigidbody2D = Player.GetComponent<Rigidbody2D> ();
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
}
