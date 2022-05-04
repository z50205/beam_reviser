// See https://aka.ms/new-console-template for more information
namespace HelloWorld
{
    using System;
    using System.IO;
    using System.Linq;

    class Program
    {
        static void Main(string[] args)
        {
            string read_path = @"d:\temp\GB_T.GBN";
            string write_path = @"d:\temp\GB_T_M.GBN";
            int pivot = 12;
            // if (!File.Exists(path))
            // {
            //     // Create a f5ile to write to.
            //     using (StreamWriter sw = File.CreateText(path))
            //     {
            //         sw.WriteLine("Hello");
            //         sw.WriteLine("And");
            //         sw.WriteLine("Welcome");
            //     }
            // }
            StreamReader sr = new StreamReader(read_path);
            StreamWriter sw = new StreamWriter(write_path);
            string temp_s = "";
            string[] s = new string[12];
            string[] s_new;
            temp_s = sr.ReadLine();
            while (temp_s != null)
            {
                if (temp_s.Substring(0, 4).Equals("BEAM"))
                {
                    //讀取階段
                    //紀錄該梁名稱->預計判別懸臂梁c
                    s[0] = temp_s;
                    for (int i = 1; i < pivot; i++)
                    {
                        s[i] = sr.ReadLine();
                    }
                    //建立資料階段

                    //修改階段
                    s_new = revise_beam(s);
                    for (int i = 0; i < pivot; i++)
                    {
                        sw.WriteLine(s_new[i]);
                    }
                }
                else if (temp_s.Substring(0, 4).Equals("BNPG"))
                {
                    sw.WriteLine(temp_s);
                }
                temp_s = sr.ReadLine();
                // sw.AutoFlush = true;
                // Console.WriteLine(s.Substring(4));
                // for
                // while (s.Replace(" ", "") != "")
                // {
                //     Console.WriteLine(s);
                //     s =sr.ReadLine();
                // }
                // Console.WriteLine("HELLO");
            }
            sw.Close();
        }
        public static string[] revise_beam(string[] s)
        {
            string[] s_new;
            double breadth = Convert.ToDouble(s[2].Substring(11, 5).Replace(" ", ""));
            double depth = Convert.ToDouble(s[2].Substring(16, 5).Replace(" ", ""));
            s_new = change_third_row(s, breadth, depth);
            s_new =change_cantilever(s_new);
            return s_new;
        }
        public static string[] change_third_row(string[] s, double B, double D)
        {
            string[] s_new = new string[12];

            s_new[0] = s[0];
            s_new[1] = s[1];
            s_new[2] = s[2];
            s_new[3] = s[3];
            s_new[10] = s[10];
            s_new[11] = s[11];
            int total_bar = 0;
            int max_bar = 0;
            string[] bar_new;
            // Console.WriteLine(s.Substring(20, 2).Replace(" ", ""));
            for (int i = 4; i <= 9; i++)
            {
                if (Int32.Parse(s[i].Substring(20, 2).Replace(" ", "")) != 0)
                {
                    //全部鋼筋根數
                    total_bar = Int32.Parse(s[i].Substring(0, 2).Replace(" ", "")) + Int32.Parse(s[i].Substring(10, 2).Replace(" ", "")) + Int32.Parse(s[i].Substring(20, 2).Replace(" ", ""));
                    //可放最大鋼筋根數
                    max_bar = max_num(B, Int32.Parse(s[i].Substring(3, 2).Replace(" ", "")));
                    //改變排列主邏輯
                    bar_new = change_bar(total_bar, max_bar);
                    //輸出並替代該端鋼筋

                    s_new[i] = bar_new[0] + s[i].Substring(2, 8) + bar_new[1] + s[i].Substring(12, 8) + bar_new[2] + s[i].Substring(22, 58);
                    Console.WriteLine( s[0].Substring(0, 17) + "\n原鋼筋排列" + s[i].Substring(0, 2) + " " + s[i].Substring(10, 2) + " " + s[i].Substring(20, 2) + " ");
                    Console.WriteLine("總鋼筋根數" + total_bar + "\n" + "調整後鋼筋排列" + bar_new[0] + " " + bar_new[1] + " " + bar_new[2]);
                    //s_new[i]=bar_new[0]+s[i].Substring(2, 9)+bar_new[1]+s[i].Substring(12, 19)+bar_new[2]+s[i].Substring(22, 80);
                }
                else
                    s_new[i] = s[i];


            }
            //Console.WriteLine(total_bar);

            return s_new;
        }
        public static int max_num(double B, int size)
        {
            double coc = 4.0;
            double rib = 1.6;
            double size_dia = 3.22;
            double size_dia_code = 2.5;
            int n = 0;
            switch (size)
            {
                case 7:
                    size_dia = 2.22;
                    size_dia_code = 2.5;
                    break;
                case 8:
                    size_dia = 2.54;
                    size_dia_code = 2.54;
                    break;
                case 10:
                    size_dia = 3.22;
                    size_dia_code = 3.22;
                    break;
            }
            n = Convert.ToInt32(Math.Floor((B - coc * 2 - 2 * rib - size_dia) / (size_dia + size_dia_code))) + 1;
            return n;
        }
        public static string[] change_bar(int total_bar, int max_bar)
        {
            int[] bar = { 0, 0, 0 };
            string[] bar_str = new string[3];
            int residual_bar = total_bar;
            if (residual_bar > max_bar)
                bar[0] = max_bar;
            else
                bar[0] = residual_bar;
            residual_bar -= bar[0];
            if (residual_bar > max_bar)
                bar[1] = max_bar;
            else
                bar[1] = residual_bar;
            residual_bar -= bar[1];
            bar[2] = residual_bar;

            for (int i = 0; i <= 2; i++)
                bar_str[i] = bar[i].ToString().PadLeft(2, ' ');
            return bar_str;
        }
        public static string[] change_cantilever(string[] s)
        {
            string[] s_new = new string[12];
            string[] s_tempt = s[0].Split(')');
            if (s_tempt[1].Substring(0, 1) == "C" | s_tempt[1].Substring(0, 1) == "c")
            {
                char keyin = '7';
                //while防呆輸入，設定初始值必進入迴圈
                while (!(keyin == '1'||keyin == '2'||keyin == '3'))
                {
                    Console.WriteLine(s[0].Substring(0,17)+"\n本梁前綴為懸臂梁，修改左端：1；修改右端：2；不修改：3\n");
                    keyin = Console.ReadKey(true).KeyChar;
                }
                if(keyin=='1')
                {
                    s[2]=s[2].Substring(0,37)+"15. - 0"+s[2].Substring(44,36);
                }
                if(keyin=='2')
                {
                    s[3]=s[3].Substring(0,37)+"15. - 0"+s[3].Substring(44,36);
                }
            }
            return s;
        }
    }

    // class beam
    // {
    //     public string beam_name;
    //     public string floor;
    //     public
    // }
}

