using System.Collections;
using System.Collections.Generic;
using Camera;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Managers
{
    public class GameManager : MonoBehaviour
    {
        public Color[] playerColors = {Color.blue, Color.red};

        public int numRoundsToWin = 5; // The number of rounds a single player has to win to win the game.
        public float startDelay = 3f; // The delay between the start of RoundStarting and RoundPlaying phases.
        public float endDelay = 3f; // The delay between the end of RoundPlaying and RoundEnding phases.

        public CameraControl cameraControl; // Reference to the CameraControl script for control during different phases.

        public Text messageText; // Reference to the overlay Text to display winning text, etc.
        public GameObject tankPrefab; // Reference to the prefab the players will control.

        public TankManager[] tankManagers; // A collection of managers for enabling and disabling different aspects of the tanks.
        public GameObject[] spawnPoints; // A collection of managers for enabling and disabling different aspects of the tanks.

        public RoundStatus RoundStatus { get; private set; }
        private int roundNumber; // Which round the game is currently on.

        private TankManager roundWinner; // Reference to the winner of the current round.  Used to make an announcement of who won.
        private TankManager gameWinner; // Reference to the winner of the game.  Used to make an announcement of who won.

        private void Awake()
        {
            RoundStatus = RoundStatus.Starting;
        }

        private void Start()
        {
            // Create the delays so they only have to be made once.
            SpawnAllTanks();
            SetCameraTargets();

            // Once the tanks have been created and the camera is using them as targets, start the game.
            StartCoroutine(GameLoop());
        }

        private void SpawnAllTanks()
        {
            // For all the tanks...
            for (var i = 0; i < spawnPoints.Length; i++)
            {
                // ... create them, set their player number and references needed for control.
                var spawnPoint = spawnPoints[i].transform;
                var tankGameObject = Instantiate(tankPrefab, spawnPoint.position, spawnPoint.rotation);

                var tankManager = tankGameObject.GetComponent<TankManager>();
                tankManager.playerColor = playerColors[i];
                tankManager.playerNumber = i + 1;
                tankManager.spawnPoint = spawnPoint;
                tankManager.Setup();

                tankManagers[i] = tankManager;
            }

            foreach (var tankManager in tankManagers)
            {
                var opponents = new List<TankManager>(tankManagers.Length - 1);

                foreach (var tankManagerOpponent in tankManagers)
                    if (tankManagerOpponent != tankManager)
                        opponents.Add(tankManagerOpponent);

                tankManager.opponents = opponents;
            }
        }

        private void SetCameraTargets()
        {
            // Create a collection of transforms the same size as the number of tanks.
            var targets = new Transform[tankManagers.Length];

            // For each of these transforms...
            for (var i = 0; i < targets.Length; i++)
            {
                // ... set it to the appropriate tank transform.
                targets[i] = tankManagers[i].transform;
            }

            // These are the targets the camera should follow.
            cameraControl.targets = targets;
        }

        // This is called from start and will run each phase of the game one after another.
        private IEnumerator GameLoop()
        {
            // Start off by running the 'RoundStarting' coroutine but don't return until it's finished.
            yield return StartCoroutine(RoundStarting());

            // Once the 'RoundStarting' coroutine is finished, run the 'RoundPlaying' coroutine but don't return until it's finished.
            yield return StartCoroutine(RoundPlaying());

            // Once execution has returned here, run the 'RoundEnding' coroutine, again don't return until it's finished.
            yield return StartCoroutine(RoundEnding());

            // This code is not run until 'RoundEnding' has finished.  At which point, check if a game winner has been found.
            if (gameWinner != null)
            {
                // If there is a game winner, restart the level.
                SceneManager.LoadScene(0);
            }
            else
            {
                // If there isn't a winner yet, restart this coroutine so the loop continues.
                // Note that this coroutine doesn't yield.  This means that the current version of the GameLoop will end.
                StartCoroutine(GameLoop());
            }
        }

        private IEnumerator RoundStarting()
        {
            RoundStatus = RoundStatus.Starting;
            // As soon as the round starts reset the tanks and make sure they can't move.
            ResetAllTanks();
            DisableTankControl();

            // Snap the camera's zoom and position to something appropriate for the reset tanks.
            cameraControl.SetStartPositionAndSize();

            // Increment the round number and display text showing the players what round it is.
            roundNumber++;
            messageText.text = "ROUND " + roundNumber;

            // Wait for the specified length of time until yielding control back to the game loop.
            yield return new WaitForSeconds(startDelay);
        }

        private IEnumerator RoundPlaying()
        {
            RoundStatus = RoundStatus.Playing;
            // As soon as the round begins playing let the players control the tanks.
            EnableTankControl();

            // Clear the text from the screen.
            messageText.text = string.Empty;

            // While there is not one tank left...
            while (!OneTankLeft())
            {
                // ... return on the next frame.
                yield return null;
            }
        }

        private IEnumerator RoundEnding()
        {
            RoundStatus = RoundStatus.Ending;

            // Stop tanks from moving.
            DisableTankControl();

            // Clear the winner from the previous round.
            roundWinner = null;

            // See if there is a winner now the round is over.
            roundWinner = GetRoundWinner();

            // If there is a winner, increment their score.
            if (roundWinner != null)
                roundWinner.wins++;

            // Now the winner's score has been incremented, see if someone has one the game.
            gameWinner = GetGameWinner();

            // Get a message based on the scores and whether or not there is a game winner and display it.
            var message = EndMessage();
            messageText.text = message;

            // Wait for the specified length of time until yielding control back to the game loop.
            yield return new WaitForSeconds(endDelay);
        }

        // This is used to check if there is one or fewer tanks remaining and thus the round should end.
        private bool OneTankLeft()
        {
            // Start the count of tanks left at zero.
            var numTanksLeft = 0;

            // Go through all the tanks...
            foreach (var tankManager in tankManagers)
            {
                // ... and if they are active, increment the counter.
                if (tankManager.gameObject.activeSelf)
                    numTanksLeft++;
            }

            // If there are one or fewer tanks remaining return true, otherwise return false.
            return numTanksLeft <= 1;
        }

        // This function is to find out if there is a winner of the round.
        // This function is called with the assumption that 1 or fewer tanks are currently active.
        private TankManager GetRoundWinner()
        {
            // Go through all the tanks...
            foreach (var tankManager in tankManagers)
            {
                // ... and if one of them is active, it is the winner so return it.
                if (tankManager.gameObject.activeSelf)
                    return tankManager;
            }

            // If none of the tanks are active it is a draw so return null.
            return null;
        }

        // This function is to find out if there is a winner of the game.
        private TankManager GetGameWinner()
        {
            // Go through all the tanks...
            foreach (var tankManager in tankManagers)
            {
                // ... and if one of them has enough rounds to win the game, return it.
                if (tankManager.wins == numRoundsToWin)
                    return tankManager;
            }

            // If no tanks have enough rounds to win, return null.
            return null;
        }

        // Returns a string message to display at the end of each round.
        private string EndMessage()
        {
            // By default when a round ends there are no winners so the default end message is a draw.
            var message = "DRAW!";

            // If there is a winner then change the message to reflect that.
            if (roundWinner != null)
                message = roundWinner.coloredPlayerText + " WINS THE ROUND!";

            // Add some line breaks after the initial message.
            message += "\n\n\n\n";

            // Go through all the tanks and add each of their scores to the message.
            foreach (var tankManager in tankManagers)
                message += string.Format("{0}: {1} WINS\n", tankManager.coloredPlayerText, tankManager.wins);

            // If there is a game winner, change the entire message to reflect that.
            if (gameWinner != null)
                message = string.Format("{0} WINS THE GAME!", gameWinner.coloredPlayerText);

            return message;
        }

        // This function is used to turn all the tanks back on and reset their positions and properties.
        private void ResetAllTanks()
        {
            foreach (var tankManager in tankManagers)
                tankManager.Reset();
        }

        private void EnableTankControl()
        {
            foreach (var tankManager in tankManagers)
                tankManager.EnableControl();
        }

        private void DisableTankControl()
        {
            foreach (var tankManager in tankManagers)
                tankManager.DisableControl();
        }
    }

    public enum RoundStatus
    {
        Starting,
        Playing,
        Ending
    }
}