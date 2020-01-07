﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameUI : MonoBehaviour {

    public GameObject gameLoseUI;
    public GameObject gameWinUI;
    bool gameIsOver;

    // Start is called before the first frame update
    void Start () {
        Guard.OnGuardHasSpotedPlayer += ShowGameLoseUI;
        FindObjectOfType<Player> ().OnReachEndOfLevel += ShowGameWinUI;
    }

    // Update is called once per frame
    void Update () {
        if (gameIsOver) {
            if (Input.GetKeyDown (KeyCode.Space)) {
                SceneManager.LoadScene (0);
            }
        }
    }

    void ShowGameWinUI () {
        OnGameOver (gameWinUI);
    }

    void ShowGameLoseUI () {
        OnGameOver (gameLoseUI);
    }

    void OnGameOver (GameObject gameOverUI) {
        gameOverUI.SetActive (true);
        gameIsOver = true;
        Guard.OnGuardHasSpotedPlayer -= ShowGameLoseUI;
        FindObjectOfType<Player> ().OnReachEndOfLevel -= ShowGameWinUI;
    }
}