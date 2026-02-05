using System;
using System.Collections.Generic;
using UnityEngine;

public static class GLExtensions
{
    public static void DrawGrid2D(Vector2 gridSize, Vector2Int gridCount)
    {
        var offset = new Vector2
        {
            x = gridCount.x % 2 == 0 ? 0 : gridSize.x / 2,
            y = gridCount.y % 2 == 0 ? 0 : gridSize.y / 2,
        };

        var alignCenter = new Vector2
        {
            x = gridCount.x * gridSize.x / 2 * -1,
            y = gridCount.y * gridSize.y / 2 * -1,
        };

        // 绘制垂直线
        for (int i = 0; i < gridCount.x + 1; i++)
        {
            var x = i * gridSize.x + alignCenter.x + offset.x;
            var yMin = 0 * gridSize.y + alignCenter.y + offset.y;
            var yMax = gridCount.y * gridSize.y + alignCenter.y + offset.y;

            DrawLine(new Vector2(x, yMin), new Vector2(x, yMax));
        }

        // 绘制水平线
        for (int i = 0; i < gridCount.y + 1; i++)
        {
            var y = i * gridSize.y + alignCenter.y + offset.y;
            var xMin = 0 * gridSize.x + alignCenter.x + offset.x;
            var xMax = gridCount.x * gridSize.x + alignCenter.x + offset.x;

            DrawLine(new Vector2(xMin, y), new Vector2(xMax, y));
        }
    }

    public static void DrawLine(Vector3 start, Vector3 end)
    {
        GL.Vertex(start);
        GL.Vertex(end);
    }

    public struct LineScope : IDisposable
    {
        public LineScope(Matrix4x4 matrix)
        {
            GL.PushMatrix();
            GL.MultMatrix(matrix);
            GL.Begin(GL.LINES);
        }

        public LineScope(Color color)
        {
            GL.PushMatrix();
            GL.Begin(GL.LINES);
            GL.Color(color);
        }

        public LineScope(Matrix4x4 matrix, Color color) : this(matrix)
        {
            GL.Color(color);
        }

        public void Dispose()
        {
            GL.End();
            GL.PopMatrix();
        }
    }
}
