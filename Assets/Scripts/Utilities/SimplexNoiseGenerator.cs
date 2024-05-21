using System;
using UnityEngine;

public static class SimplexNoiseGenerator
{
    public static float Mod(float x, float y)
    {
        return x - y * Mathf.Floor(x / y);
    }

    public static Vector2 Mod(Vector2 x, Vector2 y)
    {
        return x - y * new Vector2(Mathf.Floor(x.x / y.x), Mathf.Floor(x.y / y.y));
    }

    public static Vector3 Mod(Vector3 x, Vector3 y)
    {
        return new Vector3(
            x.x - y.x * Mathf.Floor(x.x / y.x),
            x.y - y.y * Mathf.Floor(x.y / y.y),
            x.z - y.z * Mathf.Floor(x.z / y.z)
        );
    }

    public static Vector4 Mod(Vector4 x, Vector4 y)
    {
        return new Vector4(
            x.x - y.x * Mathf.Floor(x.x / y.x),
            x.y - y.y * Mathf.Floor(x.y / y.y),
            x.z - y.z * Mathf.Floor(x.z / y.z),
            x.w - y.w * Mathf.Floor(x.w / y.w)
        );
    }

    public static Vector2 Fade(Vector2 t)
    {
        return new Vector2(
            t.x * t.x * t.x * (t.x * (t.x * 6 - 15) + 10),
            t.y * t.y * t.y * (t.y * (t.y * 6 - 15) + 10)
        );
    }

    public static Vector3 Fade(Vector3 t)
    {
        return new Vector3(
            t.x * t.x * t.x * (t.x * (t.x * 6 - 15) + 10),
            t.y * t.y * t.y * (t.y * (t.y * 6 - 15) + 10),
            t.z * t.z * t.z * (t.z * (t.z * 6 - 15) + 10)
        );
    }

    public static float Mod289(float x)
    {
        return x - Mathf.Floor(x / 289) * 289;
    }

    public static Vector2 Mod289(Vector2 x)
    {
        return new Vector2(
            x.x - Mathf.Floor(x.x / 289) * 289,
            x.y - Mathf.Floor(x.y / 289) * 289
        );
    }

    public static Vector3 Mod289(Vector3 x)
    {
        return new Vector3(
            x.x - Mathf.Floor(x.x / 289) * 289,
            x.y - Mathf.Floor(x.y / 289) * 289,
            x.z - Mathf.Floor(x.z / 289) * 289
        );
    }

    public static Vector4 Mod289(Vector4 x)
    {
        return new Vector4(
            x.x - Mathf.Floor(x.x / 289) * 289,
            x.y - Mathf.Floor(x.y / 289) * 289,
            x.z - Mathf.Floor(x.z / 289) * 289,
            x.w - Mathf.Floor(x.w / 289) * 289
        );
    }

    public static Vector3 Permute(Vector3 x)
    {
        return Mod289((x * 34 + x));
    }

    public static Vector4 Permute(Vector4 x)
    {
        return Mod289((x * 34 + x));
    }

    public static Vector3 SimplexNoiseGrad(Vector2 v)
    {
        float C1 = (3f - Mathf.Sqrt(3f)) / 6f;
        float C2 = (Mathf.Sqrt(3f) - 1f) / 2f;

        // First corner
        Vector2 i = new Vector2(Mathf.Floor(v.x + Vector2.Dot(v, new Vector2(C2, C2))), Mathf.Floor(v.y + Vector2.Dot(v, new Vector2(C2, C2))));
        Vector2 x0 = v - i + new Vector2(Vector2.Dot(i, new Vector2(C1, C1)), Vector2.Dot(i, new Vector2(C1, C1)));

        // Other corners
        Vector2 i1 = x0.x > x0.y ? new Vector2(1f, 0f) : new Vector2(0f, 1f);
        Vector2 x1 = x0 + new Vector2(C1, C1) - i1;
        Vector2 x2 = x0 + new Vector2(C1 * 2, C1 * 2) - new Vector2(1f, 1f);

        // Permutations
        i = Mod289(i);
        Vector3 p = Permute(new Vector3(i.y + 0f, i.y + i1.y, i.y + 1f));
        p = Permute(p + new Vector3(i.x + 0f, i.x + i1.x, i.x + 1f));

        // Gradients: 41 points uniformly over a unit circle.
        // The ring size 17*17 = 289 is close to a multiple of 41 (41*7 = 287)
        Vector3 phi = p / 41f * Mathf.PI * 2f;
        Vector2 g0 = new Vector2(Mathf.Cos(phi.x), Mathf.Sin(phi.x));
        Vector2 g1 = new Vector2(Mathf.Cos(phi.y), Mathf.Sin(phi.y));
        Vector2 g2 = new Vector2(Mathf.Cos(phi.z), Mathf.Sin(phi.z));

        // Compute noise and gradient at P
        Vector3 m = new Vector3(Vector2.Dot(x0, x0), Vector2.Dot(x1, x1), Vector2.Dot(x2, x2));
        Vector3 px = new Vector3(Vector2.Dot(g0, x0), Vector2.Dot(g1, x1), Vector2.Dot(g2, x2));

        m = Vector3.Max(Vector3.zero, 0.5f*Vector3.one - m);
        Vector3 m3 = new Vector3(m.x * m.x * m.x, m.y * m.y * m.y, m.z * m.z * m.z);
        Vector3 m4 = new Vector3(m.x * m3.x, m.y * m3.y, m.z * m3.z);

        Vector3 temp = -8f * new Vector3(m3.x * px.x, m3.y * px.y, m3.z * px.z);
        Vector2 grad = m4.x * g0 + temp.x * x0 +
                       m4.y * g1 + temp.y * x1 +
                       m4.z * g2 + temp.z * x2;

        return 99.2f * new Vector3(grad.x, grad.y, Vector3.Dot(m4, px));
    }

    public static float SimplexNoise(Vector2 v)
    {
        return SimplexNoiseGrad(v).z;
    }
}
