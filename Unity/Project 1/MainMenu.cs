using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    int CurrentLevelNumber = 1;
    Text levelNo;
    SpriteRenderer levelIcon;
    [SerializeField] Sprite lockedLevel;
    [SerializeField] Sprite notCompleted;
    [SerializeField] Sprite completed;
    [SerializeField] Sprite oneShot;

    AudioSource musicSource;
    AudioSource sfxSource;

    ButtonScript musicSwitch;
    ButtonScript sfxSwitch;

    void Start()
    {
        Time.timeScale = 1;
        EventManager.AddButtonPressListener(ButtonPressed);
        levelNo = GameObject.Find("LevelText").GetComponent<Text>();
        levelIcon = GameObject.FindGameObjectWithTag( "LevelIcon" ).GetComponent<SpriteRenderer>();

        /*
        PlayerPrefs.SetInt( "Level 1", 0 );
        PlayerPrefs.SetInt( "Level 2", 0 );
        PlayerPrefs.SetInt( "Level 3", 0 );
        PlayerPrefs.SetInt( "Level 4", 0 );
        PlayerPrefs.SetInt( "Level 5", 0 );
        PlayerPrefs.SetInt( "Level 6", 0 );
        PlayerPrefs.SetInt( "Level 7", 0 );
        PlayerPrefs.SetInt( "Level 8", 0 );
        PlayerPrefs.SetInt( "Level 9", 0 );
        PlayerPrefs.SetInt( "Level 10", 0 );
        */

        if( PlayerPrefs.GetInt( "Level 1" ) < 1 )
            PlayerPrefs.SetInt( "Level 1", 1 );

        UpdateLevelNo();
        UpdateLevelIcon();
        
        musicSource = GameObject.FindGameObjectWithTag( "GameMusicSource" ).GetComponent<AudioSource>();
        sfxSource = GameObject.FindGameObjectWithTag( "GameSFXSource" ).GetComponent<AudioSource>();

        musicSwitch = GameObject.FindGameObjectWithTag( "MusicButtonSwitch" ).GetComponent<ButtonScript>();
        sfxSwitch = GameObject.FindGameObjectWithTag( "SFXButtonSwitch" ).GetComponent<ButtonScript>();

        musicSwitch.UpdatePic(!musicSource.mute);
        sfxSwitch.UpdatePic(!sfxSource.mute);
    }

    void ButtonPressed(int value)
    {
        switch( value )
        {
            case 1:
                AddLvlNo();
                break;
            case 2:
                DetractLvlNo();
                break;
            case 3:
                SwitchMusic();
                break;
            case 4:
                SwitchSFX();
                break;
            case 5:
                LaunchLevel();
                break;
            default:
                break;
        }
    }

    void UpdateLevelNo()
    {
        levelNo.text = "Level " + CurrentLevelNumber;
    }

    void UpdateLevelIcon()
    {
        int icon = PlayerPrefs.GetInt( levelNo.text );
        
        if( icon == 0 )
        {
            levelIcon.sprite = lockedLevel;
            levelIcon.color = Color.red;
        }
        else if( icon == 1 )
        {
            levelIcon.sprite = notCompleted;
            levelIcon.color = Color.white;
        }
        else if( icon == 2 )
        {
            levelIcon.sprite = completed;
            levelIcon.color = Color.white;
        }
        else if( icon == 3 )
        {
            levelIcon.sprite = oneShot;
            levelIcon.color = Color.green;
        }
    }

    void AddLvlNo()
    {
        CurrentLevelNumber += 1;
        if( CurrentLevelNumber > 10 )
            CurrentLevelNumber = 10;
        UpdateLevelNo();
        UpdateLevelIcon();
    }

    void DetractLvlNo()
    {
        CurrentLevelNumber -= 1;
        if( CurrentLevelNumber < 1 )
            CurrentLevelNumber = 1;
        UpdateLevelNo();
        UpdateLevelIcon();
    }

    void SwitchMusic()
    {
        musicSource.mute = !musicSource.mute;
        musicSwitch.UpdatePic(!musicSource.mute);
    }

    void SwitchSFX()
    {
        sfxSource.mute = !sfxSource.mute;
        sfxSwitch.UpdatePic(!sfxSource.mute);
    }

    void LaunchLevel()
    {
        if( PlayerPrefs.GetInt( "Level " + CurrentLevelNumber ) > 0 )
            SceneManager.LoadScene( "Level" + CurrentLevelNumber );
    }
}
