using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Util 
{ 
    public static GameObject FindChildObject(GameObject parent, string childName)
    {
        var childList = parent.GetComponentsInChildren<Transform>();
        for(int i = 0; i < childList.Length; i++)
        {
            if(childList[i].name.Equals(childName))
            {
                return childList[i].gameObject;
            }
        }
        return null;
    }
}
