using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ApplyScaleToPolygon : MonoBehaviour
{
    [SerializeField] PolygonCollider2D PolygonCollider;


    private void Start()
    {
        var points = PolygonCollider.points;
        for(int i=0; i< points.Length; i++)
        {
            points[i] = points[i] * transform.localScale;
        }
        PolygonCollider.points = points;
    }
}
