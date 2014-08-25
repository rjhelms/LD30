using UnityEngine;
using System.Collections;

public class AudioController : MonoBehaviour {

	public AudioClip Checkpoint;
	public AudioClip Engine;

	public float EnginePitch { 
		get
		{
			return engine_source.pitch;
		}
		set
		{
			engine_source.pitch = value;}
	}

	private AudioSource checkpoint_source;
	private AudioSource engine_source;
	
	// Use this for initialization
	void Start () {
		checkpoint_source = transform.Find("audio_checkpoint").GetComponent<AudioSource>();
		engine_source = transform.Find("audio_engine").GetComponent<AudioSource>();

		engine_source.clip = Engine;
		engine_source.pitch = 0;
		engine_source.Play();

	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void PlaySound(AudioClip sound)
	{
		checkpoint_source.pitch = Random.Range (0.95f,1.05f);
		checkpoint_source.PlayOneShot(sound);
	}

}
