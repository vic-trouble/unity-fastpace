using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBarController : MonoBehaviour
{
    public void SetHealthPortion(float portion)    // 0..1
    {
        transform.localScale = new Vector3(portion, 1, 1);
    }
}
