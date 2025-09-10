using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class DictionaryController : MonoBehaviour
{
    public static DictionaryController Instance { get; private set; }

    [Header("UI References")]
    [SerializeField] private TMP_Text wordText;
    [SerializeField] private TMP_Text definitionText;
    [SerializeField] private TMP_Text synonymsText;
    [SerializeField] private TMP_Text phoneticText;
    [SerializeField] private TMP_Text exampleText;
    [SerializeField] private GameObject imageGameObject;

    [Header("Control Buttons")]
    [SerializeField] private GameObject audioButton;
    [SerializeField] private GameObject bookmarkButton;

    [Header("Audio")]
    [SerializeField] private AudioSource dictionaryUpdateAudioSource;

    private DictionaryLoader dictionaryLoader;

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

        InitializeDictionaryLoader();
    }

    private void InitializeDictionaryLoader()
    {
        dictionaryLoader = FindObjectOfType<DictionaryLoader>();
        if (dictionaryLoader == null)
        {
            Debug.LogError($"{nameof(DictionaryLoader)} not found in the scene!", this);
            enabled = false;
        }
    }

    public void ShowWordDefinition(string word)
    {
        if (string.IsNullOrEmpty(word))
        {
            Debug.LogWarning("Word parameter is null or empty", this);
            return;
        }

        DictionaryEntry entry = dictionaryLoader.GetDictionaryEntry(word);
        if (entry != null)
        {
            DisplayWordEntry(entry);
        }
        else
        {
            DisplayWordNotFound(word);
        }

        PlayUpdateSound();
    }

    private void DisplayWordEntry(DictionaryEntry entry)
    {
        SetButtonStates(true);
        UpdateTextFields(entry);
        LoadAndDisplayImage(entry.imageURL);
    }

    private void DisplayWordNotFound(string word)
    {
        SetButtonStates(false);
        wordText.text = $"Selected word: {word}";
        definitionText.text = "No Definitions Found";
        synonymsText.text = "";
        phoneticText.text = "";
        exampleText.text = "";
        imageGameObject.SetActive(false);

        Debug.Log($"No definition found for word: {word}", this);
    }

    private void SetButtonStates(bool isActive)
    {
        if (audioButton != null)
            audioButton.SetActive(isActive);
        if (bookmarkButton != null)
            bookmarkButton.SetActive(isActive);
    }

    private void UpdateTextFields(DictionaryEntry entry)
    {
        wordText.text = entry.word;
        definitionText.text = $"Definition: {entry.definition}";
        synonymsText.text = string.Join(", ", entry.synonyms);
        phoneticText.text = entry.phonetic;
        exampleText.text = $"Example: {entry.example}";
    }

    private void LoadAndDisplayImage(string imageURL)
    {
        if (string.IsNullOrEmpty(imageURL) || imageGameObject == null)
        {
            imageGameObject.SetActive(false);
            return;
        }

        var imageComponent = imageGameObject.GetComponent<Image>();
        if (imageComponent == null)
        {
            Debug.LogWarning($"{nameof(Image)} component not found on {nameof(imageGameObject)}", this);
            imageGameObject.SetActive(false);
            return;
        }

        var sprite = Resources.Load<Sprite>(imageURL);
        if (sprite != null)
        {
            imageComponent.sprite = sprite;
            imageGameObject.SetActive(true);
        }
        else
        {
            imageGameObject.SetActive(false);
        }
    }

    private void PlayUpdateSound()
    {
        if (dictionaryUpdateAudioSource != null)
        {
            dictionaryUpdateAudioSource.Play();
        }
    }
}