using UnityEngine;

public class BasketGoalScriptTutorial : MonoBehaviour
{
    public MeshRenderer basketRing;
    public SkinnedMeshRenderer basketNet;
    public static bool trigger1;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Ball")
        {
            if (trigger1)
            {
                trigger1 = false;
                basketRing.material.color = Color.green;
                basketNet.material.color = Color.green;
                Invoke("ResetBasketColor", 2f);
            }
        }
    }

    private void ResetBasketColor()
    {
        basketRing.material.color = Color.red;
        basketNet.material.color = Color.red;
    }
}
