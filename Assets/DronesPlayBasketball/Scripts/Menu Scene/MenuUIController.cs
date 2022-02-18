using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using Photon.Realtime;

public class MenuUIController : MonoBehaviourPunCallbacks
{
    [Header("Menu Screens")]
    public GameObject MainMenuPanel;
    public GameObject SettingsPanel;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }

    public void OnClickSettings()
    {
        SettingsPanel.SetActive(true);
        MainMenuPanel.SetActive(false);
    }

    public void OnClickBack()
    {
        MainMenuPanel.SetActive(true);
        SettingsPanel.SetActive(false);
    }

    public void OnClickMoreGames()
    {

    }

    public void OnClickRateUs()
    {

    }
}