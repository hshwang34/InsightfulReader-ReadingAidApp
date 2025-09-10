using System.Collections;
using UnityEngine;
using TMPro;

public class ReadingSessionController : MonoBehaviour
{
    public static ReadingSessionController Instance { get; private set; }

    [Header("Text Management")]
    [SerializeField] private ParagraphTextPositioner paragraphTextPositioner;
    [SerializeField] private paragraphTextGenerator paragraphTextGenerator;

    [Header("UI References")]
    [SerializeField] private GameObject placeholderLocation;
    [SerializeField] private GameObject overlayTransparentPanel;
    [SerializeField] private GameObject readingSessionUI;
    [SerializeField] private GameObject resumeMenuDisplay;
    [SerializeField] private GameObject pauseMenuDisplay;
    [SerializeField] private GameObject timerText;
    [SerializeField] private GameObject readingProgressReport;

    [Header("Animation Settings")]
    [SerializeField] private float overlayOffset = 0.1f;

    private Vector3 oldPosition;
    private GameObject selectedObject;
    private bool isReading = false;
    private float elapsedReadingTime = 0f;
    

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {
        // Reading session can be loaded manually via OnLoadButtonClick()
    }

    private void Update()
    {
        if (isReading)
        {
            elapsedReadingTime += Time.deltaTime;
            UpdateTimerDisplay();
        }
    }

    public void OnLoadButtonClick()
    {
        StartCoroutine(DelayedLoadReadingSession());
    }

    private IEnumerator DelayedLoadReadingSession()
    {
        yield return new WaitForSeconds(0.5f);
        LoadReadingSession();
    }

    public void LoadReadingSession()
    {
        const string sampleParagraph = "Stanley and his parents had tried to pretend that he was just going away to camp for a while, just like rich kids do. When Stanley was younger he used to play with stuffed animals, and pretend the animals were at camp." + 
            " Camp Fun and Games he called it. Sometimes he'd have them play soccer with a marble. Other times they'd run an obstacle course, or go bungee jumping off a table, tied to broken rubber bands. Now Stanley tried to pretend he was going to Camp Fun and Games. Maybe he'd make some friends, he thought. At least he'd get to swim in the lake." +
            " He didn't have any friends at home. He was overweight and the kids at his middle school often teased him about his size. Even his teachers sometimes made cruel comments without realizing it. On his last day of school, his math teacher, Mrs. Bell, taught ratios. As an example, she chose the heaviest kid in the class and the lightest kid in the class, and had them weigh themselves. Stanley weighed three times as much as the other boy. Mrs.Bell wrote the ratio on the board, 3:1, unaware of how much embarrassment she had caused both of them." +
            " Stanley was arrested later that day. He looked at the guard who sat slumped in his seat and wondered of he had fallen asleep. The guard was wearing sunglasses, so Stanley couldn't see his eyes.";

        InitializeTextSystem();
        paragraphTextPositioner.createAndPlaceTextObjects(sampleParagraph);
        SetupOverlayPanel();
    }

    private void InitializeTextSystem()
    {
        paragraphTextGenerator.ResetState();
        paragraphTextPositioner.ResetState();
    }

    private void SetupOverlayPanel()
    {
        if (overlayTransparentPanel != null)
        {
            overlayTransparentPanel.SetActive(false);
        }
        else
        {
            Debug.LogWarning("No overlay panel is detected", this);
        }
    }

    private void UpdateTimerDisplay()
    {
        if (timerText == null)
            return;

        int minutes = Mathf.FloorToInt(elapsedReadingTime / 60F);
        int seconds = Mathf.FloorToInt(elapsedReadingTime - minutes * 60);
        timerText.GetComponent<TextMeshProUGUI>().text = $"{minutes:0}:{seconds:00}";
    }

    public void StartReadingSession()
    {
        isReading = true;
        SetMenuDisplay(true);
        EnableHighlightingFeatures(true);
    }

    public void PauseReadingSession()
    {
        SetMenuDisplay(false);
        isReading = false;
        elapsedReadingTime = 0f;
        EnableHighlightingFeatures(false);
    }

    private void SetMenuDisplay(bool isResumeActive)
    {
        if (resumeMenuDisplay != null)
            resumeMenuDisplay.SetActive(isResumeActive);
        if (pauseMenuDisplay != null)
            pauseMenuDisplay.SetActive(!isResumeActive);
    }

    private void EnableHighlightingFeatures(bool isEnabled)
    {
        if (HighlightTextScript.Instance != null)
        {
            HighlightTextScript.Instance.isHighlightingEnabled = isEnabled;
            HighlightTextScript.Instance.isAutoPageChangeEnabled = isEnabled;
        }
    }

    public void QuitReadingSession()
    {
        if (readingProgressReport != null)
            readingProgressReport.SetActive(true);
        if (readingSessionUI != null)
            readingSessionUI.SetActive(false);
        
        paragraphTextGenerator.DestroyAllTextPrefabs();
        paragraphTextGenerator.Reset();
    }

    public void TextSelected(GameObject selectedTextObject)
    {
        if (overlayTransparentPanel != null)
            overlayTransparentPanel.SetActive(true);
        
        oldPosition = selectedTextObject.transform.position;
        selectedTextObject.transform.position = placeholderLocation.transform.position;
        selectedObject = selectedTextObject;
    }

    public void TextDeselected(GameObject deselectedTextObject)
    {
        if (overlayTransparentPanel != null)
            overlayTransparentPanel.SetActive(false);
        
        deselectedTextObject.transform.position = oldPosition;
    }

    public void ButtonTextDeselected()
    {
        if (HighlightTextScript.Instance != null)
            HighlightTextScript.Instance.ResumeHighlight();
        
        if (overlayTransparentPanel != null)
            overlayTransparentPanel.SetActive(false);
        
        if (selectedObject != null)
            selectedObject.transform.position = oldPosition;
    }

    public Vector3 GetCenterOfPanel()
    {
        if (overlayTransparentPanel == null)
            return Vector3.zero;

        var meshRenderer = overlayTransparentPanel.GetComponent<MeshRenderer>();
        return meshRenderer != null ? meshRenderer.bounds.center : Vector3.zero;
    }
}


