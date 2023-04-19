using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

public static class NetworkMessageHandler
{
    public static List<float> GetDigits(FixedString128Bytes input)
    {
        string phrase = input.ToString();
        string[] words = phrase.Split(' ');
        List<float> parsedNumbers = new List<float>();
        float parsedWord = 0;
        foreach (var word in words)
        {
            bool getDigit = float.TryParse(word, out parsedWord);
            if (getDigit)
            {
                parsedNumbers.Add(parsedWord);
            }
        }
        return parsedNumbers;
    }

    public static List<string> GetOnlyCharacters(FixedString128Bytes input)
    {
        string phrase = input.ToString();
        string[] words = phrase.Split(' ');
        List<string> allValues = new List<string>();
        foreach (var word in words)
        {
            allValues.Add(word);
        }
        List<string> onlyCharacterWords = new List<string>();
        int temp = 0;
        foreach (var word in allValues)
        {
            bool isChar = int.TryParse(word, out temp);
            if (isChar == false)
            {
                onlyCharacterWords.Add(word);
            }
        }
        return onlyCharacterWords;
    }
}
