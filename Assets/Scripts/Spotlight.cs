using System.Collections;
using UnityEngine;

public class Spotlight : MonoBehaviour
{
    [SerializeField] private float radius = 2.2f;
    [SerializeField] private float maskSpriteDiameter = 1f;
    [SerializeField] private float maxSpeed = 3f;
    [SerializeField] private float smoothTime = 0.7f;
    [SerializeField] private float arriveDistance = 0.15f;
    [SerializeField] private float minTargetDistance = 3f;
    [SerializeField] private Vector2 boundsPadding = new Vector2(0.5f, 0.5f);

    private Camera cam;
    private Vector3 target;
    private Vector3 velocity;
    private float growth = 1f;
    private bool frozen;

    public float Radius => radius * growth;
    public Vector2 Center => transform.position;

    private void Awake()
    {
        cam = Camera.main;
        ApplyRadius();
    }

    private void Start()
    {
        PickNewTarget();
    }

    private void Update()
    {
        if (frozen) return;

        transform.position = Vector3.SmoothDamp(transform.position, target, ref velocity, smoothTime, maxSpeed);

        if (Vector3.Distance(transform.position, target) < arriveDistance)
        {
            PickNewTarget();
        }
    }

    public void Configure(float newRadius, float newMaxSpeed, float newSmoothTime)
    {
        radius = newRadius;
        maxSpeed = newMaxSpeed;
        smoothTime = newSmoothTime;
        ApplyRadius();
    }

    public void ResetToCenter()
    {
        transform.position = Vector3.zero;
        velocity = Vector3.zero;
        growth = 1f;
        frozen = false;
        ApplyRadius();
        PickNewTarget();
    }

    public IEnumerator PlayIntro(float duration)
    {
        frozen = true;
        growth = 0f;
        ApplyRadius();

        float t = 0f;

        while (t < duration)
        {
            t += Time.deltaTime;
            growth = EaseOutBack(Mathf.Clamp01(t / duration));
            ApplyRadius();
            yield return null;
        }

        growth = 1f;
        ApplyRadius();
        frozen = false;
    }

    private float EaseOutBack(float t)
    {
        float c1 = 1.70158f;
        float c3 = c1 + 1f;
        return 1f + c3 * Mathf.Pow(t - 1f, 3f) + c1 * Mathf.Pow(t - 1f, 2f);
    }

    private void ApplyRadius()
    {
        float s = (radius * 2f * growth) / maskSpriteDiameter;
        transform.localScale = new Vector3(s, s, 1f);
    }

    private void PickNewTarget()
    {
        Vector2 b = Bounds();

        for (int i = 0; i < 20; i++)
        {
            Vector3 p = new Vector3(Random.Range(-b.x, b.x), Random.Range(-b.y, b.y), 0f);
            if (Vector3.Distance(p, transform.position) >= minTargetDistance)
            {
                target = p;
                return;
            }
        }

        target = new Vector3(Random.Range(-b.x, b.x), Random.Range(-b.y, b.y), 0f);
    }

    private Vector2 Bounds()
    {
        float h = cam.orthographicSize - boundsPadding.y;
        float w = cam.orthographicSize * cam.aspect - boundsPadding.x;
        return new Vector2(w, h);
    }
}