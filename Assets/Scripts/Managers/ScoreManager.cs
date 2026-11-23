using UnityEngine;
using System.Collections.Generic;
using System;

[Serializable]//This class will just keep track of what player has what score. This will be updated constantly
public class PlayerScore {
    public int playerNum = 0;
    public int currentScore = 0;
    public bool holdingRelic = false;

    public PlayerScore(int playerNum) {
        this.playerNum = playerNum;
    }
    public int AddScore(int amount) { //More stuff can be added here
        currentScore += amount;
        return currentScore;
    }
}
public class ScoreManager {
    List<PlayerScore> playerScores;
    [SerializeField] float crownHoldingMult = 2f;
    [SerializeField] float pointsPerSec = 1f;

    public ScoreManager(int playerAmount) {
        playerScores = new List<PlayerScore>();
        for (int i = 1; i <= playerAmount; i++) {
            PlayerScore tempScore = new(i);
            playerScores.Add(tempScore);
        }
    }
}

