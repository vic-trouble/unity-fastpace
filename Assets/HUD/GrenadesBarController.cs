using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrenadesBarController : MonoBehaviour
{
    private int grenades = 0;
    public GameObject grenadeImage;

    public void SetGrenades(int grenades)
    {
        if (this.grenades == grenades)
            return;
        
        this.grenades = grenades;

        var children = new List<GameObject>();
        foreach (Transform child in transform) {
            children.Add(child.gameObject);
        }
        children.ForEach(child => Destroy(child));

        for (int i = 0; i < Math.Min(grenades, 3); i++)
        {
            var grenade = Instantiate(grenadeImage, transform.position + new Vector3(20 * i, 0, 0), Quaternion.identity);
            grenade.transform.SetParent(transform);
        }
    }
}
