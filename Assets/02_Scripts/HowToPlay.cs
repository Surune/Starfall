using UnityEngine;
using UnityEngine.UI;

public class HowToPlay : MonoBehaviour {
    public Image showingImage;
    public Sprite[] images;
    private int imageNum = 0;

    public void LeftClicked() { 
        if (imageNum > 0)
            showingImage.sprite = images[--imageNum];
    }

    public void RightClicked() {
        if (imageNum < images.Length-1)
            showingImage.sprite = images[++imageNum];
    }
}
