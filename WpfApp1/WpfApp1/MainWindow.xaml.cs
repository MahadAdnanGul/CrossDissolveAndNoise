using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private BitmapImage image1;
        private BitmapImage image2;
        private double alpha = 0.0;
        private WriteableBitmap noisyImage;
        private Random random = new Random();
        private double noiseIntensity = 200.0;

        public MainWindow()
        {
            InitializeComponent();

            // Load your two images
            image1 = new BitmapImage(new Uri("C:/Users/Mahad/RiderProjects/WpfApp1/WpfApp1/images/RandomGuy.jpg"));
            image2 = new BitmapImage(new Uri("C:/Users/Mahad/RiderProjects/WpfApp1/WpfApp1/images/Dog.jpg"));

            // Set Image control properties
            imageControl1.Source = image1;
            imageControl1.Stretch = Stretch.Fill;

            //Comment/Uncomment for Cross dissolve
            
            /*imageControl2.Source = image2;
            imageControl2.Stretch = Stretch.Fill;
            
             // Set up a timer to control the cross-dissolve animation
            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(50); // Adjust the interval as needed
            timer.Tick += Timer_Tick;
            timer.Start();*/
            
             
            
            //Comment/Uncomment for Noise
            
             noisyImage = new WriteableBitmap(image1);
       
            AddRandomNoise();
       
            imageControl1.Source = noisyImage;
            

           
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            // Update alpha (blend factor) for cross-dissolve
            alpha += 0.002; // Adjust the step as needed

            // Clamp alpha to the range [0, 1]
            alpha = Math.Min(1.0, Math.Max(0.0, alpha));

            // Update the Opacity for the images
            CrossDissolve();

            // Force a redraw of the controls
            imageControl1.InvalidateVisual();
            imageControl2.InvalidateVisual();
        }

        private void CrossDissolve()
        {
            // Calculate the complement of alpha
            double complementAlpha = 1.0 - alpha;

            // Set Opacity for the images based on alpha
            imageControl1.Opacity = complementAlpha;
            imageControl2.Opacity = alpha;
        }
        
        private void AddRandomNoise()
        {
            // Get the pixel buffer of the WriteableBitmap
            Int32Rect rect = new Int32Rect(0, 0, noisyImage.PixelWidth, noisyImage.PixelHeight);
            int[] pixels = new int[noisyImage.PixelWidth * noisyImage.PixelHeight];
            noisyImage.CopyPixels(rect, pixels, noisyImage.PixelWidth * sizeof(int), 0);

            // Add random noise to each pixel
            for (int i = 0; i < pixels.Length; i++)
            {
                byte noise = (byte)(random.NextDouble() * noiseIntensity);
                pixels[i] = AddNoiseToPixel(pixels[i], noise);
            }

            // Update the pixel buffer
            noisyImage.WritePixels(rect, pixels, noisyImage.PixelWidth * sizeof(int), 0);
        }

        private int AddNoiseToPixel(int pixel, byte noise)
        {
            // Extract the color components from the pixel
            byte blue = (byte)(pixel & 0xFF);
            byte green = (byte)((pixel >> 8) & 0xFF);
            byte red = (byte)((pixel >> 16) & 0xFF);

            // Add noise to each color component
            blue = AddNoiseToColorComponent(blue, noise);
            green = AddNoiseToColorComponent(green, noise);
            red = AddNoiseToColorComponent(red, noise);

            // Recombine the color components into a pixel
            return (int)(blue | (green << 8) | (red << 16) | (0xFF << 24));
        }

        private byte AddNoiseToColorComponent(byte component, byte noise)
        {
            int result = component + (random.Next(2) == 0 ? -noise : noise);
            return (byte)Math.Max(0, Math.Min(255, result));
        }
        
    }
}