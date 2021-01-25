using System;
using UnityEngine;

public class BulletSpawnPoint : MonoBehaviour
{
    /// <summary>
    /// Draw Gizmos for easy positioning
    /// </summary>
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(transform.position, 0.01f);
    }
}
