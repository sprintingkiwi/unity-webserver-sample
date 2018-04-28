using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utils : MonoBehaviour
{
    public string GetPosition(string targetName)
    {
        if (GameObject.Find(targetName) != null)
            return (GameObject.Find(targetName).transform.position.ToString());
        else
            Debug.LogWarning("Target object not found");
        return ("Target object not found");
    }
}
