using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public enum AssessmentType
{
    Easy,
    Medium,
    Hard,
 
}

[System.Serializable]
public class AssessmentTypeInfo
{
    public string assesstmentName;
    public int assessmentID;
    public AssessmentType assessmentType;
    public string assessmentText; // default text for this type of assessment
    public float assessmentDuration; // time duration for this type of assessment

    // Load from JSON file
    public static AssessmentTypeInfo LoadFromJSON(string filePath)
    {
        TextAsset jsonFile = Resources.Load<TextAsset>(filePath);
        if (jsonFile != null)
        {
            return JsonUtility.FromJson<AssessmentTypeInfo>(jsonFile.text);
        }
        return null;
    }
}

public class SystemController : MonoBehaviour
{

    //for reset functionality
    public ParagraphTextPositioner paragraphTextPositioner;
    public paragraphTextGenerator paragraphTextGenerator;
    public PatternRecognizer patternRecognizer;
    public FeedbackVisualizer feedbackVisualizer;
    // Add other components that need resetting like PatternRecognizer patternRecognizer;

    // If a start state is desired, like a specific text to display at the start, it can be specified here
    private const string initialText = "..."; // Replace ... with your desired start text.

    //start and finish assessment functionality // 
    public GameObject timerDisplay;
    public float assessmentDuration = 180.0f;
    private bool isAssessmentOngoing = false;
    private float remainingTime;

    //UI gameobject that needs to be turned on when feedback UI is on
    public GameObject feedbackUI;

    public List<AssessmentTypeInfo> assessmentTypesInfo;
    private AssessmentData currentAssessment;

    private void Start()
    {
        // Hide timer by default
        if (timerDisplay) timerDisplay.SetActive(false);
        //loading from json files
        assessmentTypesInfo = new List<AssessmentTypeInfo>();
        var assessmentFiles = new string[] { "Assessments/assessment1", "Assessments/assessment2", "Assessments/assessment3" }; // add paths to all your files
        foreach (var file in assessmentFiles)
        {
            var info = AssessmentTypeInfo.LoadFromJSON(file);
            if (info != null)
            {
                assessmentTypesInfo.Add(info);
            }
        }
    }


    private void ResetSystem()
    {
        // 1. Reset the text generator.
        paragraphTextGenerator.Reset(); // Assuming you have a Reset method in paragraphTextGenerator.

        // 2. Reset the text positioner.
        paragraphTextPositioner.Reset(); // Assuming you have a ResetPosition method in ParagraphTextPositioner.

        // 3. Reset the pattern recognizer or any other analyzer.
        patternRecognizer.Reset();

        //feedbackVisualizer.Reset();

        currentAssessment = null;
    }

    public void OnButtonLoadAssessment(int index)
    {
        StartCoroutine(DelayedLoadAssessment(index));
    }

    private IEnumerator DelayedLoadAssessment(int index)
    {
        yield return new WaitForSeconds(0.1f); // Wait for 0.1 seconds
        LoadAssessment(index);
    }

    //this is where the words are already loaded and prefabs are instantiated //index corresponds to the difficulty level 
    public void LoadAssessment(int index)
    {
        if (index < 0 || index >= assessmentTypesInfo.Count)
        {
            Debug.LogError($"Invalid index: {index}. Cannot load assessment.");
            return;
        }

        // 1. Retrieve the assessment info
        AssessmentTypeInfo selectedAssessment = assessmentTypesInfo[index];

        // 2. Set the text in your paragraphTextGenerator
        paragraphTextPositioner.createAndPlaceTextObjects(selectedAssessment.assessmentText); // Assuming you have a SetText method.

        // Set the assessment duration
        assessmentDuration = selectedAssessment.assessmentDuration;

        // Display the timer
        if (timerDisplay) timerDisplay.SetActive(true);
    }

    //this needs to store user's data //must be called to create a new assessment
    public void StartAssessment(int index)
    {

        StudentData activeStudent = StudentDataManager.Instance.GetActiveStudent();
        if (activeStudent != null)
        {
            // Create a new assessment with an ID (for simplicity, I'm using the count, you can use another scheme)
            currentAssessment = activeStudent.CreateNewAssessment(activeStudent.assessments.Count + 1, assessmentTypesInfo[index].assesstmentName);
        }
        else
        {
            Debug.LogError("No active student found!");
            return;
        }
        //
        isAssessmentOngoing = true;
        PatternRecognizer.Instance.isAssessmentOnGoing = isAssessmentOngoing;
        remainingTime = assessmentDuration;

        // Start the countdown
        StartCoroutine(CountdownTimer());

    }

    private IEnumerator CountdownTimer()
    {
        while (isAssessmentOngoing && remainingTime > 0)
        {
            remainingTime -= Time.deltaTime;

            if (timerDisplay)
            {
                // Convert time to minutes:seconds format
                int minutes = Mathf.FloorToInt(remainingTime / 60F);
                int seconds = Mathf.FloorToInt(remainingTime - minutes * 60);
                string formattedTime = string.Format("{0:0}:{1:00}", minutes, seconds);

                timerDisplay.GetComponent<TextMeshProUGUI>().text = formattedTime;
            }

            yield return null; // Wait for next frame
        }

        if (isAssessmentOngoing) // If still ongoing, it means timer finished the assessment.
        {
            FinishAssessment();
        }
    }

    //must store user data and other data such as data required to draw the visualizer/analysis 
    public void FinishAssessment()
    {
        isAssessmentOngoing = false;
        PatternRecognizer.Instance.isAssessmentOnGoing = isAssessmentOngoing;

        StudentData activeStudent = StudentDataManager.Instance.GetActiveStudent();
        if(activeStudent != null)
        {
            currentAssessment.AddGazeIndexList(PatternRecognizer.Instance.gazeIndexHistory);
            currentAssessment.AddTimeIndexList(PatternRecognizer.Instance.timeIndexHistory);
            currentAssessment.isCompleted = true;
            currentAssessment.analysisReport = PatternRecognizer.Instance.report;
        }

        // Hide the timer
        if (timerDisplay) timerDisplay.SetActive(false);
        if (feedbackUI) feedbackUI.SetActive(true);

        //hide the text prefabs
        paragraphTextPositioner.HideAllTextObjects();
    }
}
