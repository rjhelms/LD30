using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EndSceneController : MonoBehaviour
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
		foreach (GUIText current_text in FindObjectsOfType<GUIText>()) {
			if (current_text.name.Contains ("text_money"))
				current_text.text = "$" + ScoreManager.Instance.Money;
		}

		fader = transform.Find ("texture_black").GetComponent<SceneFadeInOut> ();
		
		audio_Controller_Script = AudioControllerObject.GetComponent<AudioController> ();
		audio_Controller_Script.FadeSpeed = FadeSpeed;
		fader = FindObjectOfType<GUITexture> ().GetComponent<SceneFadeInOut> ();
		fader.FadeSpeed = FadeSpeed;
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (!fader.SceneStarting && Input.anyKeyDown) {
			is_Game_Starting = true;
		}
		if (is_Game_Starting) {
			fader.EndScene (0);
			audio_Controller_Script.FadeOut ();
		}
	}
}
