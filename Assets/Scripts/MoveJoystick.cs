using UnityEngine;

public class MoveJoystick : MonoBehaviour
{

    private bool right;
    private RectTransform joystick;
    // Use this for initialization
    void Start()
    {
        if (PlayerPrefs.HasKey("handed"))
            right = PlayerPrefs.GetString("handed") == "right";
        else
            right = true;
        joystick = FindObjectOfType<Joystick>().GetComponent<RectTransform>();
        joystick.anchorMax = joystick.anchorMin = new Vector2(right ? 1f : 0f, 0f);
        joystick.anchoredPosition = new Vector2(right ? -110f : 110f, 110f);
    }

    public void Move()
    {
        right = !right;
        joystick.anchorMax = joystick.anchorMin = new Vector2(right ? 1f : 0f, 0f);
        joystick.anchoredPosition = new Vector2(right ? -110f : 110f, 110f);
        PlayerPrefs.SetString("handed", right ? "right" : "left");
    }

}