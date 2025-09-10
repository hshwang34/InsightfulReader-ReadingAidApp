using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StudentDataManager : MonoBehaviour
{
    // Singleton pattern
    public static StudentDataManager Instance { get; private set; }

    private StudentData activeStudent;

    public List<StudentData> allStudentsData = new List<StudentData>();

    private void Awake()
    {
        // Singleton setup
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);  // Optional: Makes sure the manager persists between scenes.
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Method to set the active student
    public void SetActiveStudent(StudentData student)
    {
        activeStudent = student;
    }

    // Method to get the active student
    public StudentData GetActiveStudent()
    {
        return activeStudent;
    }


    // Method to add a student's data.
    public void AddStudentData(StudentData data)
    {
        allStudentsData.Add(data);
    }

    // Method to get a student's data based on student ID. Returns null if not found.
    public StudentData GetStudentData(int studentID)
    {
        return allStudentsData.Find(student => student.studentID == studentID);
    }

    // Method to delete a student's data based on student ID. 
    public void DeleteStudentData(int studentID)
    {
        StudentData studentToDelete = GetStudentData(studentID);
        if (studentToDelete != null)
        {
            allStudentsData.Remove(studentToDelete);
        }
    }

    // Method to get all student data.
    public List<StudentData> GetAllStudentData()
    {
        return allStudentsData;
    }

    public void UpdateStudentData(int studentID, StudentData updatedData)
    {
        StudentData studentToUpdate = GetStudentData(studentID);
        if (studentToUpdate != null)
        {
            int index = allStudentsData.IndexOf(studentToUpdate);
            allStudentsData[index] = updatedData;
        }
    }

}
