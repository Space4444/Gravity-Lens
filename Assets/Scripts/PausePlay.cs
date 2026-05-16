using UnityEngine;
using UnityEngine.UI;

public class PausePlay : MonoBehaviour {

    public GameObject question, credits, settings, slider, saved;
    public Toggle toggle;
    public Sprite pause, play;
    public RectTransform button;
    private BHScript BH;
    private AudioSource source;
    private bool paused = false;

    public void Start()
    {
        BH = FindObjectOfType<BHScript>();
        source = Camera.main.GetComponent<AudioSource>();
        if(PlayerPrefs.HasKey("Music"))
            slider.SetActive(toggle.isOn = source.enabled = (PlayerPrefs.GetString("Music") == "t"));
        if (PlayerPrefs.HasKey("volume"))
            slider.GetComponent<Slider>().value = source.volume = PlayerPrefs.GetFloat("volume");
    }

    public void OnClick()
    {
        PlayerPrefs.Save();
        paused = !paused;
        Time.timeScale = paused ? 0f : 1f;
        GetComponent<Image>().sprite = paused ? play : pause;
        saved.SetActive(!saved.activeSelf);
        if (paused)
        {
            PlayerPrefs.SetFloat("mass", BH.mass);
            PlayerPrefs.SetString("x", BH.truePosition.x.ToString());
            PlayerPrefs.SetString("y", BH.truePosition.y.ToString());
            PlayerPrefs.Save();
        }
    }

    public void Music(bool value)
    {
        source.enabled = value;
        slider.SetActive(value);
        PlayerPrefs.SetString("Music", value ? "t" : "f");
    }

    public void Volume(float value)
    {
        source.volume = value;
        PlayerPrefs.SetFloat("volume", value);
    }

    public void Play()
    {
        paused = false;
        Time.timeScale = 1f;
        GetComponent<Image>().sprite = pause;
        question.SetActive(false);
        credits.SetActive(false);
        settings.SetActive(false);
        saved.SetActive(false);
    }

    public void Asc()
    {
        question.SetActive(true);
    }

    public void ShowCredits()
    {
        credits.SetActive(true);
    }

    public void ShowSettings()
    {
        settings.SetActive(true);
    }

    private void Update()
    {
        button.anchoredPosition = new Vector2(button.anchoredPosition.x + ((paused ? 29 : -29) - button.anchoredPosition.x) * 0.1f, -89);
    }

}
