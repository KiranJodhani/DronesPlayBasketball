using UnityEngine;

public class BasketTriggerScript : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Ball")
        {
            BasketGoalScript.trigger1 = true;
        }
    }
}
