using System.Threading;
using UnityEngine;

public class PlanetGenerator : MonoBehaviour
{

    public int seed = 0;
    private bool generated = false, generating = false;
    private int w, h;
    private float roughness;
    private float[,] val;
    private System.Random rand;
    private Material mat;
    private Color[] c;
    private Texture2D t;

    private void Start()
    {
        mat = GetComponent<SpriteRenderer>().material;
        h = 128;
        w = h << 1;
    }

    private void Generate()
    {
        roughness = (float)rand.Next()/int.MaxValue*4f+1f;
        val = new float[w + 1, h + 1];
        val[0, 0] = val[0, h] = val[w, 0] = val[w, h] = rand.Next((int)(400f * roughness)) / 100f;
        val[h, 0] = val[h, h] = rand.Next((int)(400f * roughness)) / 100f;
        int l2, i0, j0;
        float k1 = 2f * roughness / h / 100f, k2 = 1.41f * roughness / h / 100f;
        float A, B, C, D;
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

                    A = j - l2 < 0 ? val[i, h - l] : val[i, j - l2];

                    B = val[i0, j0];

                    C = val[i, j];

                    D = val[i + l, j0];

                    val[i, j - l] = (A + B + C + D) / 4f + rand.Next(-l, l) * k2;
                }

            for (int i = l; i <= h; i += l2)
                val[w, i] = val[0, i];

            for (int i = l; i <= w; i += l2)
                val[i, h] = val[i, 0];
        }
        System.GC.Collect();
        float rand1 = (float)rand.Next()/int.MaxValue, rand2 = (float)rand.Next() / int.MaxValue;
        c = new Color[w * h];
        for (int i = 0; i < w; i++)
            for (int j = 0; j < h; j++)
            {
                float col = (val[i, j]);
                c[i + j * w] = new Color(col % 1f, (col + rand1) % 1f, (col + rand2) % 1f);
            }
        val = null;
        generated = true;
    }

    private void Update()
    {
        if (seed !=0 && !generating && System.GC.GetTotalMemory(false)<100000000L)
        {
            rand = new System.Random(seed);
            SpaceObject obj = GetComponent<SpaceObject>();
            obj.mass = rand.Next(100, 1000);
            obj.speed = new Vector2(rand.Next(-100, 100), rand.Next(-100, 100)) * 0.000037f;
            obj.angleSpeed = rand.Next(-100, 100)*0.01f;
            //obj.need = false;
            obj.type = SpaceObjectType.planet;

            #if UNITY_WEBGL

                Generate();

            #else

                (new Thread(Generate)).Start();
                
            #endif
            
            generating = true;
        }
        if (generated)
        {
            t = new Texture2D(w, h, TextureFormat.RGB24, false);
            t.SetPixels(c);
            c = null;
            t.Apply();
            
            mat.SetTexture("DiffuseMap", t);
            
            System.GC.Collect();

            Destroy(this);
        }
    }
    private void OnDestroy()
    {
        gameObject.SetActive(false);
    }
}