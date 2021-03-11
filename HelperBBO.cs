using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HelperBBO
{
    public static GameObject ShootRay(float range, Camera camera)
    {
        Ray ray = camera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hitInfo;

        if (Physics.Raycast(ray, out hitInfo, range))
            return hitInfo.collider.gameObject;
        return null;
    }
}
