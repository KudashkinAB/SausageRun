using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Predictor : MonoBehaviour
{
    [SerializeField]
    LineRenderer simulationRenderer;
    [SerializeField]
    LineRenderer parabolaRenderer;

    public Transform obstaclesHolder;

    Scene currentScene;
    Scene predictionScene;

    PhysicsScene currentPhysicsScene;
    PhysicsScene predictionPhysicsScene;

    List<GameObject> dummyObstacles = new List<GameObject>();

    GameObject dummy;

    private void Awake()
    {
        //Physics.autoSimulation = false;

        currentScene = SceneManager.GetActiveScene();
        currentPhysicsScene = currentScene.GetPhysicsScene();

        CreateSceneParameters parameters = new CreateSceneParameters(LocalPhysicsMode.Physics3D);
        predictionScene = SceneManager.CreateScene("Prediction", parameters);
        predictionPhysicsScene = predictionScene.GetPhysicsScene();

        CopyAllObstacles();
    }

    public void Simulate(PlayerController player, Vector3 currentPosition, Vector3 speed)
    {
        PlayerController simulatedPlayer;
        simulationRenderer.enabled = true;
        if (currentPhysicsScene.IsValid() && predictionPhysicsScene.IsValid())
        {
            dummy = Instantiate(player.gameObject);
            simulatedPlayer = dummy.GetComponent<PlayerController>();
            SceneManager.MoveGameObjectToScene(dummy, predictionScene);

            dummy.transform.position = currentPosition;
            simulatedPlayer.Push(speed);
            simulationRenderer.positionCount = 0;
            simulationRenderer.positionCount = UIController.predictionCount;

            for (int i = 0; i < UIController.predictionCount; i++)
            {
                predictionPhysicsScene.Simulate(Time.fixedDeltaTime);
                simulationRenderer.SetPosition(i, simulatedPlayer.positionBone.transform.position);
            }

            Destroy(dummy);
        }
    }

    public void Parabola(Vector3 startPos ,float g, Vector3 speed)
    {
        parabolaRenderer.enabled = true;
        parabolaRenderer.positionCount = UIController.predictionCount;
        for (int i = 0; i < UIController.predictionCount; i++)
        {
            Vector3 currentPos = new Vector3( startPos.x + speed.x * Time.fixedDeltaTime * i, 
                startPos.y -(g * Mathf.Pow(Time.fixedDeltaTime * i,2)) / 2 + speed.y * Time.fixedDeltaTime * i, 0);
            parabolaRenderer.SetPosition(i, currentPos);
        }
    }

    public void Stop()
    {
        simulationRenderer.enabled = false;
        parabolaRenderer.enabled = false;
    }

    public void CopyAllObstacles()
    {
        List<Transform> obstacles = new List<Transform>();
        for (int i = 0; i < obstaclesHolder.childCount; i++)
        {
            obstacles.Add(obstaclesHolder.GetChild(i));
        }

        foreach (Transform t in obstaclesHolder)
        {
            if (t.gameObject.GetComponent<Collider>() != null)
            {
                GameObject fakeT = Instantiate(t.gameObject);
                fakeT.transform.position = t.position;
                fakeT.transform.rotation = t.rotation;
                Renderer fakeR = fakeT.GetComponent<Renderer>();
                if (fakeR)
                {
                    fakeR.enabled = false;
                }
                SceneManager.MoveGameObjectToScene(fakeT, predictionScene);
                dummyObstacles.Add(fakeT);
            }
        }
    }

    private void Update()
    {
        
    }
}
