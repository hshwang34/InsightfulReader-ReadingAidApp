using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class StudentSignInManager : MonoBehaviour
{
    [Header("Input Fields")]
    [SerializeField] private TMP_InputField studentNameInput;
    [SerializeField] private TMP_InputField studentIDInput;

    [Header("Default Values")]
    [SerializeField] private string defaultStudentName = "Jacob";
    [SerializeField] private int defaultStudentID = 1;

    private List<StudentData> allStudents;

    private void Start()
    {
        allStudents = new List<StudentData>();
    }

    public void OnSignInButtonClicked()
    {
        // For now using hardcoded values, but structure is ready for dynamic input
        var studentName = GetStudentName();
        var studentID = GetStudentID();

        if (string.IsNullOrEmpty(studentName))
        {
            Debug.LogWarning("Student name is empty! Please enter a name.", this);
            return;
        }

        CreateAndRegisterStudent(studentName, studentID);
    }

    private string GetStudentName()
    {
        // Currently using default, but can be switched to: studentNameInput?.text ?? defaultStudentName
        return defaultStudentName;
    }

    private int GetStudentID()
    {
        // Currently using default, but can be switched to parse from studentIDInput
        return defaultStudentID;
    }

    private void CreateAndRegisterStudent(string name, int id)
    {
        var newStudent = new StudentData(name, id);
        
        if (StudentDataManager.Instance != null)
        {
            StudentDataManager.Instance.allStudentsData.Add(newStudent);
            StudentDataManager.Instance.SetActiveStudent(newStudent);
        }
        else
        {
            Debug.LogError($"{nameof(StudentDataManager)} instance not found!", this);
        }
    }
}
