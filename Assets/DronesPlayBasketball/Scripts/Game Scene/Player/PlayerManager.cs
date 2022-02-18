using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager playerManagerInstance;
    public PlayerProperty myPlayerProperty;

    public GameObject[] Drones;

    bool IsSceneLoaded = false;

    private void Awake()
    {
        //if (!playerManagerInstance)
        //    playerManagerInstance = this;
        //else
        //    Destroy(this);
        DontDestroyOnLoad(gameObject);
    }

    void OnEnable()
    {
        //Tell our 'OnLevelFinishedLoading' function to start listening for a scene change as soon as this script is enabled.
        SceneManager.sceneLoaded += OnSceneFinishedLoading;
    }

    void OnDisable()
    {
        //Tell our 'OnLevelFinishedLoading' function to stop listening for a scene change as soon as this script is disabled. Remember to always have an unsubscription for every delegate you subscribe to!
        SceneManager.sceneLoaded -= OnSceneFinishedLoading;
    }

    void OnSceneFinishedLoading(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Game" && !IsSceneLoaded)
        {
            IsSceneLoaded = true;

            myPlayerProperty.CancelInvoke();

            for (int i = 0; i < Drones.Length; i++)
            {
                if (Drones[i])
                {
                    if (Drones[i].name == myPlayerProperty.SelectedDrone)
                    {
                        Drones[i].gameObject.SetActive(true);
                        break;
                    }
                }
            }

            for (int i = 0; i < Drones.Length; i++)
            {
                if (Drones[i])
                {
                    if (!Drones[i].activeSelf)
                    {
                        Destroy(Drones[i]);
                        Drones[i] = null;
                    }
                }
            }

            GetComponent<PlayerController>().enabled = true;


            //if (myPlayerProperty.isLocalPlayer)
            //{
            //    GetComponent<PlayerController>().enabled = true;
            //    //FindObjectOfType<GameUIController>().GetComponent<GameUIController>().ArrangePlayersPositions();
            //}

            //myPlayerProperty.enabled = false;
        }
    }
}