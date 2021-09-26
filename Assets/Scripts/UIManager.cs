using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public Image BlackoutImage;
    // Update is called once per frame
    void Start()
    {
        BlackoutImage.canvasRenderer.SetAlpha(0.003f);
        BlackoutImage.color = new Color(BlackoutImage.color.r, BlackoutImage.color.g, BlackoutImage.color.b, 1);
    }
    public void BlackFadein(float Opacity, float duration)
    {
        BlackoutImage.CrossFadeAlpha(Opacity, duration, true);
    }
    public void BlackFadeout(float duration)
    {
        BlackoutImage.CrossFadeAlpha(0.003f, duration, true);
    }
}