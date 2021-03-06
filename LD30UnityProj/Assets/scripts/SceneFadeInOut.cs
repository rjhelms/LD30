using UnityEngine;
using System.Collections;

public class SceneFadeInOut : MonoBehaviour
{
	public float FadeSpeed = 1.5f;          // Speed that the screen fades to and from black;
    
	private bool scene_Starting = true;      // Whether or not the scene is still fading in.
    
    
	void Awake ()
	{
		// Set the texture so that it is the the size of the screen and covers it.
		guiTexture.pixelInset = new Rect (0f, 0f, Screen.width, Screen.height);
		guiTexture.enabled = true;
	}
    
	void Update ()
	{
		// If the scene is starting...
		if (scene_Starting)
            // ... call the StartScene function.
			StartScene ();
	}
    
	void FadeToClear ()
	{
		// Lerp the colour of the texture between itself and transparent.
		guiTexture.color = Color.Lerp (guiTexture.color, Color.clear, FadeSpeed * Time.deltaTime);
	}
    
	void FadeToBlack ()
	{
		// Lerp the colour of the texture between itself and black.
		guiTexture.color = Color.Lerp (guiTexture.color, Color.black, FadeSpeed * Time.deltaTime);
	}
    
	void StartScene ()
	{
		// Fade the texture to clear.
		FadeToClear ();
        
		// If the texture is almost clear...
		if (guiTexture.color.a <= 0.05f) {
			// ... set the colour to clear and disable the GUITexture.
			guiTexture.color = Color.clear;
			guiTexture.enabled = false;
            
			// The scene is no longer starting.
			scene_Starting = false;
		}
	}
    
	public void EndScene (int level)
	{
		scene_Starting = false;

		// Make sure the texture is enabled.
		guiTexture.enabled = true;
        
		// Start fading towards black.
		FadeToBlack ();
        
		// If the screen is almost black...
		if (guiTexture.color.a >= 0.95f)
            // ... reload the level.
			Application.LoadLevel (level);
	}
}