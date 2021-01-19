using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoBarController : MonoBehaviour
{
    private int ammo = 0;
    public GameObject bulletImage;

    public void SetAmmo(int ammo)
    {
        if (this.ammo == ammo)
            return;
        
        this.ammo = ammo;

        var children = new List<GameObject>();
        foreach (Transform child in transform) {
            children.Add(child.gameObject);
        }
        children.ForEach(child => Destroy(child));

        for (int i = 0; i < ammo; i++)
        {
            var bullet = Instantiate(bulletImage, transform.position + new Vector3(20 * i, 0, 0), Quaternion.identity);
            bullet.transform.SetParent(transform);
        }
    }


}
