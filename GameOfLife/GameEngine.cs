﻿namespace GameOfLife
{
    using System;
    using System.Collections.Generic;
    using System.Security.Cryptography;
    using System.Text;
    using System.Threading;

    /// <summary>
    /// Handles game logic and objects
    /// </summary>
    class GameEngine
    {
        private readonly List<Game> games;
        private FileManager fileManager;
        private UIManager uiManager;

        public GameEngine()
        {
            games = new List<Game>();
            fileManager = new FileManager();
            uiManager = new UIManager();
        }

        /// <summary>
        /// Inits game object and starts game loop
        /// </summary>
        public void InitGame()
        {
            var isFile = uiManager.IsFromFile();

            var game = isFile ?
                InitFromFile(fileManager.LoadState()) :
                InitRandom(uiManager.GetRows(), uiManager.GetColumns());

            while (!Console.KeyAvailable)
            {
                Draw(game);
                Iterate(game);
                game.IterationCount++;
            }
        }

        /// <summary>
        /// Draws current iteration on screen
        /// </summary>
        /// <param name="game"></param>
        private void Draw(Game game)
        {
            var stringBuilder = new StringBuilder();
            game.CellCount = 0;

            for (var row = 0; row < game.Rows; row++)
            {
                for (var column = 0; column < game.Columns; column++)
                {
                    var cell = game.Grid[row, column];
                    stringBuilder.Append(cell == CellEnum.Alive ? 'O' : ' ');

                    if (cell == CellEnum.Alive)
                        game.CellCount++;
                }
                stringBuilder.Append("\n");
            }

            uiManager.ShowActiveGame(stringBuilder, game.IterationCount, game.CellCount);
            fileManager.SaveState(stringBuilder);
            Thread.Sleep(1000);
        }

        /// <summary>
        /// Updates game grid based on game rules
        /// </summary>
        /// <param name="game"></param>
        private void Iterate(Game game)
        {
            var nextGrid = new CellEnum[game.Rows, game.Columns];

            for (var row = 1; row < game.Rows - 1; row++)
                for (var column = 1; column < game.Columns - 1; column++)
                {
                    var aliveNeighbors = 0;
                    for (var i = -1; i <= 1; i++)
                    {
                        for (var j = -1; j <= 1; j++)
                        {
                            if (i != 0 || j != 0)
                            {
                                aliveNeighbors += game.Grid[row + i, column + j] == CellEnum.Alive ? 1 : 0;
                            }
                        }
                    }

                    var currentCell = game.Grid[row, column];

                    if (currentCell == CellEnum.Alive && aliveNeighbors < 2)
                    {
                        nextGrid[row, column] = CellEnum.Dead;
                    }
                    else if (currentCell == CellEnum.Alive && aliveNeighbors > 3)
                    {
                        nextGrid[row, column] = CellEnum.Dead;
                    }
                    else if (currentCell == CellEnum.Dead && aliveNeighbors == 3)
                    {
                        nextGrid[row, column] = CellEnum.Alive;
                    }
                    else
                    {
                        nextGrid[row, column] = currentCell;
                    }
                }

            game.Grid = nextGrid;
        }

        /// <summary>
        /// Init game object from file data
        /// </summary>
        /// <param name="fileContents"></param>
        /// <returns></returns>
        private Game InitFromFile(string[] fileContents)
        {
            var rows = fileManager.FileRows;
            var columns = fileManager.FileColumns;
            var game = new Game(rows, columns, new CellEnum[rows, columns]);

            for (var row = 0; row < rows; row++)
            {
                for (var column = 0; column < columns; column++)
                {
                    game.Grid[row, column] = (CellEnum)(fileContents[row][column].Equals('O') ? 1 : 0);
                }
            }

            return game;
        }

        /// <summary>
        /// Init game object using random numbers generator
        /// </summary>
        /// <param name="rows"></param>
        /// <param name="columns"></param>
        /// <returns></returns>
        private Game InitRandom(int rows, int columns)
        {
            var game = new Game(rows, columns, new CellEnum[rows, columns]);

            for (var row = 0; row < rows; row++)
            {
                for (var column = 0; column < columns; column++)
                {
                    game.Grid[row, column] = (CellEnum)RandomNumberGenerator.GetInt32(0, 2);
                }
            }

            return game;
        }
    }
}
