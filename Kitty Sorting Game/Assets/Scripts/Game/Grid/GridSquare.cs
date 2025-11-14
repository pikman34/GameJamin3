using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GridSquare : MonoBehaviour
{
    public Image hoverImage;
    public Image activeImage;
    public Image normalImage;
    public List<Sprite> normalImages;
    public bool Selected { get; set; }
    public int SquareIndex { get; set; }
    public bool SquareOccupied { get; set; }

    void Start()
    {
        Selected = false;
        SquareOccupied = false;
    }

    //temp function
    public bool CanWeUseThisSquare()
    {
        return hoverImage.gameObject.activeSelf;
    }

    public void PlaceShapeOnBoard()
    {
        ActivateSquare();
    }

    public void ActivateSquare()
    {
        hoverImage.gameObject.SetActive(false);
        activeImage.gameObject.SetActive(true);
        Selected = true;
        SquareOccupied = true;
    }

    public void SetImage(bool setFirstImage)
    {
        normalImage.GetComponent<Image>().sprite = setFirstImage ? normalImages[1] : normalImages[0];
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        var shapeSquare = collision.GetComponent<ShapeSquare>();
        if (shapeSquare == null) return;

        hoverImage.gameObject.SetActive(true);
        shapeSquare.currentGridSquare = this;

        UpdateShapeSquareHover(shapeSquare);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        var shapeSquare = collision.GetComponent<ShapeSquare>();
        if (shapeSquare == null) return;

        hoverImage.gameObject.SetActive(true);
        shapeSquare.currentGridSquare = this;

        UpdateShapeSquareHover(shapeSquare);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        var shapeSquare = collision.GetComponent<ShapeSquare>();
        if (shapeSquare == null)
        {
            return;
        }

        hoverImage.gameObject.SetActive(false);

         if (shapeSquare.currentGridSquare == this)
        {
            shapeSquare.currentGridSquare = null;
        }
        shapeSquare.RemoveHoveredSquare(this);
        shapeSquare.UnSetOccupiedImage();
    }
    
    private void UpdateShapeSquareHover(ShapeSquare shapeSquare)
    {
        if (SquareOccupied)
        {
            shapeSquare.SetOccupiedImage();
        }
        else
        {
            shapeSquare.UnSetOccupiedImage();
        }
    }
}
