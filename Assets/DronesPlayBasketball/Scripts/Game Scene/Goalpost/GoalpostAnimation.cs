using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class GoalpostAnimation : MonoBehaviour
{
    private Vector3 pos1 = new Vector3(-58, 0, 0);
    private Vector3 pos2 = new Vector3(68, 0, 0);
    public float speed = 1.0f;

    private void Start()
    {
        if (!PhotonNetwork.IsMasterClient)
            enabled = false;
    }

    void Update()
    {
        transform.localPosition = Vector3.Lerp(pos1, pos2, (Mathf.Sin(speed * Time.time) + 1.0f) / 2.0f);
    }
}
