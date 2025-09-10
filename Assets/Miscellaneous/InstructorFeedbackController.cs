using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InstructorFeedbackController : MonoBehaviour
{
    [Header("System References")]
    [SerializeField] private SystemController systemController;

    [Header("UI Canvas References")]
    [SerializeField] private GameObject analysisReportCanvas;
    [SerializeField] private GameObject instructorFeedbackCanvas;
    [SerializeField] private GameObject preAnalysisFeedbackObject;
    [SerializeField] private GameObject preAnalysisFeedbackText;
    [SerializeField] private GameObject pageAnalysisReportText;

    [Header("Control Elements")]
    [SerializeField] private GameObject newTextPrefabPlane;
    [SerializeField] private Slider newProgressBar;
    [SerializeField] private Toggle fixationToggle;
    [SerializeField] private Toggle eyeTraceToggle;

    private AssessmentData lastAssessment;
    private List<AssessmentTypeInfo> assessmentTypesInfo;
    private int startIndex = 0;
    private int endIndex = 0;

    private void Start()
    {
        InitializeController();
    }

    private void InitializeController()
    {
        if (systemController == null)
        {
            Debug.LogError($"{nameof(SystemController)} reference is missing!", this);
            enabled = false;
            return;
        }

        assessmentTypesInfo = systemController.assessmentTypesInfo;
        systemController.paragraphTextPositioner.updateSlider(newProgressBar);
        ShowInstructorFeedbackCanvas();
    }

    public void ShowInstructorFeedbackCanvas()
    {
        SetCanvasStates(true, false, false);
    }

    public void ShowPreAnalysisFeedbackCanvas()
    {
        SetCanvasStates(false, false, true);
        UpdatePreAnalysisIndices();
        DisplayPreAnalysisReport();
    }

    public void ShowAnalysisReportCanvas()
    {
        SetCanvasStates(false, true, false);
        LoadLastAssessment();
    }

    private void SetCanvasStates(bool instructor, bool analysis, bool preAnalysis)
    {
        if (instructorFeedbackCanvas != null)
            instructorFeedbackCanvas.SetActive(instructor);
        if (analysisReportCanvas != null)
            analysisReportCanvas.SetActive(analysis);
        if (preAnalysisFeedbackObject != null)
            preAnalysisFeedbackObject.SetActive(preAnalysis);
    }

    private void UpdatePreAnalysisIndices()
    {
        startIndex = 0;
        endIndex = PatternRecognizer.Instance.allTextPrefabs.Count - 1;
    }

    private void DisplayPreAnalysisReport()
    {
        if (PatternRecognizer.Instance != null && preAnalysisFeedbackText != null)
        {
            PatternRecognizer.Instance.DisplayAnalysisReport(preAnalysisFeedbackText, startIndex, endIndex);
        }
    }

    public void LoadLastAssessment()
    {
        var activeStudent = StudentDataManager.Instance.GetActiveStudent();

        if (activeStudent?.assessments.Count > 0)
        {
            lastAssessment = activeStudent.assessments[activeStudent.assessments.Count - 1];
        }
        else
        {
            Debug.LogError("No active student found or student has no assessments.", this);
            return;
        }

        SetupAssessmentDisplay();
        DisplayEyeTraceAndFixations();
    }

    private void SetupAssessmentDisplay()
    {
        systemController.paragraphTextPositioner.updateReaderPlane(newTextPrefabPlane);
        systemController.paragraphTextPositioner.resetCurrentPageIndex();
        systemController.paragraphTextPositioner.DisplayCurrentPage();
    }

    private void DisplayEyeTraceAndFixations()
    {
        UpdatePageIndices();
        CreateVisualFeedback();
        DisplayPageAnalysisReport();
    }

    private void UpdatePageIndices()
    {
        startIndex = systemController.paragraphTextPositioner.GetFirstWordIndexOnCurrentPage();
        endIndex = systemController.paragraphTextPositioner.GetLastWordIndexOnCurrentPage();
    }

    private void CreateVisualFeedback()
    {
        systemController.feedbackVisualizer.CreateVisualFeedback(startIndex, endIndex);
    }

    private void DisplayPageAnalysisReport()
    {
        if (PatternRecognizer.Instance != null && pageAnalysisReportText != null)
        {
            PatternRecognizer.Instance.DisplayAnalysisReport(pageAnalysisReportText, startIndex, endIndex);
        }
    }

    public void NextPage()
    {
        RefreshPageDisplay();
        systemController.paragraphTextPositioner.NextPage();
        DisplayEyeTraceAndFixations();
    }

    public void PreviousPage()
    {
        RefreshPageDisplay();
        systemController.paragraphTextPositioner.PreviousPage();
        DisplayEyeTraceAndFixations();
    }

    private void RefreshPageDisplay()
    {
        systemController.feedbackVisualizer.DestroyVisualFeedback();
    }

    public void TurnOffPreAnalysisReport()
    {
        if (preAnalysisFeedbackObject != null)
            preAnalysisFeedbackObject.SetActive(false);
    }

    public void TurnOnPreAnalysisReport()
    {
        if (preAnalysisFeedbackObject != null)
            preAnalysisFeedbackObject.SetActive(true);
    }

    public void TurnOnAnalysisReportCanvas()
    {
        if (analysisReportCanvas != null)
            analysisReportCanvas.SetActive(true);
    }

    public void OnApplyChangesPressed()
    {
        UpdateVisualizationSettings();
        RefreshPageDisplay();
        DisplayEyeTraceAndFixations();
    }

    private void UpdateVisualizationSettings()
    {
        if (fixationToggle != null)
        {
            systemController.feedbackVisualizer.showFixations = fixationToggle.isOn;
        }

        if (eyeTraceToggle != null)
        {
            systemController.feedbackVisualizer.showEyeTrace = eyeTraceToggle.isOn;
        }
    }

    public void OnQuitInstructorView()
    {
        systemController.feedbackVisualizer.DestroyVisualFeedback();
        systemController.paragraphTextGenerator.DestroyAllTextPrefabs();
    }
}
