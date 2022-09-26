using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WanderingEnemyLook : MonoBehaviour
{
    private Renderer rend;

    [SerializeField] Material matNormal;
    [SerializeField] Material matBerzerker;
    [SerializeField] Material matFrozen;

    public void Init()
    {
        rend = GetComponent<Renderer>();
        rend.material = matNormal;
    }

    public float GetRadius()
    {
        Bounds bounds = rend.bounds;
        return Mathf.Sqrt(Mathf.Pow(bounds.max.z - bounds.min.z, 2) * 2);
    }

    public void Freeze()
    {
        rend.material = matFrozen;
    }

    public void Unfreeze(bool berzerkerModeOn)
    {
        if (rend.sharedMaterial == matFrozen)
            rend.material = berzerkerModeOn ? matBerzerker : matNormal;
    }
}
