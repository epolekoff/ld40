using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowSnowball : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        transform.position = GameManager.Instance.Snowball.transform.position;	
	}
}
