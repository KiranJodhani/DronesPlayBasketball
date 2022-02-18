using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using DG.Tweening;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections.Generic;



public class GameUIController : MonoBehaviourPunCallbacks
{
    [Header("Panel")]
    public GameObject gameOverPanel;
    public TMP_Text playerLeftText;

    [Header("Buttons")]
    public Button fireButton;
    public Button goalButton;

    [Header("Joysticks")]
    public SimpleTouchController leftJoystick;
    public SimpleTouchController rightJoystick;

    [Header("PLAYER POSITIONS")]
    public List<PlayerInstance> PlayerInstanceList = new List<PlayerInstance>();
    public Team1_Positions[] Team1_Positions_Instance;
    public Team2_Positions[] Team2_Positions_Instance;

    [Header("Team Scores")]
    public TMP_Text team1ScoreText;
    public TMP_Text team2ScoreText;
    public TMP_Text team1_3D_ScoreText;
    public TMP_Text team2_3D_ScoreText;
    int team1Score, team2Score;
    public ConstantData.BasketSide team1BasketSide;
    public ConstantData.BasketSide team2BasketSide;

    [Header("Team Info")]
    public string Team1_Name;
    public string Team2_Name;
    public TMP_Text team1NameText;
    public TMP_Text team2NameText;
    public TMP_Text team1_3D_NameText;
    public TMP_Text team2_3D_NameText;
    public Image team1Image;
    public Image team2Image;
    public Sprite[] flagImages;

    [Space(10)]
    public Transform BallSpawnPoint;
    public static bool CanFireBullet = false;


    void Start()
    {
        Screen.sleepTimeout = SleepTimeout.NeverSleep;

        if (!PhotonNetwork.IsConnected)
        {
            SceneManager.LoadScene("Menu");
            return;
        }

        ArrangePlayersPositions();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        if (newMasterClient.IsMasterClient)
        {
            foreach (var goalPost in FindObjectsOfType<GoalpostAnimation>())
                goalPost.enabled = true;
        }
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.LogWarningFormat("OnDisconnected() was called by PUN with reason {0}", cause);
        if (cause == DisconnectCause.ExceptionOnConnect || cause == DisconnectCause.ServerTimeout || cause == DisconnectCause.DisconnectByServerReasonUnknown)
        {
            SceneManager.LoadScene("Menu");
        }
    }

    public override void OnPlayerLeftRoom(Player other)
    {
        Debug.Log("OnPlayerLeftRoom() called!");
        playerLeftText.DOFade(1f, 0.5f);
        playerLeftText.DOFade(0f, 0.5f).SetDelay(1.5f);

        //Check whether player has ball or not
        Invoke("CheckBallExistanceDelay", 1f);
        CheckPlayersInTeam();
    }

    void CheckBallExistanceDelay()
    {
        GameObject Game_Ball = GameObject.FindWithTag("Ball");
        if (!Game_Ball && PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.Instantiate("Ball", BallSpawnPoint.position, Quaternion.identity);
        }
    }

    public void MatchOverGoToMainMenu()
    {
        PhotonNetwork.LeaveRoom();
        SceneManager.LoadScene("Menu");
    }

    void ArrangePlayersPositions()
    {
        PlayerProperty[] playerPropertiesArray = (PlayerProperty[])FindObjectsOfType(typeof(PlayerProperty));

        for (int i = 0; i < playerPropertiesArray.Length; i++)
        {
            PlayerInstance playerInstanceTemp = new PlayerInstance();
            playerInstanceTemp.Player = playerPropertiesArray[i].gameObject;
            playerInstanceTemp.PlayerName = playerPropertiesArray[i].gameObject.name;
            playerInstanceTemp.SelectedCountry = playerPropertiesArray[i].SelectedCountry;
            playerInstanceTemp.SelectedDrone = playerPropertiesArray[i].SelectedDrone;
            PlayerInstanceList.Add(playerInstanceTemp);
        }

        PlayerInstanceList.Sort((p1, p2) => p1.PlayerName.CompareTo(p2.PlayerName));

        if (playerPropertiesArray.Length > 0)
        {
            Team1_Name = playerPropertiesArray[0].Team1;
            Team2_Name = playerPropertiesArray[0].Team2;
            team1NameText.text = Team1_Name;
            team2NameText.text = Team2_Name;
            team1_3D_NameText.text = Team1_Name;
            team2_3D_NameText.text = Team2_Name;
            team1Image.sprite = flagImages[(int)Enum.Parse(typeof(ConstantData.Country), Team1_Name)];
            team2Image.sprite = flagImages[(int)Enum.Parse(typeof(ConstantData.Country), Team2_Name)];
        }

        for (int i = 0; i < PlayerInstanceList.Count; i++)
        {
            if (PlayerInstanceList[i].SelectedCountry == Team1_Name)
            {
                for (int j = 0; j < Team1_Positions_Instance.Length; j++)
                {
                    if (!Team1_Positions_Instance[j].IsFilled)
                    {
                        PlayerInstanceList[i].Player.transform.localPosition = Team1_Positions_Instance[j].PlayerPostion.localPosition;
                        Team1_Positions_Instance[j].IsFilled = true;
                        break;
                    }
                }
            }

            if (PlayerInstanceList[i].SelectedCountry == Team2_Name)
            {
                for (int j = 0; j < Team2_Positions_Instance.Length; j++)
                {
                    if (!Team2_Positions_Instance[j].IsFilled)
                    {
                        PlayerInstanceList[i].Player.transform.localPosition = Team2_Positions_Instance[j].PlayerPostion.localPosition;
                        Team2_Positions_Instance[j].IsFilled = true;
                        break;
                    }
                }
            }
        }

        for (int i = 0; i < playerPropertiesArray.Length; i++)
        {
            playerPropertiesArray[i].enabled = false;
        }
    }

    public void ReArrangePlayersPositions()
    {
        foreach (var pos in Team1_Positions_Instance)
            pos.IsFilled = false;
        foreach (var pos in Team2_Positions_Instance)
            pos.IsFilled = false;

        for (int i = 0; i < PlayerInstanceList.Count; i++)
        {
            if (PlayerInstanceList[i].Player)
            {
                if (PlayerInstanceList[i].SelectedCountry == Team1_Name)
                {
                    for (int j = 0; j < Team2_Positions_Instance.Length; j++)
                    {
                        if (!Team2_Positions_Instance[j].IsFilled)
                        {
                            PlayerInstanceList[i].Player.transform.localPosition = Team2_Positions_Instance[j].PlayerPostion.localPosition;
                            Team2_Positions_Instance[j].IsFilled = true;
                            break;
                        }
                    }
                }

                if (PlayerInstanceList[i].SelectedCountry == Team2_Name)
                {
                    for (int j = 0; j < Team1_Positions_Instance.Length; j++)
                    {
                        if (!Team1_Positions_Instance[j].IsFilled)
                        {
                            PlayerInstanceList[i].Player.transform.localPosition = Team1_Positions_Instance[j].PlayerPostion.localPosition;
                            Team1_Positions_Instance[j].IsFilled = true;
                            break;
                        }
                    }
                }
            }
        }
    }

    public void HandlePlayerGoal(ConstantData.BasketSide basketSide)
    {
        if (basketSide == team1BasketSide)
        {
            team1Score += 1;
            team1ScoreText.text = team1Score.ToString();
            team1_3D_ScoreText.text = team1Score.ToString();
        }
        else if (basketSide == team2BasketSide)
        {
            team2Score += 1;
            team2ScoreText.text = team2Score.ToString();
            team2_3D_ScoreText.text = team2Score.ToString();
        }
    }

    void CheckPlayersInTeam()
    {
        int team1Count = 0;
        int team2Count = 0;

        for (int i = 0; i < PlayerInstanceList.Count; i++)
        {
            if (PlayerInstanceList[i].Player)
            {
                if (PlayerInstanceList[i].SelectedCountry == Team1_Name)
                {
                    team1Count++;
                }

                if (PlayerInstanceList[i].SelectedCountry == Team2_Name)
                {
                    team2Count++;
                }
            }
        }

        if (team1Count == 0 || team2Count == 0)
        {
            CancelInvoke("CheckBallExistanceDelay");
            gameOverPanel.SetActive(true);
        }
    }
}



[Serializable]
public class PlayerInstance
{
    public GameObject Player;
    public string PlayerName;
    public string SelectedCountry;
    public string SelectedDrone;
}

[Serializable]
public class Team1_Positions
{
    public Transform PlayerPostion;
    public bool IsFilled;
}

[Serializable]
public class Team2_Positions
{
    public Transform PlayerPostion;
    public bool IsFilled;
}