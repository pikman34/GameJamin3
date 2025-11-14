using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ShapeSquare : MonoBehaviour
{
    public Image occupiedImage;
    public Image normalImage;
    [HideInInspector] public GridSquare currentGridSquare;
    private List<GridSquare> hoveredSquares = new List<GridSquare>();

    void Start()
    {
        occupiedImage.gameObject.SetActive(false);
    }

    public void DeactivateShape()
    {
        var color = normalImage.color;
        color.a = 0f;
        normalImage.color = color;

        normalImage.raycastTarget = true;
    }

    public void ActivateShape()
    {
        var color = normalImage.color;
        color.a = 1f;
        normalImage.color = color;

        normalImage.raycastTarget = true;
    }
    public void SetOccupiedImage()
    {
        if (occupiedImage != null)
            occupiedImage.gameObject.SetActive(true);
    }

    public void UnSetOccupiedImage()
    {
        if (occupiedImage != null)
            occupiedImage.gameObject.SetActive(false);
    }

    // Called by GridSquare OnTriggerEnter/Stay
    public void AddHoveredSquare(GridSquare gridSquare)
    {
        if (!hoveredSquares.Contains(gridSquare))
            hoveredSquares.Add(gridSquare);
    }

    public void RemoveHoveredSquare(GridSquare gridSquare)
    {
        hoveredSquares.Remove(gridSquare);
    }

    public bool CanPlace()
    {
        foreach (var sq in hoveredSquares)
        {
            if (sq.SquareOccupied)
                return false;
        }
        return true;
    }

    public Vector3 GetSnapPosition()
    {
        if (hoveredSquares.Count == 0) return transform.position;
        return hoveredSquares[0].transform.position;
    }
}
