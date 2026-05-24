using UnityEngine;

public class MoveJoystick : MonoBehaviour
{

    private bool right;
    private RectTransform joystick;
    // Use this for initialization
    void Start()
    {
        #if UNITY_WEBGL

            if (!Application.isMobilePlatform)
                gameObject.SetActive(false);

        #elif !UNITY_ANDROID

            gameObject.SetActive(false);

        #endif

        if (PlayerPrefs.HasKey("handed"))
            right = PlayerPrefs.GetString("handed") == "right";
        else
            right = true;
        joystick = FindAnyObjectByType<Joystick>().GetComponent<RectTransform>();
        joystick.anchorMax = joystick.anchorMin = new Vector2(right ? 1f : 0f, 0f);
        joystick.anchoredPosition = new Vector2(right ? -150f : 150f, 150f);
        
        float scaleRatio = Screen.height / 600f;
        joystick.anchoredPosition *= scaleRatio;
    }

    public void Move()
    {
        right = !right;
        joystick.anchorMax = joystick.anchorMin = new Vector2(right ? 1f : 0f, 0f);
        joystick.anchoredPosition *= new Vector2(-1f, 1f);
        PlayerPrefs.SetString("handed", right ? "right" : "left");
    }

}