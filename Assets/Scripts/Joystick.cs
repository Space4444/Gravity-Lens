using UnityEngine;
using System;
using UnityEngine.EventSystems;

public class Joystick : MonoBehaviour
{

    public Transform control;
    private bool holding = false;
    private int fingerId;
    private float angle, value, width, k;
    private Vector2 tapPos, halfScreenSize;
    [Serializable]
    public class MethodContainer : UnityEngine.Events.UnityEvent<float, float> { };
    [Header("First is angle, second is value from 0 to 1")]
    public MethodContainer OnDrag;

    private void Start()
    {
        #if UNITY_WEBGL

            if (!Application.isMobilePlatform)
                gameObject.SetActive(false);

        #elif !UNITY_ANDROID

            gameObject.SetActive(false);

        #endif

        RectTransform transform = GetComponent<RectTransform>();

        float scaleRatio = Screen.height / 600f;
        transform.anchoredPosition *= scaleRatio;
        transform.localScale *= scaleRatio;
        
        k = Camera.main.orthographicSize / Screen.height * 2f;
        width = transform.rect.width * 0.5f;
        EventTrigger.Entry down = new EventTrigger.Entry(), drag = new EventTrigger.Entry(), up = new EventTrigger.Entry();
        down.eventID = EventTriggerType.PointerDown;
        down.callback.AddListener(OnPointerDown);
        gameObject.AddComponent<EventTrigger>().triggers.Add(down);
        drag.eventID = EventTriggerType.Drag;
        drag.callback.AddListener(OnPointerDrag);
        GetComponent<EventTrigger>().triggers.Add(drag);
        up.eventID = EventTriggerType.PointerUp;
        up.callback.AddListener(OnPointerUp);
        GetComponent<EventTrigger>().triggers.Add(up);
    }

    public void OnPointerDown(BaseEventData data)
    {
        holding = true;
        tapPos = (data as PointerEventData).position;
    }

    public void OnPointerDrag(BaseEventData data)
    {
        tapPos = (data as PointerEventData).position;
    }

    public void OnPointerUp(BaseEventData data)
    {
        holding = false;
        value = 0;
        control.localPosition = Vector3.zero;
    }

    private void Update()
    {
        if (holding)
        {
            k = Camera.main.orthographicSize / Screen.height * 2f;
            halfScreenSize = new Vector2(Screen.width, Screen.height)*0.5f;
            Vector2 d = tapPos - halfScreenSize - (Vector2)transform.position/k;
            float c = d.magnitude;
            value = c > width ? 1f : c / width;
            angle = Mathf.Atan2(d.y, d.x);
            control.localPosition = value * width * new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
            OnDrag.Invoke(angle, value);
        }
    }

}