using System.Collections.Generic;
using System.IO;
using System.Windows.Media;
using System;
using static Scribbles.Drawing.EType;
namespace Scribbles;

public class Drawing : IDrawable, IStorable {
   #region Constructors ---------------------------------------------
   public Drawing () => Shapes = new ();
   #endregion

   #region Properties -----------------------------------------------
   public List<IDrawable> Shapes { get; }
   #endregion

   #region Methods --------------------------------------------------
   public void Draw (DrawingContext dc) {
      foreach (var shape in Shapes)
         (shape as dynamic).Draw (dc);
   }

   public bool IsSelected (SelectionBox box) => false;

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
            ELLIPSE => (Ellipse)Ellipse.LoadBinary (reader, version),
            ARC => (Arc)Arc.LoadBinary (reader, version),
            _ => throw new NotImplementedException ()
         });
         reader.ReadChar (); // removing \u000f
      }
      return dr;
   }

   public void SaveBinary (BinaryWriter writer) {
      foreach (var shape in Shapes) (shape as dynamic).SaveBinary (writer);
   }
   #endregion

   #region Enum -----------------------------------------------------
   public enum EType { POINT, DOODLE, LINE, CONNECTEDLINE, RECT, FILLEDRECT, CIRCLE1, CIRCLE2, ELLIPSE, ARC, SELECTIONBOX };
   #endregion
}