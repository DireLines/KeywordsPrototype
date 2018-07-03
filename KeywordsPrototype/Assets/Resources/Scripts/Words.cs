using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class Words : MonoBehaviour {
	private const int numLettersInSource = 6;
	string[] words;//all words in the dictionary file
	string[] numletterwords;//all words of exactly numLettersInSource letters in length
	string[] currentSourceWords;//a selection of words which each floor in the dungeon will be based on
	public int numLevels;//how many levels in the dungeon
	List<string> currentLevelWords;//all words it's possible to make with the letters of the current source word
	List<string> unmadeLevelWords;//all words from currentLevelWords not yet made (by somebody). 
	List<char> currentSourceChars;//all chars in the current source word.

	void Awake(){
		words = File.ReadAllLines("Assets/Words.txt");
		numletterwords = GetNumLetterWords ();
		currentSourceWords = GetSomeSourceWords (numLevels, 30, 250);
		currentSourceChars = new List<char> ();
		UpdateLevelWords (0);
	}

	public void UpdateLevelWords(int level){
		currentLevelWords = GetWords (currentSourceWords[level]);
		char[] sourceChars = currentSourceWords [level].ToCharArray ();
		currentSourceChars.Clear ();
		foreach (char c in sourceChars) {
			if (!currentSourceChars.Contains (c)) {
				currentSourceChars.Add (c);
			}
		}
		unmadeLevelWords = currentLevelWords;
	}

	string[] GetNumLetterWords(){
		List<string> result = new List<string> ();
		foreach (string word in words) {
			if (word.Length == numLettersInSource) {
				result.Add (word);
			}
		}
		return result.ToArray();
	}

	List<string> GetWords(string letters){
		List<string> result = new List<string> ();
		foreach (string word in words) {
			int c = 0;
			bool done = false;
			while (c < word.Length && !done) {
				if (!letters.Contains (word.Substring(c,1))) {
					done = true;
				}
				c++;
			}
			if (!done) {
				result.Add (word);
			}
		}
		return result;
	}

	int GetScore(string letters){
		return GetWords (letters).Count;
	}


	public char GetRandomSourceChar(){
		return currentSourceChars [Random.Range (0, currentSourceChars.Count)];
	}

	public bool ValidateWord(string word){
		if (unmadeLevelWords.Contains (word)) {
			unmadeLevelWords.Remove (word);
			return true;
		}
		return false;
	}

	string[] GetSomeSourceWords(int howMany,int lowerThreshold,int upperThreshold){
		List<string> result = new List<string> ();
		for (int i = 0; i < howMany; i++) {
			string randomword = numletterwords[Random.Range (0, numletterwords.Length)];
			int score = GetScore (randomword);
			while (score < lowerThreshold || score > upperThreshold) {
				randomword = numletterwords[Random.Range (0, numletterwords.Length)];
				score = GetScore (randomword);
			}
			print (randomword + score);
			result.Add (randomword);
		}
		return result.ToArray ();
	}
}
