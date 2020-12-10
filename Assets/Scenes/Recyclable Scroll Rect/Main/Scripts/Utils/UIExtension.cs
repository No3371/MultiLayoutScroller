//MIT License
//Copyright (c) 2020 Mohammed Iqubal Hussain
//Website : Polyandcode.com 

using UnityEngine;

/// <summary>
/// Extension methods for Rect Transform
/// </summary>
public static class UIExtension
{
    public static Vector3[] GetCorners(this RectTransform rectTransform)
    {
        Vector3[] corners = new Vector3[4];
        rectTransform.GetWorldCorners(corners);
        return corners;
    }

    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public static float MaxY(this RectTransform rectTransform)
    {
        return rectTransform.GetCorners()[1].y;
    }

    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public static float MinY(this RectTransform rectTransform)
    {
        return rectTransform.GetCorners()[0].y;
    }

    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public static float MaxX(this RectTransform rectTransform)
    {
        return rectTransform.GetCorners()[2].x;
    }

    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.AggressiveInlining)]
    public static float MinX(this RectTransform rectTransform)
    {
        return rectTransform.GetCorners()[0].x;
    }

}