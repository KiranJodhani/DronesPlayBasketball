using UnityEngine;

public class Timer : MonoBehaviour
{
    public float targetTime = 900f;
    public float timerSpeed = 5f;
    bool timerEnded, isHalfTime, isMatchEnded;
    float timeRemained;

    private void Start()
    {
        timeRemained = targetTime;
    }

    void Update()
    {
        if (!timerEnded)
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
        }
    }

    void ResetTimer()
    {
        if (isMatchEnded)
        {
            timeRemained = targetTime;
            timerEnded = false;
        }
    }
}