using System;
using System.Collections;
using System.Collections.Generic;
using Health;
using UnityEngine;

[RequireComponent(typeof(HealthObj))]
public class HealthObj : MonoBehaviour
{
    public HealthSystem Health;
    public HealthBar healthBar;
    public void OnEnable()
    {
        Health = new HealthSystem(10);
        healthBar.SetUp(Health);
    }

    public void Update()
    {
        
    }
}
