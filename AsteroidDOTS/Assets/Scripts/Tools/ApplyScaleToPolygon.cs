using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ApplyScaleToPolygon : MonoBehaviour
{
    PolygonCollider2D PolygonCollider;


    private void Start()
    {
        PolygonCollider = GetComponent<PolygonCollider2D>();
        var points = PolygonCollider.points;
        for(int i=0; i< points.Length; i++)
        {
            points[i] = points[i] * transform.localScale;
        }
        PolygonCollider.points = points;
    }
}
