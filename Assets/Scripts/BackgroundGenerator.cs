using UnityEngine;
using System.Threading;
using System.IO;

public class BackgroundGenerator : MonoBehaviour
{

    private bool generatingStarted = false, generated = false, tGenerated;
    private int w, h, w0, h0, w1, h1, W = 2048;
    private float roughness, orthographicSize;
    private float[,] val;
    private System.Random rand;
    private Perlin2D p;
    private Color32[] c;
    private Texture2D t;
    private Sprite s;
    private SpriteRenderer rend;
    // Use this for initialization
    void Start()
    {
        orthographicSize = Camera.main.orthographicSize;
        h = (int)(orthographicSize * 200f);
        w = (int)(orthographicSize * Screen.width / Screen.height * 200f);
        rend = GetComponent<SpriteRenderer>();
        rend.sprite = Sprite.Create(new Texture2D(w,h), new Rect(0, 0, w, h), new Vector2(0.5f, 0.5f));
        t = new Texture2D(W, W, TextureFormat.RGB24, false);
    }
    void Update()
    {
            if (!generatingStarted)
            {
                generatingStarted = true;
                rand = new System.Random(Game.seed);

                if (tGenerated = (File.Exists(Path.Combine(Application.persistentDataPath, "t.jpg")) && File.Exists(Path.Combine(Application.persistentDataPath, "seed.txt"))))
                    if (File.ReadAllText(Path.Combine(Application.persistentDataPath, "seed.txt")) == Game.seed.ToString())
                        t.LoadImage(File.ReadAllBytes(Path.Combine(Application.persistentDataPath, "t.jpg")));
                    else
                        tGenerated = false;

                if (tGenerated)
                    generated = true;
                else
                {
                    Thread th = new Thread(Generate);
                    th.Start();
                }
            }
            if (generated)
            {
                if (!tGenerated)
                {
                    t.SetPixels32(c);
                    t.Apply();
                    File.WriteAllBytes(Path.Combine(Application.persistentDataPath, "t.jpg"), t.EncodeToJPG());
                    File.WriteAllText(Path.Combine(Application.persistentDataPath, "seed.txt"), Game.seed.ToString());
                }
                rend.material.SetTexture("_BackTex", t);
                rend.material.SetVector("scrSize", new Vector2(Screen.width, Screen.height));
                rend.material.SetVector("size2", new Vector2(w, h));

                c = null;

                Destroy(this);
            }
    }
    private void Generate()
    {
        int a, b;
        roughness = rand.Next() * 15f / int.MaxValue + 2f;
        val = new float[W + 1, W + 1];
        val[0, 0] = val[0, W] = val[W, 0] = val[W, W] = rand.Next((int)(400f * roughness)) / 100f;
        float max = float.MinValue, min = float.MaxValue;
        int l2, i0, j0;
        float k1 = 2f * roughness / W / 100f, k2 = 1.41f * roughness / W / 100f;
        float M, N, K, P;
        for (int l = W >> 1; l != 0; l >>= 1)
        {
            l2 = l << 1;
            for (int i = l; i <= W; i += l2)
                for (int j = l; j <= W; j += l2)
                {
                    M = val[i - l, j - l];
                    N = val[i + l, j - l];
                    K = val[i - l, j + l];
                    P = val[i + l, j + l];
                    val[i, j] = (M + N + K + P) / 4f + rand.Next(-l * 100, l * 100) * k1;
                    if (min > val[i, j])
                        min = val[i, j];
                    if (max < val[i, j])
                        max = val[i, j];
                }
            for (int i = l; i <= W; i += l2)
                for (int j = l; j <= W; j += l2)
                {
                    i0 = i - l;
                    j0 = j - l;

                    M = i - l2 < 0 ? val[W - l, j] : val[i - l2, j];

                    N = val[i0, j0];

                    K = val[i, j];

                    P = val[i0, j + l];

                    val[i - l, j] = (M + N + K + P) / 4f + rand.Next(-l, l) * k2;
                    if (min > val[i0, j])
                        min = val[i0, j];
                    if (max < val[i0, j])
                        max = val[i0, j];

                    M = j - l2 < 0 ? val[i, W - l] : val[i, j - l2];

                    N = val[i0, j0];

                    K = val[i, j];

                    P = val[i + l, j0];

                    val[i, j - l] = (M + N + K + P) / 4f + rand.Next(-l, l) * k2;
                    if (min > val[i, j0])
                        min = val[i, j0];
                    if (max < val[i, j0])
                        max = val[i, j0];
                }

            for (int i = l; i <= W; i += l2)
                val[W, i] = val[0, i];

            for (int i = l; i <= W; i += l2)
                val[i, W] = val[i, 0];
        }
        System.GC.Collect();
        float rand1 = Mathf.Pow(2f, rand.Next(-100, 100) / 100f), rand2 = Mathf.Pow(2f, rand.Next(-100, 100) / 100f), rand3;
        do
            rand3 = Mathf.Pow(2f, rand.Next(-100, 100) / 100f);
        while (Mathf.Abs(rand1 - rand2) < 0.5f && Mathf.Abs(rand3 - rand2) < 0.5f && Mathf.Abs(rand3 - rand1) < 0.5f);
        float delta = max - min;
        c = new Color32[W * W];
        for (int i = 0; i < W; i++)
            for (int j = 0; j < W; j++)
            {
                float col = Mathf.Pow(((val[i, j]) - min) / delta, 4f)+0.1f;
                c[i + j * W] = new Color(col * rand1, col * rand2, col * rand3) / 1.5f;
            }
        val = null;

        #region placing stars

        int R, G, B, random = rand.Next(5999, 10999);
        for (int i = 0; i < random; i++)
        {
            a = rand.Next(0, W);
            b = rand.Next(0, W);
            R = rand.Next(0, 256);
            int j = a + b * W;
            c[j] = new Color32((byte)(c[j].r + R), (byte)(c[j].g + R), (byte)(c[j].b + R), 255);
        }
        random = rand.Next(222, 345);
        for (int i = 0; i < random; i++)
        {
            int size = rand.Next(1, 10) * 2;
            a = size * 2 + 1 + rand.Next(0, W - size * 4 - 1);
            b = size * 2 + 1 + rand.Next(0, W - size * 4 - 1);
            R = rand.Next(151, 256);
            G = rand.Next(151, 256);
            B = rand.Next(151, 256);
            if (rand.Next(2) == 1)
                for (int j = 1; j < size; j++)
                {
                    float k = (float)(255 - j * 255 / size) / 511;
                    int cr = (int)(c[a + j + b * W].r + R * k);
                    int cg = (int)(c[a + j + b * W].g + G * k);
                    int cb = (int)(c[a + j + b * W].b + B * k);
                    if (cr > 255)
                        cr = 255;
                    if (cg > 255)
                        cg = 255;
                    if (cb > 255)
                        cb = 255;
                    c[a + j + b * W] = new Color32((byte)cr, (byte)cg, (byte)cb, 255);
                    cr = (int)(c[a - j + b * W].r + R * k);
                    cg = (int)(c[a - j + b * W].g + G * k);
                    cb = (int)(c[a - j + b * W].b + B * k);
                    if (cr > 255)
                        cr = 255;
                    if (cg > 255)
                        cg = 255;
                    if (cb > 255)
                        cb = 255;
                    c[a - j + b * W] = new Color32((byte)cr, (byte)cg, (byte)cb, 255);
                    cr = (int)(c[a + (b + j) * W].r + R * k);
                    cg = (int)(c[a + (b + j) * W].g + G * k);
                    cb = (int)(c[a + (b + j) * W].b + B * k);
                    if (cr > 255)
                        cr = 255;
                    if (cg > 255)
                        cg = 255;
                    if (cb > 255)
                        cb = 255;
                    c[a + (b + j) * W] = new Color32((byte)cr, (byte)cg, (byte)cb, 255);
                    cr = (int)(c[a + (b - j) * W].r + R * k);
                    cg = (int)(c[a + (b - j) * W].g + G * k);
                    cb = (int)(c[a + (b - j) * W].b + B * k);
                    if (cr > 255)
                        cr = 255;
                    if (cg > 255)
                        cg = 255;
                    if (cb > 255)
                        cb = 255;
                    c[a + (b - j) * W] = new Color32((byte)cr, (byte)cg, (byte)cb, 255);
                }
            else
                for (int j = 1; j <= size / Mathf.Sqrt(2); j++)
                {
                    float k = (float)(255 - j * 360 / size) / 511;
                    int cr = (int)(c[a + j + (b + j) * W].r + R * k);
                    int cg = (int)(c[a + j + (b + j) * W].g + G * k);
                    int cb = (int)(c[a + j + (b + j) * W].b + B * k);
                    if (cr > 255)
                        cr = 255;
                    if (cg > 255)
                        cg = 255;
                    if (cb > 255)
                        cb = 255;
                    c[a + j + (b + j) * W] = new Color32((byte)cr, (byte)cg, (byte)cb, 255);
                    cr = (int)(c[a + j + (b - j) * W].r + R * k);
                    cg = (int)(c[a + j + (b - j) * W].g + G * k);
                    cb = (int)(c[a + j + (b - j) * W].b + B * k);
                    if (cr > 255)
                        cr = 255;
                    if (cg > 255)
                        cg = 255;
                    if (cb > 255)
                        cb = 255;
                    c[a + j + (b - j) * W] = new Color32((byte)cr, (byte)cg, (byte)cb, 255);
                    cr = (int)(c[a - j + (b + j) * W].r + R * k);
                    cg = (int)(c[a - j + (b + j) * W].g + G * k);
                    cb = (int)(c[a - j + (b + j) * W].b + B * k);
                    if (cr > 255)
                        cr = 255;
                    if (cg > 255)
                        cg = 255;
                    if (cb > 255)
                        cb = 255;
                    c[a - j + (b + j) * W] = new Color32((byte)cr, (byte)cg, (byte)cb, 255);
                    cr = (int)(c[a - j + (b - j) * W].r + R * k);
                    cg = (int)(c[a - j + (b - j) * W].g + G * k);
                    cb = (int)(c[a - j + (b - j) * W].b + B * k);
                    if (cr > 255)
                        cr = 255;
                    if (cg > 255)
                        cg = 255;
                    if (cb > 255)
                        cb = 255;
                    c[a - j + (b - j) * W] = new Color32((byte)cr, (byte)cg, (byte)cb, 255);
                }
            /////////////////////////////////////////////////////////////////////////////////
            for (int j = a - size; j <= a + size; j++)
                for (int k = b - size; k <= b + size; k++)
                {
                    if (Mathf.Pow((j - a), 2) + Mathf.Pow((k - b), 2) < size * size)
                    {
                        double C = Mathf.Sqrt((j - a) * (j - a) + (k - b) * (k - b));
                        int cr = (int)(c[j + k * W].r + R * size / 8 / C - 32);
                        int cg = (int)(c[j + k * W].g + G * size / 8 / C - 32);
                        int cb = (int)(c[j + k * W].b + B * size / 8 / C - 32);
                        if (cr > 255)
                            cr = 255;
                        if (cr < c[j + k * W].r)
                            cr = c[j + k * W].r;
                        if (cg > 255)
                            cg = 255;
                        if (cg < c[j + k * W].g)
                            cg = c[j + k * W].g;
                        if (cb > 255)
                            cb = 255;
                        if (cb < c[j + k * W].b)
                            cb = c[j + k * W].b;
                        c[j + k * W] = new Color32((byte)cr, (byte)cg, (byte)cb, 255);
                    }
                }
            c[a + b * W] = Color.white;
        }

        #endregion

        generated = true;
    }

}