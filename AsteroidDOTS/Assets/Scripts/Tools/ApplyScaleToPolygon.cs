using UnityEngine;

namespace Asteroids.Tools
{
    public class ApplyScaleToPolygon : MonoBehaviour
    {
        private void Start()
        {
            var polygonCollider = GetComponent<PolygonCollider2D>();
            var points = polygonCollider.points;
            for (int i = 0; i < points.Length; i++)
            {
                points[i] = points[i] * transform.localScale;
            }
            polygonCollider.points = points;
        }
    }
}