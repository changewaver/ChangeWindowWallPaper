using System;
using System.Runtime.InteropServices;
using Microsoft.Win32;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Threading;
//using System.IO;

namespace Wallpaper
{
    class Program
    {
        public static int TimeOfSleep= 3000;
        static void Main(string[] args)
        {
            //输入文件目录
            String[] dirs=new String[10];
            for (int i = 0; i < 10; i++)
            {
                Console.Write("请输入要设为壁纸的目录:");
                string dir = Console.ReadLine();
                if (dir != "null")
                {
                    dirs[i] = dir;
                    Console.Write(i);
                }
                else
                {
                    Console.Write(i);
                    break;
                }
                    
            }
            //输入间隔时间
            Console.Write("输入间隔时间(单位毫秒):\n");
            TimeOfSleep = int.Parse(Console.ReadLine());
            /*
            if (TimeOfSleep > 1000)
            {
                Console.Write("更换壁纸开始, 停止请按下Control+C");
            }
            else
            {
                Console.Write("时间间隔太短");
                return ;
            }
            */
            //调整输出流到文件
            StreamWriter nameofWallPaper = new StreamWriter("A:\\ConsoleOutput.txt");
            
            Console.SetOut(nameofWallPaper);

            //写入jpg文件目录到文件
            //下面的foreach没有做好边界检测，等着折腾
            for(int j=0;j<dirs.Length-1;j++)
            {

                try
                {
                    if (dirs[j] == null)
                    {
                        j = 0;
                    }
                    ListFiles(new DirectoryInfo(dirs[j]));
                }
                catch (IOException e)
                {
                    Console.WriteLine(e.Message);
                }
            }
            nameofWallPaper.Flush();
            nameofWallPaper.Close();
            
        }
        //打印文件夹下所有文件的目录，包括子文件夹

        public static void ListFiles(FileSystemInfo info)
        {
            if (!info.Exists) return;

            DirectoryInfo dir = info as DirectoryInfo;
            //如果不是目录
            if (dir == null) return;

            FileSystemInfo[] files = dir.GetFileSystemInfos();
            string sPattern = "^*.jpg$";
            Random rand = new Random();

            for (int i = rand.Next(0,files.Length); i < files.Length; i=i+rand.Next(1,2))
            {
                FileInfo file = files[i] as FileInfo;
                //如果 是 文件 并且 满足 jpg 文件
                if ((file != null) && (System.Text.RegularExpressions.Regex.IsMatch(file.FullName, sPattern)))
                {                    
                    //设置壁纸调用函数入口
                    Setter.SetWallpaper(file.FullName, Style.Fill);
                    System.Threading.Thread.Sleep(TimeOfSleep);
                    //Console.WriteLine(file.FullName);

                }

                //如果 是 子目录，进行递归调用
                else
                    ListFiles(files[i]);
                
            }

        }
        public static void ListFiles(FileSystemInfo info,int name)
        {
            if (!info.Exists) return;

            DirectoryInfo dir = info as DirectoryInfo;
            //如果不是目录
            if (dir == null) return;

            FileSystemInfo[] files = dir.GetFileSystemInfos();
            string sPattern = "^*.jpg$";
            Random rand = new Random();

            for (int i = 0; i < files.Length; i++)
            {
                FileInfo file = files[i] as FileInfo;
                //如果 是 文件 并且 满足 jpg 文件
                if ((file != null) && (System.Text.RegularExpressions.Regex.IsMatch(file.FullName, sPattern)))
                {
                    //设置壁纸调用函数入口
                    Setter.SetWallpaper(file.FullName, Style.Fill);
                    System.Threading.Thread.Sleep(TimeOfSleep);
                    //Console.WriteLine(file.FullName);

                }

                //如果 是 子目录，进行递归调用
                else
                    ListFiles(files[i]);

            }

        }
    }
    public enum Style : int
    {
        Center, Stretch, Fill
    }
    public class Setter
    {
        public const int SetDesktopWallpaper = 20;
        public const int UpdateIniFile = 0x01;
        public const int SendWinIniChange = 0x02;
        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern int SystemParametersInfo(int uAction, int uParam, string lpvParam, int fuWinIni);
        public static void SetWallpaper(string path, Wallpaper.Style style)
        {
            SystemParametersInfo(SetDesktopWallpaper, 0, path, UpdateIniFile | SendWinIniChange);
            RegistryKey key = Registry.CurrentUser.OpenSubKey("Control Panel\\Desktop", true);
            switch (style)
            {
                case Style.Stretch:
                    key.SetValue(@"WallpaperStyle", "2");
                    key.SetValue(@"TileWallpaper", "0");
                    break;
                case Style.Center:
                    key.SetValue(@"WallpaperStyle", "1");
                    key.SetValue(@"TileWallpaper", "0");
                    break;
                case Style.Fill:
                    key.SetValue("WallpaperStyle", "10");
                    key.SetValue("TileWallpaper", "0");
                    break;
                
            }
            key.Close();
        }
    }
}
