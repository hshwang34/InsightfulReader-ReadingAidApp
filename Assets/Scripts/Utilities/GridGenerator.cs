using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using TMPro;

public class GridGenerator : MonoBehaviour
{

    public static GridGenerator Instance { get; private set; }

    public GameObject planePrefab;
    public Transform gridSpawnLocation;
    private const int gridWidth = 8;
    private const int gridHeight = 20;
    private Color testHighlightColor = Color.yellow;
    private Color centerHighlightColor = Color.black;
    private Color correctHighlightColor = Color.green;
    private Color userHighlightColor = Color.red;
    //private Color userGazeHighlightMaterial = Color.red;

    private GameObject[,] gridObjects = new GameObject[gridWidth, gridHeight];
    private List<Vector2Int> testPath = new List<Vector2Int>();
    private List<Vector2Int> userSelectionPath = new List<Vector2Int>();

    private bool hasRecordedGaze = false;

    public GameObject readyCanvas;
    public GameObject resultIndicatorCanvas;
    public GameObject resultCanvas;
    public GameObject resultText;

    public AudioSource testAudioSource;
    public AudioSource userSelectedAudio;

    public GameObject previousScreen;

    private void Awake()
    {
        // Singleton setup
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        GenerateGrid();
        TestLocationInitialize();
        readyCanvas.SetActive(true);
        resultIndicatorCanvas.SetActive(false);
        resultCanvas.SetActive(false);
    }


    public void startTest()
    {
        readyCanvas.SetActive(false);
        StartCoroutine(BoxAnimationCoroutine(() =>
        {
            // These functions will be called after the coroutine finishes.
            AnalyseTracking();
            DisplayTestResult();
            resultIndicatorCanvas.SetActive(true);
            resultCanvas.SetActive(true);
        }));
    }

    public void endTest()
    {
        //destroy all the plane gameobjects
        for (int y = 0; y < gridWidth; y++)
        {
            for (int z = 0; z < gridHeight; z++)
            {
                if (gridObjects[y, z] != null) // Check if the grid object at this position exists
                {
                    Destroy(gridObjects[y, z]);
                    gridObjects[y, z] = null; // Set the reference in the array to null
                }
            }
        }

        // Reset test and user paths
        testPath.Clear();
        userSelectionPath.Clear();


        readyCanvas.SetActive(false);
        resultIndicatorCanvas.SetActive(false);
        resultCanvas.SetActive(false);
        //return to the student canvas
        previousScreen.SetActive(true);
    }

    void GenerateGrid()
    {
        Renderer planeRenderer = planePrefab.GetComponent<Renderer>();
        float planeWidth = planeRenderer.bounds.size.x;
        float planeHeight = planeRenderer.bounds.size.z;
        float totalWidth = planeWidth * gridWidth;
        float totalHeight = planeHeight * gridHeight;

        Vector3 startPosition = gridSpawnLocation.position - new Vector3(0, totalWidth / 2, totalHeight / 2) + new Vector3(0, planeWidth / 2, planeHeight / 2);

        for (int y = 0; y < gridWidth; y++)
        {
            for (int z = 0; z < gridHeight; z++)
            {
                Vector3 position = new Vector3(gridSpawnLocation.position.x, startPosition.y + y * planeWidth, startPosition.z + z * planeHeight);
                Quaternion planeRotation = Quaternion.Euler(90, 90, 0);
                gridObjects[y, z] = Instantiate(planePrefab, position, planeRotation, transform);
                gridObjects[y, z].GetComponent<planePrefabController>().xCoordinate = y;
                gridObjects[y, z].GetComponent<planePrefabController>().yCoordinate = z;
            }
        }
    }


    IEnumerator BoxAnimationCoroutine(Action onComplete)
    {
        foreach (Vector2Int coordinate in testPath)
        {
            Highlight3x3Box(coordinate);
            testAudioSource.Play();
            yield return new WaitUntil(() => hasRecordedGaze);

            hasRecordedGaze = false;
            yield return new WaitForSeconds(1);
            Unhighlight3x3Box(coordinate);
        }
        onComplete?.Invoke();
    }

    //this is called by the planePrefab when selected. 
    public void RecordUserGaze(int xCoordinate, int yCoordinate)
    {
        hasRecordedGaze = true;
        userSelectionPath.Add(new Vector2Int(xCoordinate, yCoordinate));
        userSelectedAudio.Play();
        //change the gridObjects[xCoordinate, yCoordinate].planeprefabController.changeColor
    }

    private void Highlight3x3Box(Vector2Int center)
    {
        for (int i = -1; i <= 1; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                Vector2Int coord = new Vector2Int(center.x + i, center.y + j);

                // Ensure the coordinates are within the grid boundaries
                if (coord.x >= 0 && coord.x < gridWidth && coord.y >= 0 && coord.y < gridHeight)
                {
                    if (i == 0 && j == 0)
                    {
                        gridObjects[coord.x, coord.y].GetComponent<Renderer>().material.color = centerHighlightColor;
                    }
                    else
                    {
                        gridObjects[coord.x, coord.y].GetComponent<Renderer>().material.color = testHighlightColor;
                    }
                }
            }
        }
    }


    private void Unhighlight3x3Box(Vector2Int center)
    {
        for (int i = -1; i <= 1; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                Vector2Int coord = new Vector2Int(center.x + i, center.y + j);
                if (coord.x >= 0 && coord.x < gridWidth && coord.y >= 0 && coord.y < gridHeight)
                {
                    gridObjects[coord.x, coord.y].GetComponent<Renderer>().material.color = gridObjects[coord.x, coord.y].GetComponent<planePrefabController>().startingColor;
                }
            }
        }
    }

    //this will intialize where the boxese should appear //fill in the testPath
    private void TestLocationInitialize()
    {
        int[] xCoordinates = { 6, 4, 1 };
        int[] yCoordinates = { 1, 4, 8, 12, 16};

        foreach (int x in xCoordinates)
        {
            foreach (int y in yCoordinates)
            {
                testPath.Add(new Vector2Int(x, y));
            }
        }
    }


    //this will display all the test location one by one with black material filled as the test path and also show where the user selected,
    // if the testPath and user selection is the same then change the material to green.
    private void DisplayTestResult()
    {
        for (int i = 0; i < testPath.Count; i++)
        {
            if (i < userSelectionPath.Count && testPath[i] == userSelectionPath[i])
            {
                gridObjects[testPath[i].x, testPath[i].y].GetComponent<Renderer>().material.color = correctHighlightColor;
                TMP_Text numberLabel = CreateTMPObject(gridObjects[testPath[i].x, testPath[i].y].transform);
                numberLabel.text = (i + 1).ToString();
            }
            else
            {
                gridObjects[testPath[i].x, testPath[i].y].GetComponent<Renderer>().material.color = centerHighlightColor;
                TMP_Text numberLabel = CreateTMPObject(gridObjects[testPath[i].x, testPath[i].y].transform);
                numberLabel.text = (i + 1).ToString();

                // Display user's selection if it exists
                if (i < userSelectionPath.Count)
                {
                    gridObjects[userSelectionPath[i].x, userSelectionPath[i].y].GetComponent<Renderer>().material.color = userHighlightColor;

                    // Create TMP label and set its text for the user's selection
                    TMP_Text userLabel = CreateTMPObject(gridObjects[userSelectionPath[i].x, userSelectionPath[i].y].transform);
                    userLabel.text = (i + 1).ToString();
                }
            }

        }
    }

    //this function will compare each indices of the testPath and user selection Path and check if the coordinates align, if the coordinates align, increment the hit variable, and then at the end calculate the percentage of hit rate 
    private void AnalyseTracking()
    {
        int hits = 0;

        foreach (var userCoord in userSelectionPath)
        {
            if (testPath.Contains(userCoord))
            {
                hits++;
            }
        }
        float hitRate = ((float)hits / testPath.Count) * 100;
        hitRate = (float)Math.Round(hitRate, 1);
        Debug.Log($"Hit Rate: {hitRate}%");

        string resultMessage;
        if (hitRate >= 80)
        {
            resultMessage = "You are good to go.";
        }
        else
        {
            resultMessage = "Please recalibrate headset before continuing.";
        }

        resultText.GetComponent<TextMeshProUGUI>().text = $"Hit Rate: {hitRate}%\n{resultMessage}";
    }


    private TMP_Text CreateTMPObject(Transform parentTransform)
    {
        // Create new game object for TMP text
        GameObject textObject = new GameObject("TMP_NumberLabel");

        // Set the parent (so it will be placed relative to its parent)
        textObject.transform.SetParent(parentTransform);

        // Set position (in this case, just above its parent, adjust if necessary)
        textObject.transform.localPosition = new Vector3(0, 0.5f, 0);
        textObject.transform.localRotation = Quaternion.Euler(90, 180, 0);

        // Add the TMP text component and set defaults
        TMP_Text textComponent = textObject.AddComponent<TextMeshPro>();
        textComponent.alignment = TextAlignmentOptions.Center;
        textComponent.fontSize = 1; // Adjust as needed
        textComponent.color = Color.white;  // Set color to white
        textComponent.fontStyle = FontStyles.Bold;
        textComponent.enableWordWrapping = false;

        return textComponent;
    }


}
