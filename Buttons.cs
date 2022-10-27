using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace PegSolitaireGame
{
    public abstract class GameButton : Button
    {
        protected Board Board;

        protected GameButton(Board board)
        {
            this.Board = board;
            Click += board.btn_Click;
        }
        public virtual void Paint()
        {
            Style = FindResource("Peg") as Style;
        }

        public virtual void Handle()
        {
            return;
        }

        public Point GetPoint()
        {
            return new Point(Grid.GetRow(this), Grid.GetColumn(this));
        }

    }
    public class EmptyField : GameButton
    {
        public EmptyField(Board board) : base(board)
        {
            Paint();
        }

        public sealed override void Paint()
        {
            Background = Brushes.AliceBlue;
            Style = FindResource("Peg") as Style;
        }
    }

    public class AvailableField : GameButton
    {
        public AvailableField(Board board) : base(board)
        {
            Paint();
        }

        public sealed override void Paint()
        {
            base.Paint();
            Background = Brushes.Green;

        }

        public override void Handle()
        {
            var newButton = Board.ChangeState(this, new SelectedPawnButton(Board));
            Board.RemoveBetweenSelectedAnd(newButton);
            Board.SetSelected(newButton, emptySelected: true);
            Board.checkIfEnd();
        }
    }

    public class PawnButton : GameButton
    {
        public PawnButton(Board board) : base(board)
        {
            Paint();
        }

        public sealed override void Paint()
        {
            base.Paint();
            this.Background = Brushes.Gold;
        }

        public override void Handle()
        {
            var newButton = Board.ChangeState(this, new SelectedPawnButton(Board));
            Board.SetSelected(newButton);
        }
    }
    public class SelectedPawnButton : GameButton
    {
        public SelectedPawnButton(Board board) : base(board) { Paint(); }

        public sealed override void Paint()
        {
            base.Paint();
            this.Background = Brushes.Red;
        }

        public override void Handle()
        {
            Board.UnSetSelected(emptySelected: false);
        }
    }
}
