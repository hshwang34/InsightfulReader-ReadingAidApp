using UnityEngine;

public class BookAnimation : MonoBehaviour
{
    [Header("Animation Settings")]
    [SerializeField] private GameObject button;
    [SerializeField] private float offsetX = 0.5f;

    private Vector3 originalPosition;

    private void Start()
    {
        CacheOriginalPosition();
    }

    private void CacheOriginalPosition()
    {
        if (button != null)
        {
            originalPosition = button.transform.position;
        }
        else
        {
            Debug.LogWarning($"{nameof(button)} reference is missing!", this);
        }
    }

    public void HoverEntered()
    {
        if (button != null)
        {
            button.transform.position = originalPosition + new Vector3(offsetX, 0, 0);
        }
    }

    public void HoverExited()
    {
        if (button != null)
        {
            button.transform.position = originalPosition;
        }
    }
}
