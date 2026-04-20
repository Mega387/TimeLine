using UnityEngine;
using UnityEngine.UI;

public class Conductor : MonoBehaviour
{
    public static Conductor Instance;
    public float bpm = 120f;
    public AudioSource musicSource;
    public float inputTolerance = 0.15f;
    public Image rhythmIcon;
    public Color beatColor = Color.yellow;
    public Color offBeatColor = new Color(0.3f, 0.3f, 0.3f, 1f);

    public float secPerBeat { get; private set; }
    public float songPosition { get; private set; }
    private float dspSongTime;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        secPerBeat = 60f / bpm;
        dspSongTime = (float)AudioSettings.dspTime;
        if (musicSource != null) musicSource.Play();
    }

    void Update()
    {
        if (musicSource != null && musicSource.isPlaying)
        {
            songPosition = (float)(AudioSettings.dspTime - dspSongTime);
        }
        UpdateRhythmIcon();
    }

    void UpdateRhythmIcon()
    {
        if (rhythmIcon == null) return;

        bool nearBeat = IsNearBeat();
        rhythmIcon.color = Color.Lerp(rhythmIcon.color, nearBeat ? beatColor : offBeatColor, Time.deltaTime * 15f);
        rhythmIcon.transform.localScale = Vector3.Lerp(rhythmIcon.transform.localScale, Vector3.one * (nearBeat ? 1.5f : 1f), Time.deltaTime * 15f);
    }

    public bool IsNearBeat()
    {
        if (secPerBeat <= 0) return false;
        float timeInBeat = songPosition % secPerBeat;
        return (timeInBeat < inputTolerance || timeInBeat > secPerBeat - inputTolerance);
    }

    public int GetCurrentBeat()
    {
        return Mathf.FloorToInt(songPosition / secPerBeat) + 1;
    }
}