using UnityEngine;
using UnityEngine.UI;

public class Lens : MonoBehaviour
{
    public Double2 truePosition;

    public Shader shader;

    public float radius = 0, radius1 = 0;

    public BHScript BH;

    public AnotherBH BH1;

    public GameObject background;  //Объект, позиция которого берется за позицию черной дыры

    private float ratio;     //Отношение высоты к длине экрана, для правильного отображения шейдера

    private Camera cam;

    private Material material;// _material; //Материал на котором будет находится шейдер
    private Canvas canvas;

    private void Awake()
    {
        RectTransform rect = FindAnyObjectByType<RawImage>().GetComponent<RectTransform>();
        rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, Screen.width * 2f);
        rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, Screen.height * 2f);
        truePosition = BH.truePosition;
        material = background.GetComponent<SpriteRenderer>().material;
        cam = Camera.main;
        ratio = (float)Screen.height / Screen.width;
        material.SetFloat("_Rad", radius);
        material.SetFloat("_Ratio", ratio);
        canvas = FindAnyObjectByType<Canvas>();
    }

    private void FixedUpdate()
    {
        if(BH.mass!=0)
            truePosition += (BH.truePosition - truePosition) * 0.2f;
    }

    void Update()//RenderTexture source, RenderTexture destination)
    {
        radius = BH.radius;
        radius1 = BH1.radius;
        //Находим позицию черной дыры в экранных координатах
        Vector2 pos = new Vector2(
            cam.WorldToScreenPoint(BH.transform.position).x / cam.pixelWidth,
            cam.WorldToScreenPoint(BH.transform.position).y / cam.pixelHeight);
        Vector2 pos1 = new Vector2(
            cam.WorldToScreenPoint(BH1.transform.position).x / cam.pixelWidth,
            cam.WorldToScreenPoint(BH1.transform.position).y / cam.pixelHeight);

        //Устанавливаем все необходимые для шейдера параметры
        material.SetVector("_Position", pos);
        material.SetFloat("_Distance", Vector3.Distance(BH.transform.position, transform.position) );
        material.SetVector("pos", truePosition % new Vector2(2048f, 2048f)); // "-"
        material.SetFloat("_Rad", radius * radius / Game.sqrScale);
        material.SetVector("_Position1", pos1);
        material.SetFloat("_Distance1", Vector3.Distance(BH1.transform.position, transform.position) );
        material.SetFloat("_Rad1", radius1 * radius1 / Game.sqrScale);
    }
}