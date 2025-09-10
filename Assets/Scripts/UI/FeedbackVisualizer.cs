using System.Collections.Generic;
using UnityEngine;

public class FeedbackVisualizer : MonoBehaviour
{
    [Header("Visualization Settings")]
    [SerializeField] private GameObject circlePrefab;
    [SerializeField] private bool showEyeTrace = true;
    [SerializeField] private bool showFixations = true;

    [Header("Line Settings")]
    [SerializeField] private float greenLineWidth = 0.01f;
    [SerializeField] private float redLineWidth = 0.02f;
    [SerializeField] private float yLineOffset = -0.05f;
    [SerializeField] private float offsetVisibility = 0.01f;

    [Header("Circle Settings")]
    [SerializeField] private float minCircleScale = 0.05f;
    [SerializeField] private float maxCircleScale = 0.3f;

    private readonly List<LineRenderer> lines = new List<LineRenderer>();
    private readonly List<GameObject> circles = new List<GameObject>();
    private bool isFeedbackVisible = false;

    public void CreateVisualFeedback(int startIndex, int endIndex)
    {
        if (PatternRecognizer.Instance == null)
        {
            Debug.LogWarning($"{nameof(PatternRecognizer)} instance not found!", this);
            return;
        }

        var gazeIndexHistory = PatternRecognizer.Instance.gazeIndexHistory;
        var timeIndexHistory = PatternRecognizer.Instance.timeIndexHistory;
        var allTextPrefabs = PatternRecognizer.Instance.allTextPrefabs;

        if (showEyeTrace)
        {
            CreateEyeTraceLines(gazeIndexHistory, allTextPrefabs, startIndex, endIndex);
        }

        if (showFixations)
        {
            CreateFixationCircles(timeIndexHistory, allTextPrefabs, startIndex, endIndex);
        }
    }

    private void CreateEyeTraceLines(List<int> gazeIndexHistory, List<GameObject> allTextPrefabs, int startIndex, int endIndex)
    {
        for (int i = 1; i < gazeIndexHistory.Count; i++)
        {
            int currentIndex = gazeIndexHistory[i];
            int prevIndex = gazeIndexHistory[i - 1];

            if (!IsIndexInRange(currentIndex, startIndex, endIndex) || 
                !IsIndexInRange(prevIndex, startIndex, endIndex))
                continue;

            var line = CreateLine();
            var startPos = GetAdjustedPosition(allTextPrefabs[prevIndex].transform.position);
            var endPos = GetAdjustedPosition(allTextPrefabs[currentIndex].transform.position);

            line.SetPositions(new Vector3[] { startPos, endPos });
            ConfigureLineAppearance(line, currentIndex > prevIndex);
        }
    }

    private void CreateFixationCircles(List<int> timeIndexHistory, List<GameObject> allTextPrefabs, int startIndex, int endIndex)
    {
        foreach (int index in timeIndexHistory)
        {
            if (!IsIndexInRange(index, startIndex, endIndex))
                continue;

            var circle = CreateFixationCircle(allTextPrefabs[index].transform.position, index);
            circles.Add(circle);
        }
    }

    private bool IsIndexInRange(int index, int startIndex, int endIndex)
    {
        return index >= startIndex && index <= endIndex;
    }

    private Vector3 GetAdjustedPosition(Vector3 originalPosition)
    {
        return new Vector3(
            originalPosition.x,
            originalPosition.y + yLineOffset,
            originalPosition.z + offsetVisibility
        );
    }

    private void ConfigureLineAppearance(LineRenderer line, bool isForwardMovement)
    {
        if (isForwardMovement)
        {
            line.startColor = line.endColor = Color.green;
            line.startWidth = line.endWidth = greenLineWidth;
        }
        else
        {
            line.startColor = line.endColor = Color.red;
            line.startWidth = line.endWidth = redLineWidth;
        }
    }

    private GameObject CreateFixationCircle(Vector3 position, int index)
    {
        var circle = Instantiate(circlePrefab, position, Quaternion.Euler(0, -90, 0));
        circle.transform.position += new Vector3(0, 0, offsetVisibility);

        var gazeDuration = GetGazeDuration(index);
        var scaleValue = CalculateCircleScale(gazeDuration);
        circle.transform.localScale = new Vector3(scaleValue, scaleValue, scaleValue);

        return circle;
    }

    private float GetGazeDuration(int index)
    {
        // This would need to be implemented based on your gaze tracking system
        return 1.0f; // Placeholder
    }

    private float CalculateCircleScale(float gazeDuration)
    {
        var normalizedDuration = Mathf.Clamp01((gazeDuration - 1) / (4 - 1));
        return Mathf.Lerp(minCircleScale * 1.5f, maxCircleScale * 1.5f, normalizedDuration);
    }

    private LineRenderer CreateLine()
    {
        var lineObj = new GameObject("Line");
        var line = lineObj.AddComponent<LineRenderer>();
        line.material = new Material(Shader.Find("Sprites/Default"));
        line.startWidth = line.endWidth = 0.05f;
        lines.Add(line);
        return line;
    }

    public void DestroyVisualFeedback()
    {
        DestroyAllLines();
        DestroyAllCircles();
        ClearCollections();
    }

    private void DestroyAllLines()
    {
        foreach (var line in lines)
        {
            if (line != null)
            {
                Destroy(line.gameObject);
            }
        }
    }

    private void DestroyAllCircles()
    {
        foreach (var circle in circles)
        {
            if (circle != null)
            {
                Destroy(circle);
            }
        }
    }

    private void ClearCollections()
    {
        lines.Clear();
        circles.Clear();
    }

    public void ToggleVisualFeedback()
    {
        if (isFeedbackVisible)
        {
            HideFeedback();
        }
        else
        {
            ShowFeedback();
        }
        isFeedbackVisible = !isFeedbackVisible;
    }

    private void HideFeedback()
    {
        SetFeedbackVisibility(false);
    }

    private void ShowFeedback()
    {
        SetFeedbackVisibility(true);
    }

    private void SetFeedbackVisibility(bool isVisible)
    {
        foreach (var line in lines)
        {
            if (line != null)
            {
                line.gameObject.SetActive(isVisible);
            }
        }

        foreach (var circle in circles)
        {
            if (circle != null)
            {
                circle.SetActive(isVisible);
            }
        }
    }

}

