using System.Collections.Generic;
using System.IO;
using System.Windows.Media;
using System;
using static Scribbles.Drawing.EType;
namespace Scribbles;

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

   public bool IsSelected (SelectionBox box) => false;

   public void SaveBinary (BinaryWriter writer) {
      foreach (var shape in Shapes) (shape as dynamic).SaveBinary (writer);
   }

   public enum EType { POINT, DOODLE, LINE, CONNECTEDLINE, RECT, FILLEDRECT, CIRCLE1, CIRCLE2, ELLIPSE, ARC, SELECTIONBOX };
}