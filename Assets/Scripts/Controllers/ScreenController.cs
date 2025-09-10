using UnityEngine;

public class ScreenController : MonoBehaviour
{
    [Header("Screen Panels")]
    [SerializeField] private GameObject startScreenPanel;
    [SerializeField] private GameObject chooseAudiencePanel;
    [SerializeField] private GameObject studentSignInPanel;
    [SerializeField] private GameObject instructorSignInPanel;

    private void Start()
    {
        ShowStartScreen();
    }

    public void ShowStartScreen()
    {
        SetScreenStates(true, false, false, false);
    }

    public void ShowChooseAudienceScreen()
    {
        SetScreenStates(false, true, false, false);
    }

    public void ShowInstructorSignInScreen()
    {
        SetScreenStates(false, false, false, true);
    }

    public void ShowStudentSignInScreen()
    {
        SetScreenStates(false, false, true, false);
    }

    private void SetScreenStates(bool start, bool audience, bool student, bool instructor)
    {
        if (startScreenPanel != null)
            startScreenPanel.SetActive(start);
        if (chooseAudiencePanel != null)
            chooseAudiencePanel.SetActive(audience);
        if (studentSignInPanel != null)
            studentSignInPanel.SetActive(student);
        if (instructorSignInPanel != null)
            instructorSignInPanel.SetActive(instructor);
    }
}