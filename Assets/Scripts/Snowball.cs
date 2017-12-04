using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class Snowball : MonoBehaviour {

    /// <summary>
    /// Public 
    /// </summary>
    public GameObject MeshObject;
    public MeshCollider MeshCollider { get { return MeshObject.GetComponent<MeshCollider>(); } }
    public Mesh Mesh { get { return MeshObject.GetComponent<MeshFilter>().mesh; } }
    public Rigidbody Rigidbody { get { return GetComponent<Rigidbody>(); } }
    public GameObject Center;
    public Transform ForwardMarker;

    public float LargestVertexDistance { get { return m_largestVertexDistance; } }
    public float SizeRatio { get { return Mathf.Clamp01((LargestVertexDistance - MinVertexDistance) / (MaxVertexDistance - MinVertexDistance)); } }

    public float MaxMass { get { return Mesh.vertices.Length * MaxVertexDistance; } }
    public float MinMass { get { return Mesh.vertices.Length * MinVertexDistance; } }
    public float CurrentMass { get { return CalculateCurrentMass(); } }

    /// <summary>
    /// Constants
    /// </summary>
    private const float VertexGrowRate = 0.0008f;
    private const float MinVertexDistance = 0.005f;
    private const float MaxVertexDistance = 0.3f;

    private const float HeatAffectDistance = 4f;
    private const float VertexHeatShrinkRate = 0.05f;

    private const float MineForce = 4500f;
    private const float MineAffectDotProductRangeMin = 0.85f;
    private const float MineAffectDotProductRangeMax = 1f;

    private const float SnowpatchAffectDotProductRangeMin = 0.8f;
    private const float SnowpatchAffectDotProductRangeMax = 1f;
    private const float SnowpatchVertexGrowRate = 0.001f;

    private const float HorizontalTorque = 20000f;
    private const float HorizontalForce = 250f;
    private const float ForwardTorque = 10000f;
    private const float ForwardForce = 1000f;
    private const float BackwardTorque = 5000f;
    private const float BackwardForce = 0f;
    private const float JumpForce = 1500f;
    private const float JumpTimerMax = 1f;
    private const float AutomaticForwardTorque = 5f;
    private const float AutomaticForwardForce = 50f;
    private const float GravityForce = 5000f;

    /// <summary>
    /// Members
    /// </summary>
    private float m_largestVertexDistance = 0f;
    private List<int> m_verticesUnaffectedByHeat = new List<int>();
    private float m_jumpTimer = 0;


    // Use this for initialization
    void Start () {
	}
	
	// Update is called once per frame
	void Update () {

        GrowVertices();
        HandleInput();
        ApplyGravity();
        ApplyAutomaticForwardForce();
    }

    /// <summary>
    /// When colliding with some objects, shrink the snowball.
    /// </summary>
    /// <param name="col"></param>
    void OnCollisionEnter(Collision col)
    {
        if(col.collider.tag == "Heat")
        {
            m_verticesUnaffectedByHeat = Enumerable.Range(0, Mesh.vertices.Length).ToList();
            for (int i = 0; i < col.contacts.Length; i++)
            {
                ShrinkVertices(col.contacts[i].point);
            }
            m_verticesUnaffectedByHeat = new List<int>();
            AudioManager.Instance.PlayHeat();
        }
        if (col.gameObject.layer == LayerMask.NameToLayer("Tree"))
        {
            AudioManager.Instance.PlayTreeThud();
        }
    }

    /// <summary>
    /// Grow the snow when hitting a snowpatch.
    /// </summary>
    /// <param name="col"></param>
    void OnTriggerEnter(Collider col)
    {
        if (col.tag == "StartingPlatformExit")
        {
            GameManager.Instance.LeavePlatform();
            Destroy(col.gameObject);
        }
        if (col.tag == "Death")
        {
            GameManager.Instance.KillPlayer();
        }
        if (col.tag == "Victory")
        {
            GameManager.Instance.Victory();
            Destroy(col.gameObject);
        }
        if (col.tag == "Mine")
        {
            HitMine(col.transform.position);
            Destroy(col.gameObject);
            AudioManager.Instance.PlayPop();
        }
    }

    /// <summary>
    /// Grow the vertices.
    /// </summary>
    private void GrowVertices()
    {
        if(!GameManager.Instance.GameTimerActive)
        {
            return;
        }

        Vector3[] vertices = Mesh.vertices;

        for (int i = 0; i < vertices.Length; i++)
        {
            Vector3 fromCenter = vertices[i] - Center.transform.localPosition;
            float magnitude = fromCenter.magnitude;

            // Grow up to max.
            if (magnitude < MaxVertexDistance)
            {
                vertices[i] += fromCenter.normalized * VertexGrowRate * Time.deltaTime;
                m_largestVertexDistance = Mathf.Max(m_largestVertexDistance, magnitude);
            }
        }

        Mesh.vertices = vertices;
        MeshCollider.sharedMesh = Mesh;
        Mesh.RecalculateBounds();
    }

    /// <summary>
    /// Grow the vertices.
    /// </summary>
    public void GrowFromSnowpatch()
    {
        Vector3[] vertices = Mesh.vertices;

        for (int i = 0; i < vertices.Length; i++)
        {
            Vector3 worldSpaceVertex = transform.TransformPoint(vertices[i]);
            Vector3 fromCenterWorldSpace = worldSpaceVertex - Center.transform.position;
            float dotProductRange = Vector3.Dot(Vector3.down, fromCenterWorldSpace.normalized);
            float magnitude = fromCenterWorldSpace.magnitude;

            // Grow up to max, if the vertex is on the bottom.
            if (magnitude < MaxVertexDistance && dotProductRange >= SnowpatchAffectDotProductRangeMin && dotProductRange <= SnowpatchAffectDotProductRangeMax)
            {
                Vector3 fromCenterLocalSpace = vertices[i] - Center.transform.localPosition;
                float ratio = (dotProductRange - SnowpatchAffectDotProductRangeMin) / (SnowpatchAffectDotProductRangeMax - SnowpatchAffectDotProductRangeMin);
                vertices[i] += fromCenterLocalSpace.normalized * SnowpatchVertexGrowRate * ratio * Time.deltaTime;
            }
        }

        AudioManager.Instance.PlaySnowSound();

        Mesh.vertices = vertices;
    }

    /// <summary>
    /// Recalculate vertex bounds and collision.
    /// </summary>
    public void RecalculateBounds()
    {
        MeshCollider.sharedMesh = Mesh;
        Mesh.RecalculateBounds();
    }

    /// <summary>
    /// Shrink the vertices of the snowball near a point.
    /// </summary>
    /// <param name="collisionPoint"></param>
    private void ShrinkVertices(Vector3 collisionPoint)
    {
        Vector3[] vertices = Mesh.vertices;

        int count = 0;
        int[] verticesToShrink = m_verticesUnaffectedByHeat.ToArray();
        for (int i = 0; i < verticesToShrink.Length; i++)
        {
            int vertexIndex = verticesToShrink[i];
            Vector3 worldSpaceVertex = MeshObject.transform.TransformPoint(vertices[vertexIndex]);
            Vector3 fromCenter = vertices[vertexIndex] - Center.transform.localPosition;
            float magnitude = fromCenter.magnitude;
            float distanceFromCollision = Vector3.Distance(collisionPoint, worldSpaceVertex);

            if (distanceFromCollision <= HeatAffectDistance)
            {
                float shrinkAmount = VertexHeatShrinkRate * Time.deltaTime;

                // Shrink down to the min.
                if (magnitude - shrinkAmount < MinVertexDistance)
                {
                    shrinkAmount = magnitude - MinVertexDistance;
                }
                vertices[vertexIndex] -= fromCenter.normalized * shrinkAmount;
                m_verticesUnaffectedByHeat.Remove(vertexIndex);
                count++;
            }
        }
        Debug.Log("Count: " + count);

        Mesh.vertices = vertices;
        RecalculateBounds();
    }

    /// <summary>
    /// When hitting a mine, explode
    /// </summary>
    /// <param name="collisionPoint"></param>
    private void HitMine(Vector3 collisionPoint)
    {
        Vector3 fromMine = transform.position - collisionPoint;

        // Shrink the vertices around the point.
        Vector3[] vertices = Mesh.vertices;

        for (int i = 0; i < vertices.Length; i++)
        {
            Vector3 worldSpaceVertex = transform.TransformPoint(vertices[i]);
            Vector3 fromCenterWorldSpace = worldSpaceVertex - Center.transform.position;
            float dotProductRange = Vector3.Dot(-fromMine.normalized, fromCenterWorldSpace.normalized);

            if (dotProductRange >= MineAffectDotProductRangeMin)
            {
                Vector3 fromCenterLocalSpace = vertices[i] - Center.transform.localPosition;

                // Shrink down to the min.
                float shrinkAmount = fromCenterLocalSpace.magnitude - MinVertexDistance;
                vertices[i] -= fromCenterLocalSpace.normalized * shrinkAmount;
            }
        }

        Rigidbody.AddForce(fromMine.normalized * MineForce);

        Mesh.vertices = vertices;
    }

    /// <summary>
    /// Handle player input to move the snowball.
    /// </summary>
    private void HandleInput()
    {
        if (!GameManager.Instance.GameActive)
        {
            return;
        }

        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        float verticalTorque = vertical > 0 ? ForwardTorque : BackwardTorque;
        float verticalforce = vertical > 0 ? ForwardForce : BackwardForce;

        Rigidbody.AddTorque(ForwardMarker.forward * HorizontalTorque * -horizontal * Time.deltaTime);
        Rigidbody.AddTorque(ForwardMarker.right * verticalTorque * vertical * Time.deltaTime);

        Rigidbody.AddForce(ForwardMarker.right * HorizontalForce * horizontal * Time.deltaTime);
        Rigidbody.AddForce(ForwardMarker.forward * verticalforce * vertical * Time.deltaTime);

        if (Input.GetButtonDown("Jump") && m_jumpTimer <= 0)
        {
            m_jumpTimer = JumpTimerMax;
            Rigidbody.AddForce(ForwardMarker.up * JumpForce);
        }
        if(m_jumpTimer > 0)
        {
            m_jumpTimer -= Time.deltaTime;
        }
    }

    /// <summary>
    /// Apply custom gravity, instead of using Rigidbody.
    /// </summary>
    private void ApplyGravity()
    {
        Rigidbody.AddForce(Vector3.up * -GravityForce * Time.deltaTime);
    }

    /// <summary>
    /// Add extra forward torque so the ball rolls more smoothly.
    /// </summary>
    private void ApplyAutomaticForwardForce()
    {
        if(!GameManager.Instance.GameTimerActive)
        {
            return;
        }

        Rigidbody.AddTorque(ForwardMarker.right * AutomaticForwardTorque * Time.deltaTime);
        Rigidbody.AddForce(ForwardMarker.forward * AutomaticForwardForce * Time.deltaTime);
    }

    /// <summary>
    /// Look at all of the verts and determine what the max size can be.
    /// </summary>
    /// <returns></returns>
    private float CalculateCurrentMass()
    {
        float totalMagnitude = 0;
        for (int i = 0; i < Mesh.vertices.Length; i++)
        {
            Vector3 fromCenter = Mesh.vertices[i] - Center.transform.localPosition;
            totalMagnitude += fromCenter.magnitude;
        }

        return totalMagnitude;
    }
}
