using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public static class ViewLayer
{
    private static Dictionary<EViewLayer, RectTransform> _layerMap;

    public static void Init(Canvas canvas)
    {
        _layerMap = new Dictionary<EViewLayer, RectTransform>();

        var values = Enum.GetValues(typeof(EViewLayer));

        foreach (var value in values)
        {
            var layerType = (EViewLayer)value;

            var layerRect = CreateLayer(layerType, canvas);

            _layerMap.Add(layerType, layerRect);
        }
    }

    private static RectTransform CreateLayer(EViewLayer layerType, Canvas canvas)
    {
        var layer = new GameObject(layerType.ToString());

        var layerRect = layer.AddComponent<RectTransform>();

        layerRect.SetParent(canvas.transform);

        layerRect.localPosition = new Vector3(0, 0, 0);
        layerRect.localRotation = Quaternion.identity;
        layerRect.localScale = Vector3.one;

        layerRect.anchorMin = new Vector2(0, 0);
        layerRect.anchorMax = new Vector2(1, 1);

        layerRect.offsetMin = Vector2.zero;
        layerRect.offsetMax = Vector2.zero;

        return layerRect;
    }

    public static RectTransform GetLayer(EViewLayer layerType)
    {
        if (_layerMap.TryGetValue(layerType, out var layerRect))
        {
            return layerRect;
        }

        return default;
    }
}
