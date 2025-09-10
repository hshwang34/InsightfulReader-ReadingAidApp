using UnityEngine;
using TMPro;

public class ReadingTextController : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float selectThreshold = 1.0f;

    private TextMeshPro textComponent;
    private Color originalColor;
    private HighlightTextScript highlightManager;

    public int indexInList = -1;

    // Dictionary Select Tracking
    private float gazeStartTime = 0;
    public float GazeDuration { get; private set; } = 0;
    public bool HasBeenGazedAt { get; private set; } = false;
    public bool isFixation = false;

    private void Awake()
    {
        InitializeComponents();
    }

    private void InitializeComponents()
    {
        textComponent = GetComponent<TextMeshPro>();
        if (textComponent == null)
        {
            Debug.LogError($"{nameof(TextMeshPro)} component not found!", this);
            enabled = false;
            return;
        }

        originalColor = textComponent.color;
        highlightManager = HighlightTextScript.Instance;
        if (highlightManager == null)
        {
            Debug.LogError($"{nameof(HighlightTextScript)} instance not found!", this);
        }
    }

    public void OnGazeEnter()
    {
        if (HighlightTextScript.Instance?.isHighlightingEnabled == true)
        {
            ApplyGazeVisualEffects();
            highlightManager.HighlightUpToWord(indexInList);
        }
    }

    public void OnGazeExit()
    {
        // Currently no action needed on gaze exit
    }

    public void OnSelect()
    {
        if (HighlightTextScript.Instance?.isHighlightingEnabled == true)
        {
            ApplySelectionVisualEffects();
            ShowWordDefinition();
            HighlightTextScript.Instance.PauseHighlight();
            ReadingSessionController.Instance.TextSelected(gameObject);
        }
    }

    public void OnDeselect()
    {
        if (HighlightTextScript.Instance?.isHighlightingEnabled == true)
        {
            RemoveSelectionVisualEffects();
            HighlightTextScript.Instance.ResumeHighlight();
            ReadingSessionController.Instance.TextDeselected(gameObject);
        }
    }

    private void ApplyGazeVisualEffects()
    {
        textComponent.color = Color.black;
        textComponent.fontStyle |= FontStyles.Underline;
    }

    private void ApplySelectionVisualEffects()
    {
        textComponent.transform.localScale = new Vector3(1.2f, 1.2f, 1.2f);
        textComponent.fontStyle |= FontStyles.Bold;
    }

    private void RemoveSelectionVisualEffects()
    {
        textComponent.transform.localScale = Vector3.one;
        textComponent.fontStyle &= ~FontStyles.Bold;
    }

    private void ShowWordDefinition()
    {
        string word = textComponent.text.Trim();
        if (DictionaryController.Instance != null)
        {
            DictionaryController.Instance.ShowWordDefinition(word);
        }
    }

    public void SetIndex(int index)
    {
        indexInList = index;
    }

    public int GetIndex()
    {
        return indexInList;
    }

    public void Reset()
    {
        if (textComponent != null)
        {
            textComponent.color = Color.white;
        }
        
        isFixation = false;
        GazeDuration = 0;
        HasBeenGazedAt = false;
    }
}
