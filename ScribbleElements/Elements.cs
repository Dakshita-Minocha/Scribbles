using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Media;
using static Scribbles.Drawing.EType;
namespace Scribbles;

public struct Point : IStorable {
   public Point (double x, double y) => (X, Y) = (x, y);

   public double X { get; private set; }
   public double Y { get; private set; }

   public void SaveBinary (BinaryWriter writer) {
      writer.Write (X); writer.Write (Y);
   }

   public static IObject LoadBinary (BinaryReader reader, string version)
      => version switch {
         _ => new Point () { X = reader.ReadDouble (), Y = reader.ReadDouble () }
      };
}

class Line : IShape, IDrawable, IStorable {
   public Point Start { get; set; }
   public Point End { get; set; }
   public Brush Color { get; set; } = Brushes.White;
   public double Thickness { get; set; }

   public void Draw (DrawingContext dc) {
      mPen ??= new (Color, Thickness);
      dc.DrawLine (mPen, new (Start.X, Start.Y), new (End.X, End.Y));
   }
   Pen? mPen;

   public static IObject LoadBinary (BinaryReader reader, string version)
      => version switch {
         _ => new Line () {
            Color = new SolidColorBrush ((Color)ColorConverter.ConvertFromString (reader.ReadString ())),
            Thickness = reader.ReadDouble (), Start = (Point)Point.LoadBinary (reader, version), End = (Point)Point.LoadBinary (reader, version)
         }
      };

   public void SaveBinary (BinaryWriter writer) {
      writer.Write ($"{DOODLE}"); writer.Write ($"{Color}");
      writer.Write (Thickness); Start.SaveBinary (writer);
      End.SaveBinary (writer); writer.Write ('\n');
   }
}

class CLine : IShape, IDrawable, IStorable {
   public CLine ()
      => Points = new ();

   public List<Point> Points;
   public Brush Color { get; set; } = Brushes.White;
   public double Thickness { get; set; }

   public static IObject LoadBinary (BinaryReader reader, string version) {
      var cline = version switch {
         _ => new CLine () {
            Color = new SolidColorBrush ((Color)ColorConverter.ConvertFromString (reader.ReadString ())),
            Thickness = reader.ReadDouble ()
         }
      };
      while (reader.PeekChar () != '\n')
         cline.Points.Add ((Point)Point.LoadBinary (reader, version));
      return cline;
   }

   public void Draw (DrawingContext dc) {
      mPen ??= new Pen (Color, Thickness);
      for (int i = 0; i < Points.Count - 1; i++)
         dc.DrawLine (mPen, new (Points[i].X, Points[i].Y), new (Points[i + 1].X, Points[i + 1].Y));
   }
   Pen? mPen;

   public void SaveBinary (BinaryWriter writer) {
      writer.Write ($"{CONNECTEDLINE}");
      writer.Write ($"{Color}"); writer.Write (Thickness);
      foreach (var point in Points) point.SaveBinary (writer);
      writer.Write ('\n');
   }
}

class Doodle : IShape, IDrawable, IStorable {
   public Doodle () => Points = new ();
   public List<Point> Points;
   public Brush Color { get; set; } = Brushes.White;
   public double Thickness { get; set; }
   public void Add (System.Windows.Point p) => Points.Add (new Point (p.X, p.Y));

   public void SaveBinary (BinaryWriter writer) {
      writer.Write ($"{DOODLE}"); writer.Write ($"{Color}"); writer.Write (Thickness);
      foreach (Point p in Points) p.SaveBinary (writer);
      writer.Write ('\n');
   }

   public static IObject LoadBinary (BinaryReader reader, string version) {
      Doodle dood = version switch {
         _ => new () {
            Color = new SolidColorBrush ((Color)ColorConverter.ConvertFromString (reader.ReadString ())),
            Thickness = reader.ReadDouble ()
         }
      };
      while (reader.PeekChar () != '\n')
         dood.Points.Add ((Point)Point.LoadBinary (reader, version));
      return dood;
   }

   public void Draw (DrawingContext dc) {
      mPen ??= new (Color, Thickness);
      for (int i = 0; i < Points.Count - 2; i++)
         dc.DrawLine (mPen, new (Points[i].X, Points[i].Y), new (Points[i + 1].X, Points[i + 1].Y));
   }
   Pen? mPen;
}

class Rect : IShape, IDrawable, IStorable {
   public Rect (bool fill)
      => Fill = fill;
   public bool Fill;
   public Point TopLeft { get; set; }
   public Point BottomRight { get; set; }

   public Brush Color { get; set; } = Brushes.White;
   public double Thickness { get; set; }

   public static IObject LoadBinary (BinaryReader reader, string version)
      => version switch {
         _ => new Rect (reader.ReadBoolean ()) {
            Color = new SolidColorBrush ((Color)ColorConverter.ConvertFromString (reader.ReadString ())),
            Thickness = reader.ReadDouble (),
            TopLeft = (Point)Point.LoadBinary (reader, version), BottomRight = (Point)Point.LoadBinary (reader, version)
         }
      };

   public void Draw (DrawingContext dc) {
      mPen ??= new Pen (Color, Thickness);
      dc.DrawRectangle (Fill ? Color : null, mPen, new (new System.Windows.Point (TopLeft.X, TopLeft.Y), new System.Windows.Point (BottomRight.X, BottomRight.Y)));
   }
   Pen? mPen;

   public void SaveBinary (BinaryWriter writer) {
      writer.Write ($"{RECT}"); writer.Write (Fill);
      writer.Write ($"{Color}"); writer.Write (Thickness);
      TopLeft.SaveBinary (writer); BottomRight.SaveBinary (writer);
      writer.Write ('\n');
   }
}

public class Ellipse : IShape, IDrawable, IStorable {
   public Point Center { get; set; }
   public double RadiusX { get; set; }
   public double RadiusY { get; set; }
   public Brush Color { get; set; } = Brushes.White;
   public double Thickness { get; set; }

   public static IObject LoadBinary (BinaryReader reader, string version)
      => version switch {
         _ => new Ellipse () {
            Center = (Point)Point.LoadBinary (reader, version),
            RadiusX = reader.ReadDouble (), RadiusY = reader.ReadDouble (),
            Color = new SolidColorBrush ((Color)ColorConverter.ConvertFromString (reader.ReadString ())),
            Thickness = reader.ReadDouble ()
         }
      };

   public void Draw (DrawingContext dc) {
      mPen ??= new Pen (Color, Thickness);
      dc.DrawEllipse (null, mPen, new (Center.X, Center.Y), RadiusX, RadiusY);
   }
   Pen? mPen;
   public void SaveBinary (BinaryWriter writer) {
      writer.Write ($"{ELLIPSE}"); Center.SaveBinary (writer);
      writer.Write (RadiusX); writer.Write (RadiusY);
      writer.Write ($"{Color}"); writer.Write (Thickness);
      writer.Write ('\n');
   }
}

public class Circle1 : Ellipse {
   public static new IObject LoadBinary (BinaryReader reader, string version)
      => version switch {
         _ => new Circle1 () {
            Center = (Point)Point.LoadBinary (reader, version),
            RadiusX = reader.ReadDouble (), RadiusY = reader.ReadDouble (),
            Color = new SolidColorBrush ((Color)ColorConverter.ConvertFromString (reader.ReadString ())),
            Thickness = reader.ReadDouble ()
         }
      };

   public new void SaveBinary (BinaryWriter writer) {
      writer.Write ($"{CIRCLE1}"); Center.SaveBinary (writer);
      writer.Write (RadiusX); writer.Write (RadiusY);
      writer.Write ($"{Color}"); writer.Write (Thickness);
      writer.Write ('\n');
   }
}

public class Circle2 : IShape, IDrawable, IStorable {
   public Brush Color { get => throw new NotImplementedException (); set => throw new NotImplementedException (); }
   public double Thickness { get => throw new NotImplementedException (); set => throw new NotImplementedException (); }

   public static IObject LoadBinary (BinaryReader reader, string version) {
      throw new NotImplementedException ();
   }

   public void Draw (DrawingContext dc) {
      throw new NotImplementedException ();
   }

   public void SaveBinary (BinaryWriter writer) {
      throw new NotImplementedException ();
   }
}

public class Arc : IShape, IDrawable, IStorable {
   public Arc () {
      mIPF = new ();
      mPF = new ();
      mIPF.Add (mPF);
      mPG = new (mIPF);
   }
   readonly PathGeometry mPG;
   readonly PathFigure mPF;
   readonly List<PathFigure> mIPF;

   public double Radius { get; set; }
   public Point StartPoint {
      get => new (mPF.StartPoint.X, mPF.StartPoint.Y);
      set => mPF.StartPoint = new (value.X, value.Y);
   }
   public Point EndPoint { get; set; }
   public Brush Color { get; set; } = Brushes.White;
   public double Thickness { get; set; }

   public static IObject LoadBinary (BinaryReader reader, string version)
      => version switch {
         _ => new Arc () {
            StartPoint = (Point)Point.LoadBinary (reader, version),
            EndPoint = (Point)Point.LoadBinary (reader, version),
            Radius = reader.ReadDouble (),
            Color = new SolidColorBrush ((Color)ColorConverter.ConvertFromString (reader.ReadString ())),
            Thickness = reader.ReadDouble ()
         }
      };

   public void Draw (DrawingContext dc) {
      mPen ??= new (Color, Thickness);
      mPF.Segments.Clear ();
      mPF.Segments.Add (new ArcSegment (new (EndPoint.X, EndPoint.Y), new (Radius, Radius), 0, false, 0, true));
      dc.DrawGeometry (null, mPen, mPG);
   }
   Pen? mPen;

   public void SaveBinary (BinaryWriter writer) {
      writer.Write ($"{ARC}");
      StartPoint.SaveBinary (writer); EndPoint.SaveBinary (writer);
      writer.Write (Radius);
      writer.Write ($"{Color}"); writer.Write (Thickness);
      writer.Write ('\n');
   }
}

public class Drawing : IDrawable, IStorable {
   public Drawing () => Shapes = new ();
   public List<IDrawable> Shapes { get; }

   public static IObject LoadBinary (BinaryReader reader, string version) {
      Drawing dr = new ();
      string firstLine = reader.ReadString ();
      var split = firstLine.Split (':');
      if (split[0] == "Version") version = split[1].ToString ()!;
      else reader.BaseStream.Position -= firstLine.Length + 1;
      while (reader.BaseStream.Position != reader.BaseStream.Length) {
         dr.Shapes.Add (Enum.Parse (typeof (EType), reader.ReadString ()) switch {
            DOODLE => (Doodle)Doodle.LoadBinary (reader, version),
            LINE => (Line)Line.LoadBinary (reader, version),
            RECT => (Rect)Rect.LoadBinary (reader, version),
            CONNECTEDLINE => (CLine)CLine.LoadBinary (reader, version),
            CIRCLE1 => (Circle1)Circle1.LoadBinary (reader, version),
            CIRCLE2 => (Circle2)Circle2.LoadBinary (reader, version),
            ELLIPSE => (Ellipse)Ellipse.LoadBinary (reader, version),
            ARC => (Arc)Arc.LoadBinary (reader, version),
            _ => throw new NotImplementedException ()
         });
         reader.ReadChar (); // removing \u000f
      }
      return dr;
   }

   public void Draw (DrawingContext dc) {
      foreach (var shape in Shapes)
         (shape as dynamic).Draw (dc);
   }

   public void SaveBinary (BinaryWriter writer) {
      foreach (var shape in Shapes) (shape as dynamic).SaveBinary (writer);
   }

   public enum EType { POINT, DOODLE, LINE, CONNECTEDLINE, RECT, FILLEDRECT, CIRCLE1, CIRCLE2, ELLIPSE, ARC };
}
