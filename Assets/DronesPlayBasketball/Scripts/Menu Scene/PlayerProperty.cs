using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;

public class PlayerProperty : MonoBehaviour
{
    public PhotonView PhotonViewInstance;
    public SelectionManager_Multiplayer SelectionManager_MultiplayerInstance;
    public string PlayerName;
    public string SelectedCountry;
    public string SelectedDrone;
    public bool isLocalPlayer;
    public string Team1 = "";
    public string Team2 = "";

    void Start()
    {
        SelectionManager_MultiplayerInstance = GameObject.Find("SelectionManager").GetComponent<SelectionManager_Multiplayer>();
        if (PhotonViewInstance.IsMine)
        {
            SelectionManager_MultiplayerInstance.LocalPlayer = gameObject;

            SelectionManager_MultiplayerInstance.HideAllScreens();
            if (PhotonNetwork.IsMasterClient)
            {
                for (int i = 0; i < SelectionManager_MultiplayerInstance.TeamButtons.Length; i++)
                {
                    SelectionManager_MultiplayerInstance.TeamButtons[i].gameObject.SetActive(true);
                }
                SelectionManager_MultiplayerInstance.CountrySelectionPanel.SetActive(true);
            }
            else
            {
                SelectionManager_MultiplayerInstance.WaitingForMasterPanel.SetActive(true);
            }
        }
    }

    /***********************************/
    /***** PLAYER NAME SYNC STARTS *****/

    public void SetPlayerName()
    {

        PlayerName = "Player_" + PhotonViewInstance.Owner.UserId;
        gameObject.name = PlayerName;
        PhotonViewInstance.Owner.NickName = PlayerName;
        //PlayerName = LocalPlayerName;
        if (PhotonViewInstance.IsMine)
        {
            isLocalPlayer = true;
            SetPlayerNameOnNetwork();
        }
    }

    void SetPlayerNameOnNetwork()
    {
        PhotonViewInstance.RPC("SetPlayerNameOnNetwork_RPC", RpcTarget.Others, PlayerName);
        Invoke("SetPlayerNameOnNetwork", 0.5f);
    }

    [PunRPC]
    public void SetPlayerNameOnNetwork_RPC(string PName)
    {
        gameObject.name = PName;
        PlayerName = PName;
        PhotonViewInstance.Owner.NickName = PName;
    }

    /***** PLAYER NAME SYNC ENDS *****/
    /*********************************/

    /*******************************/
    /***** COUNTRY SYNC STARTS *****/

    public void OnTeamSelected(string Team_1, string Team_2)
    {
        Team1 = Team_1;
        Team2 = Team_2;
        OnTeamSelectedOnNetwork();
    }

    public void OnTeamSelectedOnNetwork()
    {
        PhotonViewInstance.RPC("OnTeamSelected_RPC", RpcTarget.All, Team1, Team2);
        Invoke("OnTeamSelectedOnNetwork", 0.5f);
    }

    [PunRPC]
    public void OnTeamSelected_RPC(string Team1_RPC, string Team2_RPC)
    {
        if (SelectionManager_MultiplayerInstance)
        {
            SelectionManager_MultiplayerInstance.IsTeamSelected = true;
            SelectionManager_MultiplayerInstance.ConnectionPanel.SetActive(false);
            SelectionManager_MultiplayerInstance.WaitingForMasterPanel.SetActive(false);
            SelectionManager_MultiplayerInstance.CountrySelectionPanel.SetActive(true);
            SelectionManager_MultiplayerInstance.Team1 = Team1_RPC;
            SelectionManager_MultiplayerInstance.Team2 = Team2_RPC;
            Team1 = Team1_RPC;
            Team2 = Team2_RPC;
            for (int i = 0; i < SelectionManager_MultiplayerInstance.TeamButtons.Length; i++)
            {
                if (SelectionManager_MultiplayerInstance.TeamButtons[i].name == Team1_RPC || SelectionManager_MultiplayerInstance.TeamButtons[i].name == Team2_RPC)
                {
                    SelectionManager_MultiplayerInstance.TeamButtons[i].gameObject.SetActive(true);
                }
                else
                {
                    SelectionManager_MultiplayerInstance.TeamButtons[i].gameObject.SetActive(false);
                }
            }
        }
    }

    public void SetPlayerCountry(string LocalPlayerCountry)
    {
        SelectedCountry = LocalPlayerCountry;
        if (!SelectionManager_MultiplayerInstance)
        {
            SelectionManager_MultiplayerInstance = GameObject.Find("SelectionManager").GetComponent<SelectionManager_Multiplayer>();
        }

        Team1 = SelectionManager_MultiplayerInstance.Team1;
        Team2 = SelectionManager_MultiplayerInstance.Team2;

        if (PhotonViewInstance.IsMine)
        {
            SetPlayerCountryOnNetwork();
        }
    }

    void SetPlayerCountryOnNetwork()
    {
        PhotonViewInstance.RPC("SetPlayerCountryOnNetwork_RPC", RpcTarget.All, SelectedCountry, Team1, Team2);
        Invoke("SetPlayerCountryOnNetwork", 0.5f);
    }

    [PunRPC]
    public void SetPlayerCountryOnNetwork_RPC(string CountryName, string Team1_tmp, string Team2_tmp)
    {
        SelectedCountry = CountryName;
        Team1 = Team1_tmp;
        Team2 = Team2_tmp;

    }

    /***** COUNTRY SYNC ENDS *****/
    /*****************************/

    /*****************************/
    /***** DRONE SYNC STARTS *****/

    public void SetPlayerDrone(string LocalPlayerDrone)
    {
        SelectedDrone = LocalPlayerDrone;
        if (PhotonViewInstance.IsMine)
        {
            SetPlayerDroneOnNetwork();
        }
    }

    void SetPlayerDroneOnNetwork()
    {
        PhotonViewInstance.RPC("SetPlayerDroneOnNetwork_RPC", RpcTarget.All, SelectedDrone);
        Invoke("SetPlayerDroneOnNetwork", 0.5f);
    }

    [PunRPC]
    public void SetPlayerDroneOnNetwork_RPC(string DroneName)
    {
        if (SelectionManager_MultiplayerInstance)
        {
            Player[] ConnectedPlayer = PhotonNetwork.PlayerList;

            for (int i = 0; i < SelectionManager_MultiplayerInstance.DronesButtons.Length; i++)
            {
                bool IsSelectedDrone = false;
                for (int j = 0; j < ConnectedPlayer.Length; j++)
                {
                    if (ConnectedPlayer[j].NickName != "" || ConnectedPlayer[j].NickName != null)
                    {
                        GameObject PlayerTemp = GameObject.Find(ConnectedPlayer[j].NickName);
                        if (PlayerTemp)
                        {
                            string DroneBtnInd = PlayerTemp.GetComponent<PlayerProperty>().SelectedDrone;
                            if (SelectionManager_MultiplayerInstance.DronesButtons[i].name == DroneBtnInd)
                            {
                                GameObject DroneBtnTemp = GameObject.Find(DroneBtnInd);
                                if (DroneBtnTemp)
                                {
                                    DroneBtnTemp.GetComponent<Button>().interactable = false;
                                    IsSelectedDrone = true;
                                }
                            }
                        }
                    }
                }

                if (!IsSelectedDrone)
                {
                    SelectionManager_MultiplayerInstance.DronesButtons[i].interactable = true;
                }
            }
            SelectedDrone = DroneName;
        }
    }

    /***** DRONE SYNC ENDS *****/
    /***************************/
}