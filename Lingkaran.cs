using System;
using System.Collections.Generic;
using System.Drawing;

namespace praktikum2
{
    // Algoritma 2.4-2.7 modul: lingkaran dasar, polar, Bresenham.
    internal static class Lingkaran
    {
        // Algoritma 2.4 - iterasi x, y = yc +- sqrt(r^2 - x^2).
        internal static List<Point> Dasar(int xc, int yc, int r)
        {
            List<Point> titik = new List<Point>();
            for (int x = -r; x <= r; x++)
            {
                double isi = (double)r * r - (double)x * x;
                if (isi < 0) continue;
                int dy = (int)Math.Round(Math.Sqrt(isi));
                titik.Add(new Point(xc + x, yc + dy));
                titik.Add(new Point(xc + x, yc - dy));
            }
            return Unik(titik);
        }

        // Algoritma 2.5 - x = r cos, y = r sin.
        internal static List<Point> Polar(int xc, int yc, int r)
        {
            List<Point> titik = new List<Point>();
            double langkah = 1.0 / Math.Max(1, r);
            for (double a = 0; a < 2 * Math.PI; a += langkah)
            {
                int x = (int)Math.Round(r * Math.Cos(a));
                int y = (int)Math.Round(r * Math.Sin(a));
                titik.Add(new Point(xc + x, yc + y));
            }
            return Unik(titik);
        }

        // Algoritma 2.6 + 2.7 - Bresenham dengan simetri 8 ok.
        internal static List<Point> Bresenham(int xc, int yc, int r)
        {
            List<Point> titik = new List<Point>();
            int x = 0, y = r;
            int d = 3 - 2 * r;
            Plot8(titik, xc, yc, x, y);
            while (x <= y)
            {
                x++;
                if (d < 0) d += 4 * x + 6;
                else { d += 4 * (x - y) + 10; y--; }
                Plot8(titik, xc, yc, x, y);
            }
            return Unik(titik);
        }

        private static void Plot8(List<Point> titik, int xc, int yc, int x, int y)
        {
            titik.Add(new Point(xc + x, yc + y));
            titik.Add(new Point(xc - x, yc + y));
            titik.Add(new Point(xc + x, yc - y));
            titik.Add(new Point(xc - x, yc - y));
            titik.Add(new Point(xc + y, yc + x));
            titik.Add(new Point(xc - y, yc + x));
            titik.Add(new Point(xc + y, yc - x));
            titik.Add(new Point(xc - y, yc - x));
        }

        private static List<Point> Unik(List<Point> titik)
        {
            HashSet<Point> sudah = new HashSet<Point>();
            List<Point> hasil = new List<Point>();
            foreach (Point p in titik)
                if (sudah.Add(p)) hasil.Add(p);
            return hasil;
        }
    }
}