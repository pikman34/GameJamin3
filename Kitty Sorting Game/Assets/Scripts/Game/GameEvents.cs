using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameEvents : MonoBehaviour
{
    public static Action CheckIfShapeCanBePlaced;
    public ShapeStorage shapeStorage;

    public void OnCompletePressed()
    {
        int currentScene = SceneManager.GetActiveScene().buildIndex;
        int totalScenes = SceneManager.sceneCountInBuildSettings;

        if (shapeStorage.ValidatePuzzle())
        {
            if (currentScene + 1 < totalScenes)
            {
                SceneManager.LoadScene(currentScene + 1);
            }

            else
            {
                print("It's the last scene");
            }
        }
        else
        {
            Debug.Log("Fail");
        }
    }
}
