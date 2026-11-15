using System;
using System.Collections.Generic;
using System.Drawing;

namespace praktikum2
{
    // Algoritma 2.1-2.3 modul: garis dasar, DDA, Bresenham.
    // Semua fungsi mengembalikan kumpulan titik piksel (x, y) yang membentuk garis.
    internal static class Garis
    {
        // Algoritma 2.1 - iterasi x, hanya untuk |m| <= 1.
        internal static List<Point> Dasar(int x1, int y1, int x2, int y2)
        {
            List<Point> titik = new List<Point>();
            int dx = x2 - x1;
            int dy = y2 - y1;
            if (dx == 0 && dy == 0) { titik.Add(new Point(x1, y1)); return titik; }
            if (Math.Abs(dx) < Math.Abs(dy))
                throw new ArgumentException("Garis dasar hanya untuk |m| <= 1. Pakai DDA/Bresenham.");

            double m = (double)dy / dx;
            int langkah = dx >= 0 ? 1 : -1;
            double y = y1;
            for (int x = x1; x != x2 + langkah; x += langkah)
            {
                titik.Add(new Point(x, (int)Math.Round(y)));
                y += m;
            }
            return titik;
        }

        // Algoritma 2.2 - iterasi sumbu dengan |delta| terbesar.
        internal static List<Point> Dda(int x1, int y1, int x2, int y2)
        {
            List<Point> titik = new List<Point>();
            int dx = x2 - x1;
            int dy = y2 - y1;
            int langkah = Math.Max(Math.Abs(dx), Math.Abs(dy));
            if (langkah == 0) { titik.Add(new Point(x1, y1)); return titik; }

            double ddx = (double)dx / langkah;
            double ddy = (double)dy / langkah;
            double x = x1, y = y1;
            for (int i = 0; i <= langkah; i++)
            {
                titik.Add(new Point((int)Math.Round(x), (int)Math.Round(y)));
                x += ddx;
                y += ddy;
            }
            return titik;
        }

        // Algoritma 2.3 - hanya integer, tangani 8 oktan.
        internal static List<Point> Bresenham(int x1, int y1, int x2, int y2)
        {
            List<Point> titik = new List<Point>();
            int dx = x2 - x1;
            int dy = y2 - y1;
            int sx = dx >= 0 ? 1 : -1;
            int sy = dy >= 0 ? 1 : -1;
            int adx = Math.Abs(dx);
            int ady = Math.Abs(dy);
            bool curam = adx < ady;
            int da = curam ? ady : adx;
            int db = curam ? adx : ady;

            int grad = 2 * db - da;
            int x = x1, y = y1;
            for (int i = 0; i <= da; i++)
            {
                titik.Add(new Point(x, y));
                if (grad >= 0)
                {
                    if (curam) x += sx; else y += sy;
                    grad += 2 * db - 2 * da;
                }
                else
                {
                    grad += 2 * db;
                }
                if (curam) y += sy; else x += sx;
            }
            return titik;
        }
    }
}