using UnityEngine;
using UnityEngine.UI;
//[ExecuteInEditMode]
public class Spagetti : MonoBehaviour
{
    public Shader shader;

    public GameObject BH, BH1;  //Объект, позиция которого берется за позицию черной дыры

    private float radius = 0, radius1 = 0, ratio = 1;     //Отношение высоты к длине экрана, для правильного отображения шейдера

    private Lens lens;

    private Material _material; //Материал на котором будет находится шейдер

    private Camera cam, cam1;
    private Canvas canvas;

    private void Awake()
    {
        ratio = (float)Screen.height / Screen.width;
        cam = Camera.main;
        cam1 = GetComponent<Camera>();
        RenderTexture rendt = new RenderTexture(Screen.width, Screen.height, 0);// (int)(cam.orthographicSize/ratio*200f), (int)(cam.orthographicSize*200f), 0);
        GetComponent<Camera>().targetTexture = rendt;
        FindAnyObjectByType<RawImage>().texture = rendt;
        lens = FindAnyObjectByType<Lens>();
        material.SetFloat("_Ratio", ratio);
        canvas = FindAnyObjectByType<Canvas>();
    }

    protected Material material
    {
        get
        {
            if (_material == null)
            {
                _material = new Material(shader);
                _material.hideFlags = HideFlags.HideAndDontSave;
            }
            return _material;
        }
    }

    protected virtual void OnDisable()
    {
        if (_material)
        {
            DestroyImmediate(_material);
        }
    }

    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (shader && material)
        {
            radius = lens.radius / 2f;
            radius1 = lens.radius1 / 2f;
            cam1.orthographicSize = 5f * Game.scale;
            //Находим позицию черной дыры в экранных координатах
            Vector2 pos = new Vector2(
                cam.WorldToScreenPoint(BH.transform.position / 2f).x / cam.pixelWidth,
                cam.WorldToScreenPoint(BH.transform.position / 2f).y / cam.pixelHeight);
            Vector2 pos1 = new Vector2(
                cam.WorldToScreenPoint(BH1.transform.position / 2f).x / cam.pixelWidth,
                cam.WorldToScreenPoint(BH1.transform.position / 2f).y / cam.pixelHeight);

            //Устанавливаем все необходимые для шейдера параметры
            material.SetVector("_Position", pos);
            material.SetFloat("_Rad", radius * radius / Game.sqrScale);
            material.SetFloat("_Distance", Vector3.Distance(BH.transform.position, transform.position) );
            material.SetVector("_Position1", pos1);
            material.SetFloat("_Distance1", Vector3.Distance(BH1.transform.position, transform.position) );
            material.SetFloat("_Rad1", radius1 * radius1 / Game.sqrScale);
            //И применяем к полученному изображению.
            Graphics.Blit(source, destination, material);
        }
    }
}