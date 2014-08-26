using UnityEngine;
using System.Collections;

public class AudioController : MonoBehaviour
{

	public AudioClip Checkpoint;
	public AudioClip Engine;
	public AudioClip Music;
	public AudioClip Lose;
	public float FadeSpeed = 1.5f;
	public float EnginePitch { 
		get {
			return engine_source.pitch;
		}
		set {
			engine_source.pitch = value;
		}
	}

	private AudioSource checkpoint_source;
	private AudioSource engine_source;
	private AudioSource music_source;

	// Use this for initialization
	void Start ()
	{
		checkpoint_source = transform.Find ("audio_checkpoint").GetComponent<AudioSource> ();
		engine_source = transform.Find ("audio_engine").GetComponent<AudioSource> ();
		music_source = transform.Find ("audio_music").GetComponent<AudioSource> ();

		engine_source.clip = Engine;
		engine_source.pitch = 0;
		engine_source.Play ();

		music_source.clip = Music;
		music_source.Play ();

	}
	
	// Update is called once per frame
	void Update ()
	{
		if (ScoreManager.Instance.Muted) {
			music_source.Stop ();
		} else {
			if (!music_source.isPlaying) {
				music_source.clip = Music;
				music_source.Play ();
			}
		}
	}

	public void PlaySound (AudioClip sound)
	{
		checkpoint_source.pitch = Random.Range (0.95f, 1.05f);
		checkpoint_source.PlayOneShot (sound);
	}

	public void FadeOut ()
	{
		checkpoint_source.volume = Mathf.Lerp (checkpoint_source.volume, 0, FadeSpeed * Time.deltaTime);
		engine_source.volume = Mathf.Lerp (engine_source.volume, 0, FadeSpeed * Time.deltaTime);
		music_source.volume = Mathf.Lerp (music_source.volume, 0, FadeSpeed * Time.deltaTime);

	}
}