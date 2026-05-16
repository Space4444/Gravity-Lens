using UnityEngine;

public enum SpaceObjectType { asteroid, planet, star, quazar };

public class SpaceObject : MonoBehaviour
{
    
    public SpaceObjectType type;

    public bool need, destroy, loaded = false;

    public float mass, radius, angleSpeed;

    public Double2 truePosition = new Double2(10d,10d);

    public Vector2 speed;

    public float maxDist, halfDist;

    private BHScript BH;

    private Minimap minimap;

    private Lens lens;

    private SpriteRenderer rend;
    // Use this for initialization
    void Start()
    {
        BH = FindObjectOfType<BHScript>();
        need = true;
        minimap = FindObjectOfType<Minimap>();
        rend = GetComponent<SpriteRenderer>();
        destroy = false;
        maxDist = Game.maxDist;
        halfDist = Game.halfDist;
        lens = FindObjectOfType<Lens>();
    }

    private void FixedUpdate()
    {
        truePosition += speed;
        transform.Rotate(0f, 0f, angleSpeed);
    }

    // Update is called once per frame
    public void Update()
    {
        if (speed.sqrMagnitude > 0.0009f*Game.sqrScale)
        {
            speed = speed.normalized * 0.03f*Game.scale;
            if (Mathf.Abs(transform.position.x) < 0.01f && Mathf.Abs(transform.position.y) < 0.01f)
            {
                BH.mass += mass;
                mass = 0f;
            }
        }
        switch (type)
        {
            case SpaceObjectType.asteroid:
                maxDist = Game.maxDistA;
                halfDist = Game.halfDistA;
                break;
            case SpaceObjectType.planet:
                maxDist = Game.maxDistP;
                halfDist = Game.halfDistP;
                break;
            case SpaceObjectType.star:
                maxDist = Game.maxDistS;
                halfDist = Game.halfDistS;
                break;
        }
        if (!double.IsNaN(truePosition.x) && !double.IsNaN(truePosition.y))
            transform.position = truePosition - lens.truePosition;

        transform.position += new Vector3(0f, 0f, 1000f);

        if (mass > 1f)
            transform.localScale = Vector2.one * Mathf.Pow(mass, 0.3333f) * 0.05f;
        radius = transform.localScale.x * 7.68f;

        if (mass <= 1f || Mathf.Abs(transform.position.x) > maxDist || Mathf.Abs(transform.position.y) > maxDist)
            destroy = true;
        
        if(destroy)
        {
            if (need)
            {
                switch (type)
                {
                    case SpaceObjectType.asteroid:
                        speed = new Vector2(Random.Range(-0.005f, 0.005f), Random.Range(-0.005f, 0.005f));
                        angleSpeed = Random.Range(-5f, 5f);
                        mass = Random.Range(10, 40);
                        break;
                    case SpaceObjectType.planet:
                        speed = new Vector2(Random.Range(-0.01f, 0.01f), Random.Range(-0.01f, 0.01f));
                        angleSpeed = Random.Range(-1f, 1f);
                        mass = Random.Range(100, 1000);
                        break;
                    case SpaceObjectType.star:
                        speed = new Vector2(Random.Range(-0.05f, 0.05f), Random.Range(-0.05f, 0.05f));
                        angleSpeed = Random.Range(-2f, 2f);
                        mass = Random.Range(2000, 20000);
                        break;
                }
                if (Random.Range(0, 2) == 0)
                    truePosition = lens.truePosition + new Double2(Random.Range(halfDist, maxDist) - (maxDist + halfDist) * Random.Range(0, 2), Random.Range(-maxDist, maxDist));
                else
                    truePosition = lens.truePosition + new Double2(Random.Range(-maxDist, maxDist), Random.Range(halfDist, maxDist) - (maxDist + halfDist) * Random.Range(0, 2));
            }
            else
                gameObject.SetActive(false);
            if (rend.enabled != need)
            {
                if (!double.IsNaN(truePosition.x) && !double.IsNaN(truePosition.y))
                    transform.position = truePosition - lens.truePosition;
                rend.enabled = need;
            }
            destroy = false;
        }
    }
    private void OnBecameInvisible()
    {
        switch (type)
        {
            case SpaceObjectType.asteroid:
                if(!minimap.asteroids.Contains(transform))
                    minimap.asteroids.Add(transform);
                break;
            case SpaceObjectType.planet:
                if (!minimap.planets.Contains(transform))
                    minimap.planets.Add(transform);
                break;
            case SpaceObjectType.star:
                if (!minimap.stars.Contains(transform))
                    minimap.stars.Add(transform);
                break;
        }
    }
    private void OnBecameVisible()
    {
        switch (type)
        {
            case SpaceObjectType.asteroid:
                if (minimap.asteroids.Contains(transform))
                    minimap.asteroids.Remove(transform);
                break;
            case SpaceObjectType.planet:
                if (minimap.planets.Contains(transform))
                    minimap.planets.Remove(transform);
                break;
            case SpaceObjectType.star:
                if (minimap.stars.Contains(transform))
                    minimap.stars.Remove(transform);
                break;
        }
    }
}