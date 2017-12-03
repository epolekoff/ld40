using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollingTextureOffset : MonoBehaviour {

    public Vector2 TilingOverride = new Vector2(0, 0);

    private const float ScrollX = 0.25f;
    private const float ScrollY = 1f;

    // Use this for initialization
    void Start () {
		if(TilingOverride != Vector2.zero)
        {
            GetComponent<MeshRenderer>().material.mainTextureScale = TilingOverride;
        }
	}
	
	// Update is called once per frame
	void Update () {
        GetComponent<MeshRenderer>().material.mainTextureOffset += new Vector2(ScrollX, ScrollY) * Time.deltaTime;
	}
}
