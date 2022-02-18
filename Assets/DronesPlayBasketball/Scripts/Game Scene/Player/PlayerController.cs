using UnityEngine;
using Photon.Pun;
using DG.Tweening;
using UnityEngine.EventSystems;
using System;

public class PlayerController : MonoBehaviourPun
{
    private GameUIController uiController;
    private ProjectilePredictorMultiplayer projectilePredictorMultiplayer;
    //public ShootingScript shootingScript;

    //public Camera myCamera;
    //public TMP_Text scoreText;

    [Header("Ball")]
    public Transform handParent;
    public Transform leftHandParent;
    public Transform rightHandParent;
    public Transform ballAttachPoint;
    public Transform trajectoryTransform;
    public float targetRotation;
    public float ballForce;
    public GameObject ball;

    [Header("Bullet")]
    public GameObject bulletPrefab;
    public Transform firePos;
    public float bulletSpeed;
    public float fireRate;
    private float nextFire;

    [Header("Controller")]
    public SimpleTouchController leftController;
    public SimpleTouchController rightController;
    public float speedMovements;
    public float speedRotation;

    [Header("Player Movement Limit")]
    public float xMin;
    public float xMax;
    public float yMin;
    public float yMax;
    public float zMin;
    public float zMax;

    [Header("Player Spot")]
    public GameObject playerSpotGO;

    // Private
    private Vector3 newPosition;
    private Vector3 newPositionYOnly;
    private Vector3 RotationAngle;
    //private float Y_Limit;
    //private int score;
    private bool hasBall;
    //private bool isShowTrajectory;


    public void Start()
    {
        if (photonView.IsMine)
        {
            uiController = FindObjectOfType<GameUIController>();
            leftController = uiController.leftJoystick;
            rightController = uiController.rightJoystick;
            uiController.fireButton.onClick.AddListener(() => FireBullets());
            //uiController.goalButton.onClick.AddListener(() => ReleaseBall());

            EventTrigger eventTrigger = uiController.goalButton.gameObject.AddComponent<EventTrigger>();
            AddEvent(eventTrigger, EventTriggerType.PointerDown, ShowTrajectory);
            AddEvent(eventTrigger, EventTriggerType.PointerUp, HideTrajectory);

            projectilePredictorMultiplayer = GetComponentInChildren<PlayerReferences>().projectile.GetComponent<ProjectilePredictorMultiplayer>();
            projectilePredictorMultiplayer.gameObject.SetActive(true);

            FindObjectOfType<CameraFollowPlayer>().Player = transform;
        }
        handParent = GetComponentInChildren<PlayerReferences>().handParent;
        leftHandParent = GetComponentInChildren<PlayerReferences>().leftHandParent;
        rightHandParent = GetComponentInChildren<PlayerReferences>().rightHandParent;
        ballAttachPoint = GetComponentInChildren<PlayerReferences>().ballAttachPoint;
        trajectoryTransform = GetComponentInChildren<PlayerReferences>().projectile.transform;
        firePos = GetComponentInChildren<PlayerReferences>().firePos;
        GetComponent<BoxCollider>().center = GetComponentInChildren<PlayerReferences>().GetComponent<BoxCollider>().center;
        GetComponent<BoxCollider>().size = GetComponentInChildren<PlayerReferences>().GetComponent<BoxCollider>().size;
        Destroy(GetComponentInChildren<PlayerReferences>().GetComponent<BoxCollider>());
        GameUIController.CanFireBullet = true;
        playerSpotGO.SetActive(true);

        if (PhotonNetwork.IsMasterClient)
        {
            Invoke("EnableReferee", 2f);
        }
    }

    void AddEvent(EventTrigger trigger, EventTriggerType eventType, Action<PointerEventData> function)
    {
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = eventType;
        entry.callback.AddListener(data => { function((PointerEventData)data); });
        trigger.triggers.Add(entry);
    }

    void Update()
    {
        if (photonView.IsMine)
        {
            JoystickControlls();
        }
    }

    void FixedUpdate()
    {
        if (photonView.IsMine)
        {
            //JoystickControlls();
            GetComponent<Rigidbody>().velocity = Vector3.zero;

            //if (isShowTrajectory)
            //    projectilePredictorMultiplayer.ShowTrajectory();
            //else
            //    projectilePredictorMultiplayer.ClearTrajectory();

            if (hasBall)
            {
                ball.GetComponent<Rigidbody>().velocity = Vector3.zero;
                ball.transform.localPosition = Vector3.zero;
                ball.transform.localRotation = Quaternion.Euler(Vector3.zero);
            }
        }
    }

    public void ShowTrajectory(PointerEventData data)
    {
        if (hasBall)
        {
            //isShowTrajectory = true;
            InvokeRepeating("RenderTrajectory", 0f, 0.05f);
        }
    }

    public void HideTrajectory(PointerEventData data)
    {
        CancelInvoke("RenderTrajectory");
        //isShowTrajectory = false;
        ReleaseBall();
        RemoveTrajectory();
    }

    void RenderTrajectory()
    {
        if (photonView.IsMine)
            projectilePredictorMultiplayer.ShowTrajectory();
    }

    void RemoveTrajectory()
    {
        if (photonView.IsMine)
            projectilePredictorMultiplayer.ClearTrajectory();
    }

    void JoystickControlls()
    {
        // move
        if (leftController)
            UpdatePosition(leftController.GetTouchPosition);

        // rotate
        if (rightController)
            UpdateAim(rightController.GetTouchPosition);
    }

    //void UpdatePosition()
    //{
    //    newPosition = transform.position +
    //        (transform.forward * leftController.GetTouchPosition.y * Time.deltaTime * speedMovements) +
    //        (transform.right * leftController.GetTouchPosition.x * Time.deltaTime * speedMovements);

    //    newPosition = new Vector3(Mathf.Clamp(newPosition.x, xMin, xMax), Mathf.Clamp(newPosition.y, yMin, yMax), Mathf.Clamp(newPosition.z, zMin, zMax));

    //    transform.position = newPosition;
    //}

    void UpdatePosition(Vector2 PosValue)
    {
        newPosition = new Vector3(transform.localPosition.x + PosValue.x * Time.deltaTime * speedMovements, transform.localPosition.y, transform.localPosition.z + PosValue.y * Time.deltaTime * speedMovements);
        newPosition = new Vector3(Mathf.Clamp(newPosition.x, xMin, xMax), Mathf.Clamp(newPosition.y, yMin, yMax), Mathf.Clamp(newPosition.z, zMin, zMax));
        transform.position = newPosition;
    }

    void UpdateAim(Vector2 value)
    {
        newPositionYOnly = transform.position + (transform.up * rightController.GetTouchPosition.y * Time.deltaTime * speedMovements);
        newPositionYOnly = new Vector3(Mathf.Clamp(newPositionYOnly.x, xMin, xMax),
            Mathf.Clamp(newPositionYOnly.y, yMin, yMax),
            Mathf.Clamp(newPositionYOnly.z, zMin, zMax));
        transform.position = newPositionYOnly;

        //RotationAngle = new Vector3(transform.localEulerAngles.x - value.y * Time.deltaTime * speedRotation,
        //        transform.localEulerAngles.y + value.x * Time.deltaTime * speedRotation,
        //        0f);

        //Y_Limit = RotationAngle.x;

        //while (Y_Limit < -180) Y_Limit += 360;
        //while (Y_Limit > 180) Y_Limit -= 360;

        ////if (Y_Limit < -90) RotationAngle.x = -90;
        ////if (Y_Limit > 90) RotationAngle.x = 90;
        //if (Y_Limit < -60) RotationAngle.x = -60;
        //if (Y_Limit > 15) RotationAngle.x = 15;

        RotationAngle = new Vector3(0, transform.localEulerAngles.y + value.x * Time.deltaTime * speedRotation, 0f);
        transform.eulerAngles = RotationAngle;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Ball")
        {
            if (!hasBall)
            {
                //photonView.RPC("AddScore", RpcTarget.All);
                //GetComponent<BoxCollider>().enabled = false;
                ball = other.gameObject;

                photonView.RPC("GrabBallRPC", RpcTarget.All, photonView.ViewID);
            }
        }
    }

    [PunRPC]
    void GrabBallRPC(int SenderViewID)
    {
        ball = GameObject.FindWithTag("Ball");

        if (ball)
        {
            GetComponent<BoxCollider>().enabled = false;
            ball.GetComponent<Rigidbody>().useGravity = false;
            ball.GetComponent<Rigidbody>().velocity = Vector3.zero;
            ball.GetComponent<PhotonTransformView>().enabled = false;
            ball.GetComponent<PhotonRigidbodyView>().enabled = false;
            ball.transform.SetParent(PhotonView.Find(SenderViewID).GetComponent<PlayerController>().ballAttachPoint);
            ball.transform.localPosition = Vector3.zero;
            ball.GetComponent<Rigidbody>().isKinematic = true;
            ball.transform.localRotation = Quaternion.Euler(Vector3.zero);
            ball.GetComponent<Rigidbody>().isKinematic = false;
            hasBall = true;
            GameUIController.CanFireBullet = false;
        }
    }

    public void FireBullets()
    {
        if (GameUIController.CanFireBullet)
        {
            if (Time.time > nextFire)
            {
                nextFire = Time.time + fireRate;
                GameObject bullet = PhotonNetwork.Instantiate(bulletPrefab.name, firePos.position, firePos.rotation);
                bullet.SetActive(true);
                bullet.GetComponent<Rigidbody>().velocity = transform.forward * bulletSpeed;
            }
        }
    }

    void DropBall()
    {
        photonView.RPC("DropBallRPC", RpcTarget.All, photonView.ViewID);
    }

    [PunRPC]
    void DropBallRPC(int SenderViewID)
    {
        if (hasBall)
        {
            hasBall = false;
            ball.transform.parent = null;
            ball.GetComponent<Rigidbody>().useGravity = true;
            ball.GetComponent<PhotonTransformView>().enabled = true;
            ball.GetComponent<PhotonRigidbodyView>().enabled = true;
            ball = null;
            GameUIController.CanFireBullet = true;
        }
        Invoke("EnableCollider", 1f);
    }

    void ReleaseBall()
    {
        photonView.RPC("OnGoalStart_RPC", RpcTarget.All, photonView.ViewID);
    }

    [PunRPC]
    void OnGoalStart_RPC(int SenderViewID)
    {
        //PhotonView.Find(SenderViewID).GetComponent<PlayerController>().leftHandParent.DOLocalRotate(Vector3.right * targetRotation, 0.25f).SetEase(Ease.Linear).OnComplete(OnGoalCompleted);
        //PhotonView.Find(SenderViewID).GetComponent<PlayerController>().rightHandParent.DOLocalRotate(Vector3.right * targetRotation, 0.25f).SetEase(Ease.Linear).OnComplete(OnGoalCompleted);
        PhotonView.Find(SenderViewID).GetComponent<PlayerController>().handParent.DOLocalRotate(Vector3.right * targetRotation, 0.25f).SetEase(Ease.Linear).OnComplete(OnGoalCompleted);
    }

    void OnGoalCompleted()
    {
        photonView.RPC("OnGoalCompletedRPC", RpcTarget.All, photonView.ViewID);
    }

    [PunRPC]
    void OnGoalCompletedRPC(int SenderViewID)
    {
        if (hasBall)
        {
            hasBall = false;
            ball.transform.parent = null;
            ball.GetComponent<Rigidbody>().useGravity = true;
            //ball.GetComponent<Rigidbody>().AddForce(ball.transform.forward * ballForce);
            ball.GetComponent<Rigidbody>().AddForce(-trajectoryTransform.forward * ballForce, ForceMode.Impulse);
            //ball.GetComponent<Rigidbody>().AddForce(-ball.transform.forward * ballForce, ForceMode.Impulse);
            ball.GetComponent<PhotonTransformView>().enabled = true;
            ball.GetComponent<PhotonRigidbodyView>().enabled = true;
            ball = null;
            GameUIController.CanFireBullet = true;
        }
        //PhotonView.Find(SenderViewID).GetComponent<PlayerController>().leftHandParent.DOLocalRotate(Vector3.zero, 0.25f).SetEase(Ease.Linear);
        //PhotonView.Find(SenderViewID).GetComponent<PlayerController>().rightHandParent.DOLocalRotate(Vector3.zero, 0.25f).SetEase(Ease.Linear);
        PhotonView.Find(SenderViewID).GetComponent<PlayerController>().handParent.DOLocalRotate(Vector3.zero, 0.25f).SetEase(Ease.Linear);
        //gameObject.GetComponent<BoxCollider>().enabled = true;
        Invoke("EnableCollider", 1f);
    }

    void EnableCollider()
    {
        GetComponent<BoxCollider>().enabled = true;
    }

    void EnableReferee()
    {
        photonView.RPC("EnableRefereeRPC", RpcTarget.All);
    }

    [PunRPC]
    void EnableRefereeRPC()
    {
        FindObjectOfType<Referee>().enabled = true;
    }
}