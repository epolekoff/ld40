using UnityEngine;

[System.Serializable]
public class SerializableBezierSpline
{
    [SerializeField]
    public Vector3[] Points;

    [SerializeField]
    public BezierControlPointMode[] Modes;

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="points"></param>
    /// <param name="modes"></param>
    public SerializableBezierSpline(Vector3[] points, BezierControlPointMode[] modes)
    {
        Points = points;
        Modes = modes;
    }
}
