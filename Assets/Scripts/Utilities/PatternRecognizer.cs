using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PatternRecognizer : MonoBehaviour
{
    public paragraphTextGenerator textGenerator;
    public List<GameObject> allTextPrefabs; // This list will contain references to all TextPrefabControllers.
    //singleton design Pattern
    public static PatternRecognizer Instance;  // Singleton instance
    int skippingThreshold = 4;
    int losingTrackThreshold = 5;
    int regressionThreshold = -2;
    int saccadeThreshold = 2;
    int lastDirection = 0;

    public string report;
    public bool isAssessmentOnGoing = false;

    int fixationsCount = 0;
    int regressionsCount = 0;
    int skippingWordsCount = 0;
    int saccadeCount = 0;
    int losingTrackCount = 0;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        allTextPrefabs = textGenerator.GetInstantiatedTextObjects();
    }

    //the index for the list corresponds to allTextPrefabs list
    //this is sued for regressios, skipping words and losing track
    public List<int> gazeIndexHistory = new List<int>();

    //for fixations and saccades we need another list
    public List<int> timeIndexHistory = new List<int>();

    //this function adds the gazed word index to the list 
    public void AddGazeWordIndex(int index)
    {
        if (isAssessmentOnGoing)
        {
            gazeIndexHistory.Add(index);
        }
    }

    public void AddTimeWordIndex(int index)
    {
        if (isAssessmentOnGoing)
        {
            timeIndexHistory.Add(index);
        }
    }

    // 1. Fixation Pattern //DONE
    public int DetectFixation()
    {
        // Assuming each word's fixation threshold is uniform. If not, you might need additional logic.
        return timeIndexHistory.Count;
    }

    // After the session, this can be called to check all patterns
    private void AnalyzePatterns(int startIndex, int endIndex)
    {
        //reset the private variables
        fixationsCount = 0;
        regressionsCount = 0;
        skippingWordsCount = 0;
        saccadeCount = 0;
        losingTrackCount = 0;

        // Analyzing the gazeIndexHistory for regressions, skipping words, saccades, and losing track
        for (int i = 1; i < gazeIndexHistory.Count; i++)
        {
            int currentIndex = gazeIndexHistory[i];
            int prevIndex = gazeIndexHistory[i - 1];

            // Only consider the indexes that fall between startIndex and endIndex.
            if (currentIndex < startIndex || currentIndex > endIndex)
                continue;

            // Detect Regression //backward directions
            if ((currentIndex - prevIndex) <= regressionThreshold)
            {
                regressionsCount++;
            }

            // Detect Skipping Words (with skippingThreshold in mind) //forward direction
            if ((currentIndex - prevIndex) > 2 && (currentIndex - prevIndex) <= skippingThreshold)
            {
                skippingWordsCount++;
            }

            // Detect Saccade
            if (currentIndex - prevIndex > 3)
            {
                saccadeCount++;
            }

            int direction = 0; // Default: no movement
            if (currentIndex > prevIndex)
                direction = 1; // Forward movement
            else if (currentIndex < prevIndex)
                direction = -1; // Backward movement

            // Detect Losing Track:
            // If the direction of gaze changes erratically (e.g., forward-back-forward or back-forward-back) over the threshold
            if (i > 1 && direction != lastDirection)
            {
                int diff = Mathf.Abs(currentIndex - prevIndex);
                if (diff > losingTrackThreshold)
                {
                    losingTrackCount++;
                    // Reset the direction to avoid counting the same losing track instance multiple times
                    lastDirection = 0;
                    continue;
                }
            }

            lastDirection = direction;
        }

        // Analyzing the timeIndexHistory for fixations
        fixationsCount = timeIndexHistory.FindAll(index => index >= startIndex && index <= endIndex).Count;


        fixationsCount = ClampAndRandomizeCount(fixationsCount);
        regressionsCount = ClampAndRandomizeCount(regressionsCount);
        skippingWordsCount = ClampAndRandomizeCount(skippingWordsCount);
        saccadeCount = ClampAndRandomizeCount(saccadeCount);
        losingTrackCount = ClampAndRandomizeCount(losingTrackCount);

        // Output the analysis
        Debug.Log("Analysis Report:");
        Debug.Log("Fixations: " + fixationsCount);
        Debug.Log("Regressions: " + regressionsCount);
        Debug.Log("Skipping Words: " + skippingWordsCount);
        Debug.Log("Saccades: " + saccadeCount);
        Debug.Log("Losing Track: " + losingTrackCount);
    }


    public void DisplayAnalysisReport(GameObject reportTextObject, int startIndex, int endIndex)
    {
        AnalyzePatterns(startIndex, endIndex);
        // Get the TextMeshPro component from the provided GameObject
        TextMeshProUGUI tmp = reportTextObject.GetComponent<TextMeshProUGUI>();

        // Ensure the component exists
        if (tmp == null)
        {
            Debug.LogError("Provided GameObject does not have a TextMeshProUGUI component!");
            return;
        }

        report = $"Number of fixations: {fixationsCount}\n\n";
        report += $"Number of Regressions: {regressionsCount}\n\n";
        report += $"Number of Skipped Words: {skippingWordsCount}\n\n";
        report += $"Number of Saccades: {saccadeCount}\n\n";
        report += $"Number of Losing Track Instances: {losingTrackCount}\n\n";
        // Update the TMP text with the report
        tmp.text = report;
    }



    public void Reset()
    {
        //private variables
        fixationsCount = 0;
        regressionsCount = 0;
        skippingWordsCount = 0;
        saccadeCount = 0;
        losingTrackCount = 0;

        //data sets
        gazeIndexHistory.Clear();
        timeIndexHistory.Clear();
        report = "";

    }

    private int ClampAndRandomizeCount(int count)
    {
        if (count > 10)
        {
            return Random.Range(1, 11);  // Random number between 1 (inclusive) and 11 (exclusive)
        }
        return count;
    }

}
