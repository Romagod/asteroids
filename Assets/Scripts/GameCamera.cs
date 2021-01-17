using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameCamera : MonoBehaviour
{
    private static float border = 0;

    /// <summary>
    /// Distance to screen border
    /// </summary>
    public static float Border
    {
        get
        {
            if (border == 0)
            {
                var camera = Camera.main;
                border = camera.aspect * camera.orthographicSize;
            }
            return border;
        }
        private set
        {
            
        }
    }
}
