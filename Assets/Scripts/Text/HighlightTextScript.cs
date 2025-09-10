using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HighlightTextScript : MonoBehaviour
{
    public static HighlightTextScript Instance { get; private set; }

    [Header("Text References")]
    [SerializeField] private List<GameObject> allTextPrefabs;
    [SerializeField] private paragraphTextGenerator textGenerator;
    [SerializeField] private ParagraphTextPositioner paragraphTextPositioner;

    [Header("Highlight Settings")]
    [SerializeField] private Color highlightBackgroundColor = Color.blue;
    [SerializeField] private Color highlightTextColor = Color.white;
    [SerializeField] private bool isAutoPageChangeEnabled = false;
    [SerializeField] private bool isHighlightingEnabled = false;

    [Header("Behavior Settings")]
    [SerializeField] private float cooldownDuration = 1.5f;
    [SerializeField] private int jumpingForwardThreshold = 7;

    private float nextSelectableTime = 0;
    private int lastHighlightedIndex = -1;
    private int pageLastIndex = -1;
    private bool isTextSelected = false;
    private Material quadMaterial;

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

        InitializeTextReferences();
    }

    private void InitializeTextReferences()
    {
        if (textGenerator != null)
        {
            allTextPrefabs = textGenerator.GetInstantiatedTextObjects();
        }
        
        if (paragraphTextPositioner != null)
        {
            pageLastIndex = paragraphTextPositioner.GetLastWordIndexOnCurrentPage();
        }
    }

    public void HighlightUpToWord(int index)
    {
        if (!CanHighlight(index))
            return;

        pageLastIndex = paragraphTextPositioner.GetLastWordIndexOnCurrentPage();

        if (index < lastHighlightedIndex)
        {
            HandleBacktracking(index);
        }
        else if (index - lastHighlightedIndex > jumpingForwardThreshold)
        {
            HighlightWord(index);
        }
        else
        {
            HandleSequentialHighlighting(index);
        }

        lastHighlightedIndex = index;
        
        if (isAutoPageChangeEnabled)
        {
            AutoPageChange(index);
        }
    }

    private bool CanHighlight(int index)
    {
        return Time.time >= nextSelectableTime && 
               isHighlightingEnabled && 
               !isTextSelected && 
               index != lastHighlightedIndex;
    }

    private void HandleBacktracking(int index)
    {
        for (int i = lastHighlightedIndex; i > index; i--)
        {
            DehighlightWord(i);
        }
    }

    private void HandleSequentialHighlighting(int index)
    {
        if (index == 0)
        {
            HighlightWord(index);
        }

        for (int i = lastHighlightedIndex + 1; i <= index; i++)
        {
            HighlightWord(i);
        }
    }

    private void HighlightWord(int index)
    {
        if (!IsValidIndex(index))
            return;

        var textComponent = allTextPrefabs[index].GetComponent<TextMeshPro>();
        var quadChild = allTextPrefabs[index].transform.Find("HighlightBackgroundQuad");
        
        if (textComponent == null)
            return;

        textComponent.color = highlightTextColor;
        
        if (quadChild != null)
        {
            quadChild.gameObject.SetActive(true);
            var quadRenderer = quadChild.GetComponent<Renderer>();
            if (quadRenderer != null)
            {
                if (quadMaterial == null)
                {
                    quadMaterial = quadRenderer.material;
                }
                quadMaterial.color = highlightBackgroundColor;
            }
        }
    }

    private void DehighlightWord(int index)
    {
        if (!IsValidIndex(index))
            return;

        var textComponent = allTextPrefabs[index].GetComponent<TextMeshPro>();
        var quadChild = allTextPrefabs[index].transform.Find("HighlightBackgroundQuad");
        
        if (textComponent == null)
            return;

        textComponent.color = Color.black;
        
        if (quadChild != null)
        {
            quadChild.gameObject.SetActive(false);
        }
    }

    private bool IsValidIndex(int index)
    {
        return index >= 0 && index < allTextPrefabs.Count;
    }

    public void ToggleHighlighting()
    {
        isHighlightingEnabled = !isHighlightingEnabled;
    }

    public void SetHighlightColor(Color newHighlightColor)
    {
        highlightBackgroundColor = newHighlightColor;
    }

    public void SetHighlightTextColor(Color newHighlightTextColor)
    {
        highlightTextColor = newHighlightTextColor;
    }

    public void ToggleAutoPageChange()
    {
        isAutoPageChangeEnabled = !isAutoPageChangeEnabled;
    }

    private void AutoPageChange(int index)
    {
        if (index == pageLastIndex && paragraphTextPositioner != null)
        {
            paragraphTextPositioner.NextPage();
            pageLastIndex = paragraphTextPositioner.GetLastWordIndexOnCurrentPage();
            nextSelectableTime = Time.time + cooldownDuration;
        }
    }

    public void PauseHighlight()
    {
        isTextSelected = true;
    }

    public void ResumeHighlight()
    {
        isTextSelected = false;
    }
    
}
