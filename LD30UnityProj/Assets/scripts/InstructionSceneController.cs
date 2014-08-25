using UnityEngine;
using System.Collections;

public class InstructionSceneController : MonoBehaviour
{

	private bool is_Game_Starting = false;
	private SceneFadeInOut fader;
	
	// Use this for initialization
	void Start ()
	{
		fader = transform.Find ("texture_black").GetComponent<SceneFadeInOut> ();
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (Input.anyKeyDown) {
			is_Game_Starting = true;
		}
		if (is_Game_Starting) {
			fader.EndScene (1);
		}
	}
}
