using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    [SerializeField] private Spotlight spotlight;
    [SerializeField] private ActorSpawner spawner;
    [SerializeField] private GameObject darknessOverlay;
    [SerializeField] private GameObject robberMarker;
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private TextMeshProUGUI gameOverText;

    [SerializeField] private Image flashImage;
    [SerializeField] private Color winFlash = new Color(0.25f, 1f, 0.4f, 0.55f);
    [SerializeField] private Color loseFlash = new Color(1f, 0.2f, 0.2f, 0.7f);
    [SerializeField] private float flashDuration = 0.35f;

    [SerializeField] private int baseDecoys = 4;
    [SerializeField] private int decoysPerLevel = 1;
    [SerializeField] private int maxDecoys = 18;

    [SerializeField] private float baseRadius = 2.2f;
    [SerializeField] private float radiusStep = 0.15f;
    [SerializeField] private float minRadius = 0.8f;

    [SerializeField] private float baseSpeed = 3f;
    [SerializeField] private float speedStep = 0.5f;
    [SerializeField] private float maxSpeedCap = 9f;

    [SerializeField] private float baseSmoothTime = 0.7f;
    [SerializeField] private float smoothTimeStep = 0.04f;
    [SerializeField] private float minSmoothTime = 0.25f;

    [SerializeField] private float baseTimeLimit = 20f;
    [SerializeField] private float timeLimitStep = 1f;
    [SerializeField] private float minTimeLimit = 8f;

    private int level = 1;
    private float timeLeft;
    private bool playing;
    private Coroutine flashRoutine;

    private void Start()
    {
        StartLevel();
    }

    private void Update()
    {
        if (playing)
        {
            timeLeft -= Time.deltaTime;
            timerText.text = Mathf.CeilToInt(Mathf.Max(0f, timeLeft)).ToString();

            if (timeLeft <= 0f)
            {
                Lose("The robber got away.");
                return;
            }

            if (Input.GetKeyDown(KeyCode.Space)) TryCatch();
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0)) Restart();
        }
    }

    public void Restart()
    {
        level = 1;
        StartLevel();
    }

    private void StartLevel()
    {
        gameOverPanel.SetActive(false);
        robberMarker.SetActive(false);
        darknessOverlay.SetActive(true);

        int decoys = Mathf.Min(maxDecoys, baseDecoys + decoysPerLevel * (level - 1));
        float radius = Mathf.Max(minRadius, baseRadius - radiusStep * (level - 1));
        float speed = Mathf.Min(maxSpeedCap, baseSpeed + speedStep * (level - 1));
        float smooth = Mathf.Max(minSmoothTime, baseSmoothTime - smoothTimeStep * (level - 1));

        spotlight.Configure(radius, speed, smooth);
        spotlight.ResetToCenter();
        spawner.Spawn(decoys);

        timeLeft = Mathf.Max(minTimeLimit, baseTimeLimit - timeLimitStep * (level - 1));
        levelText.text = "Level " + level;
        timerText.text = Mathf.CeilToInt(timeLeft).ToString();
        playing = true;
    }

    private void TryCatch()
    {
        float d = Vector2.Distance(spawner.Robber.transform.position, spotlight.Center);

        if (d <= spotlight.Radius)
        {
            Flash(winFlash);
            level++;
            StartLevel();
        }
        else
        {
            Lose("That wasn't the robber.");
        }
    }

    private void Lose(string reason)
    {
        Flash(loseFlash);
        playing = false;
        darknessOverlay.SetActive(false);

        robberMarker.transform.position = spawner.Robber.transform.position;
        robberMarker.SetActive(true);

        gameOverText.text = reason + "\nYou reached level " + level + ".";
        gameOverPanel.SetActive(true);
    }

    private void Flash(Color c)
    {
        if (flashRoutine != null) StopCoroutine(flashRoutine);
        flashRoutine = StartCoroutine(FlashRoutine(c));
    }

    private IEnumerator FlashRoutine(Color c)
    {
        float t = 0f;

        while (t < flashDuration)
        {
            t += Time.deltaTime;
            float k = 1f - (t / flashDuration);
            flashImage.color = new Color(c.r, c.g, c.b, c.a * k * k);
            yield return null;
        }

        flashImage.color = new Color(c.r, c.g, c.b, 0f);
    }
}