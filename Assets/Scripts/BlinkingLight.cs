using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlinkingLight : MonoBehaviour {

    private float timer = 0f;
    private const float MaxTimer = 1.5f;
    private const float MinTimer = 0.25f;

    private GameObject Light;

    // Use this for initialization
    void Start () {
        Light = GetComponentInChildren<Light>().gameObject;
	}
	
	// Update is called once per frame
	void Update () {
        timer -= Time.deltaTime;

        if(timer <= 0)
        {
            timer = Random.Range(MinTimer, MaxTimer);
            Light.SetActive(!Light.activeInHierarchy);
        }
	}
}
