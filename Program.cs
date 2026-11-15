using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace praktikum2
{
    // Praktikum 2 - Algoritma Penggambaran Objek Primitif.
    internal static class Program
    {
        [STAThread]
        private static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(BuatForm());
        }

        private const int Lebar = 250, Tinggi = 250, Jarak = 14;
        private const int MaxNilai = 300;

        private static readonly string[] NamaGaris = { "X1", "Y1", "X2", "Y2" };
        private static readonly string[] NamaLing = { "X", "Y", "Radius" };

        internal static Form BuatForm()
        {
            Form form = new Form
            {
                Text = "Praktikum 2 - Objek Primitif",
                ClientSize = new Size(830, 800),
                StartPosition = FormStartPosition.CenterScreen,
                Font = new Font("Segoe UI", 9)
            };

            // ---- Penggambaran Garis ----
            Label(form, "Penggambaran Garis", 10, 10, true);
            int[] nilaiGaris = { 40, 60, 210, 200 };
            TrackBar[] barGaris = new TrackBar[4];
            Label[] angkaGaris = new Label[4];
            for (int i = 0; i < 4; i++)
            {
                int kx = 14 + i * 200;
                angkaGaris[i] = Label(form, NamaGaris[i], kx, 40);
                barGaris[i] = Slider(form, nilaiGaris[i], kx, 62, 180);
            }
            PictureBox[,] kanvasGaris = BarisKanvas(form, 96, new[] { "DASAR", "DDA", "Bresenham" });
            Button tombolGaris = new Button { Text = "Gambar", Location = new Point(14, 370), Size = new Size(100, 30) };
            Label hasilGaris = Label(form, "", 124, 376);
            form.Controls.Add(tombolGaris);

            // ---- Penggambaran Lingkaran ----
            Label(form, "Penggambaran Lingkaran", 10, 415, true);
            int[] nilaiLing = { 125, 125, 80 };
            TrackBar[] barLing = new TrackBar[3];
            Label[] angkaLing = new Label[3];
            for (int i = 0; i < 3; i++)
            {
                int kx = 14 + i * 265;
                angkaLing[i] = Label(form, NamaLing[i], kx, 445);
                barLing[i] = Slider(form, nilaiLing[i], kx, 467, 245);
            }
            PictureBox[,] kanvasLing = BarisKanvas(form, 502, new[] { "DASAR", "Polar", "Bresenham" });
            Button tombolLing = new Button { Text = "Gambar", Location = new Point(14, 776), Size = new Size(100, 30) };
            Label hasilLing = Label(form, "", 124, 782);
            form.Controls.Add(tombolLing);

            void Segarkan(TrackBar[] bar, Label[] angka, string[] nama)
            {
                for (int i = 0; i < bar.Length; i++)
                    angka[i].Text = nama[i] + " " + bar[i].Value;
            }

            foreach (TrackBar b in barGaris) b.ValueChanged += (s, e) => Segarkan(barGaris, angkaGaris, NamaGaris);
            foreach (TrackBar b in barLing) b.ValueChanged += (s, e) => Segarkan(barLing, angkaLing, NamaLing);

            tombolGaris.Click += (s, e) =>
            {
                int x1 = barGaris[0].Value, y1 = barGaris[1].Value;
                int x2 = barGaris[2].Value, y2 = barGaris[3].Value;
                for (int i = 0; i < 3; i++)
                {
                    List<Point> titik = i == 0 ? Garis.Dasar(x1, y1, x2, y2)
                                   : i == 1 ? Garis.Dda(x1, y1, x2, y2)
                                   : Garis.Bresenham(x1, y1, x2, y2);
                    Gambar(kanvasGaris[i, 0], titik);
                }
                hasilGaris.Text = "Garis (" + x1 + "," + y1 + ")->(" + x2 + "," + y2 + ")";
            };

            tombolLing.Click += (s, e) =>
            {
                int cx = barLing[0].Value, cy = barLing[1].Value, r = Math.Max(1, barLing[2].Value);
                for (int i = 0; i < 3; i++)
                {
                    List<Point> titik = i == 0 ? Lingkaran.Dasar(cx, cy, r)
                                   : i == 1 ? Lingkaran.Polar(cx, cy, r)
                                   : Lingkaran.Bresenham(cx, cy, r);
                    Gambar(kanvasLing[i, 0], titik);
                }
                hasilLing.Text = "Lingkaran pusat(" + cx + "," + cy + ") r=" + r;
            };

            Segarkan(barGaris, angkaGaris, NamaGaris);
            Segarkan(barLing, angkaLing, NamaLing);
            form.Shown += (s, e) => { tombolGaris.PerformClick(); tombolLing.PerformClick(); };
            return form;
        }

        private static Label Label(Form form, string teks, int x, int y, bool tebal = false)
        {
            Label l = new Label
            {
                Text = teks, Location = new Point(x, y), AutoSize = true,
                Font = tebal ? new Font("Segoe UI", 10, FontStyle.Bold) : form.Font
            };
            form.Controls.Add(l);
            return l;
        }

        // Tiga kanvas sejajar, ukuran seragam.
        private static PictureBox[,] BarisKanvas(Form form, int y, string[] judul)
        {
            PictureBox[,] hasil = new PictureBox[3, 1];
            for (int i = 0; i < 3; i++)
            {
                int x = 14 + i * (Lebar + Jarak);
                Label(form, judul[i], x, y - 20, true);
                PictureBox p = new PictureBox
                {
                    Location = new Point(x, y), Size = new Size(Lebar, Tinggi),
                    BackColor = Color.White, BorderStyle = BorderStyle.FixedSingle
                };
                form.Controls.Add(p);
                hasil[i, 0] = p;
            }
            return hasil;
        }

        private static TrackBar Slider(Form form, int nilai, int x, int y, int lebar)
        {
            TrackBar t = new TrackBar
            {
                Location = new Point(x, y), Width = lebar,
                Minimum = 0, Maximum = MaxNilai,
                Value = Math.Min(MaxNilai, Math.Max(0, nilai)),
                TickFrequency = 20
            };
            form.Controls.Add(t);
            return t;
        }

        // Gambar kumpulan titik piksel ke kanvas.
        private static void Gambar(PictureBox kanvas, List<Point> titik)
        {
            if (kanvas.Width <= 0 || kanvas.Height <= 0) return;
            Bitmap gambar = new Bitmap(kanvas.Width, kanvas.Height);
            using (Graphics g = Graphics.FromImage(gambar))
            {
                g.Clear(Color.White);
                using (Pen kisi = new Pen(Color.FromArgb(235, 235, 235)))
                {
                    for (int x = 0; x < kanvas.Width; x += 25) g.DrawLine(kisi, x, 0, x, kanvas.Height);
                    for (int y = 0; y < kanvas.Height; y += 25) g.DrawLine(kisi, 0, y, kanvas.Width, y);
                }
                using (Brush kuas = new SolidBrush(Color.Blue))
                    foreach (Point p in titik) g.FillRectangle(kuas, p.X, p.Y, 2, 2);
            }
            Image lama = kanvas.Image;
            kanvas.Image = gambar;
            if (lama != null) lama.Dispose();
        }
    }
}