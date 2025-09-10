using UnityEngine;

public class GodCanvasController : MonoBehaviour
{
    [Header("Main Canvas References")]
    [SerializeField] private GameObject introCanvas;
    [SerializeField] private GameObject studentCanvas;
    [SerializeField] private GameObject teacherCanvas;
    [SerializeField] private GameObject assessmentGameObject;
    [SerializeField] private GameObject readingProgressReportUI;

    private void Start()
    {
        ShowIntroCanvas();
    }

    public void ShowIntroCanvas()
    {
        SetCanvasStates(true, false, false, false, false);
    }

    public void ShowStudentCanvas()
    {
        SetCanvasStates(false, true, false, false, false);
    }

    public void ShowTeacherCanvas()
    {
        SetCanvasStates(false, false, true, false, false);
    }

    public void ShowReadingAssessment()
    {
        SetCanvasStates(false, false, false, true, false);
    }

    public void CloseReadingProgressUI()
    {
        SetCanvasStates(false, true, false, false, false);
    }

    private void SetCanvasStates(bool intro, bool student, bool teacher, bool assessment, bool progress)
    {
        if (introCanvas != null)
            introCanvas.SetActive(intro);
        if (studentCanvas != null)
            studentCanvas.SetActive(student);
        if (teacherCanvas != null)
            teacherCanvas.SetActive(teacher);
        if (assessmentGameObject != null)
            assessmentGameObject.SetActive(assessment);
        if (readingProgressReportUI != null)
            readingProgressReportUI.SetActive(progress);
    }
}
