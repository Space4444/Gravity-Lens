using UnityEngine;

public class Loading : MonoBehaviour {

    private bool loaded;
    private Game game;
    private BackgroundGenerator backGen;
	// Use this for initialization
	void Start () {
        game = FindObjectOfType<Game>();
        backGen = FindObjectOfType<BackgroundGenerator>();
    }
	
	// Update is called once per frame
	void Update () {
        if (game.asteroids.Count != 0 && game.planets.Count != 0 && game.stars.Count != 0)
        {
            loaded = true;
            int count = game.asteroids.Count;
            for (int i = 0; i < count; i++)
                if (game.asteroids[i].GetComponents<AsteroidGenerator>().Length != 0)
                    loaded = false;
            count = game.planets.Count;
            for (int i = 0; i < count; i++)
                if (game.planets[i].GetComponents<PlanetGenerator>().Length != 0)
                    loaded = false;
            count = game.stars.Count;
            for (int i = 0; i < count; i++)
                if (game.stars[i].GetComponents<StarGenerator>().Length != 0)
                    loaded = false;
            if (backGen)
                loaded = false;
            if(loaded)
            {
                game.enabled = true;
                Destroy(gameObject);
            }
        }
	}
}
