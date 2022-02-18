using UnityEngine;
using DG.Tweening;
using TMPro;

public class SliderColliderScript : MonoBehaviour
{
    public GameObject sliderRing;
    public bool isEntryCollider;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Ball")
        {
            if (isEntryCollider)
            {
                sliderRing.SetActive(true);
                sliderRing.transform.SetParent(other.transform);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Ball")
        {
            if (!isEntryCollider)
            {
                sliderRing.SetActive(false);
                sliderRing.transform.parent = null;

                TMP_Text ballText = GameObject.Find("Ball Text (TMP)").GetComponent<TMP_Text>();
                if (gameObject.name == "Collider2")
                    ballText.text = "Team1 Won The Ball";
                else
                    ballText.text = "Team2 Won The Ball";
                ballText.DOFade(1f, 0.5f);
                ballText.DOFade(0f, 0.5f).SetDelay(1.5f);
            }
        }
    }
}
