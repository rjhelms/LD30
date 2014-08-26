using UnityEngine;
using System.Collections;

public class TitleScreenController : MonoBehaviour
{
	public float FadeSpeed = 2.0f;

	private bool is_Game_Starting = false;
	private bool is_Instructions_Loading = false;
	private SceneFadeInOut fader;
	private AudioController audio_Controller_Script;
	public GameObject AudioControllerObject;

	// Use this for initialization
	void Start ()
	{
		fader = transform.Find ("texture_black").GetComponent<SceneFadeInOut> ();

		audio_Controller_Script = AudioControllerObject.GetComponent<AudioController> ();
		audio_Controller_Script.FadeSpeed = FadeSpeed;
		fader = FindObjectOfType<GUITexture> ().GetComponent<SceneFadeInOut> ();
		fader.FadeSpeed = FadeSpeed;

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
			audio_Controller_Script.FadeOut ();
		}
		if (is_Game_Starting) {
			fader.EndScene (1);
			audio_Controller_Script.FadeOut ();
		}
	}
}
