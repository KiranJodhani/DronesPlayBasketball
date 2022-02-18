using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ProjectilePredictorMultiplayer : MonoBehaviour
{
    public PlayerController playerController;
    public GameObject referenceBall;
    public GameObject markerPrefab;
    public int maxMarkerPoints = 100;

    private List<GameObject> markers = new List<GameObject>();
    private Vector3 forceVector;

    private Scene sceneMain;
    private Scene scenePrediction;

    private PhysicsScene sceneMainPhysics;
    private PhysicsScene scenePredictionPhysics;

    GameObject pathMarkSphere;
    GameObject predictionBall;
    Rigidbody rb;

    private void Start()
    {
        Physics.autoSimulation = false;
        sceneMain = SceneManager.GetActiveScene();
        sceneMainPhysics = sceneMain.GetPhysicsScene();

        CreateSceneParameters sceneParam = new CreateSceneParameters(LocalPhysicsMode.Physics3D);
        scenePrediction = SceneManager.CreateScene("PhysicsPredicitonScene", sceneParam);
        scenePredictionPhysics = scenePrediction.GetPhysicsScene();
    }

    private void FixedUpdate()
    {
        if (!sceneMainPhysics.IsValid())
            return;

        sceneMainPhysics.Simulate(Time.fixedDeltaTime);
    }

    public void ShowTrajectory()
    {
        if (!sceneMainPhysics.IsValid() || !scenePredictionPhysics.IsValid())
            return;

        ClearTrajectory();

        forceVector = -transform.forward * playerController.ballForce;

        predictionBall = Instantiate(referenceBall);
        predictionBall.transform.position = transform.position;
        SceneManager.MoveGameObjectToScene(predictionBall, scenePrediction);

        rb = predictionBall.GetComponent<Rigidbody>();
        rb.useGravity = true;
        rb.AddForce(forceVector, ForceMode.Impulse);

        for (int i = 0; i < maxMarkerPoints; i++)
        {
            scenePredictionPhysics.Simulate(Time.fixedDeltaTime);

            if (markers.Count < maxMarkerPoints)
            {
                pathMarkSphere = Instantiate(markerPrefab);
                markers.Add(pathMarkSphere);
            }
            else
            {
                pathMarkSphere = markers[i];
                pathMarkSphere.SetActive(true);
            }

            pathMarkSphere.transform.position = predictionBall.transform.position;

            if (pathMarkSphere.scene.name != scenePrediction.name)
                SceneManager.MoveGameObjectToScene(pathMarkSphere, scenePrediction);
        }

        Destroy(predictionBall);
    }

    public void ClearTrajectory()
    {
        foreach (var GO in markers)
            GO.SetActive(false);
    }
}