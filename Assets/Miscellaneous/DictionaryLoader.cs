using UnityEngine;
using System.Collections.Generic;

public class DictionaryLoader : MonoBehaviour
{
    private const string DictionaryResourcePath = "dictionary";
    
    private DictionaryData dictionaryData;

    private void Start()
    {
        LoadDictionaryFromJSON();
    }

    private void LoadDictionaryFromJSON()
    {
        var jsonText = Resources.Load<TextAsset>(DictionaryResourcePath);
        if (jsonText == null)
        {
            Debug.LogError($"Dictionary file not found at path: {DictionaryResourcePath}", this);
            enabled = false;
            return;
        }

        try
        {
            dictionaryData = JsonUtility.FromJson<DictionaryData>("{\"entries\":" + jsonText.text + "}");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Failed to parse dictionary JSON: {e.Message}", this);
            enabled = false;
        }
    }

    public DictionaryEntry GetDictionaryEntry(string word)
    {
        if (string.IsNullOrEmpty(word) || dictionaryData?.entries == null)
        {
            return null;
        }

        foreach (var entry in dictionaryData.entries)
        {
            if (entry.word.Equals(word, System.StringComparison.OrdinalIgnoreCase))
            {
                return entry;
            }
        }
        
        return null;
    }
}