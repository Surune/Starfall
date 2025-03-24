using UnityEngine;
using UnityEngine.UI;

public class HowToPlay : MonoBehaviour
{
    public Image ShowingImage;
    public Sprite[] Images;
    int imageNum = 0;

    public void LeftClicked()
    {
        if (imageNum > 0)
        {
            ShowingImage.sprite = Images[--imageNum];
        }
    }

    public void RightClicked()
    {
        if (imageNum < Images.Length - 1)
        {
            ShowingImage.sprite = Images[++imageNum];
        }
    }
}
