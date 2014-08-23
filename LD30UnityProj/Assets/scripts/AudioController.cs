using UnityEngine;
using System.Collections;

public class AudioController : MonoBehaviour {

	public AudioClip Checkpoint;
	private AudioSource source;

	// Use this for initialization
	void Start () {
		source = audio;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void PlaySound(AudioClip sound)
	{
		source.pitch = Random.Range (0.9f,1.1f);
		source.PlayOneShot(sound);
	}
}
