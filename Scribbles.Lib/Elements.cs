using static Lib.Drawing.EType;
namespace Lib;

public struct Point : IStorable {
   #region Constructors ---------------------------------------------
   public Point (double x, double y) => (X, Y) = (x, y);
   #endregion

   #region Properties -----------------------------------------------
   public double X { get; private set; }
   public double Y { get; private set; }
   #endregion

   #region Methods --------------------------------------------------
   public override bool Equals (object? obj) {
      if (obj == null) return false;
      return (Point)obj == this;
   }

   public override int GetHashCode () => base.GetHashCode ();

   public bool IsSelected (SelectionBox box)
      => X > box.TopLeft.X && Y > box.TopLeft.Y && X < box.BottomRight.X && Y < box.BottomRight.Y;

   public static IObject LoadBinary (BinaryReader reader, string version)
      => version switch {
         _ => new Point () { X = reader.ReadDouble (), Y = reader.ReadDouble () }
      };

   public void SaveBinary (BinaryWriter writer) {
      writer.Write (X); writer.Write (Y);
   }
   #endregion

   #region Operators ------------------------------------------------
   public static bool operator == (Point x, Point y)
      => x.X == y.X && x.Y == y.Y;

   public static bool operator != (Point x, Point y)
      => x.X != y.X || x.Y != y.Y;
   #endregion
}

public class Line : IShape, IDrawable, IStorable {
   #region Properties -----------------------------------------------
   public string Color { get; set; } = "#FFFFFF";

   public Point End { get; set; }

   public Point Start { get; set; }

   public double Thickness {
      get => mThickness;
      set {
         if (mThickness == value) return;
         mThickness = value;
         //mPen = null;
      }
   }
   double mThickness;
   #endregion

   #region Methods --------------------------------------------------
   public bool IsSelected (SelectionBox box)
      => Start.IsSelected (box) || End.IsSelected (box);

   public static IObject LoadBinary (BinaryReader reader, string version)
      => version switch {
         _ => new Line () {
            //Color = new SolidColorBrush ((Color)ColorConverter.ConvertFromString (reader.ReadString ())),
            Thickness = reader.ReadDouble (), Start = (Point)Point.LoadBinary (reader, version), End = (Point)Point.LoadBinary (reader, version)
         }
      };

   public void SaveBinary (BinaryWriter writer) {
      writer.Write ($"{DOODLE}"); writer.Write ($"{Color}");
      writer.Write (Thickness); Start.SaveBinary (writer);
      End.SaveBinary (writer); writer.Write ('\n');
   }
   #endregion
}

public class CLine : IShape, IDrawable, IStorable {
   #region Constructors ---------------------------------------------
   public CLine () => Points = new ();
   #endregion

   #region Properties -----------------------------------------------
   public string Color { get; set; } = "#FFFFFF";

   public List<Point> Points;

   public double Thickness {
      get => mThickness;
      set {
         if (mThickness == value) return;
         mThickness = value;
         //mPen = null;
      }
   }
   double mThickness;
   #endregion

   #region Methods --------------------------------------------------
   public bool IsSelected (SelectionBox box)
      => Points.Any (point => point.IsSelected (box));

   public static IObject LoadBinary (BinaryReader reader, string version) {
      var cline = version switch {
         _ => new CLine () {
            //Color = new SolidColorBrush ((Color)ColorConverter.ConvertFromString (reader.ReadString ())),
            Thickness = reader.ReadDouble ()
         }
      };
      while (reader.PeekChar () != '\n')
         cline.Points.Add ((Point)Point.LoadBinary (reader, version));
      return cline;
   }

   public void SaveBinary (BinaryWriter writer) {
      writer.Write ($"{CONNECTEDLINE}");
      writer.Write ($"{Color}"); writer.Write (Thickness);
      foreach (var point in Points) point.SaveBinary (writer);
      writer.Write ('\n');
   }
   #endregion
}

public class Doodle : IShape, IDrawable, IStorable {
   #region Constructors ---------------------------------------------
   public Doodle () => Points = new ();
   #endregion

   #region Properties -----------------------------------------------
   public string Color { get; set; } = "#FFFFFF";

   //public Brush Color {
   //   get => mColor;
   //   set {
   //      if (mColor == value) return;
   //      mColor = value;
   //      mPen = null;
   //   }
   //}
   //Brush mColor = Brushes.White;

   public List<Point> Points;

   public double Thickness {
      get => mThickness;
      set {
         if (mThickness == value) return;
         mThickness = value;
         //mPen = null;
      }
   }
   double mThickness;
   #endregion

   #region Methods --------------------------------------------------
   //public void Draw (DrawingContext dc) {
   //   mPen ??= new (Color, Thickness);
   //   for (int i = 0; i < Points.Count - 2; i++)
   //      dc.DrawLine (mPen, new (Points[i].X, Points[i].Y), new (Points[i + 1].X, Points[i + 1].Y));
   //}
   //Pen? mPen;

   public bool IsSelected (SelectionBox box)
      => Points.Any (point => point.IsSelected (box));

   public static IObject LoadBinary (BinaryReader reader, string version) {
      Doodle dood = version switch {
         _ => new () {
            //Color = new SolidColorBrush ((Color)ColorConverter.ConvertFromString (reader.ReadString ())),
            Thickness = reader.ReadDouble ()
         }
      };
      while (reader.PeekChar () != '\n')
         dood.Points.Add ((Point)Point.LoadBinary (reader, version));
      return dood;
   }

   public void SaveBinary (BinaryWriter writer) {
      writer.Write ($"{DOODLE}"); writer.Write ($"{Color}"); writer.Write (Thickness);
      foreach (Point p in Points) p.SaveBinary (writer);
      writer.Write ('\n');
   }
   #endregion
}

public class Rect : IShape, IDrawable, IStorable {
   #region Constructors ---------------------------------------------
   public Rect (bool fill) => Fill = fill;
   #endregion

   #region Properties -----------------------------------------------
   public Point BottomRight { get; set; }
   public string Color { get; set; } = "#FFFFFF";

   //public Brush Color {
   //   get => mColor;
   //   set {
   //      if (mColor == value) return;
   //      mColor = value;
   //      mPen = null;
   //   }
   //}
   //Brush mColor = Brushes.White;

   public bool Fill;

   public double Thickness {
      get => mThickness;
      set {
         if (mThickness == value) return;
         mThickness = value;
         //mPen = null;
      }
   }
   double mThickness;

   public Point TopLeft { get; set; }
   #endregion

   #region Methods --------------------------------------------------
   //public void Draw (DrawingContext dc) {
   //   mPen ??= new Pen (Color, Thickness);
   //   dc.DrawRectangle (Fill ? Color : null, mPen, new (new System.Windows.Point (TopLeft.X, TopLeft.Y), new System.Windows.Point (BottomRight.X, BottomRight.Y)));
   //}
   //Pen? mPen;

   public bool IsSelected (SelectionBox box)
      => mSelectionParams.Any (pt => pt.IsSelected (box));
   List<IObject> mSelectionParams => new () { TopLeft, BottomRight, new Point (TopLeft.X, BottomRight.Y), new Point (BottomRight.X, TopLeft.Y) };

   public static IObject LoadBinary (BinaryReader reader, string version)
      => version switch {
         _ => new Rect (reader.ReadBoolean ()) {
            //Color = new SolidColorBrush ((Color)ColorConverter.ConvertFromString (reader.ReadString ())),
            Thickness = reader.ReadDouble (),
            TopLeft = (Point)Point.LoadBinary (reader, version), BottomRight = (Point)Point.LoadBinary (reader, version)
         }
      };

   public void SaveBinary (BinaryWriter writer) {
      writer.Write ($"{RECT}"); writer.Write (Fill);
      writer.Write ($"{Color}"); writer.Write (Thickness);
      TopLeft.SaveBinary (writer); BottomRight.SaveBinary (writer);
      writer.Write ('\n');
   }
   #endregion
}

public class Ellipse : IShape, IDrawable, IStorable {
   #region Properties -----------------------------------------------
   public Point Center { get; set; }
   public string Color { get; set; } = "#FFFFFF";

   //public Brush Color {
   //   get => mColor;
   //   set {
   //      if (mColor == value) return;
   //      mColor = value;
   //      mPen = null;
   //   }
   //}
   //Brush mColor = Brushes.White;

   public double RadiusX { get; set; }

   public double RadiusY { get; set; }

   public double Thickness {
      get => mThickness;
      set {
         if (mThickness == value) return;
         mThickness = value;
         //mPen = null;
      }
   }
   double mThickness;
   #endregion

   #region Methods --------------------------------------------------
   //public void Draw (DrawingContext dc) {
   //   mPen ??= new Pen (Color, Thickness);
   //   dc.DrawEllipse (null, mPen, new (Center.X, Center.Y), RadiusX, RadiusY);
   //}
   //Pen? mPen;

   public bool IsSelected (SelectionBox box)
      => SelectionParams.Any (param => param.IsSelected (box));
   List<IObject> SelectionParams => new () { Center,
      new Point (Center.X + RadiusX / 2, Center.Y), new Point (Center.X - RadiusX / 2, Center.Y),
      new Point (Center.X, Center.Y - RadiusY/2), new Point (Center.X, Center.Y + RadiusY / 2) };

   public static IObject LoadBinary (BinaryReader reader, string version)
      => version switch {
         _ => new Ellipse () {
            Center = (Point)Point.LoadBinary (reader, version),
            RadiusX = reader.ReadDouble (), RadiusY = reader.ReadDouble (),
            //Color = new SolidColorBrush ((Color)ColorConverter.ConvertFromString (reader.ReadString ())),
            Thickness = reader.ReadDouble ()
         }
      };

   public void SaveBinary (BinaryWriter writer) {
      writer.Write ($"{ELLIPSE}"); Center.SaveBinary (writer);
      writer.Write (RadiusX); writer.Write (RadiusY);
      writer.Write ($"{Color}"); writer.Write (Thickness);
      writer.Write ('\n');
   }
   #endregion
}

public class Circle1 : Ellipse {
   #region Methods --------------------------------------------------
   public static new IObject LoadBinary (BinaryReader reader, string version)
      => version switch {
         _ => new Circle1 () {
            Center = (Point)Point.LoadBinary (reader, version),
            RadiusX = reader.ReadDouble (), RadiusY = reader.ReadDouble (),
            //Color = new SolidColorBrush ((Color)ColorConverter.ConvertFromString (reader.ReadString ())),
            Thickness = reader.ReadDouble ()
         }
      };

   public new void SaveBinary (BinaryWriter writer) {
      writer.Write ($"{CIRCLE1}"); Center.SaveBinary (writer);
      writer.Write (RadiusX); writer.Write (RadiusY);
      writer.Write ($"{Color}"); writer.Write (Thickness);
      writer.Write ('\n');
   }
   #endregion
}

public class Arc : IShape, IDrawable, IStorable {
   #region Constructors ---------------------------------------------
   public Arc () {
      //mIPF = new ();
      //mPF = new ();
      //mIPF.Add (mPF);
      //mPG = new (mIPF);
   }
   //readonly PathGeometry mPG;
   //readonly PathFigure mPF;
   //readonly List<PathFigure> mIPF;
   #endregion

   #region Properties -----------------------------------------------
   public string Color { get; set; } = "#FFFFFFFF";

   //public Brush Color {
   //   get => mColor;
   //   set {
   //      if (mColor == value) return;
   //      mColor = value;
   //      mPen = null;
   //   }
   //}
   //Brush mColor = Brushes.White;

   public Point EndPoint { get; set; }
   
   public double Radius { get; set; }

   public Point StartPoint { get; set; }
   //   {
   //   get => new (mPF.StartPoint.X, mPF.StartPoint.Y);
   //   set => mPF.StartPoint = new (value.X, value.Y);
   //}
   public double Thickness {
      get => mThickness;
      set {
         if (mThickness == value) return;
         mThickness = value;
         //mPen = null;
      }
   }
   double mThickness;
   #endregion

   #region Methods --------------------------------------------------
   //public void Draw (DrawingContext dc) {
   //   mPen ??= new (Color, Thickness);
   //   mPF.Segments.Clear ();
   //   mPF.Segments.Add (new ArcSegment (new (EndPoint.X, EndPoint.Y), new (Radius, Radius), 0, false, 0, true));
   //   dc.DrawGeometry (null, mPen, mPG);
   //}
   //Pen? mPen;

   public bool IsSelected (SelectionBox box)
      => StartPoint.IsSelected (box) || EndPoint.IsSelected (box);

   public static IObject LoadBinary (BinaryReader reader, string version)
      => version switch {
         _ => new Arc () {
            StartPoint = (Point)Point.LoadBinary (reader, version),
            EndPoint = (Point)Point.LoadBinary (reader, version),
            Radius = reader.ReadDouble (),
            //Color = new SolidColorBrush ((Color)ColorConverter.ConvertFromString (reader.ReadString ())),
            Thickness = reader.ReadDouble ()
         }
      };

   public void SaveBinary (BinaryWriter writer) {
      writer.Write ($"{ARC}");
      StartPoint.SaveBinary (writer); EndPoint.SaveBinary (writer);
      writer.Write (Radius);
      writer.Write ($"{Color}"); writer.Write (Thickness);
      writer.Write ('\n');
   }
   #endregion
}