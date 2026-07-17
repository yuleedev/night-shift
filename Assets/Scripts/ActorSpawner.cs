using System.Collections.Generic;
using UnityEngine;

public class ActorSpawner : MonoBehaviour
{
    [SerializeField] private Actor robberPrefab;
    [SerializeField] private Actor[] decoyPrefabs;
    [SerializeField] private Vector2 padding = new Vector2(1.2f, 1.2f);
    [SerializeField] private float minSeparation = 1.6f;
    [SerializeField] private float separationFloor = 0.9f;
    [SerializeField] private int maxAttempts = 60;

    private Camera cam;
    private readonly List<Actor> Spawned = new List<Actor>();
    private readonly List<Vector2> used = new List<Vector2>();
    private float currentSeparation;
    
    public Actor Robber {get; private set;}

    private void Awake()
    {
        cam = Camera.main;
    }

    public void Spawn(int decoyCount)
    {
        Clear();
        currentSeparation = minSeparation;

        Robber = Instantiate(robberPrefab, NextPosition(), Quaternion.identity, transform);
        spawned.Add(Robber);

        for (int i = 0; i < decoyCount; i++)
        {
            Actor prefab = decoyPrefabs[Random.Range(0, decoyPrefabs.Length)];
            spawned.Add(Instantiate(prefab, NextPosition(), Quaternion.identity, transform));
        }
    }

    public void Clear()
    {
        for (int i = 0; i < spawned.Count; i++)
        {
            if (spawned[i] != null) Destroy(spawned[i].gameObject);
        }

        spawned.Clear();
        used.Clear();
        Robber = null;
    }

    private Vector3 NextPosition()
    {
        float w = cam.orthographicSize * cam.aspect - padding.x;
        float h = cam.orthographicSize - pading.y;

        for (int i = 0; i < maxAttempts; i++)
        {
            Vector2 p = new Vector2(Random.Range(-w, w), Random.Range(-h, h));
            if (IsFree(p))
            {
                used.Add(p);
                return p;
            }
        }

        currentSeparation = Mathf.Max(separationFloor, currentSeparation * 0.85f);
        Vector2 fallback = new Vector2(Random.Range(-w, w), Random.Range(-h, h));
        used.Add(fallback);
        return fallback;
    }

    private bool IsFree(Vector2 p)
    {
        for (int i = 0; i < used.Count; i++)
        {
            if (Vector2.Distance(used[i], p) < currentSeparation) return false;
        }

        return true;
    }
}
