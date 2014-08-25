using UnityEngine;
using System.Collections;

public class TitleScreenController : MonoBehaviour
{

	private bool is_Game_Starting = false;
	private bool is_Instructions_Loading = false;
	private SceneFadeInOut fader;

	// Use this for initialization
	void Start ()
	{
		fader = transform.Find ("texture_black").GetComponent<SceneFadeInOut> ();

		// reset the score manager.
		ScoreManager.Instance.Reset ();
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (!is_Game_Starting && Input.GetKeyDown (KeyCode.I)) {
			is_Instructions_Loading = true;
		}
		if (!is_Instructions_Loading && Input.anyKeyDown) {
			is_Game_Starting = true;
		}
		if (is_Instructions_Loading) {
			fader.EndScene (3);
		}
		if (is_Game_Starting) {
			fader.EndScene (1);
		}
	}
}
