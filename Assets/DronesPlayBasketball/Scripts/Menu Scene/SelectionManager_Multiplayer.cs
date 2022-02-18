using System;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using DG.Tweening;

public class SelectionManager_Multiplayer : MonoBehaviourPunCallbacks
{
    [Header("Photon References")]
    [Tooltip("The maximum number of players per room. When a room is full, it can't be joined by new players, and so new room will be created")]
    public byte maxPlayersPerRoom = 4;

    [Header("UI SCREENS")]
    public Canvas UICanvas;
    public GameObject NoInternetPanel;
    public GameObject LoadingPanel;
    public GameObject ConnectionPanel;
    public GameObject CountrySelectionPanel;
    public GameObject DroneSelectionPanel;
    public GameObject WaitingForMasterPanel;
    public GameObject ConfirmationPopup;
    public GameObject WaitingForOtherPlayerPanel;
    public HorizontalScrollSnap horizontalScrollSnap;

    [Header("PLAYER DATA")]
    private GameObject PlayerPrefab;
    [HideInInspector] public GameObject LocalPlayer;

    [Header("TEAMS")]
    private bool IsDroneSelected;
    [HideInInspector] public bool IsTeamSelected;
    [HideInInspector] public string Team1;
    [HideInInspector] public string Team2;
    private int Team1_PlayerCount;
    private int Team2_PlayerCount;
    private int Team1Index;
    private int Team2Index;

    [Header("BUTTONS")]
    public Button LoadGameButton;
    public Button[] TeamButtons;
    public Button[] DronesButtons;
    //private Button CountryBtn;
    private Button Team1_Button;
    private Button Team2_Button;

    [Header("Player Waiting Info Panel")]
    public GameObject waitingText;
    public Image[] playerTeam1Info;
    public Image[] playerTeam2Info;
    public Sprite[] droneImages;

    public TMP_Text validationText;

    void Start()
    {
        HideAllScreens();
        PhotonNetwork.AutomaticallySyncScene = true;
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        ConnectToPhoton();
        DontDestroyOnLoad(gameObject);
        ManagePlayersInTeam();
        ManagePlayersWaitingInTeam();
    }

    public void OnApplicationPause(bool paused)
    {
        if (paused)
        {
            if (PhotonNetwork.IsConnected && PhotonNetwork.InRoom)
                LeaveRoom();
        }
        else
        {
            if (!PhotonNetwork.IsConnected)
                ConnectToPhoton();
        }
    }

    void ConnectToPhoton()
    {
        ConnectionPanel.SetActive(true);
        PhotonNetwork.GameVersion = Application.version;
        PhotonNetwork.ConnectUsingSettings();
    }

    public void ReConnect()
    {
        ConnectToPhoton();
    }

    public void JoinRoom()
    {
        if (PhotonNetwork.IsConnected)
        {
            LoadingPanel.SetActive(true);
            PhotonNetwork.JoinRandomRoom();
        }
    }

    public void HideAllScreens()
    {
        NoInternetPanel.SetActive(false);
        ConnectionPanel.SetActive(false);
        CountrySelectionPanel.SetActive(false);
        WaitingForMasterPanel.SetActive(false);
    }

    public void CreateRoom()
    {
        PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = maxPlayersPerRoom });
    }

    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
    }

    private void OnLeaveRoomResetData()
    {
        CountrySelectionPanel.SetActive(false);
        DroneSelectionPanel.SetActive(false);
        WaitingForOtherPlayerPanel.SetActive(false);
        ConfirmationPopup.SetActive(false);

        UICanvas.renderMode = RenderMode.ScreenSpaceOverlay;

        foreach (var tbutton in TeamButtons)
        {
            tbutton.transform.GetComponentsInChildren<Image>()[0].enabled = false;
            tbutton.interactable = true;
        }
        foreach (var dbutton in DronesButtons)
            dbutton.interactable = true;

        LoadGameButton.gameObject.SetActive(false);

        IsTeamSelected = false;
        IsDroneSelected = false;
        //CountryBtn = null;
        Team1_Button = null;
        Team2_Button = null;
        Team1 = null;
        Team2 = null;
    }

    public void WaitForGameScene()
    {
        if (!IsDroneSelected)
        {
            validationText.rectTransform.anchorMin = new Vector2(0f, 1f);
            validationText.rectTransform.anchorMax = new Vector2(1f, 1f);
            validationText.rectTransform.pivot = new Vector2(0.5f, 1f);
            validationText.text = "Please select Drone!";
            validationText.DOFade(1f, 0.5f);
            validationText.DOFade(0f, 0.5f).SetDelay(1.5f);
            return;
        }

        if (PhotonNetwork.IsMasterClient)
            LoadGameButton.gameObject.SetActive(true);

        UICanvas.renderMode = RenderMode.ScreenSpaceOverlay;
        WaitingForOtherPlayerPanel.SetActive(true);
    }

    public void OnClickBack()
    {
        ConfirmationPopup.SetActive(true);
        if (DroneSelectionPanel.activeSelf)
            UICanvas.renderMode = RenderMode.ScreenSpaceOverlay;
    }

    public void OnClickBackNo()
    {
        ConfirmationPopup.SetActive(false);
        UICanvas.renderMode = RenderMode.ScreenSpaceCamera;
    }

    public void OnSelectTeam(int TeamIndex)
    {
        if (PhotonNetwork.IsMasterClient && LocalPlayer && !IsTeamSelected)
        {
            if (Team1 == "")
            {
                Team1Index = TeamIndex;
                Team1 = ((ConstantData.Country)TeamIndex).ToString();
                TeamButtons[TeamIndex].transform.GetComponentsInChildren<Image>()[0].enabled = true;
            }
            else if (Team2 == "")
            {
                Team2Index = TeamIndex;
                Team2 = ((ConstantData.Country)TeamIndex).ToString();
                TeamButtons[TeamIndex].transform.GetComponentsInChildren<Image>()[0].enabled = true;
            }
            else if (Team1 != "" && Team2 != "")
            {
                if (((ConstantData.Country)TeamIndex).ToString() == Team2)
                    return;

                TeamButtons[Team1Index].transform.GetComponentsInChildren<Image>()[0].enabled = false;
                Team1 = Team2;
                Team2 = ((ConstantData.Country)TeamIndex).ToString();
                Team1Index = Team2Index;
                Team2Index = TeamIndex;
                TeamButtons[TeamIndex].transform.GetComponentsInChildren<Image>()[0].enabled = true;
            }
        }
    }

    public void OnCountrySelected(string countryName)
    {
        if (LocalPlayer && IsTeamSelected)
        {
            //TeamButtons[Team1Index].transform.GetComponentsInChildren<Image>()[0].enabled = false;
            //TeamButtons[Team2Index].transform.GetComponentsInChildren<Image>()[0].enabled = false;
            foreach (var tbutton in TeamButtons)
                tbutton.transform.GetComponentsInChildren<Image>()[0].enabled = false;

            TeamButtons[(int)Enum.Parse(typeof(ConstantData.Country), countryName)].transform.GetComponentsInChildren<Image>()[0].enabled = true;
            LocalPlayer.GetComponent<PlayerProperty>().SetPlayerCountry(countryName);
        }
    }

    public void OnDroneSelected()
    {
        if (LocalPlayer)
        {
            LocalPlayer.GetComponent<PlayerProperty>().SetPlayerDrone("Drone" + horizontalScrollSnap.CurrentPage);
            IsDroneSelected = true;
        }
    }

    public void OnClickConfirmTeam()
    {
        if (string.IsNullOrEmpty(Team1))
        {
            validationText.rectTransform.anchorMin = new Vector2(0f, 0f);
            validationText.rectTransform.anchorMax = new Vector2(1f, 0f);
            validationText.rectTransform.pivot = new Vector2(0.5f, 0f);
            validationText.text = "Please select 2 countries!";
            validationText.DOFade(1f, 0.5f);
            validationText.DOFade(0f, 0.5f).SetDelay(1.5f);
            return;
        }
        if (string.IsNullOrEmpty(Team2))
        {
            validationText.rectTransform.anchorMin = new Vector2(0f, 0f);
            validationText.rectTransform.anchorMax = new Vector2(1f, 0f);
            validationText.rectTransform.pivot = new Vector2(0.5f, 0f);
            validationText.text = "Please select 2 countries!";
            validationText.DOFade(1f, 0.5f);
            validationText.DOFade(0f, 0.5f).SetDelay(1.5f);
            return;
        }

        if (string.IsNullOrEmpty(LocalPlayer.GetComponent<PlayerProperty>().SelectedCountry) && IsTeamSelected)
        {
            validationText.rectTransform.anchorMin = new Vector2(0f, 0f);
            validationText.rectTransform.anchorMax = new Vector2(1f, 0f);
            validationText.rectTransform.pivot = new Vector2(0.5f, 0f);
            validationText.text = "Please select your country!";
            validationText.DOFade(1f, 0.5f);
            validationText.DOFade(0f, 0.5f).SetDelay(1.5f);
            return;
        }

        if (LocalPlayer.GetComponent<PlayerProperty>().SelectedCountry != "" && IsTeamSelected)
        {
            CountrySelectionPanel.SetActive(false);
            UICanvas.renderMode = RenderMode.ScreenSpaceCamera;
            DroneSelectionPanel.SetActive(true);
        }

        if (LocalPlayer && !IsTeamSelected)
        {
            LocalPlayer.GetComponent<PlayerProperty>().OnTeamSelected(Team1, Team2);
            IsTeamSelected = true;
            //TeamButtons[Team1Index].transform.GetComponentsInChildren<Image>()[0].enabled = false;
            //TeamButtons[Team2Index].transform.GetComponentsInChildren<Image>()[0].enabled = false;
            foreach (var tbutton in TeamButtons)
                tbutton.transform.GetComponentsInChildren<Image>()[0].enabled = false;
        }
    }

    void ManagePlayersInTeam()
    {
        if (CountrySelectionPanel.activeSelf)
        {
            Team1_PlayerCount = 0;
            Team2_PlayerCount = 0;

            if (Team1 != "" && Team1 != null)
            {
                Team1_Button = GameObject.Find(Team1).GetComponent<Button>();
            }

            if (Team2 != "" && Team2 != null)
            {
                Team2_Button = GameObject.Find(Team2).GetComponent<Button>();
            }

            PlayerProperty[] playerPropertiesArray = (PlayerProperty[])FindObjectsOfType(typeof(PlayerProperty));

            for (int i = 0; i < playerPropertiesArray.Length; i++)
            {
                string CountryNameTemp = playerPropertiesArray[i].SelectedCountry;

                if (CountryNameTemp != "" && CountryNameTemp != null)
                {
                    //CountryBtn = GameObject.Find(CountryNameTemp).GetComponent<Button>();
                    if (CountryNameTemp == Team1)
                    {
                        Team1_PlayerCount++;
                    }
                    if (CountryNameTemp == Team2)
                    {
                        Team2_PlayerCount++;
                    }
                }
            }

            if (Team1_PlayerCount >= maxPlayersPerRoom / 2)
            {
                if (Team1_Button)
                {
                    Team1_Button.interactable = false;
                }
            }
            else
            {
                if (Team1_Button)
                {
                    Team1_Button.interactable = true;
                }
            }

            if (Team2_PlayerCount >= maxPlayersPerRoom / 2)
            {
                if (Team2_Button)
                {
                    Team2_Button.interactable = false;
                }
            }
            else
            {
                if (Team2_Button)
                {
                    Team2_Button.interactable = true;
                }
            }
        }
        Invoke("ManagePlayersInTeam", 0.5f);
    }

    void ManagePlayersWaitingInTeam()
    {
        if (WaitingForOtherPlayerPanel.activeSelf)
        {
            PlayerProperty[] playerPropertiesArray = (PlayerProperty[])FindObjectsOfType(typeof(PlayerProperty));

            foreach (var p in playerTeam1Info)
                p.gameObject.SetActive(false);

            foreach (var p in playerTeam2Info)
                p.gameObject.SetActive(false);


            for (int i = 0; i < playerPropertiesArray.Length; i++)
            {
                string DroneNameTemp = playerPropertiesArray[i].SelectedDrone;
                string TeamNameTemp = playerPropertiesArray[i].SelectedCountry;

                if (DroneNameTemp != "" && DroneNameTemp != null && TeamNameTemp != "" && TeamNameTemp != null)
                {
                    string[] tokens = DroneNameTemp.Split('e');
                    int numVal = int.Parse(tokens[1]);

                    if (TeamNameTemp == Team1)
                    {
                        playerTeam1Info[i].sprite = droneImages[numVal];
                        playerTeam1Info[i].gameObject.SetActive(true);
                    }

                    if (TeamNameTemp == Team2)
                    {
                        playerTeam2Info[i].sprite = droneImages[numVal];
                        playerTeam2Info[i].gameObject.SetActive(true);
                    }
                }
            }
        }

        Invoke("ManagePlayersWaitingInTeam", 0.5f);
    }

    /************************************ PHOTON STARTS ************************************/
    /***************************************************************************************/
    #region Photon Callbacks

    public override void OnConnectedToMaster()
    {
        Debug.Log("OnConnectedToMaster() was called by PUN");
        HideAllScreens();
        //Add player in shared data List
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.LogWarningFormat("OnDisconnected() was called by PUN with reason {0}", cause);
        if (cause == DisconnectCause.ExceptionOnConnect)
        {
            Debug.Log("Please check your internet connection!");
            ConnectionPanel.SetActive(false);
            NoInternetPanel.SetActive(true);
        }
        if (cause == DisconnectCause.ServerTimeout || cause == DisconnectCause.DisconnectByServerReasonUnknown)
        {
            ConnectToPhoton();
        }
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("OnJoinRandomFailed() was called by PUN. No random room available, so we create one.\nCalling: PhotonNetwork.CreateRoom");

        CreateRoom();
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.Log("OnCreateRoomFailed() was called by PUN. Failed to create room... trying again");

        CreateRoom();
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("OnJoinedRoom Player Counter : " + PhotonNetwork.CurrentRoom.PlayerCount);
        PlayerPrefab = PhotonNetwork.Instantiate("Drone", Vector3.zero, Quaternion.identity);
        //PlayerPrefab.name = "Player_" + PhotonNetwork.CurrentRoom.PlayerCount.ToString();
        //PhotonNetwork.NickName = PlayerPrefab.name;
        PlayerPrefab.GetComponent<PlayerProperty>().SetPlayerName();
        LoadingPanel.SetActive(false);
        if (PhotonNetwork.CurrentRoom.PlayerCount == PhotonNetwork.CurrentRoom.MaxPlayers)
            waitingText.SetActive(false);
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log("OnPlayerEnteredRoom() " + newPlayer.NickName);
        if (PhotonNetwork.CurrentRoom.PlayerCount == PhotonNetwork.CurrentRoom.MaxPlayers)
            waitingText.SetActive(false);
    }

    public override void OnLeftRoom()
    {
        //SceneManager.LoadScene(1);
        OnLeaveRoomResetData();
    }

    public override void OnPlayerLeftRoom(Player other)
    {
        Debug.Log("OnPlayerLeftRoom() " + other.NickName); // seen when other disconnects

        if (PhotonNetwork.IsMasterClient)
        {
            Debug.LogFormat("OnPlayerEnteredRoom IsMasterClient {0}", PhotonNetwork.IsMasterClient); // called before OnPlayerLeftRoom
        }
    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        GameObject NewMasterClient_Player = GameObject.Find("Player_" + newMasterClient.UserId);
        if (newMasterClient.IsMasterClient)
        {
            if (!IsTeamSelected)
            {
                HideAllScreens();
                for (int i = 0; i < TeamButtons.Length; i++)
                {
                    TeamButtons[i].gameObject.SetActive(true);
                }
                CountrySelectionPanel.SetActive(true);
            }
            else
            {
                if (NewMasterClient_Player)
                {
                    NewMasterClient_Player.GetComponent<PlayerProperty>().OnTeamSelected(Team1, Team2);
                }
            }

            LoadGameButton.gameObject.SetActive(true);
        }
    }

    #endregion

    /************************************* PHOTON ENDS *************************************/
    /***************************************************************************************/
}