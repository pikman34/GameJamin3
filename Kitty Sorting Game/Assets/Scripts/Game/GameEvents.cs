using System;
using UnityEngine;

public class GameEvents : MonoBehaviour
{
    public static Action CheckIfShapeCanBePlaced;

    public ShapeStorage shapeStorage;

    public void OnCompletePressed()
    {
        if (shapeStorage.ValidatePuzzle())
        {
            Debug.Log("Passed!");
        }
        else
        {
            Debug.Log("Fail");
        }
    }
}
