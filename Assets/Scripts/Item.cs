using UnityEngine;
using UnityEngine.UI;

public class Item : MonoBehaviour
{
    public enum Class
    {
        A,
        B,
        C,
        D,
        E,
        F,
        G,
        H,
        I,
        J,
        K,
        L,
        M,
        N,
        O,
        P,
        Q,
        R,
        S,
        T,
        U,
        V,
        W,
        X,
        Y,
        Z,
        Length
    }

    public enum TextSize
    {
        Small = 12,
        Big = 24
    }

    // Editor fields

    public Class currentClass = Class.A;
    public TextSize classTextSize = TextSize.Big;
    public Text classText;

    public Color correctClassColor = new Color(81 / 255f, 229 / 255f, 81 / 255f);
    public Color incorrectClassColor = new Color(226 / 255f, 84 / 255f, 83 / 255f);
    public Image background;

    // Methods

    public void UpdateInfo(Color classColor)
    {
        classText.fontSize = (int)classTextSize;
        classText.text = new string((char)((int)currentClass + 65), 1);

        background.color = classColor;
    }

    protected void OnValidate()
    {
        if (classText != null && background != null)
        {
            UpdateInfo(correctClassColor);
        }
    }
}
