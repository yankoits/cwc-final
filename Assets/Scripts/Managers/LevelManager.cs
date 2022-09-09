using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour, IGameManager
{
    public ManagerStatus status { get; private set; }
    public float tileSize { get; private set; }
    public int EnemyCount { get; private set; }
    public float gravity { get; private set; }
    public float LavaBound { get; private set; }
    public Bounds SurfaceBounds { get; private set; }
    public void Init()
    {
        Debug.Log("Level manager starting...");

        // starting settings
        tileSize = 1.0f;
        gravity = -9.8f;

        // should update on next levels (when they will be existing)
        EnemyCount = 10;
        LavaBound = GetLavaBoundary();
        SurfaceBounds = GetSurfaceBounds();

        status = ManagerStatus.Started;
    }

    public float GetLavaBoundary()
    {
        GameObject[] lava = GameObject.FindGameObjectsWithTag("Lava");
        float max = 0;
        for (int i = 0; i < lava.Length; i++)
        {
            Vector3 lavaCenter = lava[i].transform.position;
            Vector3 lavaScale = lava[i].transform.localScale;
            float lavaMax = Mathf.Max(
                Mathf.Abs(lavaCenter.x + lavaScale.x / 2),
                Mathf.Abs(lavaCenter.z + lavaScale.z / 2),
                Mathf.Abs(lavaCenter.x - lavaScale.x / 2),
                Mathf.Abs(lavaCenter.z - lavaScale.z / 2)
            );
            if (lavaMax > max)
                max = lavaMax;
        }
        return max;
    }

    public Bounds GetSurfaceBounds()
    {
        Bounds bounds = new Bounds(Vector3.zero, Vector3.zero);
        GameObject[] surface = GameObject.FindGameObjectsWithTag("Surface");
        
        for (int i = 0; i < surface.Length; i++)
        {
            Vector3 surfaceCenter = surface[i].transform.position;
            Vector3 surfaceScale = surface[i].transform.localScale;

            float minX = surfaceCenter.x - surfaceScale.x / 2;
            float maxX = surfaceCenter.x + surfaceScale.x / 2;
            float minZ = surfaceCenter.z - surfaceScale.z / 2;
            float maxZ = surfaceCenter.z + surfaceScale.z / 2;

            Vector3 min = new Vector3(Mathf.Min(bounds.min.x, minX), 0, Mathf.Min(bounds.min.z, minZ));
            Vector3 max = new Vector3(Mathf.Max(bounds.max.x, maxX), 0, Mathf.Max(bounds.max.z, maxZ));

            bounds.SetMinMax(min, max);
        }
        Debug.Log(bounds);
        return bounds;
    }
}
