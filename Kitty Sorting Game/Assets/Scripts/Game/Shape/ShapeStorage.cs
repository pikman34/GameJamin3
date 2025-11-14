using System.Collections.Generic;
using UnityEngine;

public class ShapeStorage : MonoBehaviour
{
    public List<ShapeData> shapeData;
    public List<Shape> shapeList;

    void Start()
    {
        foreach (var shape in shapeList)
        {
            var shapeIndex = shapeData.Count;
        }
    }
}
