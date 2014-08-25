using UnityEngine;
using System.Collections;
using System;

public class ScoreManager : Singleton<ScoreManager>
{
	protected ScoreManager ()
	{
	} // guarantee this will be always a singleton only - can't use the constructor!

	public int Money = 10000;
	public int Checkpoints = 1;
	public int Patrols {
		get { return Mathf.CeilToInt (Convert.ToSingle (Checkpoints - 1) / 2);}
	}
	// Use this for initialization
	void Start ()
	{
	
	}
	
	// Update is called once per frame
	void Update ()
	{
	
	}
}
