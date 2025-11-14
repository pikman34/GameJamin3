using System.Collections.Generic;
using UnityEngine;

public class ShapeStorage : MonoBehaviour
{
    public List<ShapeData> shapeData;
    public List<Shape> shapeList;
    public Shape currentSelectedShape;

    void Start()
    {
        foreach (var shape in shapeList)
        {
            var shapeIndex = shapeData.Count;
        }
    }

    public Shape GetCurrentSelectedShape()
    {
        foreach (var shape in shapeList)
        {
            if(/*shape.IsOnStartPosition() == false && */shape.IsAnyOfShapeSquareActive())
                return currentSelectedShape;
        }

        Debug.LogError("There is no shape selected!");
        return null;
    }
}
