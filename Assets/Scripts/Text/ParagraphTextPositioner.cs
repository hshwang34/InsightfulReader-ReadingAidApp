using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;

public class ParagraphTextPositioner : MonoBehaviour
{
    public paragraphTextGenerator textGenerator;
    public GameObject plane;
    private float spaceBetweenWords = 0.1f;
    private float spaceBetweenSentences = 0.1f;
    private float ZFightingOffset = 0.12f;

    private Vector3 currentPoint;
    private float planeWidth;
    private float planeHeight;
    private List<List<GameObject>> pages = new List<List<GameObject>>();
    private int currentPageIndex = 0;

    public Slider progressBar;
    public GameObject pageDisplayText;

    void Start()
    {
        planeWidth = plane.GetComponent<MeshRenderer>().bounds.size.z;
        planeHeight = plane.GetComponent<MeshRenderer>().bounds.size.y;

        currentPoint = plane.transform.position;
        currentPoint.y += planeHeight / 2;
        currentPoint.z -= planeWidth / 2;

        if (progressBar)
        {
            progressBar.interactable = false;  // Disable manual changes to the progress bar by users.
            progressBar.minValue = 1;
        }
    }

    public void updateSlider(Slider newProgressBar)
    {
        progressBar = newProgressBar;
        progressBar.minValue = 1;
        progressBar.maxValue = GetTotalPages();
        progressBar.value = 0;
    }

    public void createAndPlaceTextObjects(string paragraph)
    {
        textGenerator.GenerateTextObjects(paragraph);
        SplitIntoPages();
        DisplayCurrentPage();
        if (progressBar)
        {
            progressBar.maxValue = GetTotalPages();
            UpdateProgressBarAndPageDisplay();
        }
    }

   // public void UpdateReader

    private void UpdateProgressBarAndPageDisplay()
    {
        progressBar.value = GetCurrentPageNumber();
        //pageDisplayText.GetComponent<>text = $"Page {GetCurrentPageNumber()} of {GetTotalPages()}";
    }

    void SplitIntoPages()
    {
        List<GameObject> currentWords = new List<GameObject>();
        List<GameObject> words = textGenerator.GetInstantiatedTextObjects();
    

        foreach (GameObject wordObj in words)
        {
            TextMeshPro tmp = wordObj.GetComponent<TextMeshPro>();
            float wordWidth = tmp.preferredWidth;

            if (currentPoint.z + wordWidth > plane.transform.position.z + planeWidth / 2)
            {
                currentPoint.z = plane.transform.position.z - planeWidth / 2;
                currentPoint.y -= tmp.preferredHeight + spaceBetweenSentences;

                if (plane.transform.position.y - currentPoint.y > planeHeight / 2)
                {
                    pages.Add(new List<GameObject>(currentWords));
                    currentWords.Clear();
                    currentPoint.y = plane.transform.position.y + planeHeight / 2;
                }
            }

            currentWords.Add(wordObj);
            currentPoint.z += wordWidth + spaceBetweenWords;
        }

        if (currentWords.Count > 0)
            pages.Add(currentWords);
    }

    public void updateReaderPlane(GameObject planeObject)
    {
        plane = planeObject;

        planeWidth = plane.GetComponent<MeshRenderer>().bounds.size.z;
        planeHeight = plane.GetComponent<MeshRenderer>().bounds.size.y;

        currentPoint = plane.transform.position;
        currentPoint.y += planeHeight / 2;
        currentPoint.z -= planeWidth / 2;
    }
    
    public void DisplayCurrentPage()
    {
        // Hide all words first
        foreach (var word in textGenerator.GetInstantiatedTextObjects())
        {
            word.SetActive(false);
        }

        // Only show words for the current page
        List<GameObject> currentPageWords = pages[currentPageIndex];
        currentPoint.y = plane.transform.position.y + planeHeight / 2;
        currentPoint.z = plane.transform.position.z - planeWidth / 2;

        foreach (GameObject wordObj in currentPageWords)
        {
            TextMeshPro tmp = wordObj.GetComponent<TextMeshPro>();
            float wordWidth = tmp.preferredWidth;

            if (currentPoint.z + wordWidth > plane.transform.position.z + planeWidth / 2)
            {
                currentPoint.z = plane.transform.position.z - planeWidth / 2;
                currentPoint.y -= tmp.preferredHeight + spaceBetweenSentences;
            }

            wordObj.transform.position = currentPoint + new Vector3(ZFightingOffset, -tmp.preferredHeight / 2, wordWidth / 2);
            wordObj.SetActive(true);

            currentPoint.z += wordWidth + spaceBetweenWords;
        }
    }

    public void NextPage()
    {
        if (currentPageIndex < pages.Count - 1)
        {
            currentPageIndex++;
            if (progressBar)
            {
                UpdateProgressBarAndPageDisplay();
            }
            DisplayCurrentPage();
        }
        
    }

    public void PreviousPage()
    {
        if (currentPageIndex > 0)
        {
            currentPageIndex--;
            if (progressBar)
            {
                UpdateProgressBarAndPageDisplay();
            }
            DisplayCurrentPage();
        }
    }

    public void HideAllTextObjects()
    {
        foreach (var word in textGenerator.GetInstantiatedTextObjects())
        {
            word.SetActive(false);
        }
    }

    public int GetTotalPages()
    {
        return pages.Count;
    }

    // Function to retrieve the current page number
    public int GetCurrentPageNumber()
    {
        return currentPageIndex + 1;
    }

    public void Reset()
    {
        currentPageIndex = 0;
        DisplayCurrentPage();
    }

    public void ResetState()
    {
        pages.Clear();
        currentPageIndex = 0;
        //destroy all gameobjects
    }

    public int GetLastWordIndexOnCurrentPage()
    {
        if (currentPageIndex < pages.Count)
        {
            List<GameObject> currentPageWords = pages[currentPageIndex];
            GameObject lastWord = currentPageWords[currentPageWords.Count - 1];
            return textGenerator.GetInstantiatedTextObjects().IndexOf(lastWord);
        }
        return -1;
    }

    public void resetCurrentPageIndex()
    {
        currentPageIndex = 0;
    }

    public int GetFirstWordIndexOnCurrentPage()
    {
        if (currentPageIndex < pages.Count)
        {
            List<GameObject> currentPageWords = pages[currentPageIndex];
            GameObject firstWord = currentPageWords[0];
            return textGenerator.GetInstantiatedTextObjects().IndexOf(firstWord);
        }
        return -1;
    }
}
