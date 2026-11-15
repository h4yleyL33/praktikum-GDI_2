using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace praktikum2
{
    // Praktikum 2 - Penggambaran Objek Primitif (garis dan lingkaran).
    internal static class Program
    {
        [STAThread]
        private static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(BuatForm());
        }

        internal static Form BuatForm()
        {
            Form form = new Form
            {
                Text = "Praktikum 2 - Objek Primitif",
                ClientSize = new Size(900, 620),
                StartPosition = FormStartPosition.CenterScreen,
                Font = new Font("Segoe UI", 10)
            };

            ComboBox objek = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                Location = new Point(12, 12), Width = 120
            };
            objek.Items.AddRange(new object[] { "Garis", "Lingkaran" });
            objek.SelectedIndex = 0;

            ComboBox algoritma = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                Location = new Point(140, 12), Width = 120
            };
            algoritma.Items.AddRange(new object[] { "Dasar", "DDA", "Bresenham", "Polar" });
            algoritma.SelectedIndex = 2;

            CheckBox tampilTitik = new CheckBox
            {
                Text = "Tampilkan titik", Location = new Point(270, 14), Checked = true
            };
            Button acak = new Button { Text = "Acak", Location = new Point(400, 10), Width = 70 };
            Button bersih = new Button { Text = "Bersihkan", Location = new Point(476, 10), Width = 90 };
            Label info = new Label
            {
                Location = new Point(12, 44), AutoSize = true, ForeColor = Color.DimGray
            };

            PictureBox kanvas = new PictureBox
            {
                Location = new Point(12, 72), Size = new Size(876, 536),
                BackColor = Color.White, BorderStyle = BorderStyle.FixedSingle
            };

            form.Controls.Add(kanvas);
            form.Controls.Add(info);
            form.Controls.Add(objek);
            form.Controls.Add(algoritma);
            form.Controls.Add(tampilTitik);
            form.Controls.Add(acak);
            form.Controls.Add(bersih);

            // Titik ujung garis (atau diameter lingkaran) pada koordinat kanvas.
            Point a = new Point(80, 80);
            Point b = new Point(700, 460);

            Action gambar = () =>
            {
                if (kanvas.Width <= 0 || kanvas.Height <= 0) return;
                string teks;
                Bitmap gambarBaru = BuatGambar(kanvas.Width, kanvas.Height,
                    objek.SelectedItem as string, algoritma.SelectedItem as string,
                    tampilTitik.Checked, a, b, out teks);
                Image lama = kanvas.Image;
                kanvas.Image = gambarBaru;
                if (lama != null) lama.Dispose();
                info.Text = teks;
            };

            objek.SelectedIndexChanged += (s, e) => gambar();
            algoritma.SelectedIndexChanged += (s, e) => gambar();
            tampilTitik.CheckedChanged += (s, e) => gambar();
            acak.Click += (s, e) =>
            {
                Random rnd = new Random();
                a = new Point(rnd.Next(60, 400), rnd.Next(60, 300));
                b = new Point(rnd.Next(400, 760), rnd.Next(300, 520));
                gambar();
            };
            bersih.Click += (s, e) =>
            {
                if (kanvas.Image != null) { kanvas.Image.Dispose(); kanvas.Image = null; }
                info.Text = "";
            };
            form.Shown += (s, e) => gambar();
            form.FormClosed += (s, e) =>
            {
                if (kanvas.Image != null) kanvas.Image.Dispose();
            };
            return form;
        }

        // Menggambar objek + algoritma terpilih ke bitmap, mengembalikan keterangan.
        internal static Bitmap BuatGambar(int lebar, int tinggi, string objek, string algoritma,
            bool tampilTitik, Point a, Point b, out string keterangan)
        {
            Bitmap gambar = new Bitmap(lebar, tinggi);
            using (Graphics g = Graphics.FromImage(gambar))
            {
                g.Clear(Color.White);
                using (Pen kisi = new Pen(Color.FromArgb(235, 235, 235)))
                {
                    for (int x = 0; x < lebar; x += 40) g.DrawLine(kisi, x, 0, x, tinggi);
                    for (int y = 0; y < tinggi; y += 40) g.DrawLine(kisi, 0, y, lebar, y);
                }

                List<Point> titik;
                if (objek == "Lingkaran")
                {
                    int cx = (a.X + b.X) / 2, cy = (a.Y + b.Y) / 2;
                    int r = Math.Max(20, (int)Math.Sqrt(
                        (b.X - cx) * (double)(b.X - cx) + (b.Y - cy) * (double)(b.Y - cy)));
                    titik = algoritma == "Polar" ? Lingkaran.Polar(cx, cy, r)
                          : algoritma == "Dasar" ? Lingkaran.Dasar(cx, cy, r)
                          : Lingkaran.Bresenham(cx, cy, r);
                    keterangan = "Lingkaran pusat(" + cx + "," + cy + ") r=" + r +
                                 " | " + algoritma + " | " + titik.Count + " titik";
                }
                else
                {
                    titik = algoritma == "Dasar" ? Garis.Dasar(a.X, a.Y, b.X, b.Y)
                          : algoritma == "Bresenham" ? Garis.Bresenham(a.X, a.Y, b.X, b.Y)
                          : Garis.Dda(a.X, a.Y, b.X, b.Y);
                    keterangan = "Garis (" + a.X + "," + a.Y + ")->(" + b.X + "," + b.Y + ")" +
                                 " | " + algoritma + " | " + titik.Count + " titik";
                }

                if (tampilTitik)
                {
                    using (Brush kuas = new SolidBrush(Color.Blue))
                        foreach (Point p in titik) g.FillRectangle(kuas, p.X, p.Y, 2, 2);
                }
                else if (titik.Count >= 2)
                {
                    using (Pen pena = new Pen(Color.Blue, 1)) g.DrawLines(pena, titik.ToArray());
                }
            }
            return gambar;
        }
    }
}