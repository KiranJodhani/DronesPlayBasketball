using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class GameSceneLoader : MonoBehaviour
{
    bool isClicked = false;

    void FixedUpdate()
    {
        CheckForAllPlayer();
    }

    void CheckForAllPlayer()
    {
        bool allCountrySelected = true;
        bool allDroneSelected = true;

        Player[] ConnectedPlayer = PhotonNetwork.PlayerList;

        for (int i = 0; i < ConnectedPlayer.Length; i++)
        {
            GameObject PlayerGO = GameObject.Find(ConnectedPlayer[i].NickName);
            if (PlayerGO)
            {
                if (string.IsNullOrEmpty(PlayerGO.GetComponent<PlayerProperty>().SelectedCountry))
                {
                    allCountrySelected = false;
                    break;
                }

                if (string.IsNullOrEmpty(PlayerGO.GetComponent<PlayerProperty>().SelectedDrone))
                {
                    allDroneSelected = false;
                    break;
                }
            }
        }

        if (!isClicked && allCountrySelected && allDroneSelected && PhotonNetwork.CurrentRoom.PlayerCount == PhotonNetwork.CurrentRoom.MaxPlayers)
            GetComponent<Button>().interactable = true;
        else
            GetComponent<Button>().interactable = false;
    }

    public void LoadGameScene()
    {
        isClicked = true;
        GetComponent<Button>().interactable = false;
        PhotonNetwork.CurrentRoom.IsVisible = false;
        PhotonNetwork.CurrentRoom.IsOpen = false;
        PhotonNetwork.LoadLevel("Game");
    }
}
