using System.Collections;
using System.Collections.Generic;
using Health;
using UnityEngine;

[RequireComponent(typeof(HealthObj))]
public class HealthObj : MonoBehaviour
{
    public HealthSystem Health { get; set; }
    public void OnEnable()
    {
        Health = new HealthSystem(2);
    }
}
