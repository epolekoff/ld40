using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Snowpatch : MonoBehaviour {

    private const float MinFlattenDistance = 1f;
    private const float MaxFlattenDistance = 5f;

    private const float VertexFlattenAmount = 0.002f;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnTriggerStay(Collider col)
    {
        if(col.tag == "Player")
        {
            RaycastHit hit;
            Vector3 collisionPoint = col.transform.position;
            if (Physics.Raycast(col.transform.position, Vector3.down, out hit, 50f, LayerMask.GetMask("Ground")))
            {
                collisionPoint = hit.point;
            }
            ShrinkVerticesAroundPoint(collisionPoint);
        }
    }

    /// <summary>
    /// Flatten the snow patch around the player.
    /// </summary>
    /// <param name="point"></param>
    private void ShrinkVerticesAroundPoint(Vector3 point)
    {
        Mesh mesh = GetComponent<MeshFilter>().mesh;
        Vector3[] vertices = mesh.vertices;
        
        for(int i = 0; i < vertices.Length; i++)
        {
            Vector3 worldSpaceVertex = transform.TransformPoint(vertices[i]);
            float distance = Vector3.Distance(worldSpaceVertex, point);

            if (distance > MaxFlattenDistance || distance < MinFlattenDistance)
            {
                continue;
            }

            float distanceRatio = (distance - MinFlattenDistance) / (MaxFlattenDistance - MinFlattenDistance);
            vertices[i].z -= VertexFlattenAmount * distanceRatio;
            GameManager.Instance.Snowball.GrowFromSnowpatch();
        }

        mesh.vertices = vertices;
        mesh.RecalculateBounds();
        GameManager.Instance.Snowball.RecalculateBounds();
    }
}
