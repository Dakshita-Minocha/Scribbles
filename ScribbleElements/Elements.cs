using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Media;

interface IDrawable {
   public void Draw (DrawingContext dc);
   public void SaveBinary (BinaryWriter writer);
   public static abstract IObject LoadBinary (BinaryReader reader);
   public void SaveText (StreamWriter writer);
   public static abstract IObject LoadText (StreamReader reader);
}

interface IObject { }

interface IShape : IObject {
   public Brush Color { get; set; }
   public double Thickness { get; set; }
}

struct Point : IObject {
   public Point (double x, double y) => (X, Y) = (x, y);

   public double X { get; private set; }
   public double Y { get; private set; }

   public void SaveBinary (BinaryWriter writer) {
      writer.Write (X); writer.Write (Y);
   }

   public static List<Point> LoadBinary (BinaryReader reader) {
      var list = new List<Point> ();
      while (reader.PeekChar () != '\n')
         list.Add (new Point () { X = reader.ReadDouble (), Y = reader.ReadDouble () });
      return list;
   }

   public void SaveText (StreamWriter writer)
      => writer.Write ($"{X},{Y} ");

   public static List<Point> LoadText (StreamReader reader) {
      var list = new List<Point> ();
      foreach (var strPoint in reader.ReadLine ()?.Split (' ')!) {
         var strValue = strPoint.Split (',');
         if (strValue.Length != 2 || strValue[0] == "" || strValue[1] == "") break;
         list.Add (new () { X = double.Parse (strValue[0]), Y = double.Parse (strValue[1]) });
      }
      return list;
   }
}

class Line : IShape, IDrawable {
   public Line () => Points = new ();
   public List<Point> Points;
   public Brush Color { get; set; } = Brushes.White;
   public double Thickness { get; set; }
   public void Add (System.Windows.Point p) => Points.Add (new Point (p.X, p.Y));

   public void SaveBinary (BinaryWriter writer) {
      writer.Write ($"{nameof (Line)}");
      writer.Write ($"{Color}");
      writer.Write (Thickness);
      foreach (Point p in Points) p.SaveBinary (writer);
      writer.Write ('\n');
   }

   public static IObject LoadBinary (BinaryReader reader) => new Line () {
      Color = new SolidColorBrush ((Color)ColorConverter.ConvertFromString (reader.ReadString ())),
      Thickness = reader.ReadDouble (),
      Points = Point.LoadBinary (reader)
   };

   public void SaveText (StreamWriter writer) {
      writer.WriteLine ($"{nameof (Line)}\n{Color}\n{Thickness}");
      foreach (Point p in Points) p.SaveText (writer);
      writer.WriteLine ();
   }

   public static IObject LoadText (StreamReader reader) => new Line () {
      Color = new SolidColorBrush ((Color)ColorConverter.ConvertFromString (reader.ReadLine ())),
      Thickness = double.Parse (reader.ReadLine ()!),
      Points = Point.LoadText (reader)
   };

   public void Draw (DrawingContext dc) {
      Pen pen = new (Color, Thickness);
      for (int i = 0; i < Points.Count - 2; i++)
         dc.DrawLine (pen, new (Points[i].X, Points[i].Y), new (Points[i + 1].X, Points[i + 1].Y));
   }
}

class Circle : IShape, IDrawable {
   public Brush Color { get; set; } = Brushes.White;
   public double Thickness { get; set; }

   public static IObject LoadBinary (BinaryReader reader) {
      throw new System.NotImplementedException ();
   }

   public static IObject LoadText (StreamReader reader) {
      throw new System.NotImplementedException ();
   }

   public void Draw (DrawingContext dc) {
      throw new NotImplementedException ();
   }

   public void SaveBinary (BinaryWriter writer) {
      throw new System.NotImplementedException ();
   }

   public void SaveText (StreamWriter writer) {
      throw new System.NotImplementedException ();
   }
}

class Drawing : IObject, IDrawable {
   public Drawing () => Shapes = new ();
   public List<IShape> Shapes { get; }

   public static IObject LoadBinary (BinaryReader reader) {
      Drawing dr = new ();
      while (reader.BaseStream.Position != reader.BaseStream.Length) {
         string type = reader.ReadString ();
         dr.Shapes.Add (type switch {
            "Line" => (Line)Line.LoadBinary (reader),
            "Circle" => (Circle)Circle.LoadBinary (reader),
            _ => throw new NotImplementedException ()
         });
         reader.ReadChar (); // removing \u000f
      }
      return dr;
   }

   public static IObject LoadText (StreamReader reader) {
      Drawing dr = new ();
      string type;
      for (; ; ) {
         if ((type = reader.ReadLine ()!) == null) break;
         dr.Shapes.Add (type switch {
            "Line" => (Line)Line.LoadText (reader),
            "Circle" => (Circle)Circle.LoadText (reader),
            _ => throw new NotImplementedException ()
         });
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

   public void SaveText (StreamWriter writer) {
      foreach (var shape in Shapes) (shape as dynamic).SaveText (writer);
   }
}
