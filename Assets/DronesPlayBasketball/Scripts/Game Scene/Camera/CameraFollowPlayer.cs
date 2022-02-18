using UnityEngine;

public class CameraFollowPlayer : MonoBehaviour
{
    public Transform Player;
    public Vector3 offset;  //Offset value x -1.5,y -3.95,z 8  
    public float xMin = -6f;
    public float xMax = 6f;
    public float yMin = 4.6f;
    public float yMax = 5.5f;
    public float zMin = -16.5f;
    public float zMax = -8f;
    private Vector3 PlayerPosition;



    void Update()
    {
        if (Player)
        {
            PlayerPosition = Player.localPosition;
            PlayerPosition = PlayerPosition - offset;

            if (PlayerPosition.z < zMin)
            {
                PlayerPosition.z = zMin;
            }

            if (PlayerPosition.z > zMax)
            {
                PlayerPosition.z = zMax;
            }

            if (PlayerPosition.x < xMin)
            {
                PlayerPosition.x = xMin;
            }

            if (PlayerPosition.x > xMax)
            {
                PlayerPosition.x = xMax;
            }

            //if (PlayerPosition.y < yMin)
            //{
            //    PlayerPosition.y = yMin;
            //}

            if (PlayerPosition.y > yMax)
            {
                PlayerPosition.y = yMax;
            }

            transform.localPosition = Vector3.Lerp(transform.localPosition, PlayerPosition, 0.2f);
        }
    }
}