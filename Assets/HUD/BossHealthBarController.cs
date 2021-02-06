using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossHealthBarController : MonoBehaviour
{
    public GameObject vignette;

    void Start()
    {
        Hide();
    }

    public void Hide()
    {
        gameObject.SetActive(false);
        vignette.SetActive(false);
    }

    public void Show()
    {
        gameObject.SetActive(true);
        vignette.SetActive(true);
    }

    public void SetHealthPortion(float portion)    // 0..1
    {
        transform.localScale = new Vector3(-portion, 1, 1);
    }
}
