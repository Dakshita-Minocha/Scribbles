using Lib;
using System;
using System.Windows.Media;
namespace Scribbles;

public static class Transform {
   public static Matrix ComputeProjectionXfm (double canvasHeight, double canvasWidth, double canvasMargin, Bound b) {
      // Compute scaling Matrix
      Matrix projectionMatrix = Matrix.Identity;
      double scaleX = (canvasWidth - 2 * canvasMargin) / b.Width,
             scaleY = (canvasHeight - 2 * canvasMargin) / b.Height;
      double scale = Math.Min (scaleX, scaleY);
      projectionMatrix.Scale (scale, scale);
      // Compute translation Matrix
      var pt = projectionMatrix.Transform (new System.Windows.Point (b.MidPt.X, b.MidPt.Y));
      Point projMidPt = new (pt.X, pt.Y);
      Matrix translationMatrix = Matrix.Identity;
      double translateX = b.MidPt.X - projMidPt.X;
      double translateY = b.MidPt.Y - projMidPt.Y;
      translationMatrix.Translate (translateX, translateY);
      // Append
      projectionMatrix.Append (translationMatrix);
      return projectionMatrix;
   }
}

public readonly struct Bound {
   public Bound (Point A, Point B) {
      XMin = Math.Min (A.X, B.X);
      YMin = Math.Min (A.Y, B.Y);
      XMax = Math.Max (A.X, B.X);
      YMax = Math.Max (A.Y, B.Y);
      Width = XMax - XMin;
      Height = YMax - YMin;
      MidPt = new Point (Width / 2, Height / 2);
   }

   public Bound ()
      => this = Empty;

   public double XMin { get; init; }
   public double XMax { get; init; }
   public double YMin { get; init; }
   public double YMax { get; init; }
   public double Width { get; init; }
   public double Height { get; init; }
   public Point MidPt { get; init; }
   public static Bound Empty
      => new (new Point(double.MinValue, double.MaxValue), new Point( double.MinValue, double.MaxValue));
   public bool IsEmpty => !(XMin == double.MinValue || XMax == double.MaxValue || YMin == double.MinValue || YMax == double.MaxValue);
   public Bound Inflated (Point ptAt, double factor)
      => IsEmpty ? this : new Bound () {
         XMin = ptAt.X - (ptAt.X - XMin) * factor,
         XMax = ptAt.X - (XMax - ptAt.X) * factor,
         YMin = ptAt.Y - (ptAt.Y - YMin) * factor,
         YMax = ptAt.Y - (YMax - ptAt.Y) * factor
      };
}