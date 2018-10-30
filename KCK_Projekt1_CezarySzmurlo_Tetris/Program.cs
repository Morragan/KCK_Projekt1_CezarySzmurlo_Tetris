using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Windows.Media;
using System.Windows.Threading;

namespace KCK_Projekt1_CezarySzmurlo_Tetris
{
    class Program
    {
        public static string sqr = "■";
        public static int[,] grid = new int[23, 10];
        public static int[,] tetrominoeGrid = new int[23, 10];
        public static Stopwatch timer = new Stopwatch();
        public static Stopwatch dropTimer = new Stopwatch();
        public static int dropTime, dropRate = 300;
        public static bool isDropped = false;
        static Tetromino tet, nexttet;
        static MediaPlayer mp = new MediaPlayer();
        public static ConsoleKeyInfo key;
        public static bool isKeyPressed = false;
        public static int linesCleared = 0, score = 0, level = 1;


        private static void WelcomeScreen()
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("\n\t\t████████╗███████╗████████╗██████╗ ██╗███████╗" +
    "\n\t\t╚══██╔══╝██╔════╝╚══██╔══╝██╔══██╗██║██╔════╝" +
   "\n\t\t   ██║   █████╗     ██║   ██████╔╝██║███████╗" +
   "\n\t\t   ██║   ██╔══╝     ██║   ██╔══██╗██║╚════██║" +
   "\n\t\t   ██║   ███████╗   ██║   ██║  ██║██║███████║" +
   "\n\t\t   ╚═╝   ╚══════╝   ╚═╝   ╚═╝  ╚═╝╚═╝╚══════╝");

            string s = "Witaj!";
            Console.SetCursorPosition((Console.WindowWidth - s.Length) / 2, Console.CursorTop + 1);
            Console.WriteLine(s);
            s = "Twoim zadaniem jest opuszczanie bloków w taki sposób,";
            Console.SetCursorPosition((Console.WindowWidth - s.Length) / 2 + 3, Console.CursorTop);
            Console.WriteLine(s);
            s = "by układały się w poziome linie";
            Console.SetCursorPosition((Console.WindowWidth - s.Length) / 2, Console.CursorTop);
            Console.WriteLine(s);
            s = "Opadające bloki możesz przesuwać przy użyciu strzałek";
            Console.SetCursorPosition((Console.WindowWidth - s.Length) / 2, Console.CursorTop + 1);
            Console.WriteLine(s);
            s = "i obracać je, naciskając spację";
            Console.SetCursorPosition((Console.WindowWidth - s.Length) / 2, Console.CursorTop);
            Console.WriteLine(s);
            s = "Powodzenia :)";
            Console.SetCursorPosition((Console.WindowWidth - s.Length) / 2, Console.CursorTop);
            Console.WriteLine(s);
            s = "Aby zacząć, naciśnij dowolny klawisz";
            Console.SetCursorPosition((Console.WindowWidth - s.Length) / 2, Console.CursorTop + 1);
            Console.WriteLine(s);

            Console.ReadKey(true);
            Console.Clear();
        }

        private static void GameOverScreen()
        {
            string text = @"                         _   __               _              
                        | | / /              (_)             
                        | |/ /   ___   _ __   _   ___   ___  
                        |    \  / _ \ | '_ \ | | / _ \ / __| 
                        | |\  \| (_) || | | || ||  __/| (__  
                        \_| \_/ \___/ |_| |_||_| \___| \___| 
                                __ _  _ __  _   _            
                               / _` || '__|| | | |           
                              | (_| || |   | |_| |           
                               \__, ||_|    \__, |           
                                __/ |        __/ |           
                               |___/        |___/            ";

            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(text);

            text = "Twój wynik: ";
            Console.SetCursorPosition((Console.WindowWidth - text.Length) / 2, Console.CursorTop + 1);
            Console.WriteLine(text);
            text = "Poziom: " + level;
            Console.SetCursorPosition((Console.WindowWidth - text.Length) / 2, Console.CursorTop);
            Console.WriteLine(text);
            text = "Punkty: " + score;
            Console.SetCursorPosition((Console.WindowWidth - text.Length) / 2, Console.CursorTop);
            Console.WriteLine(text);
            text = "Czy chcesz zagrać jeszcze raz? (T/N)";
            Console.SetCursorPosition((Console.WindowWidth - text.Length) / 2, Console.CursorTop + 1);
            Console.WriteLine(text);
        }

        private static void Update()
        {
            while (true)
            {
                dropTime = (int)dropTimer.ElapsedMilliseconds;
                if (dropTime > dropRate)
                {
                    dropTime = 0;
                    dropTimer.Restart();
                    tet.Drop();
                }
                if (isDropped == true)
                {
                    tet = nexttet;
                    nexttet = new Tetromino();
                    tet.Spawn();

                    isDropped = false;
                }
                int j;
                for (j = 0; j < 10; j++)
                {
                    if (tetrominoeGrid[0, j] == 1)
                        return;
                }
                Input();
                ClearBlock();
            }
        }
        private static void ClearBlock()
        {
            int combo = 0;
            for (int i = 0; i < 23; i++)
            {
                int j;
                for (j = 0; j < 10; j++)
                {
                    if (tetrominoeGrid[i, j] == 0)
                        break;
                }
                if (j == 10)
                {
                    linesCleared++;
                    combo++;
                    for (j = 0; j < 10; j++)
                    {
                        tetrominoeGrid[i, j] = 0;
                    }
                    int[,] newTetrominoeGrid = new int[23, 10];
                    for (int k = 1; k < i; k++)
                    {
                        for (int l = 0; l < 10; l++)
                        {
                            newTetrominoeGrid[k + 1, l] = tetrominoeGrid[k, l];
                        }
                    }
                    for (int k = 1; k < i; k++)
                    {
                        for (int l = 0; l < 10; l++)
                        {
                            tetrominoeGrid[k, l] = 0;
                        }
                    }
                    for (int k = 0; k < 23; k++)
                        for (int l = 0; l < 10; l++)
                            if (newTetrominoeGrid[k, l] == 1)
                                tetrominoeGrid[k, l] = 1;
                    Draw();
                }
            }
            if (combo == 1)
                score += 40 * level;
            else if (combo == 2)
                score += 100 * level;
            else if (combo == 3)
                score += 300 * level;
            else if (combo > 3)
                score += 300 * combo * level;

            if (linesCleared < 5) level = 1;
            else if (linesCleared < 10) level = 2;
            else if (linesCleared < 15) level = 3;
            else if (linesCleared < 25) level = 4;
            else if (linesCleared < 35) level = 5;
            else if (linesCleared < 50) level = 6;
            else if (linesCleared < 70) level = 7;
            else if (linesCleared < 90) level = 8;
            else if (linesCleared < 110) level = 9;
            else if (linesCleared < 150) level = 10;


            if (combo > 0)
            {
                Console.SetCursorPosition(25, 0);
                Console.WriteLine("Level " + level);
                Console.SetCursorPosition(25, 1);
                Console.WriteLine("Score " + score);
                Console.SetCursorPosition(25, 2);
                Console.WriteLine("LinesCleared " + linesCleared);
            }

            dropRate = 300 - 22 * level;

        }
        private static void Input()
        {
            if (Console.KeyAvailable)
            {
                key = Console.ReadKey();
                isKeyPressed = true;
            }
            else
                isKeyPressed = false;

            if (key.Key == ConsoleKey.LeftArrow & !tet.IsSomethingLeft() & isKeyPressed)
            {
                for (int i = 0; i < 4; i++)
                {
                    tet.location[i][1] -= 1;
                }
                tet.Update();
            }
            else if (key.Key == ConsoleKey.RightArrow & !tet.IsSomethingRight() & isKeyPressed)
            {
                for (int i = 0; i < 4; i++)
                {
                    tet.location[i][1] += 1;
                }
                tet.Update();
            }
            if (key.Key == ConsoleKey.DownArrow & isKeyPressed)
            {
                tet.Drop();
            }
            if (key.Key == ConsoleKey.UpArrow & isKeyPressed)
            {
                for (; tet.IsSomethingBelow() != true;)
                {
                    tet.Drop();
                }
            }
            if (key.Key == ConsoleKey.Spacebar & isKeyPressed)
            {
                tet.Rotate();
                tet.Update();
            }
        }
        public static void Draw()
        {
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            for (int i = 0; i < 23; ++i)
            {
                for (int j = 0; j < 10; j++)
                {
                    Console.SetCursorPosition(1 + 2 * j, i);
                    if (grid[i, j] == 1 | tetrominoeGrid[i, j] == 1)
                    {
                        Console.SetCursorPosition(1 + 2 * j, i);
                        Console.Write(sqr);
                    }
                    else
                    {
                        Console.Write("  ");
                    }
                }

            }
            Console.ResetColor();
        }

        public static void DrawBorder()
        {
            for (int lengthCount = 0; lengthCount <= 22; ++lengthCount)
            {
                Console.SetCursorPosition(0, lengthCount);
                Console.Write("*");
                Console.SetCursorPosition(21, lengthCount);
                Console.Write("*");
            }
            Console.SetCursorPosition(0, 23);
            for (int widthCount = 0; widthCount <= 10; widthCount++)
            {
                Console.Write("*-");
            }

        }



        static void Main()
        {
            mp.Open(new Uri(@"C:\Users\Admin\source\repos\KCK_Projekt1_CezarySzmurlo_Tetris\KCK_Projekt1_CezarySzmurlo_Tetris\Dźwięki\Game_start.wav"));
            mp.Play();

            WelcomeScreen();

            mp.Stop();
            var p = new Program();
            //p.PlayAudio(@"C:\Users\Admin\source\repos\KCK_Projekt1_CezarySzmurlo_Tetris\KCK_Projekt1_CezarySzmurlo_Tetris\Dźwięki\Gameplay_music.wav");

            mp.Open(new Uri(Path.GetFullPath(@"C:\Users\Admin\source\repos\KCK_Projekt1_CezarySzmurlo_Tetris\KCK_Projekt1_CezarySzmurlo_Tetris\Dźwięki\Gameplay_music.wav")));
            mp.Play();

            timer.Start();
            dropTimer.Start();

            Console.SetCursorPosition(25, 0);
            Console.WriteLine("Poziom: " + level);
            Console.SetCursorPosition(25, 1);
            Console.WriteLine("Punkty: " + score);
            Console.SetCursorPosition(25, 2);
            Console.WriteLine("Linie: " + linesCleared);
            nexttet = new Tetromino();
            tet = nexttet;
            tet.Spawn();
            nexttet = new Tetromino();

            Update();

            //mediaPlayer.Close();
            //p.musicThread.Abort();
            mp.Open(new Uri(@"C:\Users\Admin\source\repos\KCK_Projekt1_CezarySzmurlo_Tetris\KCK_Projekt1_CezarySzmurlo_Tetris\Dźwięki\Game_over.wav"));
            mp.Play();
            Console.Clear();
            Console.SetCursorPosition(0, 0);

            GameOverScreen();

            string input;
            while ((input = Console.ReadKey(true).KeyChar.ToString()) is Object)
            {
                if (input == "t" || input == "T")
                {
                    int[,] grid = new int[23, 10];
                    tetrominoeGrid = new int[23, 10];
                    timer = new Stopwatch();
                    dropTimer = new Stopwatch();
                    dropRate = 300;
                    isDropped = false;
                    isKeyPressed = false;
                    linesCleared = 0;
                    score = 0;
                    level = 1;
                    GC.Collect();
                    Console.Clear();
                    Main();
                }
                else if (input == "n" || input == "N") Environment.Exit(0);
            }
        }
    }
}
