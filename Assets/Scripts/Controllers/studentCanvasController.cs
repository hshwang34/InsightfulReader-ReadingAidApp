using UnityEngine;

public class StudentCanvasController : MonoBehaviour
{
    [Header("Student Panels")]
    [SerializeField] private GameObject readingBookPanel;
    [SerializeField] private GameObject takeAssessmentPanel;
    [SerializeField] private GameObject studentHomePanel;

    private void Start()
    {
        ShowStartScreen();
    }

    public void ShowStartScreen()
    {
        SetPanelStates(true, false, false);
    }

    public void ShowAssessmentPanel()
    {
        SetPanelStates(false, true, false);
    }

    public void ShowReadingBookPanel()
    {
        SetPanelStates(false, false, true);
    }

    private void SetPanelStates(bool home, bool assessment, bool reading)
    {
        if (studentHomePanel != null)
            studentHomePanel.SetActive(home);
        if (takeAssessmentPanel != null)
            takeAssessmentPanel.SetActive(assessment);
        if (readingBookPanel != null)
            readingBookPanel.SetActive(reading);
    }
}
