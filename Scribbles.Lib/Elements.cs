namespace Lib;

public struct Point : IStorable, IDrawable {
   #region Constructors ---------------------------------------------
   public Point (double x, double y) => (X, Y) = (x, y);
   #endregion

   #region Properties -----------------------------------------------
   public double X { get; private set; }
   public double Y { get; private set; }
   public bool IsSelected { get; set; }
   #endregion

   #region Methods --------------------------------------------------
   public void Draw (IDrawer d)
      => d.DrawPoint (this);

   /// <summary>Returns Euclidian distance between this and point p.</summary>
   public double DistanceTo (Point p)
      => Math.Sqrt (Math.Pow (X - p.X, 2) + Math.Pow (Y - p.Y, 2));

   public override bool Equals (object? obj) {
      if (obj == null) return false;
      return (Point)obj == this;
   }

   public override int GetHashCode () => base.GetHashCode ();

   public static IStorable Load (IFileReader reader)
      => reader.ReadPoint ();

   public bool SetSelected (SelectionBox box)
      => IsSelected = X > box.TopLeft.X && Y > box.TopLeft.Y && X < box.BottomRight.X && Y < box.BottomRight.Y;

   public void Save (IFileWriter writer)
      => writer.WritePoint (this);
   #endregion

   #region Operators ------------------------------------------------
   public static bool operator == (Point x, Point y)
      => x.X == y.X && x.Y == y.Y;

   public static bool operator != (Point x, Point y)
      => x.X != y.X || x.Y != y.Y;
   #endregion
}

public class PLine : IDrawable, IStorable {
   #region Constructors ---------------------------------------------
   public PLine () { }
   /// <summary>Creates new open PLine with no curves </summary>
   public PLine (IEnumerable<Point> pts) { mPoints = pts.ToList (); }

   /// <summary>Creates new open PLine with no curves </summary>
   public PLine (params Point[] pts) { mPoints = pts.ToList (); }
   #endregion

   #region Properties -----------------------------------------------
   public EFlag Flag {
      get => mFlag;
      set => mFlag = value;
   }

   public bool HasCurves {
      get => mHasCurves;
      set => mHasCurves = value;
   }

   public List<Point> Points { get => mPoints; }
   public bool IsSelected { get; set; }
   #endregion

   #region Methods --------------------------------------------------
   public static PLine CreateArc () { throw new NotImplementedException (); }
   public static PLine CreateEllipse (Point center, double radiusX, double radiusY)
      => new (center,
         new Point (center.X + radiusX, center.Y + radiusX),
         new Point (center.X + radiusY, center.Y + radiusY)) { Flag = EFlag.Closed, HasCurves = true };
   public static PLine CreateLine (IEnumerable<Point> pts)
      => new (pts);
   public static PLine CreateLine (params Point[] pts)
      => new (pts);
   public static PLine CreateRect (Point start, Point end)
      => new (start, end) { Flag = EFlag.Closed };

   public static PLine CreateEllipse (Point center, Point top, Point side)
      => new (center, top, side);

   public bool SetSelected (SelectionBox box)
       => Points.Any (x => x.SetSelected (box));

   public void Draw (IDrawer d) {
      if (HasCurves)
         d.DrawArc (this);
      else if (mFlag == EFlag.Open)
         d.DrawLine (this);
      else d.DrawRect (this);
   }

   public void Save (IFileWriter writer)
      => writer.WritePLine (this);

   public static IStorable Load (IFileReader reader)
      => reader.ReadPLine ();
   #endregion

   #region Nested Types ---------------------------------------------
   public enum EFlag {
      Open, Closed
   }
   #endregion

   #region Private Data ---------------------------------------------
   EFlag mFlag;
   List<Point> mPoints = new ();
   bool mHasCurves = false;
   #endregion
}

//public class Line : IDrawable, IStorable {
//   #region Properties -----------------------------------------------
//   public Point End { get; set; }
//   public Point Start { get; set; }
//   public bool IsSelected { get; set; }
//   #endregion

//   #region Methods --------------------------------------------------
//   public void Draw (IDrawer d)
//        => d.Draw (this);

//   public bool SetSelected (SelectionBox box)
//      => IsSelected = Start.SetSelected (box) || End.SetSelected (box);

//   public static IObject LoadBinary (BinaryReader reader, string version)
//      => version switch {
//         _ => new Line () {
//            Start = (Point)Point.LoadBinary (reader, version), End = (Point)Point.LoadBinary (reader, version)
//         }
//      };

//   public void SaveBinary (BinaryWriter writer) {
//      writer.Write ($"{LINE}");
//      Start.SaveBinary (writer);
//      End.SaveBinary (writer); writer.Write ('\n');
//   }
//   #endregion
//}

//public class CLine : IDrawable, IStorable {
//   #region Constructors ---------------------------------------------
//   public CLine () => Points = new ();
//   #endregion

//   #region Properties -----------------------------------------------
//   public List<Point> Points;
//   public bool IsSelected { get; set; }
//   #endregion

//   #region Methods --------------------------------------------------
//   public void Draw (IDrawer d)
//        => d.Draw (this);

//   public bool SetSelected (SelectionBox box)
//      => IsSelected = Points.Any (point => point.SetSelected (box));

//   public static IObject LoadBinary (BinaryReader reader, string version) {
//      var cline = version switch {
//         _ => new CLine ()
//      };
//      while (reader.PeekChar () != '\n')
//         cline.Points.Add ((Point)Point.LoadBinary (reader, version));
//      return cline;
//   }

//   public void SaveBinary (BinaryWriter writer) {
//      writer.Write ($"{CONNECTEDLINE}");
//      foreach (var point in Points) point.SaveBinary (writer);
//      writer.Write ('\n');
//   }
//   #endregion
//}

//public class Rect : IDrawable, IStorable {
//   #region Properties -----------------------------------------------
//   public Point TopLeft { get; set; }

//   public Point BottomRight { get; set; }

//   public bool IsSelected { get; set; }
//   #endregion

//   #region Methods --------------------------------------------------
//   public void Draw (IDrawer d)
//     => d.Draw (this);

//   public bool SetSelected (SelectionBox box)
//      => IsSelected = mSelectionParams.Any (pt => pt.SetSelected (box));
//   List<IObject> mSelectionParams => new () { TopLeft, BottomRight, new Point (TopLeft.X, BottomRight.Y), new Point (BottomRight.X, TopLeft.Y) };

//   public static IObject LoadBinary (BinaryReader reader, string version)
//      => version switch {
//         _ => new Rect () {
//            TopLeft = (Point)Point.LoadBinary (reader, version), BottomRight = (Point)Point.LoadBinary (reader, version)
//         }
//      };

//   public void SaveBinary (BinaryWriter writer) {
//      writer.Write ($"{RECT}");
//      TopLeft.SaveBinary (writer); BottomRight.SaveBinary (writer);
//      writer.Write ('\n');
//   }
//   #endregion
//}

//public class Ellipse : IDrawable, IStorable {
//   #region Properties -----------------------------------------------
//   public Point Center { get; set; }

//   public double RadiusX { get; set; }

//   public double RadiusY { get; set; }

//   public bool IsSelected { get; set; }
//   #endregion

//   #region Methods --------------------------------------------------
//   public void Draw (IDrawer d)
//        => d.Draw (this);

//   public bool SetSelected (SelectionBox box)
//      => IsSelected = SelectionParams.Any (param => param.SetSelected (box));
//   List<IObject> SelectionParams => new () { Center,
//      new Point (Center.X + RadiusX / 2, Center.Y), new Point (Center.X - RadiusX / 2, Center.Y),
//      new Point (Center.X, Center.Y - RadiusY/2), new Point (Center.X, Center.Y + RadiusY / 2) };

//   public static IObject LoadBinary (BinaryReader reader, string version)
//      => version switch {
//         _ => new Ellipse () {
//            Center = (Point)Point.LoadBinary (reader, version),
//            RadiusX = reader.ReadDouble (), RadiusY = reader.ReadDouble (),
//         }
//      };

//   public void SaveBinary (BinaryWriter writer) {
//      writer.Write ($"{ELLIPSE}"); Center.SaveBinary (writer);
//      writer.Write (RadiusX); writer.Write (RadiusY);
//      writer.Write ('\n');
//   }
//   #endregion
//}

//public class Circle1 : Ellipse {
//   #region Methods --------------------------------------------------
//   public static new IObject LoadBinary (BinaryReader reader, string version)
//      => version switch {
//         _ => new Circle1 () {
//            Center = (Point)Point.LoadBinary (reader, version),
//            RadiusX = reader.ReadDouble (), RadiusY = reader.ReadDouble (),
//         }
//      };

//   public new void SaveBinary (BinaryWriter writer) {
//      writer.Write ($"{CIRCLE1}"); Center.SaveBinary (writer);
//      writer.Write (RadiusX); writer.Write (RadiusY);
//      writer.Write ('\n');
//   }
//   #endregion
//}

//public class Arc : IDrawable, IStorable {
//   #region Properties -----------------------------------------------
//   public Point EndPoint { get; set; }

//   public double Radius { get; set; }

//   public Point StartPoint { get; set; }

//   public bool IsSelected { get; set; }
//   #endregion

//   #region Methods --------------------------------------------------
//   public void Draw (IDrawer d)
//        => d.Draw (this);

//   public bool SetSelected (SelectionBox box)
//      => IsSelected = StartPoint.SetSelected (box) || EndPoint.SetSelected (box);

//   public static IObject LoadBinary (BinaryReader reader, string version)
//      => version switch {
//         _ => new Arc () {
//            StartPoint = (Point)Point.LoadBinary (reader, version),
//            EndPoint = (Point)Point.LoadBinary (reader, version),
//            Radius = reader.ReadDouble (),
//         }
//      };

//   public void SaveBinary (BinaryWriter writer) {
//      writer.Write ($"{ARC}");
//      StartPoint.SaveBinary (writer); EndPoint.SaveBinary (writer);
//      writer.Write (Radius);
//      writer.Write ('\n');
//   }
//   #endregion
//}