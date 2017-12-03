using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class GameCanvas : MonoBehaviour {

    public Image SnowballFill;
    public Image SnowballBackground;
    public Text SnowballMassText;

    public GameObject DeathGroup;
    public GameObject VictoryGroup;

    public Text TimerText;

    private const float SnowballFillAmountLerpSpeed = 1f;

	// Use this for initialization
	void Start () {
        SnowballFill.fillAmount = GetMassPercent();
    }
	
	// Update is called once per frame
	void Update () {

        float percent = GetMassPercent();
        SnowballMassText.text = Mathf.Floor(percent * 100) + "%";
        SnowballFill.fillAmount = Mathf.Lerp(SnowballFill.fillAmount, percent, SnowballFillAmountLerpSpeed * Time.deltaTime);

        if(GameManager.Instance.GameTimer > 0)
        {
            TimerText.gameObject.SetActive(true);
            float value = GameManager.Instance.GameTimer;
            int milliseconds = Mathf.FloorToInt((value - Mathf.Floor(value)) * 1000);
            var span = new TimeSpan(0, 0, 0, Mathf.FloorToInt(value), milliseconds);
            TimerText.text = string.Format("{0:00}:{1:00}:{2:000}", (int)span.TotalMinutes, span.Seconds, span.Milliseconds);
        }
        else
        {
            TimerText.gameObject.SetActive(false);
        }
    }

    private float GetMassPercent()
    {
        float mass = GameManager.Instance.Snowball.CurrentMass;
        float maxMass = GameManager.Instance.Snowball.MaxMass;
        float minMass = GameManager.Instance.Snowball.MinMass;

        return (mass - minMass) / (maxMass - minMass);
    }
}
