using System.Collections.Generic;
using UnityEngine;

public class SwordVFXManager : MonoBehaviour
{
    [SerializeField] private List<GameObject> vfxObjects;

    public void PlayVFX(string type)
    {
        Debug.Log("PlayVFX " + type);
        StopAllVFX();
        GameObject vfx = vfxObjects.Find(v => v.name == type);
        {
            vfx.SetActive(true);
        }
    }

    public void StopAllVFX()
    {
        foreach (var vfx in vfxObjects)
        {
            vfx.SetActive(false);
        }
    }
}
