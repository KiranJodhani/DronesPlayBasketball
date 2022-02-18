using UnityEngine;
using DG.Tweening;

public class Referee : MonoBehaviour
{
    public GameObject ball;
    public GameObject slider;

    void Start()
    {
        BringSlider();
    }

    void BringSlider()
    {
        if (Random.value > 0.5f)
            slider.transform.position = new Vector3(0.01f, slider.transform.position.y, slider.transform.position.z);

        slider.transform.DOMoveY(2.5f, 2.5f).SetDelay(5f).SetEase(Ease.Linear).OnComplete(MoveReferee);
    }

    void MoveReferee()
    {
        Camera.main.transform.DOMoveY(11, 2).SetEase(Ease.Linear);
        transform.DOMoveY(6, 2).SetDelay(3f).SetEase(Ease.Linear).OnComplete(ReleaseBall);
        Camera.main.transform.DOMoveY(4, 3).SetDelay(3f).SetEase(Ease.Linear);
    }

    void ReleaseBall()
    {
        ball.transform.parent = null;
        ball.GetComponent<Rigidbody>().useGravity = true;
        transform.DOMoveY(10, 2.5f).SetEase(Ease.Linear).OnComplete(RemoveSlider);
    }

    void RemoveSlider()
    {
        slider.transform.DOMoveY(-2.5f, 2.5f).SetDelay(2f).SetEase(Ease.Linear);
        DisableMe();
    }

    void DisableMe()
    {
        gameObject.SetActive(false);
    }
}
