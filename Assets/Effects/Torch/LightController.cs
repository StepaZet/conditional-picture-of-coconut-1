using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Game;
using UnityEngine;

public class LightController : MonoBehaviour
{
    private void OnEnable()
    {
        IsLightOff = GameData.IsLightOff;
    }

    public bool IsLightOff;
    public Material material;
}
