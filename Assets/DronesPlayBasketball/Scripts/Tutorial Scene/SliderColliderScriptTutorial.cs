using UnityEngine;
using DG.Tweening;
using TMPro;

public class SliderColliderScriptTutorial : MonoBehaviour
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
            }
        }
    }
}
