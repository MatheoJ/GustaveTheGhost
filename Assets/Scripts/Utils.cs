using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utils
{
    public static Vector3 GetMouseWorldDirection(Vector3 sourcePosition)
    {
        Vector3 mousePosition = Input.mousePosition;
        Vector3 playerPositionInScreen = Camera.main.WorldToScreenPoint(sourcePosition);
        Vector3 directionInScreen = mousePosition - playerPositionInScreen;
        Vector3 direction = new Vector3(0, directionInScreen.y, directionInScreen.x);

        return direction.normalized;
    }
}
