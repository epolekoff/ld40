using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour {

    public Snowball Snowball;
    public Camera Camera { get { return GetComponent<Camera>(); } }

    private const float MinFollowDistanceZ = 10f;
    private const float MaxFollowDistanceZ = 50f;
    private float FollowDistanceZ { get { return (Snowball.SizeRatio * (MaxFollowDistanceZ - MinFollowDistanceZ)) + MinFollowDistanceZ; } }

    private const float MinFollowDistanceY = 10f;
    private const float MaxFollowDistanceY = 50f;
    private float FollowDistanceY { get { return (Snowball.SizeRatio * (MaxFollowDistanceY - MinFollowDistanceY)) + MinFollowDistanceY; } }

    private const float PositionLerpSpeed = 5f;
    private const float RotationLerpSpeed = 2f;

    private const float StartStateRotationSpeed = -10f;


    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

        
    }

    /// <summary>
    /// Follow the snowball
    /// </summary>
    public void FollowSnowball()
    {
        Vector3 cameraPosition = new Vector3(
            Snowball.transform.position.x,
            Snowball.transform.position.y + FollowDistanceY,
            Snowball.transform.position.z - FollowDistanceZ);
        transform.position = Vector3.Lerp(transform.position, cameraPosition, PositionLerpSpeed * Time.deltaTime);

        LookAtSnowball();
    }

    /// <summary>
    /// Rotate around the starting platform.
    /// </summary>
    public void ShowStartingPlatform()
    {
        transform.RotateAround(GameManager.Instance.SnowballSpawnPoint.position, Vector3.up, StartStateRotationSpeed * Time.deltaTime);
        transform.LookAt(GameManager.Instance.SnowballSpawnPoint.position);
    }

    /// <summary>
    /// Look at the snowball.
    /// </summary>
    public void LookAtSnowball()
    {
        var lookRotation = Quaternion.LookRotation(Snowball.transform.position - transform.position);
        transform.rotation = Quaternion.Lerp(transform.rotation, lookRotation, RotationLerpSpeed * Time.deltaTime);
    }
}
