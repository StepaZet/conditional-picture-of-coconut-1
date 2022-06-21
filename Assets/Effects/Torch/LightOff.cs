using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class LightOff : MonoBehaviour
{
    public SpriteRenderer spriteRender;
    public TilemapRenderer mapRender;

    private void OnEnable()
    {
        var lightTumbler = GameObject.Find("LightTumbler").GetComponent<LightController>();
        try
        {
            spriteRender = GetComponent<SpriteRenderer>();
        }
        catch (Exception e)
        {
            
        }
        try
        {
            mapRender = GetComponent<TilemapRenderer>();
        }
        catch
        {

        }


        if (lightTumbler.IsLightOff)
        {
            if (spriteRender != null)
                spriteRender.material = lightTumbler.material;
            if (mapRender != null)
                mapRender.material = lightTumbler.material;
        }
    }
}
