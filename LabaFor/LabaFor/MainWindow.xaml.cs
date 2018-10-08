using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Runtime.InteropServices;

namespace LabaFor
{
    public partial class MainWindow : Window
    {
        [DllImport("Kernel32.dll", CharSet = CharSet.Auto)]
        public static extern unsafe uint GetDriveTypeW(string path);

       [DllImport("Kernel32.dll", CharSet = CharSet.Auto)]
        public static extern unsafe bool GetDiskFreeSpaceExW(string path,long *FBAC, long* TB, long* TFB);
        [DllImport("Kernel32.dll", CharSet = CharSet.Auto)]
        public static extern unsafe bool GetDiskFreeSpaceW(string path, int* SPC, int* BPS, int* NFC, int*TNC);

        [DllImport("Kernel32.dll", CharSet = CharSet.Auto)]
        public static extern unsafe uint GetVolumeInformation(string path, StringBuilder NB,int namesize,int*SN,int*MCL,int*FSF,  StringBuilder FSNB,int FSnamesize=64);

        [DllImport("Kernel32.dll", CharSet = CharSet.Auto)]
        public static extern unsafe uint GetLogicalDriveStringsW(int lenght, StringBuilder buffer);

        bool isFullscreen;
        bool isPressed;
        double x, y;

        public MainWindow()
        {
            
            InitializeComponent();
            AllowsTransparency = true;
            CheckDisk();
            isFullscreen = false;
            isPressed = false;
            this.WindowStartupLocation = WindowStartupLocation.Manual;
        }

        private void CheckDisk()
        {

            for (int i = 65; i < 90; i++)
            {
                string call = Convert.ToChar(i) + ":";
                uint code = GetDriveTypeW(call);
                
                switch (code)
                {
                    
                    case 0:
                        break;
                    case 1:
                        break;
                    case 2:
                        this.DiskAnalize.Text += $"Диск {call} - Съемный носитель\n";
                        break;
                    case 3:
                        this.DiskAnalize.Text += $"Диск {call} - Жесткий диск\n";
                        break;
                    case 4:
                        this.DiskAnalize.Text += $"Диск {call} - Сетевой диск\n";
                        break;
                    case 5:
                        this.DiskAnalize.Text += $"Диск {call} - Дисковод\n";
                        break;
                    case 6:
                        this.DiskAnalize.Text += $"Диск {call} - RAM диск\n";
                        break;

                }

                if(code!=1)
                {
                    unsafe
                    {
                        long FBAC = 0;
                        long TB = 0;
                        long TFB = 0;
                        int SPC = 0;
                        int BPS = 0;
                        int NFC = 0;
                        int TNC = 0;
                        StringBuilder NB = new StringBuilder(64);
                        StringBuilder FSNB = new StringBuilder(64);
                        int SN = 0, MCL = 0, FSF = 0;



                        GetVolumeInformation(call, NB, 63, &SN, &MCL, &FSF, FSNB, 63);
                        GetDiskFreeSpaceW(call, &SPC, &BPS, &NFC, &TNC);
                        GetDiskFreeSpaceExW(call, &FBAC, &TB, &TFB);

                        this.DiskAnalize.Text += $"Всего места:{TB/1024/1024} Мб\n";
                        this.DiskAnalize.Text += $"Свободного места:{TFB/1024/1024} Мб\n";
                        this.DiskAnalize.Text += $"Секторов на кластер:{SPC}\n";
                        this.DiskAnalize.Text += $"Байтов в секторе:{BPS} байт\n";
                        this.DiskAnalize.Text += $"Число свободных кластеров:{NFC}\n";
                        this.DiskAnalize.Text += $"Общее число кластеров:{TNC}\n";
                        this.DiskAnalize.Text += $"Метка тома:{NB}\n";
                        this.DiskAnalize.Text += $"Серийный номер:{SN}\n";
                        this.DiskAnalize.Text += $"Файловая система:{FSNB}\n";
                    }
                    this.DiskAnalize.Text += '\n';
                }

            }
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key==Key.Escape)
            {
                this.Close();
            }

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

                this.Close();
            
        }
        
        private void buttonScreen_Click(object sender, RoutedEventArgs e)
        {
            if (isFullscreen)
            {
                this.WindowState = WindowState.Normal;
                isFullscreen = false;
            }
            else
            {
                this.WindowState = WindowState.Maximized;
                isFullscreen = true;
            }
        }
        
        private void buttonMinimize_Click(object sender, RoutedEventArgs e)
        {

                this.WindowState = WindowState.Minimized;
            
        }
        

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            isPressed = true;
            x = e.GetPosition(e.MouseDevice.Captured).X;
            y = e.GetPosition(e.MouseDevice.Captured).Y;
        }

        private void Grid_MouseMove(object sender, MouseEventArgs e)
        {
            if(isPressed && !isFullscreen)
            {

                double nx, ny;
                nx = e.GetPosition(e.MouseDevice.Captured).X;
                ny = e.GetPosition(e.MouseDevice.Captured).Y;
                double rx = nx - x;
                double ry = ny - y;
               

                this.Top = this.Top + ry;
                this.Left = this.Left + rx;
                x = nx;
                y = ny;

            }
        }

        private void Grid_MouseUp(object sender, MouseButtonEventArgs e)
        {
            isPressed = false;
        }
    }
}
