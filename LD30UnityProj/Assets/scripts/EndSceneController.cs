using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EndSceneController : MonoBehaviour
{
	
	private bool is_Game_Starting = false;
	private SceneFadeInOut fader;

	// Use this for initialization
	void Start ()
	{
		foreach (GUIText current_text in FindObjectsOfType<GUIText>()) {
			if (current_text.name.Contains ("text_money"))
				current_text.text = "$" + ScoreManager.Instance.Money;
		}

		fader = transform.Find ("texture_black").GetComponent<SceneFadeInOut> ();
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (Input.anyKeyDown) {
			is_Game_Starting = true;
		}
		if (is_Game_Starting) {
			fader.EndScene (0);
		}
	}
}
