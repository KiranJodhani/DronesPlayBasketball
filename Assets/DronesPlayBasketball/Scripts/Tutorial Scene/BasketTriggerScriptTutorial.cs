using UnityEngine;

public class BasketTriggerScriptTutorial : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Ball")
        {
            BasketGoalScriptTutorial.trigger1 = true;
        }
    }
}
