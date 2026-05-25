using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class BHScript : MonoBehaviour {
    
    public float mass, radius;

    public Vector2 speed = new Vector2();

    public Double2 truePosition;

    public GameObject text;

    public GameObject[] obj;

    //private bool PC;

    private float maxMass;

    private Vector2 wh, speed1 = new Vector2();

    private Lens lens;

    private SpriteRenderer rend;

    // Use this for initialization
    void Awake () {
        //PC = Application.platform != RuntimePlatform.Android && Application.platform != RuntimePlatform.IPhonePlayer;
        radius = Mathf.Pow(mass * 0.000005f, 0.333333333f);
        lens = FindAnyObjectByType<Lens>();
        rend = FindAnyObjectByType<BackgroundGenerator>().GetComponent<SpriteRenderer>();
	}
    private void FixedUpdate()
    {
        /*if(PC)
            if(Input.GetAxis("Horizontal")!=0 || 0!=Input.GetAxis("Vertical"))
                speed1 = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")).normalized*0.02f*Game.scale;
        if(Input.GetMouseButton(0))
            speed1 = ((Vector2)Input.mousePosition - wh).normalized * 0.02f * Game.scale;
            */
        //float m = speed.magnitude, s = 0.2f * Game.scale;
        //if (m > s)
        //    speed *= s / m;

        if (mass != 0)
            truePosition += speed += (speed1 - speed) * 0.1f;
        speed1 = new Vector2();

    }
    // Update is called once per frame
    void CheckPCInput()
    {
        if (Mouse.current.leftButton.isPressed)
        {
            wh = new Vector2(Screen.width, Screen.height) * 0.5f;
            Vector2 mouseDelta = Mouse.current.delta.ReadValue();
            Vector2 d = Mouse.current.position.ReadValue() - wh;
            OnJoystickDrag(Mathf.Atan2(d.y, d.x), Mathf.Min(1f, 2f * d.magnitude / wh.y) );
            return;
        }

        float up = Keyboard.current.wKey.ReadValue();
        float left = Keyboard.current.aKey.ReadValue();
        float down = Keyboard.current.sKey.ReadValue();
        float right = Keyboard.current.dKey.ReadValue();
        if (up != 0 || left != 0 || down != 0 || right != 0)
        {
            OnJoystickDrag(Mathf.Atan2(up - down, right - left), 1f);
        }
    }
    void Update ()
    {
        #if UNITY_WEBGL

            if (!Application.isMobilePlatform)
                CheckPCInput();

        #elif !UNITY_ANDROID

            CheckPCInput();

        #endif

        if (mass == 0f != text.activeSelf)
        {
            text.GetComponent<Text>().text = "You fell into bigger black hole.\nYour mass: " + maxMass.ToString("0");
            text.SetActive(mass == 0f);
        }
        radius = Mathf.Pow(mass * 0.000005f, 0.333333333f);

        if (!double.IsNaN(truePosition.x) && !double.IsNaN(truePosition.y))
            transform.position = (truePosition - lens.truePosition)/Game.scale*2f;
        if (mass == 0)
        {
            if (Input.GetMouseButtonDown(0))
            {
                mass = 40;
                truePosition = new Double2();
                FindAnyObjectByType<Game>().Restart();
            }
        }
        else
            maxMass = mass;
    }
    public void OnJoystickDrag(float angle, float value)
    {
        speed1 = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle))*value * 0.3f * Game.scale;
    }
}
