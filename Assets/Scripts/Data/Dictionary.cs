using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DictionaryEntry
{
    public string word;
    public string definition;
    public string[] synonyms;
    public string phonetic;
    public string imageURL;
    public string example;
}

[System.Serializable]
public class DictionaryData
{
    public List<DictionaryEntry> entries;
}