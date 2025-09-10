using UnityEngine;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class planePrefabController : MonoBehaviour
{
    public Color targetColor = Color.red; // The target color you want to reach
    private Renderer planeRenderer;
    public Color startingColor;
    private int calls = 0;
    private const int maxCalls = 20;

    public int xCoordinate = 0;
    public int yCoordinate = 0;

    public bool isSelected = false;

    private void Awake()
    {
        planeRenderer = GetComponent<Renderer>();
        if (planeRenderer)
        {
            startingColor = planeRenderer.material.color;
        }
    }



    public void ChangeColorTowardTarget()
    {
        if (calls < maxCalls)
        {
            calls++;
            if (planeRenderer)
            {
                float t = (float)calls / maxCalls;
                planeRenderer.material.color = Color.Lerp(startingColor, targetColor, t);
            }
        }
    }

    public void ChangeColorToBlue()
    {
        if (planeRenderer)
        {
            planeRenderer.material.color = Color.blue;
        }
    }


    public void OnSelect()
    {
        GridGenerator.Instance.RecordUserGaze(xCoordinate, yCoordinate);
        isSelected = true;
        StartCoroutine(ColorChangeCoroutine());
    }

    private IEnumerator ColorChangeCoroutine()
    {
        if (planeRenderer)
        {
            planeRenderer.material.color = Color.red;  // Change color to red.
            yield return new WaitForSeconds(1);       // Wait for 2 seconds.
            planeRenderer.material.color = startingColor; // Revert to original color.
        }
    }
}