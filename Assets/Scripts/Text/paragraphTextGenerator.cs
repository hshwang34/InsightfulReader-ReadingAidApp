using TMPro;
using UnityEngine;
using System.Collections.Generic;
using System;

public class paragraphTextGenerator : MonoBehaviour
{
    private List<GameObject> instantiatedTextObjects = new List<GameObject>();

    public GameObject textPrefab;
  

    private float boxColliderWidthPadding = 0.1f; // Additional width padding for the box collider
    private float boxColliderHeightPadding = 0.04f;
    //making this value positive pushes up the collider in respect to the word.
    //finding: we look at words at the bottom
    private float boxColliderVerticalOffset = -0.015f;

    private Vector3 currentPoint;

   
    private void Start()
    {
      
    }

  

    //this function generates all the text prefabs with the right rigid body formation
    public void GenerateTextObjects(string paragraph)
    {
        string[] words = paragraph.Split(' ');
        Vector3 defaultPosition = new Vector3(1f, 2f, 3f);
        Quaternion rotation = Quaternion.Euler(0, -90, 0);

        foreach (string word in words)
        {
            GameObject wordObject = Instantiate(textPrefab, defaultPosition, rotation);
            instantiatedTextObjects.Add(wordObject);
            //this sets the index value for the prefab.
            SetIndexForTextPrefab(wordObject, instantiatedTextObjects.Count - 1);
            
            TextMeshPro tmp = wordObject.GetComponent<TextMeshPro>();

            if (tmp == null)
            {
                Debug.LogError("The textPrefab does not have a TextMeshPro component.");
                return;
            }

            tmp.text = word;

            // Wait for TMP to finish computing metrics.
            tmp.ForceMeshUpdate();

            // Adjust the BoxCollider to fit the word.
            BoxCollider box = wordObject.GetComponent<BoxCollider>();
            if (box == null)
            {
                box = wordObject.AddComponent<BoxCollider>();
            }

            Bounds b = tmp.bounds;
            float depth = Mathf.Max(b.size.z + 0.05f, 0.05f);
            box.size = new Vector3(b.size.x + boxColliderWidthPadding, b.size.y + boxColliderHeightPadding, depth);
            box.center = new Vector3(0, (-b.extents.y + boxColliderVerticalOffset), -b.extents.z);

            // Adjust the size of the MeshRenderer to match the text width.
            MeshRenderer renderer = wordObject.GetComponent<MeshRenderer>();
            if (renderer != null)
            {
                renderer.bounds.SetMinMax(b.min, new Vector3(b.min.x + tmp.preferredWidth, b.max.y, b.max.z));
            }
        }
    }

    public List<GameObject> GetInstantiatedTextObjects()
    {
        return instantiatedTextObjects;
    }

    public void SetIndexForTextPrefab(GameObject textPrefab, int index)
    {
        textPrefabController controller = textPrefab.GetComponent<textPrefabController>();
        ReadingTextController readingController;
        if (controller != null)
        {
            controller.SetIndex(index);
        }else
        {
            readingController = textPrefab.GetComponent<ReadingTextController>();
            readingController.SetIndex(index);
        }
    }

    public void Reset()
    {
        foreach (var obj in GetInstantiatedTextObjects())
        {
            obj.GetComponent<textPrefabController>().Reset();
        }
        // Reset other states if necessary.
    }

    public void DestroyAllTextPrefabs()
    {
        foreach (GameObject obj in instantiatedTextObjects)
        {
            Destroy(obj);
        }

        instantiatedTextObjects.Clear();
    }


    //called when starting a reading session
    public void ResetState()
    {
        instantiatedTextObjects.Clear();
    }
}
