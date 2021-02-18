using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIController : MonoBehaviour
{
    public static UIController ui { get; private set; }

    public static int predictionCount = 100;
    public static bool simulation = false;
    public static bool parabola = true;

    [SerializeField] GameObject gameOverPanel;
    [SerializeField] GameObject finishPanel;
    [SerializeField] GameObject optionsPanel;
    [SerializeField] Slider predictSlider;
    [SerializeField] Toggle simulationToggle;
    [SerializeField] Toggle parabolaToggle;

    private void Awake()
    {
        ui = this;
    }

    public void GameOver()
    {
        if (!finishPanel.activeInHierarchy)
        {
            gameOverPanel.SetActive(true);
            optionsPanel.SetActive(false);
        }
    }

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void Finish()
    {
        if (!gameOverPanel.activeInHierarchy)
        {
            optionsPanel.SetActive(false);
            finishPanel.SetActive(true);
        }
    }

    public void NextLevel()
    {
        if (SceneManager.GetActiveScene().buildIndex < SceneManager.sceneCountInBuildSettings - 1)
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        else
            SceneManager.LoadScene(0);
    }

    public void ChangeIteration()
    {
        predictionCount = (int)predictSlider.value;
    }

    public void ChangePredictionType()
    {
        simulation = simulationToggle.isOn;
        parabola = parabolaToggle.isOn;
    }
    
}
