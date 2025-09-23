using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WebCamLib;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        Bitmap imageA, imageB, greenColor;

        // for webcam stuff
        Device[] devices;
        Device currentDevice;
        Timer frameGrabber;
        public Form1()
        {
            InitializeComponent();
            devices = DeviceManager.GetAllDevices();
        }

        private void copyImage_Click(object sender, EventArgs e)
        {
            if (imageB == null) {
                pb1_label.Text = "No image loaded!";
                return;
            }

            imageA = (Bitmap)imageB.Clone();
            pictureBox2.Image = imageA;

            pb1_label.Text = "";
        }

        private void greyscaleImage_Click(object sender, EventArgs e)
        {
            if (imageB == null)
            {
                pb1_label.Text = "No image loaded!";
                return;
            }

            Bitmap greyscaleImage = new Bitmap(imageB.Width, imageB.Height);
            for (int y = 0; y < imageB.Height; y++) {

                for (int x = 0; x < imageB.Width; x++) { 
                    Color c = imageB.GetPixel(x, y);
                    // int grey = (int)(0.299 * c.R + 0.587 * c.G + 0.114 * c.B); // luminosity method

                    int gray = (c.R + c.G + c.B) / 3;
                    greyscaleImage.SetPixel(x, y, Color.FromArgb(gray, gray, gray));
                }
            }

            // ang pb1 ra machange if process, dili sa copied image on pb2

            imageB = greyscaleImage;
            pictureBox1.Image = imageB;

        }

        private void colorInversion_Click(object sender, EventArgs e)
        {

            if (imageB == null)
            {
                pb1_label.Text = "No image loaded!";
                return;
            }
            Bitmap invertedImage = new Bitmap(imageB.Width, imageB.Height);

            for (int y = 0; y < imageB.Height; y++) { 
            
                for (int x = 0; x < imageB.Width; x++) { 
                    Color c = imageB.GetPixel(x, y);
                    Color invertedColor = Color.FromArgb(255 - c.R, 255 - c.G, 255 - c.B);
                    invertedImage.SetPixel(x, y, invertedColor);
                }
            }

            imageB = invertedImage;
            pictureBox1.Image = imageB;
        }

        private void sepiaImage_Click(object sender, EventArgs e)
        {

            if (imageB == null)
            {
                pb1_label.Text = "No image loaded!";
                return;
            }
            Bitmap sepiaImage = new Bitmap(imageB.Width, imageB.Height);

            for (int y = 0; y < imageB.Height; y++)
            {
                for (int x = 0; x < imageB.Width; x++)
                {
                    Color c = imageB.GetPixel(x, y);
                    int tr = (int)(0.393 * c.R + 0.769 * c.G + 0.189 * c.B);
                    int tg = (int)(0.349 * c.R + 0.686 * c.G + 0.168 * c.B);
                    int tb = (int)(0.272 * c.R + 0.534 * c.G + 0.131 * c.B);
                    // Clamp values to be within [0, 255]
                    tr = Math.Min(255, tr);
                    tg = Math.Min(255, tg);
                    tb = Math.Min(255, tb);
                    sepiaImage.SetPixel(x, y, Color.FromArgb(tr, tg, tb));
                }
            }

            imageB = sepiaImage;    
            pictureBox1.Image = imageB;
        }

        private void histogramImage_Click(object sender, EventArgs e)
        {
            if (imageB == null) return;

            int[] histogram = new int[256];
            for (int y = 0; y < imageB.Height; y++)
            {
                for (int x = 0; x < imageB.Width; x++)
                {
                    Color c = imageB.GetPixel(x, y);
                    int gray = (c.R + c.G + c.B) / 3;
                    histogram[gray]++;
                }
            }

            int max = histogram.Max();

            int width = 256;
            int height = 100;
            Bitmap histBitmap = new Bitmap(width, height);

            using (Graphics g = Graphics.FromImage(histBitmap))
            {
                g.Clear(Color.White);
                for (int i = 0; i < histogram.Length; i++)
                {
                    int barHeight = (int)((histogram[i] / (float)max) * (height - 10));
                    g.DrawLine(Pens.Black, i, height - 1, i, height - barHeight - 1);
                }

                g.DrawLine(Pens.Black, 0, height - 1, width - 1, height - 1); 
                g.DrawLine(Pens.Black, 0, 0, 0, height - 1);
            }

            if (pictureBox2 != null)
                pictureBox3.Image = histBitmap;
            else 
                pictureBox2.Image = histBitmap;
        }



        // PB1_IMAGE B
        private void loadImageB_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                imageB = new Bitmap(ofd.FileName);
                pictureBox1.Image = imageB;
            }
        }

        private void saveImagePB1_Click(object sender, EventArgs e)
        {
            if (imageB == null) { 
                pb1_label.Text = "No image loaded!";
                return;
            }

            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "PNG Image|*.png|JPEG Image|*.jpg|Bitmap Image|*.bmp";
            sfd.Title = "Save an Image File";

            if (sfd.ShowDialog() == DialogResult.OK) {
                System.Drawing.Imaging.ImageFormat format = System.Drawing.Imaging.ImageFormat.Png;
                string ext = System.IO.Path.GetExtension(sfd.FileName).ToLower();
                if (ext == ".jpg" || ext == ".jpeg")
                    format = System.Drawing.Imaging.ImageFormat.Jpeg;
                else if (ext == ".png")
                    format = System.Drawing.Imaging.ImageFormat.Png;
                else if (ext == ".bmp")
                    format = System.Drawing.Imaging.ImageFormat.Bmp;

                imageB.Save(sfd.FileName, format);
                MessageBox.Show("Image saved successfully.");
            }

        }

        // PB2_IMAGE A
        private void loadImageA_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                imageA = new Bitmap(ofd.FileName);
                pictureBox2.Image = imageA;
            }
        }

        private void saveImagePB2_Click(object sender, EventArgs e)
        {
            if (imageB == null)
            {
                label2.Text = "No image loaded!";
                return;
            }

            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "PNG Image|*.png|JPEG Image|*.jpg|Bitmap Image|*.bmp";
            sfd.Title = "Save an Image File";

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                System.Drawing.Imaging.ImageFormat format = System.Drawing.Imaging.ImageFormat.Png;
                string ext = System.IO.Path.GetExtension(sfd.FileName).ToLower();
                if (ext == ".jpg" || ext == ".jpeg")
                    format = System.Drawing.Imaging.ImageFormat.Jpeg;
                else if (ext == ".png")
                    format = System.Drawing.Imaging.ImageFormat.Png;
                else if (ext == ".bmp")
                    format = System.Drawing.Imaging.ImageFormat.Bmp;

                imageB.Save(sfd.FileName, format);
                label2.Text = "Image saved successfully.";
            }
        }

        // PB3_IMAGE B - A

        // subtraction
        private void subtractionImage_Click(object sender, EventArgs e)
        {
            if (imageA == null || imageB == null)
            {
                label3.Text = "Cannot perform subtraction without two images loaded!";
                return;
            }

            // Ensure both images are the same size
            if (imageA.Width != imageB.Width || imageA.Height != imageB.Height)
            {
                label3.Text = "Images must be the same size for subtraction!";
                return;
            }

            // Initialize the result bitmap
            greenColor = new Bitmap(imageB.Width, imageB.Height);

            Color mygreen = Color.FromArgb(0, 0, 255);
            int greygreen = (mygreen.R + mygreen.G + mygreen.B) / 3;
            int threshold = 5;

            for (int x = 0; x < imageB.Width; x++)
            {
                for (int y = 0; y < imageB.Height; y++)
                {
                    Color pixel = imageB.GetPixel(x, y);
                    Color backpixel = imageA.GetPixel(x, y);

                    int grey = (pixel.R + pixel.G + pixel.B) / 3;
                    int subtractvalue = Math.Abs(grey - greygreen);
                    if (subtractvalue < threshold)
                    {
                        greenColor.SetPixel(x, y, backpixel);
                    }
                    else
                    {
                        greenColor.SetPixel(x, y, pixel);
                    }
                }
            }

            pictureBox3.Image = greenColor;
        }

        private void saveImagePB3_Click(object sender, EventArgs e)
        {
            if (greenColor == null)
            {
                label3.Text = "No image loaded!";
                return;
            }

            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "PNG Image|*.png|JPEG Image|*.jpg|Bitmap Image|*.bmp";
            sfd.Title = "Save an Image File";

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                System.Drawing.Imaging.ImageFormat format = System.Drawing.Imaging.ImageFormat.Png;
                string ext = System.IO.Path.GetExtension(sfd.FileName).ToLower();
                if (ext == ".jpg" || ext == ".jpeg")
                    format = System.Drawing.Imaging.ImageFormat.Jpeg;
                else if (ext == ".png")
                    format = System.Drawing.Imaging.ImageFormat.Png;
                else if (ext == ".bmp")
                    format = System.Drawing.Imaging.ImageFormat.Bmp;

                imageB.Save(sfd.FileName, format);
                label3.Text = "Image saved successfully.";
            }
        }


        // ignore this will be applied to another project :>

        private void onCameraDialog_Click(object sender, EventArgs e)
        {
            if (devices.Length > 0)
            {
                currentDevice = devices[0];

                pb1_label.Text = "Using: " + currentDevice.Name;

               
                currentDevice.Init(pictureBox1.Height, pictureBox1.Width, pictureBox1.Handle.ToInt32());

                frameGrabber = new Timer();
                frameGrabber.Interval = 100; //
                frameGrabber.Tick += CaptureFrame;
                frameGrabber.Start();
            }
            else
            {
                MessageBox.Show("No webcam found!");
            }
        }


        private void CaptureFrame(object sender, EventArgs e)
        {
            if (currentDevice != null)
            {
                currentDevice.Sendmessage();
                IDataObject data = Clipboard.GetDataObject();

                if (data != null && data.GetDataPresent(typeof(Bitmap)))
                {
                    var temp = data.GetData(typeof(Bitmap)) as Bitmap;
                    if (temp != null)
                    {
                        imageB = (Bitmap)temp.Clone();   
                        pictureBox1.Image = imageB;      
                    }
                }
                else
                {
                    
                    return;
                }
            }
        }


        private void stopCameraDialog_Click(object sender, EventArgs e)
        {
            if (currentDevice != null)
            {
                currentDevice.Stop();
                currentDevice = null;
                pictureBox1.Image = null;
            }
        }
        
    }
}
