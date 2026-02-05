using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

public static class ViewBaseExtensions
{
    public static void SetLayer(this ViewBase view, EViewLayer layerType)
    {
        var layer = ViewLayer.GetLayer(layerType);

        view.RectTransform.SetParent(layer);
        view.RectTransform.SetAsLastSibling();
        view.RectTransform.StretchToParentSize();
    }

    public static void SetVisible(this ViewBase view, bool visible)
    {
        view.gameObject.SetActive(visible);
    }
}
