using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace Scoreboards
{
    public class ScoreboardEntryUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI entryNameText = null;
        [SerializeField] private TextMeshProUGUI entryScoreText = null;
        
        public void initScoreboard(ScoreboardEntry sb)
        {
            entryNameText.text = sb.entryName;
            entryScoreText.text = sb.entryScore.ToString();
        }
    }
}
