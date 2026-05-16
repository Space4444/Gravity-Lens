using UnityEngine;

public class AnotherBH : MonoBehaviour
{

    public bool destroy, near;
    public float mass, radius;
    public Double2 truePosition = new Double2();
    public Vector2 speed = new Vector2();
    public Lens lens;

    private float maxDist, halfDist;
    private BHScript BH;
    // Use this for initialization
    void Start()
    {
        BH = FindObjectOfType<BHScript>();
        truePosition = new Double2(Random.Range(-20f, -10f), Random.Range(-20f, -10f));
        mass = Random.Range(40f, 1000f);
        lens = FindObjectOfType<Lens>();
    }
    private void FixedUpdate()
    {
        truePosition += speed;
    }
    // Update is called once per frame
    void Update()
    {
        maxDist = 150 * BH.radius;
        halfDist = maxDist * 0.5f;
        near = Mathf.Abs(transform.position.x) < 6f && Mathf.Abs(transform.position.y) < 6f;
        radius = Mathf.Pow(mass * 0.000005f, 0.333333333f);
        if (!double.IsNaN(truePosition.x) && !double.IsNaN(truePosition.y))
            transform.position = (truePosition - lens.truePosition) / Game.scale * 2f;
        if (mass <= 1f || Mathf.Abs(transform.position.x) > 400f || Mathf.Abs(transform.position.y) > 400f)
            destroy = true;
        if (destroy)
        {
            speed = new Vector2();
            mass = Random.Range(BH.mass, BH.mass * 2f);
            if (Random.Range(0, 2) == 0)
                truePosition = lens.truePosition + new Double2(Random.Range(halfDist, maxDist) - (maxDist + halfDist) * Random.Range(0, 2), Random.Range(-maxDist, maxDist));
            else
                truePosition = lens.truePosition + new Double2(Random.Range(-maxDist, maxDist), Random.Range(halfDist, maxDist) - (maxDist + halfDist) * Random.Range(0, 2));
            destroy = false;
        }
    }
}