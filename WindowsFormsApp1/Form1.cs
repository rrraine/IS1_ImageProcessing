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

    }
}


// implementation below uses aforge cus it cant detect at first my webcam and my obs. it works but laggy.
// now, im using another virtual camera called manycam and it is suitable with the driver given.

//using AForge.Video;
//using AForge.Video.DirectShow;
//using System;
//using System.Drawing;
//using System.Linq;
//using System.Windows.Forms;
//using WebCamLib;

//namespace WindowsFormsApp1
//{
//    public partial class Form1 : Form
//    {
//        Bitmap imageA, imageB, greenColor;
//        FilterInfoCollection videoDevices;
//        VideoCaptureDevice videoSource;

//        private string currentFilter = "None";

//        public Form1()
//        {
//            InitializeComponent();
//        }

//        private void onCameraDialog_Click(object sender, EventArgs e)
//        {
//            videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);

//            if (videoDevices.Count == 0)
//            {
//                MessageBox.Show("No webcam found!");
//                return;
//            }

//            videoSource = new VideoCaptureDevice(videoDevices[0].MonikerString);

//            if (videoSource.VideoCapabilities.Length > 0)
//                videoSource.VideoResolution = videoSource.VideoCapabilities[0];

//            videoSource.NewFrame += Video_NewFrame;
//            videoSource.Start();

//            pb1_label.Text = "Using Camera: " + videoDevices[0].Name;
//        }

//        private void Video_NewFrame(object sender, NewFrameEventArgs eventArgs)
//        {
//            try
//            {
//                Bitmap frame = (Bitmap)eventArgs.Frame.Clone();  
//                Bitmap processedFrame = null;

//                if (imageB != null)
//                    imageB.Dispose();
//                imageB = (Bitmap)frame.Clone(); 

//                if (currentFilter != "None")
//                    processedFrame = ApplyFilter(frame);

//                if (pictureBox1.InvokeRequired)
//                {
//                    pictureBox1.Invoke((MethodInvoker)(() =>
//                    {

//                        pictureBox1.Image?.Dispose();
//                        pictureBox1.Image = frame;


//                        if (processedFrame != null)
//                        {
//                            pictureBox2.Image?.Dispose();
//                            pictureBox2.Image = processedFrame;
//                        }
//                    }));
//                }
//                else
//                {
//                    pictureBox1.Image?.Dispose();
//                    pictureBox1.Image = frame;

//                    if (processedFrame != null)
//                    {
//                        pictureBox2.Image?.Dispose();
//                        pictureBox2.Image = processedFrame;
//                    }
//                }
//            }
//            catch { }
//        }

//        private void stopCameraDialog_Click(object sender, EventArgs e)
//        {
//            if (videoSource != null && videoSource.IsRunning)
//            {
//                videoSource.SignalToStop();
//                videoSource.WaitForStop();
//                videoSource.NewFrame -= Video_NewFrame;
//                videoSource = null;
//            }

//            pictureBox1.Image = null;
//            imageB = null;
//            pb1_label.Text = "Camera stopped.";
//        }

//        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
//        {
//            stopCameraDialog_Click(sender, e);
//        }


//        private Bitmap ApplyFilter(Bitmap source)
//        {
//            if (source == null) return null;
//            Bitmap result = (Bitmap)source.Clone();

//            switch (currentFilter)
//            {
//                case "Greyscale":
//                    for (int y = 0; y < result.Height; y++)
//                        for (int x = 0; x < result.Width; x++)
//                        {
//                            Color c = result.GetPixel(x, y);
//                            int gray = (c.R + c.G + c.B) / 3;
//                            result.SetPixel(x, y, Color.FromArgb(gray, gray, gray));
//                        }
//                    break;

//                case "Invert":
//                    for (int y = 0; y < result.Height; y++)
//                        for (int x = 0; x < result.Width; x++)
//                        {
//                            Color c = result.GetPixel(x, y);
//                            result.SetPixel(x, y, Color.FromArgb(255 - c.R, 255 - c.G, 255 - c.B));
//                        }
//                    break;

//                case "Sepia":
//                    for (int y = 0; y < result.Height; y++)
//                        for (int x = 0; x < result.Width; x++)
//                        {
//                            Color c = result.GetPixel(x, y);
//                            int tr = (int)(0.393 * c.R + 0.769 * c.G + 0.189 * c.B);
//                            int tg = (int)(0.349 * c.R + 0.686 * c.G + 0.168 * c.B);
//                            int tb = (int)(0.272 * c.R + 0.534 * c.G + 0.131 * c.B);
//                            tr = Math.Min(255, tr);
//                            tg = Math.Min(255, tg);
//                            tb = Math.Min(255, tb);
//                            result.SetPixel(x, y, Color.FromArgb(tr, tg, tb));
//                        }
//                    break;

//                case "Histogram":


//                    if (imageB == null) break;

//                    int[] histogram = new int[256];
//                    for (int y = 0; y < imageB.Height; y++)
//                    {
//                        for (int x = 0; x < imageB.Width; x++)
//                        {
//                            Color c = imageB.GetPixel(x, y);
//                            int gray = (c.R + c.G + c.B) / 3;
//                            histogram[gray]++;
//                        }
//                    }

//                    int max = histogram.Max();

//                    int width = 256;
//                    int height = 100;
//                    Bitmap histBitmap = new Bitmap(width, height);

//                    using (Graphics g = Graphics.FromImage(histBitmap))
//                    {
//                        g.Clear(Color.White);
//                        for (int i = 0; i < histogram.Length; i++)
//                        {
//                            int barHeight = (int)((histogram[i] / (float)max) * (height - 10));
//                            g.DrawLine(Pens.Black, i, height - 1, i, height - barHeight - 1);
//                        }

//                        g.DrawLine(Pens.Black, 0, height - 1, width - 1, height - 1);
//                        g.DrawLine(Pens.Black, 0, 0, 0, height - 1);
//                    }

//                    if (pictureBox2 != null)
//                        pictureBox3.Image = histBitmap;
//                    else
//                        pictureBox2.Image = histBitmap;
//                    break;

//                default:
//                    return result; // no filter
//            }

//            return result;
//        }

//        private void greyscaleImage_Click(object sender, EventArgs e)
//        {
//            if (imageB == null) return;
//            currentFilter = "Greyscale";
//            imageB = ApplyFilter(imageB);
//            pictureBox1.Image = imageB;
//        }

//        private void colorInversion_Click(object sender, EventArgs e)
//        {
//            if (imageB == null) return;
//            currentFilter = "Invert";
//            imageB = ApplyFilter(imageB);
//            pictureBox1.Image = imageB;
//        }

//        private void sepiaImage_Click(object sender, EventArgs e)
//        {
//            if (imageB == null) return;
//            currentFilter = "Sepia";
//            imageB = ApplyFilter(imageB);
//            pictureBox1.Image = imageB;
//        }

//        private void histogramImage_Click(object sender, EventArgs e)
//        {
//            if (imageB == null) return;
//            currentFilter = "Histogram";
//            imageB = ApplyFilter(imageB);
//            pictureBox1.Image = imageB;
//        }
//        private void copyImage_Click(object sender, EventArgs e)
//        {
//            if (imageB == null)
//            {
//                pb1_label.Text = "No image to copy!";
//                return;
//            }

//            imageA = (Bitmap)imageB.Clone();
//            pictureBox2.Image = imageA;
//            pb1_label.Text = "Image copied to second box.";
//        }


//        private void loadImageB_Click(object sender, EventArgs e)
//        {
//            using (OpenFileDialog ofd = new OpenFileDialog())
//            {
//                ofd.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp";
//                if (ofd.ShowDialog() == DialogResult.OK)
//                {
//                    imageB?.Dispose();
//                    imageB = new Bitmap(ofd.FileName);
//                    pictureBox1.Image = (Bitmap)imageB.Clone();
//                    pb1_label.Text = "Image loaded into PictureBox1.";
//                }
//            }
//        }

//        private void saveImagePB1_Click(object sender, EventArgs e)
//        {
//            if (pictureBox1.Image == null) return;
//            using (SaveFileDialog sfd = new SaveFileDialog())
//            {
//                sfd.Filter = "PNG Image|*.png";
//                if (sfd.ShowDialog() == DialogResult.OK)
//                    pictureBox1.Image.Save(sfd.FileName, System.Drawing.Imaging.ImageFormat.Png);
//            }
//        }

//        private void saveImagePB2_Click(object sender, EventArgs e)
//        {
//            if (pictureBox2.Image == null) return;
//            using (SaveFileDialog sfd = new SaveFileDialog())
//            {
//                sfd.Filter = "PNG Image|*.png";
//                if (sfd.ShowDialog() == DialogResult.OK)
//                    pictureBox2.Image.Save(sfd.FileName, System.Drawing.Imaging.ImageFormat.Png);
//            }
//        }

//        private void loadImageA_Click(object sender, EventArgs e)
//        {
//            using (OpenFileDialog ofd = new OpenFileDialog())
//            {
//                ofd.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp";
//                if (ofd.ShowDialog() == DialogResult.OK)
//                {
//                    imageA?.Dispose();
//                    imageA = new Bitmap(ofd.FileName);
//                    pictureBox2.Image = (Bitmap)imageA.Clone();
//                    pb1_label.Text = "Image loaded into PictureBox2.";
//                }
//            }
//        }

//        private void saveImagePB3_Click(object sender, EventArgs e)
//        {
//            if (pictureBox3.Image == null) return;
//            using (SaveFileDialog sfd = new SaveFileDialog())
//            {
//                sfd.Filter = "PNG Image|*.png";
//                if (sfd.ShowDialog() == DialogResult.OK)
//                    pictureBox3.Image.Save(sfd.FileName, System.Drawing.Imaging.ImageFormat.Png);
//            }
//        }


//        private void subtractionImage_Click(object sender, EventArgs e)
//        {
//            if (imageA == null || imageB == null)
//            {
//                label3.Text = "Cannot perform subtraction without two images loaded!";
//                return;
//            }


//            if (imageA.Width != imageB.Width || imageA.Height != imageB.Height)
//            {
//                label3.Text = "Images must be the same size for subtraction!";
//                return;
//            }


//            greenColor?.Dispose();
//            greenColor = new Bitmap(imageB.Width, imageB.Height);


//            Color mygreen = Color.FromArgb(0, 0, 255);
//            int greygreen = (mygreen.R + mygreen.G + mygreen.B) / 3;

//            int threshold = 5;

//            for (int x = 0; x < imageB.Width; x++)
//            {
//                for (int y = 0; y < imageB.Height; y++)
//                {
//                    Color pixel = imageB.GetPixel(x, y);   
//                    Color backpixel = imageA.GetPixel(x, y); 

//                    int grey = (pixel.R + pixel.G + pixel.B) / 3;
//                    int subtractvalue = Math.Abs(grey - greygreen);

//                    if (subtractvalue < threshold)
//                        greenColor.SetPixel(x, y, backpixel); 
//                    else
//                        greenColor.SetPixel(x, y, pixel);     
//                }
//            }

//            pictureBox3.Image?.Dispose();
//            pictureBox3.Image = greenColor;
//            label3.Text = "Green screen subtraction applied and shown in PictureBox3.";
//        }


//    }


//}