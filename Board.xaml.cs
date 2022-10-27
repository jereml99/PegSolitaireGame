﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace PegSolitaireGame
{
    /// <summary>
    /// Interaction logic for Board.xaml
    /// </summary>
    public partial class Board : UserControl
    {
        private GameButton[,] gameMap;
        private int boardSize = 7;
        private SelectedPawnButton? selected = null;
        private readonly List<AvailableField> availableFields = new();

        private State? state;

        public Board()
        {
            InitializeComponent();
        }


        public void InitBoard(int boardSize)
        {
            this.boardSize = boardSize;
            InitGrid();
            InitButtonMap();
        }

        private void InitGrid()
        {
            for (int i = 0; i < boardSize; i++)
            {
                BoardGrid.ColumnDefinitions.Add(new ColumnDefinition());
                BoardGrid.RowDefinitions.Add(new RowDefinition());
            }
        }
        private void InitButtonMap()
        {
            BoardGrid.Children.Clear();
            selected = null;
            gameMap = new GameButton[boardSize, boardSize];

            for (int row = 0; row < boardSize; row++)
            {
                for (int column = 0; column < boardSize; column++)
                {
                    if (!isBoard(row, column)) continue;
                    GameButton button = !isEmpty(row, column) ? new PawnButton(this) : new EmptyField(this);
                    Grid.SetColumn(button, column);
                    Grid.SetRow(button, row);
                    gameMap[row, column] = button;
                    BoardGrid.Children.Add(button);

                }
            }
        }

        public T ChangeState<T>(GameButton button, T newButton) where T : GameButton
        {
            if (button is AvailableField field) availableFields.Remove(field);

            var row = Grid.GetRow(button);
            var column  = Grid.GetColumn(button);

            Grid.SetRow(newButton, row);
            Grid.SetColumn(newButton, column);

            gameMap[row, column] = newButton;
            BoardGrid.Children.Remove(button);
            BoardGrid.Children.Add(newButton);

            return newButton;
        }

        public void SetSelected(SelectedPawnButton newSelected, bool emptySelected = false)
        {
            UnSetSelected(emptySelected);

            selected = newSelected;

            SetAvailableFields();
        }

        public void UnSetSelected(bool emptySelected)
        {
            GameButton selectedState = emptySelected ? new EmptyField(this) : new PawnButton(this);
            if (selected != null) { ChangeState(selected, selectedState); }

            selected = null;

            UnSetAvailableField();
        }


        public void btn_Click(object sender, RoutedEventArgs e)
        {
            var button = (GameButton) sender;
            button.Handle();
        }

        public void RemoveBetweenSelectedAnd(SelectedPawnButton newSelected)
        {
            var selectedPoint = selected!.GetPoint();
            var newSelectedPoint = newSelected.GetPoint();
            var vector = new Vector(selectedPoint.X - newSelectedPoint.X, selectedPoint.Y - newSelectedPoint.Y);
            var pawToRemovePoint = newSelectedPoint + (vector / 2);

            var pawnToRemove = gameMap[(int)pawToRemovePoint.X,(int) pawToRemovePoint.Y];
            ChangeState(pawnToRemove, new EmptyField(this));
        }

        public void checkIfEnd()
        {
            var isAnyMoveAvailable = gameMap
                .Cast<GameButton>()
                .Where(button => button is PawnButton or SelectedPawnButton)
                .Select(CheckIfMoveAvailable)
                .Aggregate(false, (x, y) => x || y);

            if (!isAnyMoveAvailable)
            {
                InitButtonMap();
            }
            else
            {
                SaveState();
            }
        }

        private void SetAvailableFields()
        {
            if (selected == null) return;
            var selectedPoint = new Point(Grid.GetRow(selected), Grid.GetColumn(selected));
            foreach (var direction in Direction.AllDirection().Where(direction => IsAvailable(selectedPoint, direction)))
            {
                SetAvailableField(selectedPoint + direction * 2);
            }
        }


        private bool CheckIfMoveAvailable(GameButton button)
        {
            var point = button.GetPoint();
            return Direction.AllDirection()
                .Select(direction => IsAvailable(point, direction))
                .Aggregate(false, (x, y) => x || y);
        }
        private bool IsAvailable(Point point, Vector  direction)
        {
            var pawnPoint = point + direction;
            var emptyPoint = point + 2 * direction;
            try
            {
                return gameMap[(int)pawnPoint.X, (int)pawnPoint.Y] is PawnButton or SelectedPawnButton && gameMap[(int)emptyPoint.X, (int)emptyPoint.Y] is EmptyField or AvailableField;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private void SetAvailableField(Point point)
        {
            var button = gameMap[(int) point.X, (int) point.Y];
            var availableButton = ChangeState(button, new AvailableField(this));
            availableFields.Add(availableButton);
        }

        private void UnSetAvailableField()
        {
            while (availableFields.Count > 0)
            {
                var field = availableFields[0];
                availableFields.RemoveAt(0);
                ChangeState(field, new EmptyField(this));
            }
        }

        private bool isBoard(int x, int y)
        {
            return (x >= 2 && x <= 4) ||
                   (y >= 2 && y <= 4);
        }

        private bool isEmpty(int x, int y)
        {
            return x == 3 && y == 3;
        }

        private void SaveState()
        {
            var newState = new State
            {
                gameMap = gameMap,
                selected = selected,
                availableFields = availableFields,
                boardSize = boardSize,
                lastState = state
            };
            state = newState;
        }


    }


    public static class Direction
    {
        public static Vector up = new Vector(-1, 0);
        public static Vector down = new Vector(1, 0);
        public static Vector rigth = new Vector(0, 1);
        public static Vector left = new Vector(0, -1);

        public static Vector[] AllDirection()
        {
            return new[] {up, down, rigth, left};
        }

    }

    public class State
    {
        public GameButton[,] gameMap
        {
            set => gameMap = value.Copy();
        }

        public int boardSize = 7;

        public SelectedPawnButton? selected
        {
            set => selected = value.Copy();
        }
        public List<AvailableField> availableFields
        {
            set => value.Copy();
        }

        public State? lastState
        {
            set => lastState = value.Copy();
        }
    }

}
