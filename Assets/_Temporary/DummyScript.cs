using UnityEngine;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class DummyScript : MonoBehaviour
{
    public float moveSpeed = 125;
    public float targetRotation = 50;
    public Transform hand;
    public GameObject ball;
    public Transform ballAttachPoint;
    private Vector3 newPosition;
    public Transform ballShootReferencePoint;

    [Header("Trajectory")]
    public float velocityValue = 10f;
    public float velocityErrorCorrection = 0.75f;
    public LineRenderer lineRenderer;

    void Start()
    {
        newPosition = transform.position;
    }

    void FixedUpdate()
    {
#if UNITY_EDITOR

        CheckMovement();
        CheckRotation();

        if (ball)
        {
            ball.GetComponent<Rigidbody>().velocity = Vector3.zero;
            ball.transform.localPosition = Vector3.zero;
            ball.transform.localRotation = Quaternion.Euler(Vector3.zero);
        }

        // Shooting
        if (Input.GetKeyDown(KeyCode.R))
        {
            ReleaseBall();
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            DropBall();
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        if (Input.GetKey(KeyCode.Space))
        {
            UpdateTrajectoryLineRenderer(ballShootReferencePoint.position, -ballShootReferencePoint.forward * velocityValue);
        }
        if (Input.GetKeyUp(KeyCode.Space))
        {
            ReleaseBall();
        }
#endif
    }

    void UpdateTrajectoryLineRenderer(Vector3 initialPosition, Vector3 initialVelocity)
    {
        float timeDelta = 1.0f / initialVelocity.magnitude;
        float drag = ball.GetComponent<Rigidbody>().drag;

        Vector3 position = initialPosition;
        Vector3 velocity = initialVelocity;

        for (int i = 0; i < lineRenderer.positionCount; ++i)
        {
            lineRenderer.SetPosition(i, position);

            position += velocity * timeDelta + 0.5f * Physics.gravity * timeDelta * timeDelta;
            velocity += Physics.gravity * timeDelta;
            velocity *= 1f - drag * timeDelta;
        }
    }

    void DropBall()
    {
        if (ball)
        {
            ball.transform.parent = null;
            ball.GetComponent<Rigidbody>().useGravity = true;
            ball = null;
            GameUIController.CanFireBullet = true;
        }
        Invoke("EnableCollider", 1f);
    }

    void ReleaseBall()
    {
        OnGoalStart();
    }

    void OnGoalStart()
    {
        hand.DOLocalRotate(Vector3.right * targetRotation, 0.25f).SetEase(Ease.Linear).OnComplete(OnGoalCompleted);
    }

    void OnGoalCompleted()
    {
        if (ball)
        {
            ball.transform.parent = null;
            ball.GetComponent<Rigidbody>().useGravity = true;
            //ball.GetComponent<Rigidbody>().AddForce(-ball.transform.forward * moveSpeed, ForceMode.Impulse);
            ball.GetComponent<Rigidbody>().velocity = -ball.transform.forward * (velocityValue + velocityErrorCorrection);
            ball = null;
            GameUIController.CanFireBullet = true;
        }
        hand.DOLocalRotate(Vector3.zero, 0.25f).SetEase(Ease.Linear).SetDelay(0.25f);
        Invoke("EnableCollider", 1f);
    }

    void EnableCollider()
    {
        gameObject.GetComponent<BoxCollider>().enabled = true;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Ball")
        {
            if (!ball)
            {
                GetComponent<BoxCollider>().enabled = false;
                ball = other.gameObject;
                ball.GetComponent<Rigidbody>().useGravity = false;
                ball.GetComponent<Rigidbody>().velocity = Vector3.zero;
                ball.transform.SetParent(ballAttachPoint);
                GameUIController.CanFireBullet = false;
            }
        }
    }

    void CheckMovement()
    {
        // Movement
        if (Input.GetKey(KeyCode.UpArrow))
        {
            newPosition = transform.position + transform.forward * Time.deltaTime * 5;
        }
        else if (Input.GetKey(KeyCode.DownArrow))
        {
            newPosition = transform.position - transform.forward * Time.deltaTime * 5;
        }

        if (Input.GetKey(KeyCode.RightArrow))
        {
            newPosition = transform.position + transform.right * Time.deltaTime * 5;
        }
        else if (Input.GetKey(KeyCode.LeftArrow))
        {
            newPosition = transform.position - transform.right * Time.deltaTime * 5;
        }

        if (Input.GetKey(KeyCode.W))
        {
            newPosition = transform.position + transform.up * Time.deltaTime * 5;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            newPosition = transform.position - transform.up * Time.deltaTime * 5;
        }

        newPosition = new Vector3(Mathf.Clamp(newPosition.x, -14, 14), Mathf.Clamp(newPosition.y, 0.3f, 6), Mathf.Clamp(newPosition.z, -12, 3));
        transform.position = newPosition;
    }

    void CheckRotation()
    {
        // Rotation
        //if (Input.GetKey(KeyCode.W))
        //{
        //    transform.eulerAngles = new Vector3(transform.localEulerAngles.x + 1 * Time.deltaTime * 50, transform.localEulerAngles.y, 0f);
        //}
        //else if (Input.GetKey(KeyCode.S))
        //{
        //    transform.eulerAngles = new Vector3(transform.localEulerAngles.x - 1 * Time.deltaTime * 50, transform.localEulerAngles.y, 0f);
        //}

        if (Input.GetKey(KeyCode.D))
        {
            transform.eulerAngles = new Vector3(transform.localEulerAngles.x, transform.localEulerAngles.y + 1 * Time.deltaTime * 50, 0f);
        }
        else if (Input.GetKey(KeyCode.A))
        {
            transform.eulerAngles = new Vector3(transform.localEulerAngles.x, transform.localEulerAngles.y - 1 * Time.deltaTime * 50, 0f);
        }
    }
}
