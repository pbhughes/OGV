using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System;
using Infrastructure.Enumerations;

namespace Infrastructure.ExtendedObjects
{
    public class ExtendedTreeView : TreeView
    {
        Point _start;
        Point _end;
        bool _dragging = false;
        public bool Dragging
        {
            get
            {
                return _dragging;
            }

            set
            {
                _dragging = value;
            }
        }
        ExtendedTreeNode _lastNode;
        DropLocations _dropLocation;
        public DropLocations DropLocation
        {
            get
            {
                return _dropLocation;
            }

            set
            {
                _dropLocation = value;
            }
        }

       

        public ExtendedTreeView() : base()
        {
            this.ItemHeight = this.ItemHeight * 2;
          
        }

        internal const int WM_PAINT = 0xF;

       

        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);
           
            if(m.Msg == WM_PAINT &&  _lastNode != null && Dragging )
            {
                System.Diagnostics.Debug.WriteLine("Paint called. Coordinates: ({0},{1}) - ({2},{3})",_start.X,_start.Y, _end.X,_end.Y );
                Graphics G = this.CreateGraphics();
                G.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                DrawDropIndicator(G);
            }
           
        }

        public void MarkNode(ExtendedTreeNode etn, bool dragging , Point currentMouseLocation)
        {
            this.Invalidate();

            if(_lastNode != null)
                _lastNode.BackColor = Color.Transparent;

            etn.BackColor = Color.Cyan;

            _lastNode = etn;
            int xOffSet = etn.Bounds.Width;
            Point top = new Point(etn.Bounds.X + xOffSet, etn.Bounds.Y - 5);
            Point middle = new Point(etn.Bounds.X + xOffSet, etn.Bounds.Y + (etn.Bounds.Height / 2));
            Point bottom = new Point(etn.Bounds.X + xOffSet, etn.Bounds.Y + etn.Bounds.Height);

            double fromTop = Math.Sqrt(Math.Pow(currentMouseLocation.X - top.X, 2) + Math.Pow(currentMouseLocation.Y - top.Y, 2));
            double fromMiddle = Math.Sqrt(Math.Pow(currentMouseLocation.X - middle.X, 2) + Math.Pow(currentMouseLocation.Y - middle.Y, 2));
            double fromBotton = Math.Sqrt(Math.Pow(currentMouseLocation.X - bottom.X, 2) + Math.Pow(currentMouseLocation.Y - bottom.Y, 2));

            DropLocations tempLocation;
            double temp;
            Point tempPoint;
            Point winner;

            if (fromTop < fromBotton)
            {
                temp = fromTop;
                tempPoint = top;
                tempLocation = DropLocations.Top;
            }
            else
            {
                temp = fromBotton;
                tempPoint = bottom;
                tempLocation = DropLocations.Bottom;
            }

            if(temp < fromMiddle)
            {
                winner = tempPoint;
                winner.X = winner.X - xOffSet;
                DropLocation = tempLocation;
            }
            else
            {
                winner = middle;
                DropLocation = DropLocations.Middle;
            }

            _start = winner;
            _end = new Point(_start.X + 150, _start.Y);       
        }

        public void InvalidateNodeMarker()
        {
            this.Invalidate();
            Dragging = false;
        }
        private void DrawDropIndicator(Graphics e)
        {
            Pen pen = new Pen(Color.FromArgb(255, 0, 0, 255), 8);
            pen.StartCap = LineCap.RoundAnchor;
            pen.EndCap = LineCap.RoundAnchor;
            e.DrawLine(pen, _start, _end);
        }
    }
}