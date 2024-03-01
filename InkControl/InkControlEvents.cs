using System;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;
namespace Scribbles;

public partial class InkControl : Canvas {
   #region Event Handlers -------------------------------------------
   protected override void OnStylusDown (StylusDownEventArgs e) {
      Stylus.Capture (this);
      if (e.StylusDevice == null || mShape == null || mDrawing == null) return;
      Down (e.GetPosition (this));
      if (mUndoneStrokes.Count != 0) mUndoneStrokes.Clear ();
   }
   protected override void OnStylusMove (StylusEventArgs e) {
      if (e.StylusDevice == null || mShape == null || mDrawing == null) return;
      Move (e.GetPosition (this));
      InvalidateVisual ();
   }

   protected override void OnStylusUp (StylusEventArgs e) {
      if (e.StylusDevice == null || mShape == null || mDrawing == null) return;
      Up (e.GetPosition (this));
      InvalidateVisual ();
      Stylus.Capture (null);
   }

   protected override void OnMouseLeftButtonDown (MouseButtonEventArgs e) {
      if (e.StylusDevice != null || mDrawing == null) return;
      Down (e.GetPosition (this));
      if (mUndoneStrokes.Count != 0) mUndoneStrokes.Clear ();
   }

   protected override void OnMouseMove (MouseEventArgs e) {
      if (e.StylusDevice != null || e.LeftButton == MouseButtonState.Released || mShape == null) return;
      Move (e.GetPosition (this));
      InvalidateVisual ();
   }

   protected override void OnMouseLeftButtonUp (MouseButtonEventArgs e) {
      if (e.StylusDevice != null || mShape == null) return;
      Up (e.GetPosition (this));
      InvalidateVisual ();
   }

   protected override void OnMouseRightButtonDown (MouseButtonEventArgs e) {
      mDrawing?.Shapes.ForEach (mUndoneStrokes.Push);
      mDrawing?.Shapes.Clear ();
      InvalidateVisual ();
   }
   #endregion

   #region Implementation -------------------------------------------
   void Up (System.Windows.Point pt) {
      switch (mShape) {
         case Line line:
            line.End = new (pt.X, pt.Y);
            mShape = new Line ();
            break;
         case CLine cLine:
            if (cLine.Points.Count > 1) cLine.Points[^1] = new (pt.X, pt.Y);
            break;
         case Doodle dood:
            dood.Add (pt);
            mShape = new Doodle ();
            break;
         case Rect rect:
            rect.BottomRight = new (pt.X, pt.Y);
            mShape = new Rect (false);
            break;
         case Circle1 circle:
            circle.RadiusX = circle.RadiusY = Distance (pt.X, circle.Center.X, pt.Y, circle.Center.Y);
            mShape = new Circle1 ();
            break;
         case Ellipse eli:
            eli.RadiusX = Distance (pt.X, eli.Center.X, 0, 0);
            eli.RadiusY = Distance (0, 0, pt.Y, eli.Center.Y);
            mShape = new Ellipse ();
            break;
         case Arc arc:
            arc.EndPoint = new (pt.X, pt.Y);
            arc.Radius = Distance (pt.X, arc.StartPoint.X, pt.Y, arc.StartPoint.Y);
            mShape = new Arc ();
            break;
      };
   }

   void Move (System.Windows.Point pt) {
      switch (mShape) {
         case Line line:
            line.End = new (pt.X, pt.Y); break;
         case CLine cLine:
            if (cLine.Points.Count > 1) cLine.Points[^1] = new (pt.X, pt.Y);
            else cLine.Points.Add (new (pt.X, pt.Y));
            break;
         case Doodle dood:
            dood.Add (pt);
            break;
         case Rect rect:
            rect.BottomRight = new (pt.X, pt.Y); break;
         case Circle1 circle:
            circle.RadiusX = circle.RadiusY = Distance (pt.X, circle.Center.X, pt.Y, circle.Center.Y);
            break;
         case Ellipse eli:
            eli.RadiusX = Distance (pt.X, eli.Center.X, 0, 0);
            eli.RadiusY = Distance (0, 0, pt.Y, eli.Center.Y);
            break;
         case Arc arc:
            arc.EndPoint = new (pt.X, pt.Y);
            arc.Radius = Distance (pt.X, arc.StartPoint.X, pt.Y, arc.StartPoint.Y);
            break;
      };
   }

   void Down (System.Windows.Point pt) {
      switch (mShape) {
         case Line line:
            (line.Color, line.Thickness) = (PenColor, PenThickness);
            line.Start = new (pt.X, pt.Y);
            break;
         case CLine cLine:
            (cLine.Color, cLine.Thickness) = (PenColor, PenThickness);
            cLine.Points.Add (cLine.Points.Count == 0 ? new Point (pt.X, pt.Y) : cLine.Points.Last ());
            break;
         case Doodle dood:
            dood.Color = PenColor;
            dood.Thickness = PenColor == Background ? EraserThickness : PenThickness;
            dood.Add (pt);
            break;
         case Rect rect:
            (rect.Color, rect.Thickness) = (PenColor, PenThickness);
            rect.TopLeft = new (pt.X, pt.Y);
            break;
         case Circle1 circle:
            (circle.Color, circle.Thickness) = (PenColor, PenThickness);
            circle.Center = new (pt.X, pt.Y);
            break;
         case Ellipse eli:
            (eli.Color, eli.Thickness) = (PenColor, PenThickness);
            eli.Center = new (pt.X, pt.Y);
            break;
            case Arc arc:
            (arc.Color, arc.Thickness) = (PenColor, PenThickness);
            arc.StartPoint = new (pt.X, pt.Y);
            arc.EndPoint = new (pt.X, pt.Y);
            break;
      };
      mDrawing?.Shapes.Add (mShape!);
   }

   static double Distance (double x1, double x2, double y1, double y2)
      => Math.Sqrt (Math.Pow (x1 - x2, 2) + Math.Pow (y1 - y2, 2));
   #endregion
}
