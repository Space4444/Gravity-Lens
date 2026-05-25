using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

class Game : MonoBehaviour
{
    public static int seed = 5555555;

    public static float scale = 1f, sqrScale = 1f, maxDist = 100f, halfDist, maxDistA = 100f, halfDistA, maxDistP = 100f, halfDistP, maxDistS = 100f, halfDistS;

    public Text score;

    public GameObject planet, asteroid, star;

    public List<SpaceObject> planets = new List<SpaceObject>(), asteroids = new List<SpaceObject>(), stars = new List<SpaceObject>(), nearPlanets = new List<SpaceObject>(), nearAsteroids = new List<SpaceObject>(), nearStars = new List<SpaceObject>();

    private int countPlanets = 0, countAsteroids = 0, countStars = 0, activeStars = 0, length, activePlanets = 0, activeAsteroids = 0;

    private float G = 0.000001f, trueScale = 1f;

    private BHScript BH;

    private AnotherBH BH1;

    private Lens lens;

    private enum GameStage { stage1, stage2, stage3, stage4 };

    private GameStage stage = GameStage.stage1, pstage = GameStage.stage1;

    private Minimap minimap;
    private Canvas canvas;
    private Vector2 lastResolution;
    private void Awake()
    {
        if (PlayerPrefs.HasKey("seed"))
            seed = PlayerPrefs.GetInt("seed");
        BH = FindAnyObjectByType<BHScript>();
        if (PlayerPrefs.HasKey("x") && PlayerPrefs.HasKey("y") && PlayerPrefs.HasKey("mass"))
        {
            Double2 pos = new Double2( PlayerPrefs.GetFloat("x"), PlayerPrefs.GetFloat("y") );
            float m = PlayerPrefs.GetFloat("mass");
            if (m>0 && !double.IsNaN(pos.x) && !double.IsNaN(pos.y) && !double.IsInfinity(pos.x) && !double.IsInfinity(pos.y))
            {
                BH.truePosition = pos;
                BH.mass = m;
            }
            else
                BH.mass = 40;
        }
        else
            BH.mass = 40;
        BH.radius = Mathf.Pow(BH.mass * 0.000005f, 0.333333333f);
        scale = BH.radius * 1.5f;
        maxDist = 100f * scale;
        halfDist = maxDist * 0.5f;
        canvas = FindAnyObjectByType<Canvas>();
    }

    private void Start()
    {
        lastResolution = new Vector2(Screen.width, Screen.height);
        minimap = FindAnyObjectByType<Minimap>();
        BH1 = FindAnyObjectByType<AnotherBH>();
        lens = FindAnyObjectByType<Lens>();
        for (int i = 0; i < 40; i++)
        {
            asteroids.Add(Instantiate(asteroid).GetComponent<SpaceObject>());
            asteroids[i].GetComponent<AsteroidGenerator>().seed = UnityEngine.Random.Range(int.MinValue, int.MaxValue);
        }
        for (int i = 0; i < 30; i++)
        {
            planets.Add(Instantiate(planet).GetComponent<SpaceObject>());
            planets[i].GetComponent<PlanetGenerator>().seed = UnityEngine.Random.Range(int.MinValue, int.MaxValue);
        }
        for (int i = 0; i < 30; i++)
        {
            stars.Add(Instantiate(star).GetComponent<SpaceObject>());
            stars[i].GetComponent<StarGenerator>().seed = UnityEngine.Random.Range(int.MinValue, int.MaxValue);
        }
        enabled = false;
    }
    public void Restart()
    {
        for (int i = 0; i < 40; i++)
        {
            if (UnityEngine.Random.Range(0, 2) == 0)
                asteroids[i].truePosition = lens.truePosition + new Double2(UnityEngine.Random.Range(halfDist, maxDist) - (maxDist + halfDist) * UnityEngine.Random.Range(0, 2), UnityEngine.Random.Range(-maxDist, maxDist));
            else
                asteroids[i].truePosition = lens.truePosition + new Double2(UnityEngine.Random.Range(-maxDist, maxDist), UnityEngine.Random.Range(halfDist, maxDist) - (maxDist + halfDist) * UnityEngine.Random.Range(0, 2));
        }
        for (int i = 0; i < 30; i++)
        {
            planets[i].gameObject.SetActive(false);
            stars[i].gameObject.SetActive(false);
            if (minimap.planets.Contains(planets[i].transform))
                minimap.planets.Remove(planets[i].transform);
            if (minimap.stars.Contains(stars[i].transform))
                minimap.stars.Remove(stars[i].transform);
        }
        stage = GameStage.stage1;
        activePlanets = activeStars = 0;
        BH1.mass = UnityEngine.Random.Range(BH.mass, BH.mass * 2f);
    }
    public void ChangeUniverse()
    {
        Time.timeScale = 1f;
        PlayerPrefs.SetInt("seed", UnityEngine.Random.Range(int.MinValue, int.MaxValue));
        PlayerPrefs.SetFloat("mass", 0f);
        SceneManager.LoadScene(0);
    } 
    private void OnEnable()
    {
        if (stars.Count != 0 || asteroids.Count != 0)
        {
            if (BH.mass < 1000f)
                stage = GameStage.stage1;
            else if (BH.mass < 20000f)
                stage = GameStage.stage2;
            else
                stage = GameStage.stage3;
            if (stage == GameStage.stage1)
            {
                for (int i = 0; i < 40; i++)
                {
                    if (UnityEngine.Random.Range(0, 2) == 0)
                        asteroids[i].truePosition = lens.truePosition + new Double2(UnityEngine.Random.Range(halfDist, maxDist) - (maxDist + halfDist) * UnityEngine.Random.Range(0, 2), UnityEngine.Random.Range(-maxDist, maxDist));
                    else
                        asteroids[i].truePosition = lens.truePosition + new Double2(UnityEngine.Random.Range(-maxDist, maxDist), UnityEngine.Random.Range(halfDist, maxDist) - (maxDist + halfDist) * UnityEngine.Random.Range(0, 2));
                }
                for (int i = 0; i < 30; i++)
                {
                    planets[i].gameObject.SetActive(false);
                    stars[i].gameObject.SetActive(false);
                }
                activeAsteroids = 40;
                //activePlanets = activeStars = 0;
            }

            if (stage == GameStage.stage2)
            {
                for (int i = 0; i < 40; i++)
                {
                    if (UnityEngine.Random.Range(0, 2) == 0)
                        asteroids[i].truePosition = lens.truePosition + new Double2(UnityEngine.Random.Range(halfDist, maxDist) - (maxDist + halfDist) * UnityEngine.Random.Range(0, 2), UnityEngine.Random.Range(-maxDist, maxDist));
                    else
                        asteroids[i].truePosition = lens.truePosition + new Double2(UnityEngine.Random.Range(-maxDist, maxDist), UnityEngine.Random.Range(halfDist, maxDist) - (maxDist + halfDist) * UnityEngine.Random.Range(0, 2));
                }
                for (int i = 0; i < 30; i++)
                {
                    if (UnityEngine.Random.Range(0, 2) == 0)
                        planets[i].truePosition = lens.truePosition + new Double2(UnityEngine.Random.Range(halfDist, maxDist) - (maxDist + halfDist) * UnityEngine.Random.Range(0, 2), UnityEngine.Random.Range(-maxDist, maxDist));
                    else
                        planets[i].truePosition = lens.truePosition + new Double2(UnityEngine.Random.Range(-maxDist, maxDist), UnityEngine.Random.Range(halfDist, maxDist) - (maxDist + halfDist) * UnityEngine.Random.Range(0, 2));
                }
                for (int i = 0; i < 30; i++)
                    stars[i].gameObject.SetActive(false);
                activeAsteroids = 40;
                //activePlanets = 30;
                //activeStars = 0;
            }

            if (stage == GameStage.stage3)
            {
                for (int i = 0; i < 30; i++)
                {
                    if (UnityEngine.Random.Range(0, 2) == 0)
                        stars[i].truePosition = lens.truePosition + new Double2(UnityEngine.Random.Range(halfDist, maxDist) - (maxDist + halfDist) * UnityEngine.Random.Range(0, 2), UnityEngine.Random.Range(-maxDist, maxDist));
                    else
                        stars[i].truePosition = lens.truePosition + new Double2(UnityEngine.Random.Range(-maxDist, maxDist), UnityEngine.Random.Range(halfDist, maxDist) - (maxDist + halfDist) * UnityEngine.Random.Range(0, 2));
                }
                for (int i = 0; i < 30; i++)
                {
                    if (UnityEngine.Random.Range(0, 2) == 0)
                        planets[i].truePosition = lens.truePosition + new Double2(UnityEngine.Random.Range(halfDist, maxDist) - (maxDist + halfDist) * UnityEngine.Random.Range(0, 2), UnityEngine.Random.Range(-maxDist, maxDist));
                    else
                        planets[i].truePosition = lens.truePosition + new Double2(UnityEngine.Random.Range(-maxDist, maxDist), UnityEngine.Random.Range(halfDist, maxDist) - (maxDist + halfDist) * UnityEngine.Random.Range(0, 2));
                }
                for (int i = 0; i < 40; i++)
                    asteroids[i].gameObject.SetActive(false);
                activeAsteroids = 0;
                //activePlanets = activeStars = 30;
            }
            BH1.mass = UnityEngine.Random.Range(BH.mass, BH.mass * 2f);
        }
    }

    private void FixedUpdate()
    {
        if(BH.mass!=0)
        scale += (trueScale - scale) * 0.1f;
    }

    private void Update()
    {
        if (Screen.width != (int)lastResolution.x || Screen.height != (int)lastResolution.y) {
            PlayerPrefs.SetFloat("mass", BH.mass);
            PlayerPrefs.SetFloat("x", (float)BH.truePosition.x);
            PlayerPrefs.SetFloat("y", (float)BH.truePosition.y);
            PlayerPrefs.Save();
            SceneManager.LoadScene(0);
        }

        if (Time.frameCount % 1000 == 0)
            GC.Collect();

        pstage = stage;
        if (BH.mass < 1000f)
            stage = GameStage.stage1;
        else if (BH.mass < 20000f)
            stage = GameStage.stage2;
        else
            stage = GameStage.stage3;

        if (stage == GameStage.stage1)
            if (activeAsteroids < 40)
            {
                asteroids[activeAsteroids].need = true;
                asteroids[activeAsteroids].destroy = true;
                asteroids[activeAsteroids].gameObject.SetActive(true);
                //minimap.asteroids.Add(asteroids[activeAsteroids].transform);
                activeAsteroids++;
            }
        if (stage > GameStage.stage1)
            if (activePlanets < 30)
            {
                planets[activePlanets].need = true;
                planets[activePlanets].destroy = true;
                planets[activePlanets].gameObject.SetActive(true);
                //minimap.planets.Add(planets[activePlanets].transform);
                activePlanets++;
            }
        if (stage == GameStage.stage3)
        {
            if (activeStars < 30)
            {
                stars[activeStars].need = true;
                stars[activeStars].destroy = true;
                stars[activeStars].gameObject.SetActive(true);
                //minimap.stars.Add(stars[activeStars].transform);
                activeStars++;
            }
            if (activeAsteroids > 0)
            {
                activeAsteroids--;
                asteroids[activeAsteroids].need = false;
                asteroids[activeAsteroids].destroy = true;
                minimap.asteroids.Remove(asteroids[activeAsteroids].transform);
            }
        }

        trueScale = BH.radius*1.5f;
        sqrScale = scale * scale;
        maxDist = 100f * scale;
            maxDistA = 10f;
            maxDistP = Mathf.Max(40f, maxDist * 0.3f);
            maxDistS = Mathf.Max(200f, maxDist * 0.3f);
        halfDistA = maxDistA * 0.5f;
        halfDistP = maxDistP * 0.5f;
        halfDistS = maxDistS * 0.5f;

        length = planets.Count;
        for (int i = 0; i < length; i++)
            if (planets[i].gameObject.activeSelf && Mathf.Abs(planets[i].transform.position.x) < halfDistP && Mathf.Abs(planets[i].transform.position.y) < halfDistP)
            {
                if (!nearPlanets.Contains(planets[i]))
                {
                    if (!minimap.planets.Contains(planets[i].transform))
                        minimap.planets.Add(planets[i].transform);
                    nearPlanets.Add(planets[i]);
                    countPlanets++;
                }
            }
            else if (nearPlanets.Contains(planets[i]))
            {
                if (minimap.planets.Contains(planets[i].transform))
                    minimap.planets.Remove(planets[i].transform);
                nearPlanets.Remove(planets[i]);
                countPlanets--;
            }
        length = asteroids.Count;
        for (int i = 0; i < length; i++)
            if (asteroids[i].gameObject.activeSelf && Mathf.Abs(asteroids[i].transform.position.x) < halfDistA && Mathf.Abs(asteroids[i].transform.position.y) < halfDistA)
            {
                if (!nearAsteroids.Contains(asteroids[i]))
                {
                    if (!minimap.asteroids.Contains(asteroids[i].transform))
                        minimap.asteroids.Add(asteroids[i].transform);
                    nearAsteroids.Add(asteroids[i]);
                    countAsteroids++;
                }
            }
            else if (nearAsteroids.Contains(asteroids[i]))
            {
                if (minimap.asteroids.Contains(asteroids[i].transform))
                    minimap.asteroids.Remove(asteroids[i].transform);
                nearAsteroids.Remove(asteroids[i]);
                countAsteroids--;
            }
        length = stars.Count;
        for (int i = 0; i < length; i++)
            if (stars[i].gameObject.activeSelf && Mathf.Abs(stars[i].transform.position.x) < halfDistS && Mathf.Abs(stars[i].transform.position.y) < halfDistS)
            {
                if (!nearStars.Contains(stars[i]))
                {
                    if (!minimap.stars.Contains(stars[i].transform))
                        minimap.stars.Add(stars[i].transform);
                    nearStars.Add(stars[i]);
                    countStars++;
                }
            }
            else if (nearStars.Contains(stars[i]))
            {
                if (minimap.stars.Contains(stars[i].transform))
                    minimap.stars.Remove(stars[i].transform);
                nearStars.Remove(stars[i]);
                countStars--;
            }

        for (int i = 0; i < countPlanets; i++)
        {
            float deltaR = trueScale + nearPlanets[i].radius * 0.16f - (float)Double2.Distance(nearPlanets[i].truePosition, BH.truePosition);
            if (deltaR > 0f)
            {
                //print(nearPlanets[i].truePosition);
                float deltaM = nearPlanets[i].mass - (deltaR < nearPlanets[i].radius ? Mathf.Pow(nearPlanets[i].radius - deltaR, 3f)* 17f : 1f);
                BH.mass += deltaM;
                nearPlanets[i].mass -= deltaM;
            }
            deltaR = trueScale + nearPlanets[i].radius * 0.16f - (float)Double2.Distance(nearPlanets[i].truePosition, BH1.truePosition);
            if (deltaR > 0f)
            {
                //print(nearPlanets[i].truePosition);
                float deltaM = nearPlanets[i].mass - (deltaR < nearPlanets[i].radius ? Mathf.Pow(nearPlanets[i].radius - deltaR, 3f) * 17f : 1f);
                BH1.mass += deltaM;
                nearPlanets[i].mass -= deltaM;
            }

            #region moving
            Vector2 delta = nearPlanets[i].truePosition - BH.truePosition;
            float magnitude = delta.magnitude;
            Vector2 normalized = delta / magnitude;

            Vector2 F = G * nearPlanets[i].mass * BH.mass / (magnitude * magnitude) * normalized;

            nearPlanets[i].speed -= F / nearPlanets[i].mass;
            //BH.speed += F / BH.mass;

            if (BH1.mass != 0)
            {
                Vector2 delta1 = nearPlanets[i].truePosition - BH1.truePosition;
                float magnitude1 = delta1.magnitude;
                Vector2 normalized1 = delta1 / magnitude1;

                F = G * nearPlanets[i].mass * BH1.mass / (magnitude1 * magnitude1) * normalized1;

                nearPlanets[i].speed -= F / nearPlanets[i].mass;
                BH1.speed += F / BH1.mass;
            }

            for (int j = i + 1; j < countPlanets; j++)
            {
                delta = nearPlanets[i].truePosition - nearPlanets[j].truePosition;
                magnitude = delta.magnitude;
                normalized = delta / magnitude;

                F = G * nearPlanets[i].mass * nearPlanets[j].mass / (magnitude * magnitude) * normalized;

                nearPlanets[i].speed -= F / nearPlanets[i].mass;
                nearPlanets[j].speed += F / nearPlanets[j].mass;

                float intersection = (nearPlanets[i].radius + nearPlanets[j].radius) * 0.46f - (float)Double2.Distance(nearPlanets[i].truePosition, nearPlanets[j].truePosition);
                if (intersection > 0f)
                {
                    float k = nearPlanets[j].mass / nearPlanets[i].mass;

                    Double2 bias = normalized * intersection / (nearPlanets[i].mass + nearPlanets[j].mass);
                    nearPlanets[i].truePosition += bias * nearPlanets[j].mass;
                    nearPlanets[j].truePosition -= bias * nearPlanets[i].mass;

                    float beta = Mathf.Atan2(nearPlanets[i].speed.y, nearPlanets[i].speed.x);

                    float alpha = Mathf.Atan2(delta.y, delta.x);

                    float scalar = nearPlanets[i].speed.x * delta.x + nearPlanets[i].speed.y * delta.y;
                    Vector2 normalizedX = Mathf.Sign(scalar) * normalized;

                    float v1xm = Mathf.Abs(nearPlanets[i].speed.magnitude * Mathf.Cos(alpha + beta));
                    Vector2 v1x = v1xm * normalizedX;
                    Vector2 v1y = nearPlanets[i].speed - v1x;

                    beta = Mathf.Atan2(nearPlanets[j].speed.y, nearPlanets[j].speed.x);

                    scalar = nearPlanets[j].speed.x * delta.x + nearPlanets[j].speed.y * delta.y;
                    normalizedX = Mathf.Sign(scalar) * normalized;

                    float v2xm = Mathf.Abs(nearPlanets[j].speed.magnitude * Mathf.Cos(alpha + beta));
                    Vector2 v2x = v2xm * normalizedX;
                    Vector2 v2y = nearPlanets[j].speed - v2x;

                    v1x = (v1x + k * v2x) * 0.25f;
                    v2x = (v1x / k + v2x) * 0.25f;

                    nearPlanets[i].speed = v1x + v1y;
                    nearPlanets[j].speed = v2x + v2y;
                }
            }
            #endregion

        }

        for (int i = 0; i < countAsteroids; i++)
        {
            float deltaR = trueScale + nearAsteroids[i].radius * 0.1f - (float)Double2.Distance(nearAsteroids[i].truePosition, BH.truePosition);
            if (deltaR > 0f)
            {
                float deltaM = nearAsteroids[i].mass - (deltaR < nearAsteroids[i].radius ? Mathf.Pow(nearAsteroids[i].radius - deltaR, 3f) * 17f : 0f);
                BH.mass += deltaM;
                nearAsteroids[i].mass -= deltaM;
            }
            deltaR = trueScale + nearAsteroids[i].radius * 0.1f - (float)Double2.Distance(nearAsteroids[i].truePosition, BH1.truePosition);
            if (deltaR > 0f)
            {
                float deltaM = nearAsteroids[i].mass - (deltaR < nearAsteroids[i].radius ? Mathf.Pow(nearAsteroids[i].radius - deltaR, 3f) * 17f : 0f);
                BH1.mass += deltaM;
                nearAsteroids[i].mass -= deltaM;
            }

            #region moving
            Vector2 delta = nearAsteroids[i].truePosition - BH.truePosition;
            float magnitude = delta.magnitude;
            Vector2 normalized = delta / magnitude;

            Vector2 F = G * nearAsteroids[i].mass * BH.mass / (magnitude * magnitude) * normalized;

            nearAsteroids[i].speed -= F / nearAsteroids[i].mass;
            //BH.speed += F / BH.mass;

            if (BH1.mass != 0)
            {
                Vector2 delta1 = nearAsteroids[i].truePosition - BH1.truePosition;
                float magnitude1 = delta1.magnitude;
                Vector2 normalized1 = delta1 / magnitude1;

                F = G * nearAsteroids[i].mass * BH1.mass / (magnitude1 * magnitude1) * normalized1;

                nearAsteroids[i].speed -= F / nearAsteroids[i].mass;
                BH1.speed += F / BH1.mass;
            }

            /*for (int j = i + 1; j < countAsteroids; j++)
            {
                float intersection = (nearAsteroids[i].radius + nearAsteroids[j].radius) * 0.2f - (float)Double2.Distance(nearAsteroids[i].truePosition, nearAsteroids[j].truePosition);
                if (intersection > 0f)
                {
                    nearAsteroids[i].destroy = true;
                    nearAsteroids[j].destroy = true;
                }
            }*/
            #endregion

        }

        for (int i = 0; i < countStars; i++)
        {
            float deltaR = trueScale + nearStars[i].radius * 0.16f - (float)Double2.Distance(nearStars[i].truePosition, BH.truePosition);
            if (deltaR > 0f)
            {
                float deltaM = nearStars[i].mass - (deltaR < nearStars[i].radius ? Mathf.Pow(nearStars[i].radius - deltaR, 3f) * 17f : 0f);
                BH.mass += deltaM;
                nearStars[i].mass -= deltaM;
            }
            deltaR = trueScale + nearStars[i].radius * 0.16f - (float)Double2.Distance(nearStars[i].truePosition, BH1.truePosition);
            if (deltaR > 0f)
            {
                float deltaM = nearStars[i].mass - (deltaR < nearStars[i].radius ? Mathf.Pow(nearStars[i].radius - deltaR, 3f) * 17f : 0f);
                BH1.mass += deltaM;
                nearStars[i].mass -= deltaM;
            }

            #region moving
            Vector2 delta = nearStars[i].truePosition - BH.truePosition;
            float magnitude = delta.magnitude;
            Vector2 normalized = delta / magnitude;

            Vector2 F = G * nearStars[i].mass * BH.mass / (magnitude * magnitude) * normalized;

            nearStars[i].speed -= F / nearStars[i].mass;
            //BH.speed += F / BH.mass;

            if (BH1.mass != 0)
            {
                Vector2 delta1 = nearStars[i].truePosition - BH1.truePosition;
                float magnitude1 = delta1.magnitude;
                Vector2 normalized1 = delta1 / magnitude1;

                F = G * nearStars[i].mass * BH1.mass / (magnitude1 * magnitude1) * normalized1;

                nearStars[i].speed -= F / nearStars[i].mass;
                BH1.speed += F / BH1.mass;
            }

            for (int j = i + 1; j < countStars; j++)
            {
                delta = nearStars[i].truePosition - nearStars[j].truePosition;
                magnitude = delta.magnitude;
                normalized = delta / magnitude;

                F = G * nearStars[i].mass * nearStars[j].mass / (magnitude * magnitude) * normalized;

                nearStars[i].speed -= F / nearStars[i].mass;
                nearStars[j].speed += F / nearStars[j].mass;

                float intersection = (nearStars[i].radius + nearStars[j].radius) * 0.46f - (float)Double2.Distance(nearStars[i].truePosition, nearStars[j].truePosition);
                if (intersection > 0f)
                {
                    float k = nearStars[j].mass / nearStars[i].mass;

                    Double2 bias = normalized * intersection / (nearStars[i].mass + nearStars[j].mass);
                    nearStars[i].truePosition += bias * nearStars[j].mass;
                    nearStars[j].truePosition -= bias * nearStars[i].mass;

                    float beta = Mathf.Atan2(nearStars[i].speed.y, nearStars[i].speed.x);

                    float alpha = Mathf.Atan2(delta.y, delta.x);

                    float scalar = nearStars[i].speed.x * delta.x + nearStars[i].speed.y * delta.y;
                    Vector2 normalizedX = Mathf.Sign(scalar) * normalized;

                    float v1xm = Mathf.Abs(nearStars[i].speed.magnitude * Mathf.Cos(alpha + beta));
                    Vector2 v1x = v1xm * normalizedX;
                    Vector2 v1y = nearStars[i].speed - v1x;

                    beta = Mathf.Atan2(nearStars[j].speed.y, nearStars[j].speed.x);

                    scalar = nearStars[j].speed.x * delta.x + nearStars[j].speed.y * delta.y;
                    normalizedX = Mathf.Sign(scalar) * normalized;

                    float v2xm = Mathf.Abs(nearStars[j].speed.magnitude * Mathf.Cos(alpha + beta));
                    Vector2 v2x = v2xm * normalizedX;
                    Vector2 v2y = nearStars[j].speed - v2x;

                    v1x = (v1x + k * v2x) * 0.25f;
                    v2x = (v1x / k + v2x) * 0.25f;

                    nearStars[i].speed = v1x + v1y;
                    nearStars[j].speed = v2x + v2y;
                }
            }
            #endregion

        }

        float deltaR1 = (BH.radius + BH1.radius)*0.75f - (float)Double2.Distance(BH1.truePosition, BH.truePosition);
        if (deltaR1 > 0f && BH1.mass!=0)
        {
            if (BH.mass >= BH1.mass)
            {
                BH.mass += BH1.mass;
                BH1.destroy = true;
            }
            else
            {
                BH1.mass += BH.mass;
                BH.mass = 0f;
            }
        }

        if (BH.mass != 0 && BH1.mass != 0)
        {
            Vector2 delta2 = BH.truePosition - BH1.truePosition;
            float magnitude2 = delta2.magnitude;
            Vector2 normalized2 = delta2 / magnitude2;

            Vector2 F2 = G * BH.mass * BH1.mass / (magnitude2 * magnitude2) * normalized2;

            BH.speed -= F2 / BH.mass;
            BH1.speed += F2 / BH1.mass;
        }

        score.text = "Mass: " + BH.mass.ToString("0");// + "\n" + BH.truePosition.ToString() + "\n" + "FPS: " + (1f/Time.deltaTime).ToString("0");
    }

}