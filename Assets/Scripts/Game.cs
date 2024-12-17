using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering.Universal.Internal;
using UnityEngine.SceneManagement;

namespace Assets.Scripts
{
    public class Game : MonoBehaviour, IBallObserver, IPlayerObserver
    {
        [SerializeField] private Ball ball;
        private Rigidbody ballRb;

        [SerializeField] private TextMeshProUGUI playerOneScoreText;
        [SerializeField] private TextMeshProUGUI playerTwoScoreText;
        [SerializeField] private TextMeshProUGUI setsText;

        [SerializeField] private Camera firstCamera;
        [SerializeField] private Camera secondCamera;

        [SerializeField] private Player player1;
        [SerializeField] private Player player2;

        private Vector3 initialPlayer1Pos;
        private Vector3 initialPlayer2Pos;

        private int playerOneScore = 0;
        private int playerTwoScore = 0;

        private bool isFirstPlayerTurn = true;

        private int playerOneSets = 0;
        private int playerTwoSets = 0;

        private int setsCounter = 1;

        private const int MAX_POINTS = 10;
        private const int POINTS_DIFF = 2;
        private const int MAX_SETS_WON = 1;

        private void Start()
        {
            ballRb = ball.GetComponent<Rigidbody>();

            if (ball != null)
            {
                ball.AddObserver(this);
            }

            if (player1 != null)
            {
                player1.AddObserver(this);
            }

            if (player2 != null)
            {
                player2.AddObserver(this);
            }

            UpdateScoreUI();

            SetPlayerPos();

            SetCameras(firstCamera, secondCamera);
        }

        private void OnDestroy()
        {
            if (ball != null)
            {
                ball.RemoveObserver(this);
            }

            if (player1 != null)
            {
                player1.RemoveObserver(this);
            }

            if (player2 != null)
            {
                player2.RemoveObserver(this);
            }
        }

        public void OnSideTouched(string side)
        {
            if (side == "SideA")
            {
                playerTwoScore++;
            }
            else if (side == "SideB")
            {
                playerOneScore++;
            }

            UpdateScoreUI();

            CheckAndUpdateScore(ref playerTwoScore, ref playerOneScore, ref playerTwoSets, ref playerOneSets, "SecondPlayer");

            CheckAndUpdateScore(ref playerOneScore, ref playerTwoScore, ref playerOneSets, ref playerTwoSets, "Player");

            UpdateScoreUI();
        }

        private void CheckAndUpdateScore(ref int winnerScore, ref int loserScore, ref int winnerSets, ref int loserSets, string winnerName)
        {
            if (winnerScore == MAX_POINTS && winnerScore - loserScore >= POINTS_DIFF)
            {
                string newSet = $"Set {setsCounter}: {winnerScore} - {loserScore}, Winner : {winnerName} \n";
                setsText.text += newSet;

                winnerSets++;
                winnerScore = 0;
                loserScore = 0;

                if (winnerSets == MAX_SETS_WON)
                {
                    PlayerPrefs.SetString("WinnerName", winnerName);

                    SceneManager.LoadScene("WinnerScene");
                }

                setsCounter++;
            }
        }

        private void UpdateScoreUI()
        {
            playerOneScoreText.text = $"First Player Score: {playerOneScore}";
            playerTwoScoreText.text = $"Second Player Score: {playerTwoScore}";
        }

        public void OnSwitchPlayer()
        {
            isFirstPlayerTurn = !isFirstPlayerTurn;

            if (isFirstPlayerTurn)
            {
                ball.transform.position = ball.FirstPlayerPos;

                SetCameras(firstCamera, secondCamera);
            }
            else
            {
                ball.transform.position = ball.SecondPlayerPos;

                SetCameras(secondCamera, firstCamera);
            }

            SetPlayerPos();
        }

        private void SetCameras(Camera show, Camera hide)
        {
            show.gameObject.SetActive(true);
            hide.gameObject.SetActive(false);
        }

        private void SetPlayerPos()
        {
            initialPlayer1Pos = player1.transform.position;
            initialPlayer2Pos = player2.transform.position;
        }

        public void OnServe(Vector3 direction, ForceMode mode)
        {
            ballRb.AddForce(direction, mode);
        }

        public void OnBallPosition(Player player)
        {
            player.BallPos = ball.transform.position;
        }

        public void OnBallShooter(string player)
        {
            ball.ShootingPlayer = player;
        }

        public void OnBallCollision()
        {
            ballRb.useGravity = true;
        }
    }
}
