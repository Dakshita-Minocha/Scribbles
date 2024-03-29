﻿using System;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;
namespace Scribbles;

public partial class InkControl : Canvas {
   #region Event Handlers -------------------------------------------
   protected override void OnMouseLeftButtonDown (MouseButtonEventArgs e) {
      if (e.StylusDevice != null || mDrawing == null) return;
      Down (e.GetPosition (this));
      if (mUndoneStrokes.Count != 0) mUndoneStrokes.Clear ();
   }

   protected override void OnMouseLeftButtonUp (MouseButtonEventArgs e) {
      if (e.StylusDevice != null || mShape == null) return;
      Up (e.GetPosition (this));
      InvalidateVisual ();
   }

   protected override void OnMouseMove (MouseEventArgs e) {
      if (e.StylusDevice != null || (e.LeftButton == MouseButtonState.Released && mShape is not (null or CLine))) return;
      Move (e.GetPosition (this));
      InvalidateVisual ();
   }

   protected override void OnMouseRightButtonDown (MouseButtonEventArgs e) {
      mUndoneStrokes.Push (mDrawing);
      mDrawing = new ();
      InvalidateVisual ();
      ChangesSaved = true;
   }

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
   #endregion

   #region Implementation -------------------------------------------
   static double Distance (double x1, double x2, double y1, double y2)
      => Math.Sqrt (Math.Pow (x1 - x2, 2) + Math.Pow (y1 - y2, 2));

   void Down (System.Windows.Point pt) {
      switch (mShape) {
         case Line line:
            (line.Color, line.Thickness) = (PenColor, PenThickness);
            line.Start = new (pt.X, pt.Y);
            break;
         case CLine cLine:
            (cLine.Color, cLine.Thickness) = (PenColor, PenThickness);
            if (cLine.Points.Count == 0) {
               cLine.Points.Add (new Point (pt.X, pt.Y));
               mDrawing?.Shapes.Add (mShape!);
               ChangesSaved = false;
            }
            cLine.Points.Add (new Point (pt.X, pt.Y));
            if (cLine.Points.Count > 2 && cLine.Points.First () == cLine.Points.Last ()) mShape = new CLine ();
            return;
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
         case SelectionBox sb:
            sb.TopLeft = new (pt.X, pt.Y);
            break;
         default: return;
      };
      mDrawing?.Shapes.Add (mShape!);
      ChangesSaved = false;
   }

   void Move (System.Windows.Point pt) {
      switch (mShape) {
         case Line line:
            line.End = new (pt.X, pt.Y); break;
         case CLine cLine:
            if (cLine.Points.Count > 1) cLine.Points[^1] = new (pt.X, pt.Y);
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
         case SelectionBox sb:
            sb.BottomRight = new (pt.X, pt.Y); break;
      };
   }

   void Up (System.Windows.Point pt) {
      switch (mShape) {
         case Line line:
            line.End = new (pt.X, pt.Y);
            mShape = new Line ();
            break;
         case CLine: break;
         case Doodle dood:
            dood.Add (pt);
            mShape = new Doodle ();
            break;
         case Rect rect:
            rect.BottomRight = new (pt.X, pt.Y);
            mShape = new Rect (rect.Fill);
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
         case SelectionBox sb:
            sb.BottomRight = new (pt.X, pt.Y); mDrawing.Shapes.RemoveAt (mDrawing.Shapes.Count - 1); sb.Select (mDrawing); break;
      };
   }
   #endregion
}
