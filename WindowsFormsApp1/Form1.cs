using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
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

        private string currentFilter = "None";

        public Form1()
        {
            InitializeComponent();
            devices = DeviceManager.GetAllDevices();
        }

        private void copyImage_Click(object sender, EventArgs e)
        {
            if (imageB == null)
            {
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

            currentFilter = "Greyscale";

            Bitmap greyscaleImage = new Bitmap(imageB.Width, imageB.Height);
            for (int y = 0; y < imageB.Height; y++)
            {

                for (int x = 0; x < imageB.Width; x++)
                {
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

            currentFilter = "Invert";
            Bitmap invertedImage = new Bitmap(imageB.Width, imageB.Height);

            for (int y = 0; y < imageB.Height; y++)
            {

                for (int x = 0; x < imageB.Width; x++)
                {
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

            currentFilter = "Sepia";

            Bitmap sepiaImage = new Bitmap(imageB.Width, imageB.Height);

            for (int y = 0; y < imageB.Height; y++)
            {
                for (int x = 0; x < imageB.Width; x++)
                {
                    Color c = imageB.GetPixel(x, y);
                    int tr = (int)(0.393 * c.R + 0.769 * c.G + 0.189 * c.B);
                    int tg = (int)(0.349 * c.R + 0.686 * c.G + 0.168 * c.B);
                    int tb = (int)(0.272 * c.R + 0.534 * c.G + 0.131 * c.B);

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
            if (imageB == null)
            {
                pb1_label.Text = "No image loaded!";
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


            // dapat same size ang duha ka images
            if (imageA.Width != imageB.Width || imageA.Height != imageB.Height)
            {
                label3.Text = "Images must be the same size for subtraction!";
                return;
            }


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


        // camera stuffs huu

        private void onCameraDialog_Click(object sender, EventArgs e)
        {
            if (devices.Length > 0)
            {
                currentDevice = devices[0];

                pb1_label.Text = "Using: " + currentDevice.Name;


                currentDevice.Init(pictureBox1.Height, pictureBox1.Width, pictureBox1.Handle.ToInt32());

                frameGrabber = new Timer();
                frameGrabber.Interval = 100;
                frameGrabber.Tick += CaptureFrame;
                frameGrabber.Start();
            }
            else
            {
                MessageBox.Show("No webcam found!");
            }
        }

        // for video filter, giseparate nako for video but same function will be used
        private Bitmap ApplyGreyscale(Bitmap source)
        {
            Bitmap greyscaleImage = new Bitmap(source.Width, source.Height);
            for (int y = 0; y < source.Height; y++)
            {
                for (int x = 0; x < source.Width; x++)
                {
                    Color c = source.GetPixel(x, y);
                    int gray = (c.R + c.G + c.B) / 3;
                    greyscaleImage.SetPixel(x, y, Color.FromArgb(gray, gray, gray));
                }
            }
            return greyscaleImage;
        }

        private Bitmap ApplyInvert(Bitmap source)
        {
            Bitmap invertedImage = new Bitmap(source.Width, source.Height);
            for (int y = 0; y < source.Height; y++)
            {
                for (int x = 0; x < source.Width; x++)
                {
                    Color c = source.GetPixel(x, y);
                    Color invertedColor = Color.FromArgb(255 - c.R, 255 - c.G, 255 - c.B);
                    invertedImage.SetPixel(x, y, invertedColor);
                }
            }
            return invertedImage;
        }

        private Bitmap ApplySepia(Bitmap source)
        {
            Bitmap sepiaImage = new Bitmap(source.Width, source.Height);
            for (int y = 0; y < source.Height; y++)
            {
                for (int x = 0; x < source.Width; x++)
                {
                    Color c = source.GetPixel(x, y);
                    int tr = (int)(0.393 * c.R + 0.769 * c.G + 0.189 * c.B);
                    int tg = (int)(0.349 * c.R + 0.686 * c.G + 0.168 * c.B);
                    int tb = (int)(0.272 * c.R + 0.534 * c.G + 0.131 * c.B);
                    sepiaImage.SetPixel(x, y, Color.FromArgb(Math.Min(255, tr), Math.Min(255, tg), Math.Min(255, tb)));
                }
            }
            return sepiaImage;
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

                        if (imageB != null) imageB.Dispose();
                        imageB = (Bitmap)temp.Clone();
                        pictureBox1.Image = imageB;


                        if (currentFilter != "None")
                        {
                            Bitmap frameCopy = (Bitmap)imageB.Clone();
                            Task.Run(() =>
                            {
                                Bitmap processed = frameCopy;

                                switch (currentFilter)
                                {
                                    case "Greyscale":
                                        processed = ApplyGreyscale(frameCopy);
                                        break;
                                    case "Invert":
                                        processed = ApplyInvert(frameCopy);
                                        break;
                                    case "Sepia":
                                        processed = ApplySepia(frameCopy);
                                        break;
                                }


                                if (pictureBox2.InvokeRequired)
                                {
                                    pictureBox2.Invoke(new Action(() =>
                                    {
                                        if (pictureBox2.Image != null) pictureBox2.Image.Dispose();
                                        pictureBox2.Image = processed;
                                    }));
                                }
                                else
                                {
                                    if (pictureBox2.Image != null) pictureBox2.Image.Dispose();
                                    pictureBox2.Image = processed;
                                }
                            });
                        }
                        else
                        {

                            pictureBox2.Image = (Bitmap)imageB.Clone();
                        }
                    }
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





        // convolution filters
        Bitmap resultImage;

        // slower bitmap apply filter

        /*
        private Bitmap applyFilter(Bitmap source, double[,] kernel, double factor = 1.0, double offset = 0.0)
        {
            int width = source.Width;
            int height = source.Height;
            Bitmap result = new Bitmap(width, height);

            for (int y = 1; y < height - 1; y++)
            {
                for (int x = 1; x < width - 1; x++)
                {
                    double r = 0.0, g = 0.0, b = 0.0;

                    for (int ky = -1; ky <= 1; ky++)
                    {
                        for (int kx = -1; kx <= 1; kx++)
                        {
                            Color pixel = source.GetPixel(x + kx, y + ky);
                            double kernelValue = kernel[ky + 1, kx + 1];

                            r += pixel.R * kernelValue;
                            g += pixel.G * kernelValue;
                            b += pixel.B * kernelValue;
                        }
                    }

                    int nr = Math.Min(Math.Max((int)(factor * r + offset), 0), 255);
                    int ng = Math.Min(Math.Max((int)(factor * g + offset), 0), 255);
                    int nb = Math.Min(Math.Max((int)(factor * b + offset), 0), 255);

                    result.SetPixel(x, y, Color.FromArgb(nr, ng, nb));
                }
            }
            return result;
        }
        */

        // faster bitmap apply filter
        
        private Bitmap applyFilter(Bitmap source, double[,] kernel, double factor, double bias)
        {
            if (source.PixelFormat != System.Drawing.Imaging.PixelFormat.Format32bppArgb)
            {
              
                Bitmap tmp = new Bitmap(source.Width, source.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                using (Graphics g = Graphics.FromImage(tmp)) g.DrawImage(source, 0, 0, source.Width, source.Height);
                source = tmp;
            }

            int width = source.Width;
            int height = source.Height;
            Bitmap result = new Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            var srcData = source.LockBits(new Rectangle(0, 0, width, height),
                                         System.Drawing.Imaging.ImageLockMode.ReadOnly,
                                         System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            var dstData = result.LockBits(new Rectangle(0, 0, width, height),
                                          System.Drawing.Imaging.ImageLockMode.WriteOnly,
                                          System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            int bytesPerPixel = 4;
            int srcStride = srcData.Stride;
            int dstStride = dstData.Stride;
            int srcBytes = Math.Abs(srcStride) * height;
            byte[] srcBuffer = new byte[srcBytes];
            byte[] dstBuffer = new byte[srcBytes];

            Marshal.Copy(srcData.Scan0, srcBuffer, 0, srcBytes);
           
            Array.Copy(srcBuffer, dstBuffer, srcBytes);

            int kW = kernel.GetLength(0);
            int kH = kernel.GetLength(1);
            int kCenterX = kW / 2;
            int kCenterY = kH / 2;

          
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    double[] accum = new double[3] { 0.0, 0.0, 0.0 }; // B,G,R

                    for (int ky = -kCenterY; ky <= kCenterY; ky++)
                    {
                        int posY = y + ky;
                        if (posY < 0 || posY >= height) continue;

                        for (int kx = -kCenterX; kx <= kCenterX; kx++)
                        {
                            int posX = x + kx;
                            if (posX < 0 || posX >= width) continue;

                            double kval = kernel[ky + kCenterY, kx + kCenterX];
                            int srcIndex = posY * srcStride + posX * bytesPerPixel;

                            accum[0] += srcBuffer[srcIndex + 0] * kval; // Blue
                            accum[1] += srcBuffer[srcIndex + 1] * kval; // Gren
                            accum[2] += srcBuffer[srcIndex + 2] * kval; // Red
                        }
                    }

                    int dstIndex = y * dstStride + x * bytesPerPixel;
                
                    int b = (int)(factor * accum[0] + bias);
                    int g = (int)(factor * accum[1] + bias);
                    int r = (int)(factor * accum[2] + bias);
                    dstBuffer[dstIndex + 0] = (byte)Math.Max(0, Math.Min(255, b));
                    dstBuffer[dstIndex + 1] = (byte)Math.Max(0, Math.Min(255, g));
                    dstBuffer[dstIndex + 2] = (byte)Math.Max(0, Math.Min(255, r));
                  
                    dstBuffer[dstIndex + 3] = srcBuffer[dstIndex + 3];
                }
            }

            Marshal.Copy(dstBuffer, 0, dstData.Scan0, srcBytes);
            source.UnlockBits(srcData);
            result.UnlockBits(dstData);

            return result;
        }

       

        // matrices
        private readonly double[,] SmoothKernel = {
            { 1, 1, 1 },
            { 1, 1, 1 },
            { 1, 1, 1 }
        };

        private readonly double[,] GaussianBlurKernel = {
            { 1, 2, 1 },
            { 2, 4, 2 },
            { 1, 2, 1 }
        };

        private readonly double[,] SharpenKernel = {
            {  0, -2,  0 },
            { -2, 11, -2 },
            {  0, -2,  0 }
        };

        private readonly double[,] MeanRemovalKernel = {
            { -1, -1, -1 },
            { -1,  9, -1 },
            { -1, -1, -1 }
        };

        // Emboss: Laplascian
        private readonly double[,] EmbossKernel = {
            {  -1,  0,  -1 },
            {   0,  4,   0 },
            {  -1,  0, -11 }
        };

        // Emboss: Horizontal + Vertical
        private readonly double[,] EmbossHVKernel = {
            {  0, -1,  0 },
            { -1,  4, -1 },
            {  0, -1,  0 }
        };

      
        private readonly double[,] EmbossAllKernel = {
            { -1, -1, -1 },
            { -1,  8, -1 },
            { -1, -1, -1 }
        };

        
        private readonly double[,] EmbossLossyKernel = {
            {  1, -2,  1 },
            { -2,  4, -2 },
            { -2,  1, -2 }
        };

       
        private readonly double[,] EmbossHorizontalKernel = {
            {  0,  0,  0 },
            { -1,  2, -1 },
            {  0,  0,  0 }
        };

       
        private readonly double[,] EmbossVerticalKernel = {
            {  0, -1,  0 },
            {  0,  0,  0 },
            {  0,  1,  0 }
        };


        private void smoothFilter_Click(object sender, EventArgs e)
        {
            if (imageB == null)
            {
                pb1_label.Text = "No image loaded!";
                return;
            }

            resultImage = applyFilter(imageB, SmoothKernel, 1.0 / 9.0, 0.0);
            pictureBox3.Image = resultImage;
        }

        private void gaussianBlurFilter_Click(object sender, EventArgs e)
        {
            if (imageB == null)
            {
                pb1_label.Text = "No image loaded!";
                return;
            }

           
            resultImage = applyFilter(imageB, GaussianBlurKernel, 1.0 / 16.0, 0.0);

            if (pictureBox3.Image != null)
                pictureBox3.Image.Dispose();
            pictureBox3.Image = resultImage;
        }

        private void sharpenFilter_Click(object sender, EventArgs e)
        {
            if (imageB == null)
            {
                pb1_label.Text = "No image loaded!";
                return;
            }

            // Strong kernel → scale to avoid overshoot
            resultImage = applyFilter(imageB, SharpenKernel, 1.0 / 3.0, 0.0);

            if (pictureBox3.Image != null)
                pictureBox3.Image.Dispose();
            pictureBox3.Image = resultImage;
        }

        private void meanRemovalFilter_Click(object sender, EventArgs e)
        {
            if (imageB == null)
            {
                pb1_label.Text = "No image loaded!";
                return;
            }

            resultImage = applyFilter(imageB, MeanRemovalKernel, 1.0, 0.0);

            if (pictureBox3.Image != null)
                pictureBox3.Image.Dispose();
            pictureBox3.Image = resultImage;
        }

        private void allDirectionEmbossFilter_Click(object sender, EventArgs e)
        {
            if (imageB == null)
            {
                pb1_label.Text = "No image loaded!";
                return;
            }

            resultImage = applyFilter(imageB, EmbossAllKernel, 1.0, 128.0);

            if (pictureBox3.Image != null)
                pictureBox3.Image.Dispose();
            pictureBox3.Image = resultImage;
        }

        private void lossyEmbossFilter_Click(object sender, EventArgs e)
        {
            if (imageB == null)
            {
                pb1_label.Text = "No image loaded!";
                return;
            }

            resultImage = applyFilter(imageB, EmbossLossyKernel, 1.0, 128.0);

            if (pictureBox3.Image != null)
                pictureBox3.Image.Dispose();
            pictureBox3.Image = resultImage;
        }

        private void horizontalEmbossFilter_Click(object sender, EventArgs e)
        {
            if (imageB == null)
            {
                pb1_label.Text = "No image loaded!";
                return;
            }

            resultImage = applyFilter(imageB, EmbossHorizontalKernel, 1.0, 128.0);
            pictureBox3.Image = resultImage;
        }

        private void verticalEmbossFilter_Click(object sender, EventArgs e)
        {
            if (imageB == null)
            {
                pb1_label.Text = "No image loaded!";
                return;
            }

            resultImage = applyFilter(imageB, EmbossVerticalKernel, 1.0, 128.0);
            pictureBox3.Image = resultImage;
        }

        private void embossFilter_Click(object sender, EventArgs e)
        {
            if (imageB == null)
            {
                pb1_label.Text = "No image loaded!";
                return;
            }

            resultImage = applyFilter(imageB, EmbossHVKernel, 1.0, 128.0);
            pictureBox3.Image = resultImage;
        }

        private void embossLaplascianFilter_Click(object sender, EventArgs e)
        {
            if (imageB == null)
            {
                pb1_label.Text = "No image loaded!";
                return;
            }

            resultImage = applyFilter(imageB, EmbossKernel, 1.0, 128.0);
            pictureBox3.Image = resultImage;
        }

    }

}

// now, im using another virtual camera called manycam and it is suitable with the driver given. but below uses aforge (disregard lang pls)

// implementation below uses aforge cus it cant detect at first my webcam and my obs. it works but laggy.

/*
using AForge.Video;
using AForge.Video.DirectShow;
using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using WebCamLib;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        Bitmap imageA, imageB, greenColor;
        FilterInfoCollection videoDevices;
        VideoCaptureDevice videoSource;

        private string currentFilter = "None";

        public Form1()
        {
            InitializeComponent();
        }

        private void onCameraDialog_Click(object sender, EventArgs e)
        {
            videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);

            if (videoDevices.Count == 0)
            {
                MessageBox.Show("No webcam found!");
                return;
            }

            videoSource = new VideoCaptureDevice(videoDevices[0].MonikerString);

            if (videoSource.VideoCapabilities.Length > 0)
                videoSource.VideoResolution = videoSource.VideoCapabilities[0];

            videoSource.NewFrame += Video_NewFrame;
            videoSource.Start();

            pb1_label.Text = "Using Camera: " + videoDevices[0].Name;
        }

        private void Video_NewFrame(object sender, NewFrameEventArgs eventArgs)
        {
            try
            {
                Bitmap frame = (Bitmap)eventArgs.Frame.Clone();
                Bitmap processedFrame = null;

                if (imageB != null)
                    imageB.Dispose();
                imageB = (Bitmap)frame.Clone();

                if (currentFilter != "None")
                    processedFrame = ApplyFilter(frame);

                if (pictureBox1.InvokeRequired)
                {
                    pictureBox1.Invoke((MethodInvoker)(() =>
                    {

                        pictureBox1.Image?.Dispose();
                        pictureBox1.Image = frame;


                        if (processedFrame != null)
                        {
                            pictureBox2.Image?.Dispose();
                            pictureBox2.Image = processedFrame;
                        }
                    }));
                }
                else
                {
                    pictureBox1.Image?.Dispose();
                    pictureBox1.Image = frame;

                    if (processedFrame != null)
                    {
                        pictureBox2.Image?.Dispose();
                        pictureBox2.Image = processedFrame;
                    }
                }
            }
            catch { }
        }

        private void stopCameraDialog_Click(object sender, EventArgs e)
        {
            if (videoSource != null && videoSource.IsRunning)
            {
                videoSource.SignalToStop();
                videoSource.WaitForStop();
                videoSource.NewFrame -= Video_NewFrame;
                videoSource = null;
            }

            pictureBox1.Image = null;
            imageB = null;
            pb1_label.Text = "Camera stopped.";
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            stopCameraDialog_Click(sender, e);
        }


        private Bitmap ApplyFilter(Bitmap source)
        {
            if (source == null) return null;
            Bitmap result = (Bitmap)source.Clone();

            switch (currentFilter)
            {
                case "Greyscale":
                    for (int y = 0; y < result.Height; y++)
                        for (int x = 0; x < result.Width; x++)
                        {
                            Color c = result.GetPixel(x, y);
                            int gray = (c.R + c.G + c.B) / 3;
                            result.SetPixel(x, y, Color.FromArgb(gray, gray, gray));
                        }
                    break;

                case "Invert":
                    for (int y = 0; y < result.Height; y++)
                        for (int x = 0; x < result.Width; x++)
                        {
                            Color c = result.GetPixel(x, y);
                            result.SetPixel(x, y, Color.FromArgb(255 - c.R, 255 - c.G, 255 - c.B));
                        }
                    break;

                case "Sepia":
                    for (int y = 0; y < result.Height; y++)
                        for (int x = 0; x < result.Width; x++)
                        {
                            Color c = result.GetPixel(x, y);
                            int tr = (int)(0.393 * c.R + 0.769 * c.G + 0.189 * c.B);
                            int tg = (int)(0.349 * c.R + 0.686 * c.G + 0.168 * c.B);
                            int tb = (int)(0.272 * c.R + 0.534 * c.G + 0.131 * c.B);
                            tr = Math.Min(255, tr);
                            tg = Math.Min(255, tg);
                            tb = Math.Min(255, tb);
                            result.SetPixel(x, y, Color.FromArgb(tr, tg, tb));
                        }
                    break;

                case "Histogram":


                    if (imageB == null) break;

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
                    break;

                default:
                    return result; // no filter
            }

            return result;
        }

        private void greyscaleImage_Click(object sender, EventArgs e)
        {
            if (imageB == null) return;
            currentFilter = "Greyscale";
            imageB = ApplyFilter(imageB);
            pictureBox1.Image = imageB;
        }

        private void colorInversion_Click(object sender, EventArgs e)
        {
            if (imageB == null) return;
            currentFilter = "Invert";
            imageB = ApplyFilter(imageB);
            pictureBox1.Image = imageB;
        }

        private void sepiaImage_Click(object sender, EventArgs e)
        {
            if (imageB == null) return;
            currentFilter = "Sepia";
            imageB = ApplyFilter(imageB);
            pictureBox1.Image = imageB;
        }

        private void histogramImage_Click(object sender, EventArgs e)
        {
            if (imageB == null) return;
            currentFilter = "Histogram";
            imageB = ApplyFilter(imageB);
            pictureBox1.Image = imageB;
        }
        private void copyImage_Click(object sender, EventArgs e)
        {
            if (imageB == null)
            {
                pb1_label.Text = "No image to copy!";
                return;
            }

            imageA = (Bitmap)imageB.Clone();
            pictureBox2.Image = imageA;
            pb1_label.Text = "Image copied to second box.";
        }


        private void loadImageB_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp";
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    imageB?.Dispose();
                    imageB = new Bitmap(ofd.FileName);
                    pictureBox1.Image = (Bitmap)imageB.Clone();
                    pb1_label.Text = "Image loaded into PictureBox1.";
                }
            }
        }

        private void saveImagePB1_Click(object sender, EventArgs e)
        {
            if (pictureBox1.Image == null) return;
            using (SaveFileDialog sfd = new SaveFileDialog())
            {
                sfd.Filter = "PNG Image|*.png";
                if (sfd.ShowDialog() == DialogResult.OK)
                    pictureBox1.Image.Save(sfd.FileName, System.Drawing.Imaging.ImageFormat.Png);
            }
        }

        private void saveImagePB2_Click(object sender, EventArgs e)
        {
            if (pictureBox2.Image == null) return;
            using (SaveFileDialog sfd = new SaveFileDialog())
            {
                sfd.Filter = "PNG Image|*.png";
                if (sfd.ShowDialog() == DialogResult.OK)
                    pictureBox2.Image.Save(sfd.FileName, System.Drawing.Imaging.ImageFormat.Png);
            }
        }

        private void loadImageA_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp";
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    imageA?.Dispose();
                    imageA = new Bitmap(ofd.FileName);
                    pictureBox2.Image = (Bitmap)imageA.Clone();
                    pb1_label.Text = "Image loaded into PictureBox2.";
                }
            }
        }

        private void saveImagePB3_Click(object sender, EventArgs e)
        {
            if (pictureBox3.Image == null) return;
            using (SaveFileDialog sfd = new SaveFileDialog())
            {
                sfd.Filter = "PNG Image|*.png";
                if (sfd.ShowDialog() == DialogResult.OK)
                    pictureBox3.Image.Save(sfd.FileName, System.Drawing.Imaging.ImageFormat.Png);
            }
        }


        private void subtractionImage_Click(object sender, EventArgs e)
        {
            if (imageA == null || imageB == null)
            {
                label3.Text = "Cannot perform subtraction without two images loaded!";
                return;
            }


            if (imageA.Width != imageB.Width || imageA.Height != imageB.Height)
            {
                label3.Text = "Images must be the same size for subtraction!";
                return;
            }


            greenColor?.Dispose();
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
                        greenColor.SetPixel(x, y, backpixel);
                    else
                        greenColor.SetPixel(x, y, pixel);
                }
            }

            pictureBox3.Image?.Dispose();
            pictureBox3.Image = greenColor;
            label3.Text = "Green screen subtraction applied and shown in PictureBox3.";
        }


    }


}
*/