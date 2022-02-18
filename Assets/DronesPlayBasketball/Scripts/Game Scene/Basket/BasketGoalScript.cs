using UnityEngine;

public class BasketGoalScript : MonoBehaviour
{
    //public GameObject goalParticleEffect;
    public GameUIController uiController;
    public ConstantData.BasketSide mySide;
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
                //goalParticleEffect.SetActive(true);
                //goalParticleEffect.GetComponent<ParticleSystem>().Play();
                //Invoke("DisableParticle", 5f);
                basketRing.material.color = Color.green;
                basketNet.material.color = Color.green;
                Invoke("ResetBasketColor", 2f);
                HandlePlayerGoal();
            }
        }
    }

    //private void DisableParticle()
    //{
    //    goalParticleEffect.SetActive(false);
    //}

    void HandlePlayerGoal()
    {
        uiController.HandlePlayerGoal(mySide);
    }

    private void ResetBasketColor()
    {
        basketRing.material.color = Color.red;
        basketNet.material.color = Color.red;
    }
}
