using System.Collections;
using Unity.Entities;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class PixelPerfectCameraConverter : GameObjectConversionSystem
{
      protected override void OnUpdate()
      {
        Entities.ForEach((PixelPerfectCamera pixelPerfect) =>
        {
            //AddHybridComponent(pixelPerfect);
        });
      }
       
}
