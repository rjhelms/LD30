using UnityEngine;
using System.Collections;
using System;

public class ScoreManager : Singleton<ScoreManager>
{
	protected ScoreManager ()
	{
	} // guarantee this will be always a singleton only - can't use the constructor!

	public int StartingMoney = 15000;
	public int StartingCheckpoints = 1;

	public int Money = 15000;
	public int Checkpoints = 1;
	public int WinCheckpoints = 5;
	public int Patrols {
		get { return (Checkpoints - 1);}
	}
	// Use this for initialization
	void Start ()
	{
	
	}
	
	// Update is called once per frame
	void Update ()
	{
	
	}

	public void Reset ()
	{
		Money = StartingMoney;
		Checkpoints = StartingCheckpoints;
	}
}
