using UnityEngine;
using TMPro;

public class textPrefabController : MonoBehaviour
{
    private TextMeshPro textComponent;
    private Color currentColor;
    public Color targetColor = Color.red; // Set this to whatever target color you want.
    private int numberOfCallsRequired = 20;
    private int currentCallCount = 0;
    private Color incrementValue;

    public int indexInList = -1;

    // Gaze Tracking
    private float gazeStartTime = 0;
    public float GazeDuration { get; private set; } = 0;
    public bool HasBeenGazedAt { get; private set; } = false;
    public bool isFixation = false;
    public float fixateThreshold = 1.0f;

    private bool isHovering = false;

    private void Awake()
    {
        textComponent = GetComponent<TextMeshPro>();
        if (textComponent == null)
        {
            Debug.LogError("TextMeshPro component not found in children!");
            return;
        }

        currentColor = textComponent.color;
        incrementValue = (targetColor - currentColor) / numberOfCallsRequired;
    }

    public void ChangeToTargetColor()
    {
        currentCallCount++;

        if (currentCallCount <= numberOfCallsRequired)
        {
            textComponent.color += incrementValue;
        }
        else
        {
            textComponent.color = targetColor;
        }
    }

    public void ChangeToBlue()
    {
        textComponent.color = Color.blue;
    }


    //this is the start of the script for pattern recognition
    public void OnGazeEnter()
    {
        if (!isHovering)
        {
            isHovering = true;
            gazeStartTime = Time.time;
            HasBeenGazedAt = true;
            PatternRecognizer.Instance.AddGazeWordIndex(indexInList);
        }
    }

    public void OnGazeExit()
    {
        if (isHovering)
        {
            GazeDuration = Time.time - gazeStartTime;
            if (GazeDuration > fixateThreshold)
            {
                PatternRecognizer.Instance.AddTimeWordIndex(indexInList);
                isFixation = true;
            }
            isHovering = false;
        }
    }

    //public void OnFixatedEnter()
    //{

    //    PatternRecognizer.Instance.AddTimeWordIndex(indexInList);
    //    //isHovering = true;
    //    gazeStartTime = Time.time;
    //    // HasBeenGazedAt = true;

    //}

    //public void OnFixatedExit()
    //{

    //    GazeDuration = Time.time - gazeStartTime;
    //    isFixation = true;
    //    // isHovering = false;

    //}

    //sets the index in list
    public void SetIndex(int index)
    {
        indexInList = index;
    }


    //returns the index value //this should be called in hover function.
    public int GetIndex()
    {
        return indexInList;
    }

    public void Reset()
    {
        textComponent.color = Color.black;
        currentCallCount = 0;
        isFixation = false;
        GazeDuration = 0;
        HasBeenGazedAt = false;
    }
}
