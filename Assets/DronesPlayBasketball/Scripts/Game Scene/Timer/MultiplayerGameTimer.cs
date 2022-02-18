using UnityEngine;
using Photon.Pun;
using System.Collections;

public class MultiplayerGameTimer : MonoBehaviourPun
{
    public float targetTime = 900f;
    public float timerSpeed = 5f;
    public bool pauseTimer;
    bool timerEnded, isHalfTime, isMatchEnded;
    float timeRemained;

    private void Start()
    {
        timeRemained = targetTime;
        PauseTimer();
    }

    public void StartTimer()
    {
        pauseTimer = false;
    }

    public void PauseTimer()
    {
        pauseTimer = true;
    }

    void Update()
    {
        if (!timerEnded && !pauseTimer)
        {
            timeRemained -= Time.deltaTime * timerSpeed;
            Debug.Log("timeRemained - " + timeRemained);
            if (timeRemained <= 0.0f)
            {
                timerEnded = true;
                TimerEnded();
            }
        }
        else if (Input.GetKeyDown(KeyCode.Space))
        {
            ResetTimer();
        }
    }

    void TimerEnded()
    {
        if (isHalfTime)
        {
            Debug.Log("@@@ MatchEnded!!!");
            isMatchEnded = true;
        }
        else
        {
            Debug.Log("### Half Time!!!");
            isHalfTime = true;
            // Open popup
        }
    }

    public void ResetTimer()
    {
        StartCoroutine(ResetTimerCoroutine());
    }

    IEnumerator ResetTimerCoroutine()
    {
        if (!isMatchEnded)
        {
            GetComponent<GameUIController>().ReArrangePlayersPositions();
            // Close popup
            // Open loading panel
            yield return new WaitForSeconds(1f);
            timeRemained = targetTime;
            timerEnded = false;
        }
    }
}