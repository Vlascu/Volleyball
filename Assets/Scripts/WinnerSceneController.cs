using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace Assets.Scripts
{
    public class WinnerSceneController : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI winner;

        void Start()
        {
            string winnerName = PlayerPrefs.GetString("WinnerName");

            winner.text = "Winner: " + winnerName;
        }
    }
}
