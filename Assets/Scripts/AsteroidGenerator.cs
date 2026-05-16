using System.Threading;
using UnityEngine;

public class AsteroidGenerator : MonoBehaviour
{

    public int seed = 0;
    private bool generated = false, generating = false;
    private int w, h;
    private float roughness;
    private float[,] val;
    private System.Random rand;
    private Color[] c;
    private Texture2D t;

    private void Start()
    {
        h = w = 128;
    }

    private void Generate()
    {
        roughness = rand.Next() * 8f / int.MaxValue + 5f;
        val = new float[w + 1, h + 1];
        val[0, 0] = val[0, h] = val[w, 0] = val[w, h] = rand.Next((int)(400f * roughness)) / 100f;
        float max = float.MinValue, min = float.MaxValue;
        int l2, i0, j0;
        float k1 = 2f * roughness / h / 100f, k2 = 1.41f * roughness / h / 100f;
        float A, B, C, D;
        //System.GC.Collect();
        for (int l = h >> 1; l != 0; l >>= 1)
        {
            l2 = l << 1;
            for (int i = l; i <= w; i += l2)
                for (int j = l; j <= h; j += l2)
                {
                    A = val[i - l, j - l];
                    B = val[i + l, j - l];
                    C = val[i - l, j + l];
                    D = val[i + l, j + l];
                    val[i, j] = (A + B + C + D) / 4f + rand.Next(-l * 100, l * 100) * k1;
                    if (min > val[i, j])
                        min = val[i, j];
                    if (max < val[i, j])
                        max = val[i, j];
                }
            for (int i = l; i <= w; i += l2)
                for (int j = l; j <= h; j += l2)
                {
                    i0 = i - l;
                    j0 = j - l;

                    A = i - l2 < 0 ? val[w - l, j] : val[i - l2, j];

                    B = val[i0, j0];

                    C = val[i, j];

                    D = val[i0, j + l];

                    val[i - l, j] = (A + B + C + D) / 4f + rand.Next(-l, l) * k2;
                    if (min > val[i0, j])
                        min = val[i0, j];
                    if (max < val[i0, j])
                        max = val[i0, j];

                    A = j - l2 < 0 ? val[i, h - l] : val[i, j - l2];

                    B = val[i0, j0];

                    C = val[i, j];

                    D = val[i + l, j0];

                    val[i, j - l] = (A + B + C + D) / 4f + rand.Next(-l, l) * k2;
                    if (min > val[i, j0])
                        min = val[i, j0];
                    if (max < val[i, j0])
                        max = val[i, j0];
                }

            for (int i = l; i <= h; i += l2)
                val[w, i] = val[0, i];

            for (int i = l; i <= w; i += l2)
                val[i, h] = val[i, 0];
        }
        System.GC.Collect();
        float rand1 = Mathf.Pow(2f, rand.Next(-200, 200) / 100f), rand2 = Mathf.Pow(2f, rand.Next(-200, 200) / 100f), rand3 = Mathf.Pow(2f, rand.Next(-200, 200) / 100f);

        float delta = max - min;
        c = new Color[w * h];
        for (int i = 0; i < w; i++)
            for (int j = 0; j < h; j++)
            {
                float col = ((val[i, j]) - min) / delta;
                float col1 = Mathf.Max(0f, col - (new Vector2(i - w * 0.5f, j - w * 0.5f)).magnitude / w * 2f + 0.5f);
                c[i + j * w] = new Color(col * rand1, col * rand2, col * rand3, col1 > 0.5f ? 1f : 0f);
            }
        val = null;
        //System.GC.Collect();
        generated = true;
    }

    private void Update()
    {
        if (seed !=0 && !generating && System.GC.GetTotalMemory(false) < 50000000L)
        {
            rand = new System.Random(seed);
            SpaceObject obj = GetComponent<SpaceObject>();
            obj.mass = rand.Next(10, 40);
            obj.speed = new Vector2(rand.Next(-100, 100), rand.Next(-100, 100)) * 0.000018f;
            obj.angleSpeed = rand.Next(-5, 5);
            //obj.need = true;
            obj.type = SpaceObjectType.asteroid;
            (new Thread(Generate)).Start();
            generating = true;
        }
        if (generated)
        {
            t = new Texture2D(w, h);
            t.filterMode = FilterMode.Point;
            t.SetPixels(c);
            c = null;
            t.Apply();

            GetComponent<SpriteRenderer>().sprite = Sprite.Create(t, new Rect(0, 0, t.width, t.height), new Vector2(0.5f, 0.5f));
            
            System.GC.Collect();

            Destroy(this);
        }
    }
}