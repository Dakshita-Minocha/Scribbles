using Lib;
using System.IO;

namespace Scribbles;

public class SaveBinary : IFileWriter {
   public SaveBinary (BinaryWriter writer) => mBW = writer;
   readonly BinaryWriter mBW;

   public void WriteDrawing (Drawing dwg) {
      foreach (var pline in dwg.Shapes) WritePLine (pline);
   }

   public void WritePLine (PLine pline) {
      mBW.Write (pline.HasCurves);
      mBW.Write ((int)pline.Flag);
      foreach (var p in pline.Points) { mBW.Write (p.X); mBW.Write (p.Y); }
   }

   public void WritePoint (Point pt) {
      mBW.Write (pt.X); mBW.Write (pt.Y);
   }
}
