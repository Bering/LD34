using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ScoreBoard : MonoBehaviour
{
	static public ScoreBoard instance;


	[SerializeField] protected Text Score;
	[SerializeField] protected Text ScoreShadow;

	protected int currentScore;


	void Awake()
	{
		instance = this;
	}


	void Start()
	{
		GameController.GetInstance ().FirstTurnStartedEvent.AddListener (ResetScore);
	}


	protected void ResetScore()
	{
		SetScore(0);
	}


	public int GetScore()
	{
		return currentScore;
	}


	public int IncrementScore(int increment)
	{
		return SetScore (currentScore + increment);
	}


	public int SetScore(int newScore)
	{
		currentScore = newScore;
		Score.text = "" + currentScore;
		ScoreShadow.text = "" + currentScore;

		return currentScore;
	}

}

