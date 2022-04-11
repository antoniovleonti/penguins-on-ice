using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;


namespace Scoreboards
{
    public class Scoreboard : MonoBehaviour
    {
        [SerializeField] private int maxEntries = 8;
        [SerializeField] Transform scoreHolderTransform = null;
        [SerializeField] GameObject entryObject = null;
        
        [SerializeField] ScoreboardEntry test = new ScoreboardEntry();

        private string SavePath => $"{Application.persistentDataPath}/scores.json";

        private void Start()
        {
            //grab SO
            ScoreBoardSaveData savedScores = GetSavedScores();

            UpdateUI(savedScores);

            SaveScores(savedScores);
        }

        [ContextMenu("Add Test Entry")]
        public void addTestEntry()
        {
            AddEntry(test);
        }

        public void AddEntry(ScoreboardEntry scoreboardEntryData)
        {
            ScoreBoardSaveData savedScores = GetSavedScores();

            bool scoreAdded = false;

            for (int i = 0; i < savedScores.entryList.Count; i++)
            {
                if(scoreboardEntryData.entryScore > savedScores.entryList[i].entryScore)
                {
                    savedScores.entryList.Insert(i, scoreboardEntryData);
                    scoreAdded = true;
                    break;
                }
            }

            if (!scoreAdded && savedScores.entryList.Count < maxEntries)
            {
                savedScores.entryList.Add(scoreboardEntryData);
            }

            if (savedScores.entryList.Count > maxEntries)
            {
                savedScores.entryList.RemoveRange(maxEntries, savedScores.entryList.Count - maxEntries);
            }

            UpdateUI(savedScores);

            SaveScores(savedScores);
        }

        private ScoreBoardSaveData GetSavedScores()
        {
            if(!File.Exists(SavePath))
            {
                File.Create(SavePath);
                return new ScoreBoardSaveData();
            }

            using(StreamReader stream = new StreamReader(SavePath))
            {
                string json = stream.ReadToEnd();

                return JsonUtility.FromJson<ScoreBoardSaveData>(json);
            }
        }

        private void SaveScores(ScoreBoardSaveData scoreboardSaveData)
        {
            using(StreamWriter stream = new StreamWriter(SavePath))
            {
                string json = JsonUtility.ToJson(scoreboardSaveData, true);
                stream.Write(json);
            }
        }

        void UpdateUI(ScoreBoardSaveData savedScores)
        {
            foreach(Transform child in scoreHolderTransform)
            {
                UnityEngine.Object.Destroy(child.gameObject);
            }

            foreach(ScoreboardEntry score in savedScores.entryList)
            {
                UnityEngine.Object.Instantiate(entryObject, scoreHolderTransform).
                    GetComponent<ScoreboardEntryUI>().initScoreboard(score);
            }
        }
    }
}