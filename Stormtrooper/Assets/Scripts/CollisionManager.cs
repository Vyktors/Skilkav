using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CollisionManager
{
    private static List<CollisionObject> collisionList = new List<CollisionObject>();

    public static CollisionObject CreateCollisionObject(object firstObject, object secondObject)
    {
        CollisionObject collisionObject = GetCollisionObject(firstObject, secondObject);
        if (collisionObject != null)
            return collisionObject;

        collisionObject = new CollisionObject(firstObject, secondObject);
        collisionList.Add(collisionObject);
        return collisionObject;
    }

    public static CollisionObject GetCollisionObject(object firstObject, object secondObject)
    {
        for (int i = 0; i < collisionList.Count; i++)
        {
            CollisionObject curObject = collisionList[i];
            if (curObject.first == firstObject && curObject.second == secondObject)
                return (curObject);
        }
        return null;
    }

    public static bool DeleteCollisionObject(object firstObject, object secondObject)
    {
        for (int i = 0; i < collisionList.Count; i++)
        {
            CollisionObject curObject = collisionList[i];
            if (curObject.first == firstObject && curObject.second == secondObject)
            {
                collisionList.RemoveAt(i);
                return true;
            }
        }
        return false;
    }
}

public class CollisionObject
{
    public object first, second;
    public CollisionObject(object firstObject, object secondObject)
    {
        first = firstObject;
        second = secondObject;
    }
}

public enum CollisionType { HORIZONTAL, VERTICAL }