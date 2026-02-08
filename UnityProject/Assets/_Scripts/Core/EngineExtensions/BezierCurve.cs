using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 贝塞尔曲线工具类（精简通用版）
/// </summary>
public static class BezierCurve
{
    /// <summary>
    /// 计算二阶贝塞尔曲线上的点（起点、控制点、终点）
    /// </summary>
    /// <param name="start">起点</param>
    /// <param name="control">控制点</param>
    /// <param name="end">终点</param>
    /// <param name="t">插值因子（0-1）</param>
    /// <returns>曲线上对应t的点</returns>
    public static Vector3 GetQuadraticPoint(Vector3 start, Vector3 control, Vector3 end, float t)
    {
        t = Mathf.Clamp01(t); // 确保t在0-1范围内
        // 二阶贝塞尔公式：B(t) = (1-t)²*P0 + 2*(1-t)*t*P1 + t²*P2
        float t1 = 1 - t;
        return t1 * t1 * start + 2 * t1 * t * control + t * t * end;
    }

    /// <summary>
    /// 计算三阶贝塞尔曲线上的点（起点、控制点1、控制点2、终点）
    /// </summary>
    /// <param name="start">起点</param>
    /// <param name="control1">控制点1</param>
    /// <param name="control2">控制点2</param>
    /// <param name="end">终点</param>
    /// <param name="t">插值因子（0-1）</param>
    /// <returns>曲线上对应t的点</returns>
    public static Vector3 GetCubicPoint(Vector3 start, Vector3 control1, Vector3 control2, Vector3 end, float t)
    {
        t = Mathf.Clamp01(t); // 确保t在0-1范围内
        // 三阶贝塞尔公式：B(t) = (1-t)³*P0 + 3*(1-t)²*t*P1 + 3*(1-t)*t²*P2 + t³*P3
        float t1 = 1 - t;
        return t1 * t1 * t1 * start + 3 * t1 * t1 * t * control1 + 3 * t1 * t * t * control2 + t * t * t * end;
    }

    /// <summary>
    /// 生成贝塞尔曲线的采样点列表（用于可视化/轨迹预判）
    /// </summary>
    /// <param name="points">控制点列表（2个=直线，3个=二阶，4个=三阶）</param>
    /// <param name="sampleCount">采样点数（越多越平滑）</param>
    /// <returns>曲线采样点数组</returns>
    public static List<Vector3> GenerateCurvePoints(List<Vector3> points, int sampleCount = 20)
    {
        List<Vector3> curvePoints = new List<Vector3>();
        if (points.Count < 2) return curvePoints; // 至少需要起点和终点

        sampleCount = Mathf.Max(2, sampleCount); // 最少2个点（起点+终点）
        for (int i = 0; i < sampleCount; i++)
        {
            float t = i / (float)(sampleCount - 1);
            Vector3 point;

            // 根据控制点数量选择对应的贝塞尔计算方式
            if (points.Count == 3)
            {
                point = GetQuadraticPoint(points[0], points[1], points[2], t);
            }
            else if (points.Count == 4)
            {
                point = GetCubicPoint(points[0], points[1], points[2], points[3], t);
            }
            else
            {
                // 少于3个点则直接线性插值（直线）
                point = Vector3.Lerp(points[0], points[points.Count - 1], t);
            }

            curvePoints.Add(point);
        }
        return curvePoints;
    }
}