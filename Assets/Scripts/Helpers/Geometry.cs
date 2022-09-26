using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Geometry
{
    public static Vector3 RandomPointOnCircleEdge(float maxRadius, float minRadius = 0)
    {
        if (minRadius == 0)
            minRadius = maxRadius / 2;

        Vector2 point;
        do
        {
            point = Random.insideUnitCircle.normalized;
            float randomRadius = Random.Range(minRadius, maxRadius);
            if (point != Vector2.zero)
                return new Vector3(point.x * randomRadius, 0, point.y * randomRadius);
        } while (true);
    }

    public static Vector3 RandomSpawnPoint(Bounds surfaceBounds, float enemyRadius)
    {
        return new Vector3(
            UnityEngine.Random.Range(surfaceBounds.min.x + 2 * enemyRadius, surfaceBounds.max.x - 2 * enemyRadius),
            0f,
            UnityEngine.Random.Range(surfaceBounds.min.z + 2 * enemyRadius, surfaceBounds.max.z - 2 * enemyRadius)
        );
    }
}
