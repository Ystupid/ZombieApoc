using System;
using System.Collections.Generic;
using UnityEngine;

public static class TransformExtensions
{
    public static T GetComponentInParent<T>(this Transform transform, bool recursive = true)
    {
        var root = transform.parent;

        var component = default(T);

        while (root != null)
        {
            component = root.GetComponent<T>();

            if (component != null || !recursive)
            {
                break;
            }

            root = root.parent;
        }

        return component;
    }

    public static void ResetLocal(this Transform transform)
    {
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
        transform.localScale = Vector3.one;
    }

    public static void StretchToParentSize(this RectTransform rectTransform)
    {
        rectTransform.ResetLocal();

        rectTransform.anchorMin = new Vector2(0, 0);
        rectTransform.anchorMax = new Vector2(1, 1);

        rectTransform.offsetMin = Vector2.zero;
        rectTransform.offsetMax = Vector2.zero;
    }

    public static Vector3 ToSortAxisPosition(this Transform parent, Vector3 position, bool enableZOffset = true)
    {
        var distanceY = position.y - parent.position.y;
        position.z = parent.position.z;

        if (enableZOffset)
        {
            position.z += distanceY > 0 ? -distanceY : 0;
        }

        return position;
    }

    public static void SetX(this Transform transform, float x)
    {
        var position = transform.position;
        position.x = x;
        transform.position = position;
    }

    public static void SetY(this Transform transform, float y)
    {
        var position = transform.position;
        position.y = y;
        transform.position = position;
    }

    public static void SetZ(this Transform transform, float z)
    {
        var position = transform.position;
        position.z = z;
        transform.position = position;
    }

    public static void SetLocalX(this Transform transform, float x)
    {
        var localPosition = transform.localPosition;
        localPosition.x = x;
        transform.localPosition = localPosition;
    }

    public static void SetLocalY(this Transform transform, float y)
    {
        var localPosition = transform.localPosition;
        localPosition.y = y;
        transform.localPosition = localPosition;
    }

    public static void SetLocalZ(this Transform transform, float z)
    {
        var localPosition = transform.localPosition;
        localPosition.z = z;
        transform.localPosition = localPosition;
    }

    public static void SetAnchorX(this RectTransform rectTransform, float x)
    {
        var anchorPosition = rectTransform.anchoredPosition;
        anchorPosition.x = x;
        rectTransform.anchoredPosition = anchorPosition;
    }

    public static void SetAnchorY(this RectTransform rectTransform, float y)
    {
        var anchorPosition = rectTransform.anchoredPosition;
        anchorPosition.y = y;
        rectTransform.anchoredPosition = anchorPosition;
    }
}
