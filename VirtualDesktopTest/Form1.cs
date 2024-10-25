using System.Runtime.InteropServices;
using System.Timers;

namespace VirtualDesktopTest
{
    public partial class Form1 : Form
    {
        // PInvoke: Masaüstü API'leri
        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr CreateDesktop(string lpszDesktop, IntPtr lpszDevice,
            IntPtr pDevmode, int dwFlags, uint dwDesiredAccess, IntPtr lpsa);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool SwitchDesktop(IntPtr hDesktop);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool CloseDesktop(IntPtr hDesktop);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr OpenInputDesktop(uint dwFlags, bool fInherit, uint dwDesiredAccess);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool SetThreadDesktop(IntPtr hDesktop);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr GetThreadDesktop(int dwThreadId);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern int GetCurrentThreadId();

        private IntPtr hOldDesktop;  // Eski masaüstü (console)
        private IntPtr hNewDesktop;  // Yeni masaüstü
        private const string fileName = "a.txt";  // Kontrol edilecek dosya
        private System.Timers.Timer checkFileTimer;  // Timer

        public Form1()
        {
            InitializeComponent();



            //// Eski masaüstüne dön
            //if (hOldDesktop != IntPtr.Zero)
            //{
            //    SwitchDesktop(hOldDesktop);
            //    SetThreadDesktop(hOldDesktop);
            //}



            //// Yeni masaüstünü serbest býrak
            //if (hNewDesktop != IntPtr.Zero)
            //{
            //    CloseDesktop(hNewDesktop);
            //    hNewDesktop = IntPtr.Zero;
            //}

            // Mevcut (console) masaüstünü kaydet
            hOldDesktop = GetThreadDesktop(GetCurrentThreadId());

            //// Yeni masaüstünü oluþtur
            hNewDesktop = CreateDesktop("MyNewDesktop", IntPtr.Zero, IntPtr.Zero, 0, 0x10000000, IntPtr.Zero);

            if (hNewDesktop != IntPtr.Zero)
            {
                // Yeni masaüstüne geç
                SwitchDesktop(hNewDesktop);
                SetThreadDesktop(hNewDesktop);  // Yeni masaüstünde uygulama baþlat
            }

            // Timer baþlat: 1 saniyede bir dosya kontrolü yap
            checkFileTimer = new System.Timers.Timer(1000);  // 1000 ms = 1 saniye
            checkFileTimer.Elapsed += CheckForFile;
            checkFileTimer.AutoReset = true;
            checkFileTimer.Enabled = true;
        }

        // Dosya kontrolü
        private void CheckForFile(object sender, ElapsedEventArgs e)
        {
            string appDirectory = AppDomain.CurrentDomain.BaseDirectory;  // Programýn çalýþtýðý yerel dizin
            string filePath = Path.Combine(appDirectory, fileName);

            if (File.Exists(filePath))  // a.txt dosyasý var mý kontrol et
            {
                // Timer'ý durdur
                checkFileTimer.Stop();

                // Eski masaüstüne dön
                if (hOldDesktop != IntPtr.Zero)
                {
                    SwitchDesktop(hOldDesktop);
                    SetThreadDesktop(hOldDesktop);
                }

                // Yeni masaüstünü serbest býrak
                if (hNewDesktop != IntPtr.Zero)
                {
                    CloseDesktop(hNewDesktop);
                    hNewDesktop = IntPtr.Zero;
                }

                MessageBox.Show("Eski masaüstüne dönüldü! 'a.txt' dosyasý bulundu.");
            }
        }

        // Button1: Manuel olarak eski (console) masaüstüne geri dön
        private void button1_Click(object sender, EventArgs e)
        {
            // Eski masaüstüne geç
            if (hOldDesktop != IntPtr.Zero)
            {
                SwitchDesktop(hOldDesktop);
                SetThreadDesktop(hOldDesktop);
            }

            // Yeni masaüstünü serbest býrak
            if (hNewDesktop != IntPtr.Zero)
            {
                CloseDesktop(hNewDesktop);
                hNewDesktop = IntPtr.Zero;
            }
        }

        // Uygulama kapanýrken masaüstlerini serbest býrak
        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            base.OnFormClosed(e);

            if (hNewDesktop != IntPtr.Zero)
            {
                CloseDesktop(hNewDesktop);  // Yeni masaüstünü serbest býrak
            }

            if (hOldDesktop != IntPtr.Zero)
            {
                CloseDesktop(hOldDesktop);  // Eski masaüstünü serbest býrak
            }

            // Timer'ý durdur
            checkFileTimer.Stop();
        }

        private async void Form1_Load(object sender, EventArgs e)
        {
            //await Task.Run(() =>
            //{
            //    while (true)
            //    {
            //        // Eski masaüstüne dön
            //        if (hOldDesktop != IntPtr.Zero)
            //        {
            //            SwitchDesktop(hOldDesktop);
            //            SetThreadDesktop(hOldDesktop);
            //        }
            //    }
                
            //});
        }
    }
}
