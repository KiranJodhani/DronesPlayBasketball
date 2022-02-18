using UnityEngine;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class TutorialPlayerController : MonoBehaviour
{
    public ProjectilePredictor projectilePredictor;

    [Header("Ball")]
    public Transform handParent;
    public Transform ballAttachPoint;
    public Transform trajectoryTransform;
    public float targetRotation;
    public float ballForce;
    private GameObject ball;

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

    // Private
    private Vector3 newPosition;
    private Vector3 newPositionYOnly;
    private Vector3 RotationAngle;
    private bool hasBall, applyForce;
    private float ballHoldTime;

    [Header("Trajectory")]
    public float velocityValue = 10f;
    public float velocityErrorCorrection = 0.75f;
    public LineRenderer lineRenderer;
    float timeDelta;
    float drag;
    Vector3 position;
    Vector3 velocity;
    bool isShowTrajectory;



    void Start()
    {
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
    }

    void Update()
    {
        JoystickControlls();
    }

    void FixedUpdate()
    {
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
#endif

        GetComponent<Rigidbody>().velocity = Vector3.zero;
        if (hasBall)
        {
            ball.GetComponent<Rigidbody>().velocity = Vector3.zero;
            ball.transform.localPosition = Vector3.zero;
            ball.transform.localRotation = Quaternion.Euler(Vector3.zero);

            ballHoldTime -= Time.deltaTime;
            if (ballHoldTime <= 0.0f)
            {
                //MakeGoal(false);
            }

            if (isShowTrajectory)
            {
                UpdateTrajectoryLineRenderer(trajectoryTransform.position, -trajectoryTransform.forward * velocityValue);
            }
            else
            {
                lineRenderer.enabled = false;
            }
        }

    }

    public void ShowTrajectory()
    {
        if (hasBall)
        {
            isShowTrajectory = true;
            lineRenderer.enabled = true;
        }
    }

    public void HideTrajectory()
    {
        isShowTrajectory = false;
        MakeGoal(true);
    }

    void UpdateTrajectoryLineRenderer(Vector3 initialPosition, Vector3 initialVelocity)
    {
        timeDelta = 1.0f / initialVelocity.magnitude;
        drag = ball.GetComponent<Rigidbody>().drag;

        position = initialPosition;
        velocity = initialVelocity;

        for (int i = 0; i < lineRenderer.positionCount; ++i)
        {
            lineRenderer.SetPosition(i, position);

            position += velocity * timeDelta + 0.5f * Physics.gravity * timeDelta * timeDelta;
            velocity += Physics.gravity * timeDelta;
            velocity *= 1f - drag * timeDelta;
        }
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

        RotationAngle = new Vector3(0, transform.localEulerAngles.y + value.x * Time.deltaTime * speedRotation, 0f);
        transform.eulerAngles = RotationAngle;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Ball")
        {
            if (!hasBall)
            {
                gameObject.GetComponent<BoxCollider>().enabled = false;
                ball = other.gameObject;
                GrabBall();
            }
        }
    }

    void GrabBall()
    {
        ball.GetComponent<Rigidbody>().useGravity = false;
        ball.GetComponent<Rigidbody>().velocity = Vector3.zero;
        ball.transform.SetParent(ballAttachPoint);
        hasBall = true;
        GameUIController.CanFireBullet = false;
        ballHoldTime = 10f;
    }

    //[PunRPC]
    //public void AddScore()
    //{
    //    score += 1;
    //    scoreText.text = score.ToString();
    //}

    public void FireBullets()
    {
        if (GameUIController.CanFireBullet)
        {
            if (Time.time > nextFire)
            {
                nextFire = Time.time + fireRate;
                GameObject bullet = Instantiate(bulletPrefab, firePos.position, firePos.rotation);
                bullet.SetActive(true);
                bullet.GetComponent<Rigidbody>().velocity = transform.forward * bulletSpeed;
            }
        }
    }

    public void MakeGoal(bool addForce)
    {
        if (hasBall)
        {
            applyForce = addForce;
            if (addForce)
            {
                handParent.DOLocalRotate(Vector3.right * targetRotation, 0.25f).SetEase(Ease.Linear).OnComplete(ReleaseBall);
            }
            else
            {
                ReleaseBall();
            }
        }
    }

    void ReleaseBall()
    {
        hasBall = false;
        ball.transform.parent = null;
        ball.GetComponent<Rigidbody>().useGravity = true;

        if (applyForce)
        {
            //ball.GetComponent<Rigidbody>().AddForce(-trajectoryTransform.forward * ballForce, ForceMode.Impulse);
            ball.GetComponent<Rigidbody>().velocity = -trajectoryTransform.forward * (velocityValue + velocityErrorCorrection);
            handParent.DOLocalRotate(Vector3.zero, 0.25f).SetEase(Ease.Linear).SetDelay(0.25f);
        }

        ball = null;
        GameUIController.CanFireBullet = true;
        Invoke("EnableCollider", 1f);
    }

    void EnableCollider()
    {
        gameObject.GetComponent<BoxCollider>().enabled = true;
    }
}