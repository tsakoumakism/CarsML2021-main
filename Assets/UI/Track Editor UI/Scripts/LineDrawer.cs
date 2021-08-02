using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineDrawer
{
 private LineRenderer lineRenderer;

    public LineDrawer(Vector3 start, Vector3 end, Color color, float lineSize){

        GameObject lineObj = new GameObject("LineObj");
        lineObj.gameObject.transform.SetParent(GameObject.Find("LineParent").transform);
        lineRenderer = lineObj.AddComponent<LineRenderer>();
        //Particles/Additive
        lineRenderer.material = new Material(Shader.Find("Hidden/Internal-Colored"));
        //Set color
        lineRenderer.startColor = color;
        lineRenderer.endColor = color;

        //Set width
        lineRenderer.startWidth = lineSize;
        lineRenderer.endWidth = lineSize;

        //Set line count which is 2
        lineRenderer.positionCount = 2;

        //Set the postion of both two lines
        lineRenderer.SetPosition(0, start);
        lineRenderer.SetPosition(1, end);
        
    }

    public void Destroy()
    {
        if (lineRenderer != null)
        {
            UnityEngine.Object.Destroy(lineRenderer.gameObject);
        }
    }
}
