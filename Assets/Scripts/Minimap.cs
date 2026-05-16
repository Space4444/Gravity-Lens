using System.Collections.Generic;
using UnityEngine;

public class Minimap : MonoBehaviour
{
    public List<Transform> asteroids = new List<Transform>(), planets = new List<Transform>(), stars = new List<Transform>();

    private BHScript BH;
    private AnotherBH BH1;

    private float r = 10f, r1 = 11f, r2 = 12f, r3 = 13f, R = 150f;

    static Material Material;
    static void CreateLineMaterial()
    {
        if (!Material)
        {
            // Unity has a built-in shader that is useful for drawing
            // simple colored things.
            Shader shader = Shader.Find("Hidden/Internal-Colored");
            Material = new Material(shader);
            Material.hideFlags = HideFlags.HideAndDontSave;
            // Turn on alpha blending
            Material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            Material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            // Turn backface culling off
            Material.SetInt("_Cull", (int)UnityEngine.Rendering.CullMode.Off);
            // Turn off depth writes
            Material.SetInt("_ZWrite", 0);
        }
    }

    private void Start()
    {
        R = Mathf.Min(Screen.width, Screen.height) * 0.5f - 10f;
        BH = FindObjectOfType<BHScript>();
        BH1 = FindObjectOfType<AnotherBH>();
        CreateLineMaterial();
        // Apply the line material
    }

    // Will be called after all regular rendering is done
    public void OnRenderObject()
    {
        Material.SetPass(0);

        GL.PushMatrix();
        // Set transformation matrix for drawing to
        // match our transform
        GL.MultMatrix(transform.localToWorldMatrix);

        int count = asteroids.Count;
        for (int i = 0; i < count; i++)
        {
            Vector2 delta = (Vector2)asteroids[i].position - (Vector2)transform.position;

            GL.Begin(GL.TRIANGLES);
            GL.Color(new Color(0.5f, 0.5f, 0.3f, 0.7f));
            float ang = Mathf.Atan2(delta.y,delta.x);
            float ang0 = ang;
            GL.Vertex(new Vector2(Mathf.Cos(ang0), Mathf.Sin(ang0)) * R + new Vector2(Mathf.Cos(ang), Mathf.Sin(ang)) * r);
            ang += Mathf.PI * 0.66666666666666f;
            GL.Vertex(new Vector2(Mathf.Cos(ang0), Mathf.Sin(ang0)) * R + new Vector2(Mathf.Cos(ang), Mathf.Sin(ang)) * r);
            ang += Mathf.PI * 0.66666666666666f;
            GL.Vertex(new Vector2(Mathf.Cos(ang0), Mathf.Sin(ang0)) * R + new Vector2(Mathf.Cos(ang), Mathf.Sin(ang)) * r);
            GL.End();
        }
        count = planets.Count;
        for (int i = 0; i < count; i++)
        {
            Vector2 delta = (Vector2)planets[i].position - (Vector2)transform.position;

            GL.Begin(GL.TRIANGLES);
            GL.Color(new Color(0f, 0.8f, 0f, 0.5f));
            float ang = Mathf.Atan2(delta.y, delta.x);
            float ang0 = ang;
            GL.Vertex(new Vector2(Mathf.Cos(ang0), Mathf.Sin(ang0)) * R + new Vector2(Mathf.Cos(ang), Mathf.Sin(ang)) * r1);
            ang += Mathf.PI * 0.66666666666666f;
            GL.Vertex(new Vector2(Mathf.Cos(ang0), Mathf.Sin(ang0)) * R + new Vector2(Mathf.Cos(ang), Mathf.Sin(ang)) * r1);
            ang += Mathf.PI * 0.66666666666666f;
            GL.Vertex(new Vector2(Mathf.Cos(ang0), Mathf.Sin(ang0)) * R + new Vector2(Mathf.Cos(ang), Mathf.Sin(ang)) * r1);
            GL.End();
        }
        count = stars.Count;
        for (int i = 0; i < count; i++)
        {
            Vector2 delta = (Vector2)stars[i].position - (Vector2)transform.position;

            GL.Begin(GL.TRIANGLES);
            GL.Color(new Color(1f, 1f, 0f, 0.5f));
            float ang = Mathf.Atan2(delta.y, delta.x);
            float ang0 = ang;
            GL.Vertex(new Vector2(Mathf.Cos(ang0), Mathf.Sin(ang0)) * R + new Vector2(Mathf.Cos(ang), Mathf.Sin(ang)) * r2);
            ang += Mathf.PI * 0.66666666666666f;
            GL.Vertex(new Vector2(Mathf.Cos(ang0), Mathf.Sin(ang0)) * R + new Vector2(Mathf.Cos(ang), Mathf.Sin(ang)) * r2);
            ang += Mathf.PI * 0.66666666666666f;
            GL.Vertex(new Vector2(Mathf.Cos(ang0), Mathf.Sin(ang0)) * R + new Vector2(Mathf.Cos(ang), Mathf.Sin(ang)) * r2);
            GL.End();
        }
        if (!BH1.near)
        {
            Vector2 delta1 = (Vector2)BH1.transform.position - (Vector2)transform.position;

            GL.Begin(GL.TRIANGLES);
            GL.Color(BH.mass > BH1.mass ? new Color(0f, 1f, 0f, 0.75f) : new Color(1f, 0f, 0f, 0.75f));
            float ang1 = Mathf.Atan2(delta1.y, delta1.x);
            float ang01 = ang1;
            GL.Vertex(new Vector2(Mathf.Cos(ang01), Mathf.Sin(ang01)) * R + new Vector2(Mathf.Cos(ang1), Mathf.Sin(ang1)) * r3);
            ang1 += Mathf.PI * 0.66666666666666f;
            GL.Vertex(new Vector2(Mathf.Cos(ang01), Mathf.Sin(ang01)) * R + new Vector2(Mathf.Cos(ang1), Mathf.Sin(ang1)) * r3);
            ang1 += Mathf.PI * 0.66666666666666f;
            GL.Vertex(new Vector2(Mathf.Cos(ang01), Mathf.Sin(ang01)) * R + new Vector2(Mathf.Cos(ang1), Mathf.Sin(ang1)) * r3);
            GL.End();
        }
        GL.PopMatrix();
    }

}