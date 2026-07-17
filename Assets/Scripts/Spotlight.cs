using UnityEngine;

public class Spotlight : MonoBehaviour
{
    [SerializeField] private float radius = 2.2f;
    [SerializeField] private float maskSpriteDiameter = 1f;
    [SerializeField] private float maxSpeed = 15f;
    [SerializeField] private float smoothTime = 0.7f;
    [SerializeField] private float arriveDistance = 0.15f;
    [SerializeField] private float minTargetDistance = 3f;
    [SerializeField] private Vector2 boundsPadding = new Vector2(0.5f, 0.5f);

    private Camera cam;
    private Vector3 target;
    private Vector3 velocity;

    public float Radius => radius;
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
        PickNewTarget();
    }

    private void ApplyRadius()
    {
        float s = (radius * 2f) / maskSpriteDiameter;
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