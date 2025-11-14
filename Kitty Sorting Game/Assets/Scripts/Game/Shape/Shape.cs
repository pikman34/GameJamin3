using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class Shape : MonoBehaviour, IPointerClickHandler, IPointerUpHandler, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerDownHandler
{
    public GameObject squareShapeImage;
    public Vector3 shapeSelectedScale;
    public Vector2 offset = new Vector2(0f, 700f);

    //[HideInInspector]
    public ShapeData CurrentShapeData;
    public ShapeStorage shapeStorage;
    public int TotalSquareNumber { get; set; }

    private List<GameObject> _currentShape = new List<GameObject>();
    public List<GridSquare> _occupiedSquares = new List<GridSquare>();
    private Vector3 _shapeStartScale;
    private RectTransform _transform;
    private bool _shapeDraggable = true;
    private Canvas _canvas;

    private bool _shapeActive = true;
    public bool isPlaced = false;
    

    public void Awake()
    {
        _shapeStartScale = this.GetComponent<RectTransform>().localScale;
        _transform = this.GetComponent<RectTransform>();
        _canvas = GetComponentInParent<Canvas>();
        _shapeDraggable = true;
        _shapeActive = true;
    }

    /*public bool IsOnStartPosition()
    {
        return _transform.localPosition == _startposition;
    }*/

    public bool IsAnyOfShapeSquareActive()
    {
        foreach (var square in _currentShape)
        {
            if (square.gameObject.activeSelf)
                return true;
        }
        return false;
    }

    public void DeactivateShape()
    {
        if (_shapeActive)
        {
            foreach (var square in _currentShape)
            {
                square.GetComponent<BoxCollider2D>().enabled = false;
                square.GetComponent<ShapeSquare>().DeactivateShape();
            }
        }

        _shapeActive = false;
    }

    public void ActivateShape()
    {
        if (!_shapeActive)
        {
            foreach (var square in _currentShape)
            {
                square.GetComponent<BoxCollider2D>().enabled = true;
                square.GetComponent<ShapeSquare>().ActivateShape();
            }
        }

        _shapeActive = true;
    }

    void Start()
    {
        RequestNewShape(CurrentShapeData);
    }

    public void RequestNewShape(ShapeData shapeData)
    {
        CreateShape(shapeData);
        //_transform.localScale = _startposition;
    }

    public void CreateShape(ShapeData shapeData)
    {
        CurrentShapeData = shapeData;
        TotalSquareNumber = GetNumberOfSquares(shapeData);

        while (_currentShape.Count <= TotalSquareNumber)
        {
            _currentShape.Add(Instantiate(squareShapeImage, transform) as GameObject);
        }

        foreach (var square in _currentShape)
        {
            square.gameObject.transform.position = Vector3.zero;
            square.gameObject.SetActive(false);
        }

        var squareRect = squareShapeImage.GetComponent<RectTransform>();
        var moveDistance = new Vector2(squareRect.rect.width * squareRect.localScale.x, squareRect.rect.height * squareRect.localScale.y);

        int currentIndexInList = 0;

        //set position to form final shape
        for (var row = 0; row < shapeData.rows; row++)
        {
            for (var column = 0; column < shapeData.columns; column++)
            {
                if (shapeData.board[row].column[column])
                {
                    _currentShape[currentIndexInList].SetActive(true);
                    _currentShape[currentIndexInList].GetComponent<RectTransform>().localPosition = new Vector2(GetXPositionForShapeSquare(shapeData, column, moveDistance), GetYPositionForShapeSquare(shapeData, row, moveDistance));

                    currentIndexInList++;
                }
            }
        }
    }

    private float GetYPositionForShapeSquare(ShapeData shapeData, int row, Vector2 moveDistance)
    {
        float shiftOnY = 0f;

        if (shapeData.rows > 1)
        {
            if (shapeData.rows % 2 != 0)
            {
                var middleSqaureIndex = (shapeData.rows - 1) / 2;
                var multiplier = (shapeData.rows - 1) / 2;

                if (row < middleSqaureIndex) //move it on minus 
                {
                    shiftOnY = moveDistance.y * 1;
                    shiftOnY *= multiplier;
                }
                else if (row > middleSqaureIndex) // move it on plus 
                {
                    shiftOnY = moveDistance.y * -1;
                    shiftOnY *= multiplier;
                }
            }
            else
            {
                var middleSqaureIndex2 = (shapeData.rows == 2) ? 1 : (shapeData.rows / 2);
                var middleSqaureIndex1 = (shapeData.rows == 2) ? 0 : shapeData.rows - 2;
                var multiplier = shapeData.rows / 2;

                if (row == middleSqaureIndex1 || row == middleSqaureIndex2)
                {
                    if (row == middleSqaureIndex2)
                    {
                        shiftOnY = (moveDistance.y / 2) * -1;
                    }
                    if (row == middleSqaureIndex1)
                    {
                        shiftOnY = (moveDistance.y / 2);
                    }
                }

                if (row < middleSqaureIndex1 && row < middleSqaureIndex2) //move it on minus 
                {
                    shiftOnY = moveDistance.y * 1;
                    shiftOnY *= multiplier;
                }
                else if (row > middleSqaureIndex1 && row > middleSqaureIndex2) //move it on plus 
                {
                    shiftOnY = moveDistance.y * -1;
                    shiftOnY *= multiplier;
                }
            }
        }

        return shiftOnY;
    }

    private float GetXPositionForShapeSquare(ShapeData shapeData, int column, Vector2 moveDistance)
    {
        float shiftOnX = 0f;

        if (shapeData.columns > 1)
        {
            if (shapeData.columns % 2 != 0)
            {
                var middleSquareIndex = (shapeData.columns - 1) / 2;
                var multiplier = (shapeData.columns - 1) / 2;
                if (column < middleSquareIndex)
                {
                    shiftOnX = moveDistance.x * -1;
                    shiftOnX *= multiplier;
                }
                else if (column > middleSquareIndex)
                {
                    shiftOnX = moveDistance.x * 1;
                    shiftOnX *= multiplier;
                }
            }
            else
            {
                var middleSquareIndex2 = (shapeData.columns == 2) ? 1 : (shapeData.columns / 2);
                var middleSquareIndex1 = (shapeData.columns == 2) ? 0 : shapeData.columns - 1;
                var multiplier = shapeData.columns / 2;

                if (column == middleSquareIndex1 || column == middleSquareIndex2)
                {
                    if (column == middleSquareIndex2)
                    {
                        shiftOnX = moveDistance.x / 2;
                    }
                    if (column == middleSquareIndex1)
                    {
                        shiftOnX = (moveDistance.x / 2) * -1;
                    }
                }

                if (column < middleSquareIndex1 && column < middleSquareIndex2) //move it on negative 
                {
                    shiftOnX = moveDistance.x * -1;
                    shiftOnX *= multiplier;
                }
                else if (column > middleSquareIndex1 && column > middleSquareIndex2) //move it on positive 
                {
                    shiftOnX = moveDistance.x * 1;
                    shiftOnX *= multiplier;
                }
            }
        }
        return shiftOnX;
    }


    private int GetNumberOfSquares(ShapeData shapeData)
    {
        int number = 0;

        foreach (var rowData in shapeData.board)
        {
            foreach (var active in rowData.column)
            {
                if (active)
                {
                    number++;
                }
            }
        }

        return number;
    }

    public void OnPointerClick(PointerEventData eventData)
    {

    }

    public void OnPointerUp(PointerEventData eventData)
    {
        
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        this.GetComponent<RectTransform>().localScale = shapeSelectedScale;
    }

    public void OnDrag(PointerEventData eventData)
    {
        _transform.anchorMin = new Vector2(0, 0);
        _transform.anchorMax = new Vector2(0, 0);
        _transform.pivot = new Vector2(0, 0);

        Vector2 pos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(_canvas.transform as RectTransform, eventData.position, Camera.main, out pos);
        _transform.localPosition = pos + offset;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        this.GetComponent<RectTransform>().localScale = _shapeStartScale;
        GameEvents.CheckIfShapeCanBePlaced();

        bool canPlace = true;

        foreach (var square in _currentShape)
        {
            var shapeSquare = square.GetComponent<ShapeSquare>();


            if (shapeSquare.currentGridSquare == null || shapeSquare.currentGridSquare.SquareOccupied)
            {
                canPlace = false;
                break;
            }

            _occupiedSquares.Add(shapeSquare.currentGridSquare);
        }

        if (canPlace)
        {
            var firstSquarePos = _occupiedSquares[0].transform.position;
            transform.position = firstSquarePos;

            foreach (var gridSquare in _occupiedSquares)
            {
                gridSquare.PlaceShapeOnBoard();
            }

            isPlaced = true;
        }
    }
    
    public void OnPointerDown(PointerEventData eventData)
    {
        shapeStorage.currentSelectedShape = this;
        
        if (isPlaced)
        {
            ActivateShape();
            isPlaced = false;
        }

        foreach (var gridSquare in _occupiedSquares)
        {
            gridSquare.SquareOccupied = false;
            gridSquare.Selected = false;
            gridSquare.activeImage.gameObject.SetActive(false);
        }

        _occupiedSquares.Clear();
    }
}
