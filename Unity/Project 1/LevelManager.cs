using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Events;

public class LevelManager : MonoBehaviour
{
    GameObject pauseMenu;
    GameObject victoryMenu;

    Button nextLevelButton;

    bool hasWon = false;

    bool pauseStatus = false;

    void Start()
    {
        GameObject[] backToMenuButtons = GameObject.FindGameObjectsWithTag( "BackToMenuButton" );

        foreach( GameObject gObj in backToMenuButtons )
            gObj.GetComponentInChildren<Button>().onClick.AddListener( BackToMenu );

        nextLevelButton = GameObject.FindGameObjectWithTag( "NextLevelButton" ).GetComponent<Button>();

        nextLevelButton.onClick.AddListener( NextLevel );

        EventManager.AddFreePrisonerListener( PlayerWon );

        pauseMenu = GameObject.FindGameObjectWithTag( "PauseMenu" );
        victoryMenu = GameObject.FindGameObjectWithTag( "VictoryMenu" );

        pauseMenu.SetActive( false );
        victoryMenu.SetActive( false );
    }

    void Update()
    {
        if( Input.GetKeyDown( KeyCode.P ) && !hasWon )
        {
            pauseStatus = !pauseStatus;
            pauseMenu.SetActive( pauseStatus );
            Time.timeScale = pauseStatus ? 0 : 1;
        }
    }

    void PlayerWon()
    {
        hasWon = true;
        victoryMenu.SetActive( true );
        string curLevel = SceneManager.GetActiveScene().name;
        string levelNum = curLevel.Substring( curLevel.Length - 1 );
        PlayerPrefs.SetInt( "Level " + levelNum, 2 );

        int index = Int32.Parse( levelNum ) + 1;
        PlayerPrefs.SetInt( "Level " + index, 1 );
    }

    public void BackToMenu()
    {
        SceneManager.LoadScene( "MainMenu" );
    }

    public void NextLevel()
    {
        string curLevel = SceneManager.GetActiveScene().name;
        string levelNum = curLevel.Substring( curLevel.Length - 1 );
        int index = Int32.Parse( levelNum ) + 1;
        string nextLevelName = "Level" + index;

        if( index > 10 )
            nextLevelName = "MainMenu";
        
        //Debug.Log( nextLevelName );
        
        SceneManager.LoadScene( nextLevelName );
    }
}
