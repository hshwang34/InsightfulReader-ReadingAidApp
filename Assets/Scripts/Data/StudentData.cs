using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AssessmentData
{
    // Assessment Details
    public int assessmentID;
    public string assessmentName;
    public string assessmentDate;  // Storing the date of the assessment

    public List<int> gazeIndexHistory; // Storing the gaze history
    public List<int> timeIndexHistory; // Storing the time history

    public string analysisReport;  // Report for this specific assessment

    public bool isCompleted;

    public AssessmentData(int id, string name)
    {
        assessmentID = id;
        assessmentName = name;
        assessmentDate = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"); // Get current date and time

        gazeIndexHistory = new List<int>();
        timeIndexHistory = new List<int>();
        analysisReport = "";
        isCompleted = false;
    }

    public void AddGazeIndexList(List<int> gazeIndexList)
    {
        gazeIndexHistory = gazeIndexList;
    }

    public void AddTimeIndexList(List<int> timeIndexList)
    {
        timeIndexHistory = timeIndexList;
    }
}

[System.Serializable]
public class StudentData
{
    // Basic Student Details
    public string studentName;
    public int studentID; // Unique identifier

    public List<AssessmentData> assessments;

    public StudentData(string name, int id)
    {
        studentName = name;
        studentID = id;
        assessments = new List<AssessmentData>();
    }

    public AssessmentData CreateNewAssessment(int assessmentID, string name)
    {
        AssessmentData newAssessment = new AssessmentData(assessmentID, name);
        assessments.Add(newAssessment);
        return newAssessment;
    }

    //returns the last assessment recorded
    public AssessmentData GetLastAssessment()
    {
        if (assessments.Count > 0)
            return assessments[assessments.Count - 1]; // Return the last assessment
        else
            return null;
    }

    //may need to implement returning all the assessments to display data
}