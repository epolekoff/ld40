using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;

public class BezierSplineBehaviour : MonoBehaviour {

	[SerializeField]
	private Vector3[] points;

	[SerializeField]
	private BezierControlPointMode[] modes;

	[SerializeField]
	private bool loop;

	public bool Loop {
		get {
			return loop;
		}
		set {
			loop = value;
			if (value == true) {
				modes[modes.Length - 1] = modes[0];
				SetControlPoint(0, points[0]);
			}
		}
	}

	public int ControlPointCount {
		get {
			return points.Length;
		}
	}

	public Vector3 GetControlPoint (int index) {
        index = Mathf.Clamp(index, 0, ControlPointCount - 1);
		return points[index];
	}

	public void SetControlPoint (int index, Vector3 point) {
		if (index % 3 == 0) {
			Vector3 delta = point - points[index];
			if (loop) {
				if (index == 0) {
					points[1] += delta;
					points[points.Length - 2] += delta;
					points[points.Length - 1] = point;
				}
				else if (index == points.Length - 1) {
					points[0] = point;
					points[1] += delta;
					points[index - 1] += delta;
				}
				else {
					points[index - 1] += delta;
					points[index + 1] += delta;
				}
			}
			else {
				if (index > 0) {
					points[index - 1] += delta;
				}
				if (index + 1 < points.Length) {
					points[index + 1] += delta;
				}
			}
		}
		points[index] = point;
		EnforceMode(index);
	}

	public BezierControlPointMode GetControlPointMode (int index) {
		return modes[(index + 1) / 3];
	}

	public void SetControlPointMode (int index, BezierControlPointMode mode) {
		int modeIndex = (index + 1) / 3;
		modes[modeIndex] = mode;
		if (loop) {
			if (modeIndex == 0) {
				modes[modes.Length - 1] = mode;
			}
			else if (modeIndex == modes.Length - 1) {
				modes[0] = mode;
			}
		}
		EnforceMode(index);
	}

	private void EnforceMode (int index) {
		int modeIndex = (index + 1) / 3;
		BezierControlPointMode mode = modes[modeIndex];
		if (mode == BezierControlPointMode.Free || !loop && (modeIndex == 0 || modeIndex == modes.Length - 1)) {
			return;
		}

		int middleIndex = modeIndex * 3;
		int fixedIndex, enforcedIndex;
		if (index <= middleIndex) {
			fixedIndex = middleIndex - 1;
			if (fixedIndex < 0) {
				fixedIndex = points.Length - 2;
			}
			enforcedIndex = middleIndex + 1;
			if (enforcedIndex >= points.Length) {
				enforcedIndex = 1;
			}
		}
		else {
			fixedIndex = middleIndex + 1;
			if (fixedIndex >= points.Length) {
				fixedIndex = 1;
			}
			enforcedIndex = middleIndex - 1;
			if (enforcedIndex < 0) {
				enforcedIndex = points.Length - 2;
			}
		}

		Vector3 middle = points[middleIndex];
		Vector3 enforcedTangent = middle - points[fixedIndex];
		if (mode == BezierControlPointMode.Aligned) {
			enforcedTangent = enforcedTangent.normalized * Vector3.Distance(middle, points[enforcedIndex]);
		}
		points[enforcedIndex] = middle + enforcedTangent;
	}

	public int CurveCount {
		get {
			return (points.Length - 1) / 3;
		}
	}

	public Vector3 GetPoint (float t) {
        int i;
        t = WrapSplinePositionValue(t); // I added this line to put an end to the annoying wrapping problem.
        if (t >= 1f)
        {
            t = 1f;
            i = points.Length - 4;
        }
        else
        {
            t = Mathf.Clamp01(t) * CurveCount;
            i = (int)t;
            t -= i;
            i *= 3;
        }
        return transform.TransformPoint(Bezier.GetPoint(points[i], points[i + 1], points[i + 2], points[i + 3], t));
	}
	
	public Vector3 GetVelocity (float t) {
        int i;
        if (t >= 1f)
        {
            t = 1f;
            i = points.Length - 4;
        }
        else
        {
            t = Mathf.Clamp01(t) * CurveCount;
            i = (int)t;
            t -= i;
            i *= 3;
        }
        return transform.TransformPoint(Bezier.GetFirstDerivative(points[i], points[i + 1], points[i + 2], points[i + 3], t)) - transform.position;
	}
	
	public Vector3 GetDirection (float t) {
		return GetVelocity(t).normalized;
	}

    /// <summary>
    /// Given a percent, return a new percent that is close to the expectedDistance.
    /// </summary>
    /// <returns></returns>
    public float GetOffsetAtSpecifiedDistance(float currentPercent, float expectedDistance)
    {
        float percentAdjustment = 0.00001f * Mathf.Sign(expectedDistance);
        float currentOffset = 0;

        Vector3 currentPoint = GetPoint(currentPercent);
        Vector3 attemptedPoint = currentPoint;
        float cumulativeDistance = 0;

        // Loop and increase the offset until the distance is what we expected.
        do {
            Vector3 oldAttemptedPoint = attemptedPoint;
            currentOffset += percentAdjustment;
            attemptedPoint = GetPoint(currentPercent + currentOffset);
            cumulativeDistance += Vector3.Distance(oldAttemptedPoint, attemptedPoint);

        } while(cumulativeDistance < Mathf.Abs(expectedDistance));
        
        return currentOffset;
    }

    public int GetControlPointIndex(float t)
    {
        int i;
        if (t >= 1f)
        {
            t = 1f;
            i = points.Length - 4;
        }
        else
        {
            t = Mathf.Clamp01(t) * CurveCount;
            i = (int)t;
            t -= i;
            i *= 3;
        }
        return i+3;
    }

    public void AddCurve () {
		Vector3 point = points[points.Length - 1];
		Array.Resize(ref points, points.Length + 3);
		point.x += 1f;
		points[points.Length - 3] = point;
		point.x += 1f;
		points[points.Length - 2] = point;
		point.x += 1f;
		points[points.Length - 1] = point;

		Array.Resize(ref modes, modes.Length + 1);
		modes[modes.Length - 1] = modes[modes.Length - 2];
		EnforceMode(points.Length - 4);

		if (loop) {
			points[points.Length - 1] = points[0];
			modes[modes.Length - 1] = modes[0];
			EnforceMode(0);
		}
	}

    public void AddCurve(int index)
    {
        // Correct bad parameters.
        if(index >= points.Length)
        {
            index = points.Length - 1;
        }

        // Resize the array to match the new size.
        int oldLength = points.Length;
        Array.Resize(ref points, oldLength + 3);

        // Shift everything in the array, moving from the back to one notch in front of index.
        for(int i = points.Length - 4; i > index + 1; i --)
        {
            points[i + 3] = points[i];
        }

        // Set the position of the new points in world space.
        int newPointIndex = index + 3;
        int newPointLeftHandle = index + 2;
        int newPointRightHandle = index + 4;

        // Get the old index handles, taking wrapping into account.
        int indexLeftHandle = index == 0 ? points.Length - 2 : index - 1;
        int indexRightHandle = index == oldLength - 1 ? 1 : index + 1;
        //points[indexRightHandle] = points[indexLeftHandle];

        // Offset the new points at an angle.
        var offsetVector = new Vector3(0, 70, 0);
        points[newPointIndex] = points[index] + offsetVector;
        points[newPointLeftHandle] = points[indexLeftHandle] + offsetVector;
        points[newPointRightHandle] = points[indexRightHandle] + offsetVector;

        Array.Resize(ref modes, modes.Length + 1);
        modes[modes.Length - 1] = modes[modes.Length - 2];
        EnforceMode(points.Length - 4);

        if (loop)
        {
            points[points.Length - 1] = points[0];
            modes[modes.Length - 1] = modes[0];
            EnforceMode(0);
        }
    }

    public void DeleteCurves(IEnumerable<int> selectedPoints)
    {
        // Sort the selected points
        IOrderedEnumerable<int> sortedPoints = selectedPoints.OrderByDescending(x => x);

        int affectedCount = selectedPoints.Count();

        foreach (var sortedPoint in sortedPoints)
        {
            var index = sortedPoint;

            // Skip bad parameters.
            if (index >= points.Length || index < 0)
            {
                affectedCount -= 1;
                continue;
            }

            // Shift everything in the array to the left 3.
            // Start from the deleted index and move right.
            // This overwrites the deleted index and handles.
            for (int i = index - 1; i < points.Length - 3; i++)
            {
                points[i] = points[i + 3];
            }
        }

        // Resize the array to match the new size.
        Array.Resize(ref points, points.Length - (affectedCount * 3));
        Array.Resize(ref modes, modes.Length - affectedCount);
    }

    public void Reset () {
		points = new Vector3[] {
			new Vector3(1f, 0f, 0f),
			new Vector3(2f, 0f, 0f),
			new Vector3(3f, 0f, 0f),
			new Vector3(4f, 0f, 0f)
		};
		modes = new BezierControlPointMode[] {
			BezierControlPointMode.Free,
			BezierControlPointMode.Free
		};
	}

    /// <summary>
    /// Serialize this spline to a more serializable data type.
    /// </summary>
    /// <returns></returns>
    public SerializableBezierSpline GetSerializableBezierSpline()
    {
        return new SerializableBezierSpline(points, modes);
    }

    /// <summary>
    /// Given a serialized spline, set the behavior's spline to match.
    /// </summary>
    /// <param name="serializableBezierSpline"></param>
    public void SetSpline(SerializableBezierSpline serializableBezierSpline)
    {
        this.points = serializableBezierSpline.Points;
        this.modes = serializableBezierSpline.Modes;
    }

    /// <summary>
    /// Read through the spline at a set rate to pass into the VertexArray
    /// </summary>
    /// <param name="spline">The spline to convert into the track.</param>
    public List<Vector3> ReadSplineIntoVertexArray(float pointReadStepSize, bool loop)
    {
        List<Vector3> points = new List<Vector3>();
        float step = 0;
        while (step < 1)
        {
            Vector3 point = this.GetPoint(step);
            points.Add(point);
            step += pointReadStepSize;
        }

        // Make sure the track loops
        if(loop)
        {
            points.RemoveAt(points.Count - 1);
            points.Add(this.GetPoint(0));
        }
        
        return points;
    }

    /// <summary>
    /// Helper to wrap a spline point value when outside of the 0-1 range.
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    private float WrapSplinePositionValue(float value)
    {
        while (value > 1)
        {
            value -= 1;
        }
        while (value < 0)
        {
            value += 1;
        }
        return value;
    }
}