using System;
using System.Collections.Generic;

class MazeGame
{
    static char[,] maze;
    static int playerX;
    static int playerY;
    static int exitX;
    static int exitY;
    static bool mazeHidden = false;
    static bool exitHintVisible = false;  // Çıkış ipucu görünme durumu
    static Random random = new Random();
    static ConsoleColor currentBackgroundColor = ConsoleColor.Gray;
    static ConsoleColor[] colorPalette = { ConsoleColor.DarkGreen, ConsoleColor.Black, ConsoleColor.DarkRed, ConsoleColor.DarkYellow };
    static List<Point> route = new List<Point>();

    static void Main()
    {
        StartGame();
    }

    static void StartGame()
    {
        for (int level = 1; level <= 100; level++)
        {
            PrepareMaze(level);

            if (level % 33 == 1)
            {
                currentBackgroundColor = colorPalette[(level / 11) % colorPalette.Length];
            }

            Console.BackgroundColor = currentBackgroundColor;
            Console.Clear();

            ShowMaze();

            Console.WriteLine($"Level {level}");
            Console.WriteLine("Press any key to see the maze...");
            ConsoleKeyInfo keyInfo = Console.ReadKey(true);

            if (keyInfo.KeyChar == 'h')
            {
                mazeHidden = !mazeHidden;
                ShowMaze();
            }
            else if (keyInfo.KeyChar == 'r')
            {
                PrepareMaze(level);
                ShowMaze();
            }
            else if (keyInfo.KeyChar == 'i')
            {
                if (!exitHintVisible)
                {
                    exitHintVisible = true;  // Çıkış ipucu görünme durumu etkinleştiriliyor
                }
                ShowMaze();
            }
            while (true)
            {
                keyInfo = Console.ReadKey(true);
                char movementDirection = keyInfo.KeyChar;

                if (movementDirection == 'h')
                {
                    mazeHidden = !mazeHidden;
                    ShowMaze();
                }
                else if (movementDirection == 'r')
                {
                    PrepareMaze(level);
                    ShowMaze();
                }
                else if (movementDirection == 'i')
                {
                    if (!exitHintVisible)
                    {
                        exitHintVisible = true;  // Çıkış ipucu görünme durumu etkinleştiriliyor
                    }
                    ShowMaze();
                }
                else if (CheckMovement(movementDirection))
                {
                    Console.BackgroundColor = currentBackgroundColor;
                    Console.Clear();
                    ShowMaze();

                    if (playerX == exitX && playerY == exitY)
                    {
                        Console.WriteLine("Congratulations! You reached the exit!");
                        break;
                    }

                    int distance = Math.Abs(playerX - exitX) + Math.Abs(playerY - exitY);
                    if (distance <= 3)
                        Console.ForegroundColor = ConsoleColor.Red;
                    else
                        Console.ForegroundColor = ConsoleColor.Blue;

                    Console.WriteLine(distance <= 3 ? "Warm" : "Cold");
                    Console.ResetColor();
                }
            }
        }

        Console.WriteLine("Game over!");
    }

    static void DrawExitHints()
    {
        int targetX = exitX;
        int targetY = exitY;

        while (playerX != targetX || playerY != targetY)
        {
            maze[playerY, playerX] = '.';
            route.Add(new Point(playerX, playerY));
            if (playerX < targetX) playerX++;
            else if (playerX > targetX) playerX--;

            if (playerY < targetY) playerY++;
            else if (playerY > targetY) playerY--;
        }
    }

    static void InitializePlayer()
    {
        do
        {
            playerX = random.Next(1, maze.GetLength(1) - 1);
            playerY = random.Next(1, maze.GetLength(0) - 1);
        } while (maze[playerY, playerX] == '|');
    }

    static bool CheckMovement(char direction)
    {
        int targetX = playerX;
        int targetY = playerY;

        switch (direction)
        {
            case 'w':
                targetY--;
                break;
            case 's':
                targetY++;
                break;
            case 'a':
                targetX--;
                break;
            case 'd':
                targetX++;
                break;
        }

        if (targetX >= 0 && targetX < maze.GetLength(1) && targetY >= 0 && targetY < maze.GetLength(0) && maze[targetY, targetX] != '|')
        {
            playerX = targetX;
            playerY = targetY;
            return true;
        }

        return false;
    }

    static void PrepareMaze(int level)
    {
        int size = 5 + level * 2;
        maze = new char[size, size];

        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                maze[i, j] = ' ';
            }
        }

        for (int i = 0; i < size; i++)
        {
            maze[i, 0] = '|';
            maze[i, size - 1] = '|';
            maze[0, i] = '|';
            maze[size - 1, i] = '|';
        }

        for (int i = 1; i < size - 1; i++)
        {
            for (int j = 1; j < size - 1; j++)
            {
                if (random.Next(10) < 2)
                {
                    maze[i, j] = '|';
                }
            }
        }

        InitializePlayer();
        InitializeExit(level);

        Console.ForegroundColor = ConsoleColor.White;
        route.Clear();
        exitHintVisible = false;  // Çıkış ipucu görünme durumu sıfırlanıyor
    }

    static void ShowMaze()
    {
        if (mazeHidden)
        {
            Console.Clear();
            Console.WriteLine("The maze is hidden. Press 'h' to reveal it.");
            return;
        }

        Console.ForegroundColor = ConsoleColor.White;
        for (int i = 0; i < maze.GetLength(0); i++)
        {
            for (int j = 0; j < maze.GetLength(1); j++)
            {
                if (i == playerY && j == playerX)
                    Console.Write('©');
                else if (route.Exists(p => p.X == j && p.Y == i))
                    Console.Write('█'); // Display route as black blocks
                else if (exitHintVisible && j == exitX && i == exitY)
                    Console.Write('█');  // Çıkış ipucunu göster
                else
                    Console.Write(maze[i, j]);
            }
            Console.WriteLine();
        }

        Console.ResetColor();

        Console.BackgroundColor = ConsoleColor.Black;
        Console.ForegroundColor = ConsoleColor.DarkGreen;
        Console.Write("Easy/");
        Console.ForegroundColor = ConsoleColor.DarkYellow;
        Console.Write("Medium/");
        Console.ForegroundColor = ConsoleColor.DarkRed;
        Console.Write("Hard");
        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.WriteLine("");
        Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine("If you get stuck or it's impossible to exit, press 'R' to move.");
        Console.ResetColor();
    }

    static void InitializeExit(int level)
    {
        int size = 5 + level * 2;

        do
        {
            exitX = random.Next(1, size - 1);
            exitY = random.Next(1, size - 1);
        } while (maze[exitY, exitX] != ' ');

        maze[exitY, exitX] = ' ';
    }
}

class Point
{
    public int X { get; set; }
    public int Y { get; set; }

    public Point(int x, int y)
    {
        X = x;
        Y = y;
    }
}
