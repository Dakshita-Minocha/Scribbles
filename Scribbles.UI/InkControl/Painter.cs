using Lib;
using System;
using System.Collections.Generic;
using System.Windows.Media;

namespace Scribbles;

/// <summary>
/// Drawing Context Adapter -- Converts drawing points to Screen Points and then draws the shape.
/// </summary>
class Painter: IDrawer {
   #region Constructor ----------------------------------------------
   public Painter (DrawingContext dc, Matrix InvXfm) { mDC = dc; mXfm = InvXfm; }
   DrawingContext mDC;
   Matrix mXfm;
   #endregion

   #region Properties -----------------------------------------------
   public Brush Color {
      get => mColor;
      set {
         if (mColor == value) return;
         mColor = value;
         mPen = null;
      }
   }
   Brush mColor = Brushes.Black;
   #endregion

   #region Methods --------------------------------------------------
   public void DrawLine (PLine line) {
      mPen = new (line.IsSelected ? Brushes.Blue : Color, 1);
      var pt = line.Points[0];
      var start = mXfm.Transform (new System.Windows.Point (pt.X, pt.Y));
      pt = line.Points[1];
      var end = mXfm.Transform (new System.Windows.Point (pt.X, pt.Y));
      mDC.DrawLine (mPen, start, end);
   }

   public void DrawPoint (Point dood) {
      //mPen ??= new (Color, 1);
      //for (int i = 0; i < dood.Points.Count - 2; i++)
      //   mDC.DrawLine (mPen, new (dood.Points[i].X, dood.Points[i].Y), new (dood.Points[i + 1].X, dood.Points[i + 1].Y));
   }

   public void DrawRect (PLine rect) {
      mPen = new (rect.IsSelected ? Brushes.Blue : Color, 1);
      var cornerA = rect.Points[0];
      var cornerB = rect.Points[1];
      mDC.DrawRectangle (null, mPen, new System.Windows.Rect (new (cornerA.X, cornerA.Y), new System.Windows.Point (cornerB.X, cornerB.Y)));
   }

   public void DrawArc (PLine arc) {
      mPen ??= new Pen (Color, 1);
      var center = arc.Points[0];
      var top = arc.Points[1];
      var side = arc.Points[2];
      if (arc.Flag == PLine.EFlag.Open) mDC.DrawEllipse (null, mPen, new System.Windows.Point (center.X, center.Y), center.DistanceTo(top), center.DistanceTo(side));
      else throw new NotImplementedException ();
   }

   //public void DrawArc (PLine arc) {
   //   throw new KeyNotFoundException ();
      //mPen ??= new (Color, 1);
      //mPF.Segments.Clear ();
      //mPF.StartPoint = new (arc.StartPoint.X, arc.StartPoint.Y);
      //mPF.Segments.Add (new ArcSegment (new (arc.EndPoint.X, arc.EndPoint.Y), new (arc.Radius, arc.Radius), 0, true, 0, true));
      //mDC.DrawGeometry (null, mPen, mPG);
   //}

   public void DrawSelection (SelectionBox box) {
      Color = Brushes.Blue;
      mDC.DrawRectangle (null, mSelectionPen, new (new (box.TopLeft.X, box.TopLeft.Y), new System.Windows.Point (box.BottomRight.X, box.BottomRight.Y)));
   }
   static readonly Pen mSelectionPen = new (Brushes.BlueViolet, 1) { DashStyle = new () { Dashes = new DoubleCollection (new List<double> () { 8, 8 }) } };
   
   public void DrawDrawing (Lib.Drawing dwg) {
      foreach (var shape in dwg.Shapes)
         shape.Draw (this);
   }
   #endregion

   #region Private Data ---------------------------------------------
   Pen? mPen;
   static readonly PathFigure mPF = new ();
   static readonly List<PathFigure> mIPF = new () { mPF };
   static readonly PathGeometry mPG = new (mIPF);
   #endregion
}
