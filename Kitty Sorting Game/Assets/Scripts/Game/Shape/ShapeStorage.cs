using System.Collections.Generic;
using UnityEngine;

public class ShapeStorage : MonoBehaviour
{
    public List<ShapeData> shapeData;
    public List<Shape> shapeList;
    public List<Shape> placedShapes = new List<Shape>();
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
        if (currentSelectedShape != null)
            return currentSelectedShape;

        Debug.LogError("There is no shape selected!");
        return null;
    }

    public static class ShapeFacingRules
    {
        // increased dot threshold to reduce false positives
        private const float FacingDotThreshold = 0.7f;

        public static bool IsFacing(Transform from, Transform to)
        {
            Vector2 forward = from.up.normalized;
            Vector2 direction = ((Vector2)to.position - (Vector2)from.position).normalized;
            float dot = Vector2.Dot(forward, direction);
            return dot > FacingDotThreshold;
        }

        public static bool IsBackFacing(Transform from, Transform to)
        {
            Vector2 backward = (-from.up).normalized;
            Vector2 direction = ((Vector2)to.position - (Vector2)from.position).normalized;
            float dot = Vector2.Dot(backward, direction);
            return dot > FacingDotThreshold;
        }
    }

    // Main validator
    public bool ValidatePuzzle()
    {
        if (placedShapes.Count != shapeList.Count)
        {
            Debug.Log("Not all shapes are placed.");
            return false;
        }

        // precompute adjacency distance threshold from currently placed grid squares
        float adjacencyDistance = ComputeGridSpacingApproximate();
        // small tolerance multiplier so we accept minor floating point offsets
        float adjacencyThreshold = adjacencyDistance * 1.2f;

        for (int i = 0; i < placedShapes.Count; i++)
        {
            for (int j = i + 1; j < placedShapes.Count; j++)
            {
                var A = placedShapes[i];
                var B = placedShapes[j];

                if (!CheckPairRules(A, B, adjacencyThreshold))
                {
                    return false;
                }
            }
        }

        return true;
    }

    // Return approximate grid spacing (distance between adjacent squares).
    // We look at occupied GridSquare transforms across placed shapes and pick the smallest non-zero distance found.
    // Fallback to 1f if we can't compute it.
    private float ComputeGridSpacingApproximate()
    {
        List<Vector2> positions = new List<Vector2>();

        foreach (var s in placedShapes)
        {
            foreach (var gs in s._occupiedSquares)
            {
                if (gs != null)
                    positions.Add(gs.transform.position);
            }
        }

        float minNonZero = float.MaxValue;

        // Limit work if there are many squares
        int limit = Mathf.Min(positions.Count, 60);
        for (int i = 0; i < limit; i++)
        {
            for (int k = i + 1; k < limit; k++)
            {
                float d = Vector2.Distance(positions[i], positions[k]);
                if (d > 0.0001f && d < minNonZero)
                {
                    minNonZero = d;
                }
            }
        }

        if (minNonZero == float.MaxValue)
        {
            // fallback guess
            return 1f;
        }

        return minNonZero;
    }

    // Pair rule check now requires both facing AND adjacency (within adjacencyThreshold)
    private bool CheckPairRules(Shape A, Shape B, float adjacencyThreshold)
    {
        // positions we'll compare & their distances
        float distFrontFront = Vector2.Distance(A.frontPoint.position, B.frontPoint.position);
        float distFrontBack  = Vector2.Distance(A.frontPoint.position, B.backPoint.position);
        float distBackFront  = Vector2.Distance(A.backPoint.position, B.frontPoint.position);
        float distBackBack   = Vector2.Distance(A.backPoint.position, B.backPoint.position);

        // compute boolean facing tests (directional only)
        bool A_faces_B_front = ShapeFacingRules.IsFacing(A.frontPoint, B.frontPoint);
        bool A_faces_B_back  = ShapeFacingRules.IsFacing(A.frontPoint, B.backPoint);
        bool B_faces_A_front = ShapeFacingRules.IsFacing(B.frontPoint, A.frontPoint);
        bool B_faces_A_back  = ShapeFacingRules.IsFacing(B.frontPoint, A.backPoint);

        // enforce rules only when the relevant faces are close enough (i.e. adjacent)
        // Smelly: A.back cannot have B.front adjacent -> check A.back vs B.front
        if (A.trait == Shape.ShapeTrait.Smelly)
        {
            // A back facing B front and positions are adjacent
            if (B_faces_A_back && distBackFront <= adjacencyThreshold) return false;
        }
        if (B.trait == Shape.ShapeTrait.Smelly)
        {
            if (A_faces_B_back && distFrontBack <= adjacencyThreshold) return false;
        }

        // Loud: A.front cannot be adjacent to B.front
        if (A.trait == Shape.ShapeTrait.Loud)
        {
            if (A_faces_B_front && distFrontFront <= adjacencyThreshold) return false;
        }
        if (B.trait == Shape.ShapeTrait.Loud)
        {
            if (B_faces_A_front && distFrontFront <= adjacencyThreshold) return false;
        }

        // BFF: both shapes must have front facing each other AND be adjacent
        if (A.trait == Shape.ShapeTrait.Bff && B.trait == Shape.ShapeTrait.Bff)
        {
            bool frontsFaceEachOther = (A_faces_B_front && B_faces_A_front);
            bool areFrontsAdjacent = distFrontFront <= adjacencyThreshold;
            if (!(frontsFaceEachOther && areFrontsAdjacent))
            {
                return false;
            }
        }

        return true;
    }
}