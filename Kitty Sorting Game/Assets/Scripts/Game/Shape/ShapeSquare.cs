using UnityEngine;
using UnityEngine.UI;
public class ShapeSquare : MonoBehaviour
{
    public Image occupiedImage;
    public Image NormalImage;

    void Start()
    {
        occupiedImage.gameObject.SetActive(false);
    }

    public void DeactivateShape()
    {
        NormalImage.gameObject.SetActive(false);
    }
}
