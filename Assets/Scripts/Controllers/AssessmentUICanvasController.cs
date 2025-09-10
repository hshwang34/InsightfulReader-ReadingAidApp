using UnityEngine;

public class AssessmentUICanvasController : MonoBehaviour
{
    [Header("Canvas References")]
    [SerializeField] private GameObject instructorFeedbackCanvasObjects;
    [SerializeField] private GameObject preAssessmentCanvas;
    [SerializeField] private GameObject assessmentCanvas;
    [SerializeField] private GameObject assessmentFeedbackCanvas;

    private void Start()
    {
        ShowAssessmentStartScreen();
    }

    private void OnEnable()
    {
        ShowAssessmentStartScreen();
    }

    public void ShowAssessmentStartScreen()
    {
        SetCanvasStates(true, true, false, false);
    }

    public void ShowAssessmentScreen()
    {
        SetCanvasStates(false, true, false, false);
    }

    public void ShowAssessmentFeedbackScreen()
    {
        SetCanvasStates(false, false, true, false);
    }

    public void ShowInstructorFeedbackScreen()
    {
        SetCanvasStates(false, false, false, true);
    }

    public void TurnOffAllCanvasObjects()
    {
        SetCanvasStates(false, false, false, false);
    }

    private void SetCanvasStates(bool preAssessment, bool assessment, bool feedback, bool instructor)
    {
        if (preAssessmentCanvas != null)
            preAssessmentCanvas.SetActive(preAssessment);
        if (assessmentCanvas != null)
            assessmentCanvas.SetActive(assessment);
        if (assessmentFeedbackCanvas != null)
            assessmentFeedbackCanvas.SetActive(feedback);
        if (instructorFeedbackCanvasObjects != null)
            instructorFeedbackCanvasObjects.SetActive(instructor);
    }
}
