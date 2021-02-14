using System;
using System.IO;
using System.Drawing;

namespace VkScript_BD
{
    class Images
    {
        public Image ResizeImage(Image ImgToResize, Size Size)
        {
            return (Image)(new Bitmap(ImgToResize, Size));
        }

        public Bitmap ChangeColor(Bitmap Bitmap, int Height, int Width)
        {
            Color NewColor = Color.White;

            for (int i = Width; i < Bitmap.Width; i++)
            {
                for (int j = Height; j < Bitmap.Height; j++)
                {
                    Bitmap.SetPixel(i, j, NewColor);
                }
            }

            return Bitmap;
        }

        public void CombineImages(FileInfo[] Files)
        {
            string FinalImage = "..\\..\\..\\Images\\FinalImage.jpeg";
            int Width = Files.Length >= 6 ? 6 * 200 : Files.Length * 200;
            int Height = Files.Length >= 30 ? 5 * 200 : (int)(Math.Ceiling((double)Files.Length / 6.0)) * 200;

            Bitmap New_Bitmap = new Bitmap(Width, Height);

            Graphics Graphics = Graphics.FromImage(New_Bitmap);
            Graphics.Clear(SystemColors.AppWorkspace);

            Width = 0;
            Height = 0;
            int count = 0;

            for (int i = 0; i < Files.Length; i++)
            {
                if (Height == 1000)
                    break;

                Image img = Image.FromFile(Files[i].FullName);

                if (img.Width < 200 || img.Height < 200)
                {
                    img = ResizeImage(img, new Size(200, 200));
                }

                Graphics.DrawImage(img, new Point(Width, Height));
                Width += 200;
                img.Dispose();

                if ((count + 1) % 6 == 0)
                {
                    Height += 200;
                    Width = 0;
                }
                count++;
            }

            Graphics.Dispose();

            New_Bitmap = ChangeColor(New_Bitmap, Height, Width);
            New_Bitmap.Save(FinalImage, System.Drawing.Imaging.ImageFormat.Jpeg);
            New_Bitmap.Dispose();
        }
    }
}