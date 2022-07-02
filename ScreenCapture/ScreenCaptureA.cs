using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Media.Imaging;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Interop;
using System.Runtime.InteropServices;
using System.Windows.Media;
using System.Windows;

namespace ScreenCapture
{
    class ScreenCaptureA
    {
        [DllImport("GDI32.dll")]
        public static extern bool BitBlt(int hdcDest, int nXDest, int nYDest,
       int nWidth, int nHeight, int hdcSrc, int nXSrc, int nYSrc, int dwRop);

        [DllImport("GDI32.dll")]
        public static extern int CreateCompatibleBitmap(int hdc, int nWidth, int nHeight);[DllImport("GDI32.dll")]
        public static extern int CreateCompatibleDC(int hdc);

        [DllImport("GDI32.dll")]
        public static extern bool DeleteDC(int hdc);

        [DllImport("GDI32.dll")]
        public static extern bool DeleteObject(int hObject);


        [DllImport("gdi32.dll")]
        static extern int CreateDC(string lpszDriver, string lpszDevice, string lpszOutput, IntPtr lpInitData);

        [DllImport("GDI32.dll")]
        public static extern int GetDeviceCaps(int hdc, int nIndex);

        [DllImport("GDI32.dll")]
        public static extern int SelectObject(int hdc, int hgdiobj);

        [DllImport("GDI32.dll")]
        public static extern int GetObject(int h, int c, IntPtr pv);

        [DllImport("User32.dll")]
        public static extern int GetDesktopWindow();

        [DllImport("User32.dll")]
        public static extern int GetWindowDC(int hWnd);

        [DllImport("User32.dll")]
        public static extern int ReleaseDC(int hWnd, int hDC);

        [DllImport("User32.dll")]
        public static extern int GetSystemMetrics(int index);

        //function to capture screen section       
        public static void CaptureScreentoClipboard(int x, int y, int wid, int hei)
        {
            ////create DC for the entire virtual screen
            //int hdcSrc = CreateDC("DISPLAY", null, null, IntPtr.Zero);
            //int hdcDest = CreateCompatibleDC(hdcSrc);
            //int hBitmap = CreateCompatibleBitmap(hdcSrc, wid, hei);
            //_ = SelectObject(hdcDest, hBitmap);

            //// set the destination area White - a little complicated
            //Bitmap bmp = new Bitmap(wid, hei);
            //Image ii = (Image)bmp;
            //Graphics gf = Graphics.FromImage(ii);
            //IntPtr hdc = gf.GetHdc();
            ////use whiteness flag to make destination screen white
            //BitBlt(hdcDest, 0, 0, wid, hei, (int)hdc, 0, 0, 0x00FF0062);
            //gf.Dispose();
            //ii.Dispose();
            //bmp.Dispose();

            //System.Drawing.Rectangle totalSize = System.Drawing.Rectangle.Empty;
            //var allScreens = Screen.AllScreens;
            //foreach (Screen s in Screen.AllScreens)
            //    totalSize = System.Drawing.Rectangle.Union(totalSize, s.Bounds);

            //var translateX = -totalSize.X;
            //var translateY = -totalSize.Y;

            ////Now copy the areas from each screen on the destination hbitmap
            //foreach (Screen s in Screen.AllScreens)
            //{
            //    BitBlt(hdcDest, X + translateX, Y + translateX, X1 - X, Y1 - Y, hdcSrc, X, Y,
            //                 0x40000000 | 0x00CC0020); //SRCCOPY AND CAPTUREBLT
            //}

            //// send image to clipboard
            //Image imf = Image.FromHbitmap(new IntPtr(hBitmap));
            //Clipboard.SetImage(imf);
            //DeleteDC(hdcSrc);
            //DeleteDC(hdcDest);
            //DeleteObject(hBitmap);
            //imf.Dispose();
        }

        public static BitmapSource Convert(System.Drawing.Bitmap bitmap)
        {
            var bitmapData = bitmap.LockBits(
                new System.Drawing.Rectangle(0, 0, bitmap.Width, bitmap.Height),
                System.Drawing.Imaging.ImageLockMode.ReadOnly, bitmap.PixelFormat);

            var bitmapSource = BitmapSource.Create(
                bitmapData.Width, bitmapData.Height,
                bitmap.HorizontalResolution, bitmap.VerticalResolution,
                PixelFormats.Bgr24, null,
                bitmapData.Scan0, bitmapData.Stride * bitmapData.Height, bitmapData.Stride);

            bitmap.UnlockBits(bitmapData);

            return bitmapSource;
        }

        public static BitmapSource CopyAllScreen()
        {
            //create DC for the entire virtual screen
            int hdcSrc = CreateDC("DISPLAY", null, null, IntPtr.Zero);
            int Width = GetSystemMetrics(78);
            int Height = GetSystemMetrics(79);

            int hdcDest = CreateCompatibleDC(hdcSrc);
            int hBitmap = CreateCompatibleBitmap(hdcSrc, Width, Height);
            _ = SelectObject(hdcDest, hBitmap);

            BitBlt(hdcDest, 0, 0, Width, Height, hdcSrc, 0, 0,
                             0x40000000 | 0x00CC0020); //SRCCOPY AND CAPTUREBLT

            // send image to clipboard
            //var bitmapSource = Convert(Bitmap.FromHbitmap(new IntPtr(hBitmap)));

            var bitmapSource = Imaging.CreateBitmapSourceFromHBitmap(new IntPtr(hBitmap), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
            DeleteDC(hdcSrc);
            DeleteDC(hdcDest);
            DeleteObject(hBitmap);
            return bitmapSource;
        }
    }
}
