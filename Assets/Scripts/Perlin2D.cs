using UnityEngine;

public class Perlin2D
{

    public int a, b, c, d, e;
    private System.Random rand;

    public Perlin2D(int seed)
    {
        rand = new System.Random(seed);
        a = nextSimple(rand.Next(400, 1200));
        b = nextSimple(rand.Next(7, 19));
        c = nextSimple(rand.Next(4000, 17000));
        d = nextSimple(rand.Next(200000, 900000));
        e = nextSimple(rand.Next(400000000, 1500000000));
    }

    private int nextSimple(int a)
    {
        bool simple = false;
        while (!simple)
        {
            simple = true;
            for (int i = 2; i <= Mathf.Sqrt(a); i++)
                if (a % i == 0)
                {
                    simple = false;
                    a++;
                    break;
                }
        }
        return a;
    }

    public float Noise(int x, int y)
    {
        int v = x + y * a;
        v = (v << b) ^ v;
        return ((v * (v * v * c + d) + e) & 2147483647) / 2147483647f;
    }

    private float QunticCurve(float t)
    {
        return t * t * t * (t * (t * 6f - 15f) + 10f);
    }

    private float Dot(float[] a, float[] b)
    {
        return a[0] * b[0] + a[1] * b[1];
    }

    private float Lerp(float a, float b, float t)
    {
        return Mathf.Lerp(a, b, t);
    }

    private float SmoothedNoise(int x, int y)
    {
        float corners = (Noise(x - 1, y - 1) + Noise(x + 1, y - 1) + Noise(x - 1, y + 1) + Noise(x + 1, y + 1)) / 16;
        float sides = (Noise(x - 1, y) + Noise(x + 1, y) + Noise(x, y - 1) + Noise(x, y + 1)) / 8;
        float center = Noise(x, y) / 4;
        return corners + sides + center;
    }

    private float InterpolatedNoise(float x, float y)
    {
        int integer_X = (int)x;
        float fractional_X = x - integer_X;
        int integer_Y = (int)y;
        float fractional_Y = y - integer_Y;
        float v1 = SmoothedNoise(integer_X, integer_Y);
        float v2 = SmoothedNoise(integer_X + 1, integer_Y);
        float v3 = SmoothedNoise(integer_X, integer_Y + 1);
        float v4 = SmoothedNoise(integer_X + 1, integer_Y + 1);
        float i1 = Lerp(v1, v2, fractional_X);
        float i2 = Lerp(v3, v4, fractional_X);
        return Lerp(i1, i2, fractional_Y);
    }

    public float PerlinNoise(float x, float y, float Number_Of_Octaves, float persistence)
    {
        float total = 0;
        float frequency = 1;
        float amplitude = 1;
        float maxValue = 0;  // Used for normalizing result to 0.0 - 1.0
        for (int i = 0; i < Number_Of_Octaves; i++)
        {
            total += InterpolatedNoise(x * frequency, y * frequency) * amplitude;

            maxValue += amplitude;

            amplitude *= persistence;
            frequency *= 2;
        }

        return total / maxValue;
    }

}