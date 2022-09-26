using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLook : MonoBehaviour
{
    private Renderer rend;

    [SerializeField] Material matNormal;
    [SerializeField] Material matBlinking;

    [SerializeField] GameObject ReloadIndicator;

    private int reloadAnimX = 8;
    private int reloadAnimY = 4;
    private int reloadAnimLength = 28;

    public bool IsBlinking { get; private set; }
    public bool IsReloading { get; private set; }
    private float blinkTime = 0.2f;

    public void Init()
    {
        rend = GetComponent<Renderer>();
        rend.material = matNormal;
        ReloadIndicator.SetActive(false);
    }
    public IEnumerator Blink()
    {
        IsBlinking = true;
        rend.material = matBlinking;
        yield return new WaitForSeconds(blinkTime);
        rend.material = matNormal;
        yield return new WaitForSeconds(blinkTime);
        IsBlinking = false;
    }

    public IEnumerator Reload(float reloadTime)
    {
        if (IsReloading == true)
            yield return null;
        else
        {
            IsReloading = true;
            ReloadIndicator.SetActive(true);
            Renderer loader = ReloadIndicator.GetComponent<Renderer>();

            for (int i = 0; i < reloadAnimLength; i++)
            {
                Vector2 offset = new Vector2((1f / reloadAnimX) * (i % reloadAnimX), 1 - (1f / reloadAnimY) - 0.25f * Mathf.Floor(i / reloadAnimX));
                loader.material.SetTextureOffset("_MainTex", offset);
                yield return new WaitForSeconds(reloadTime / reloadAnimLength);
            }
            ReloadIndicator.SetActive(false);
            Messenger.Broadcast("GUN_RELOADED");
            IsReloading = false;
        }
    }
}
