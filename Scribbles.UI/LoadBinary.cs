using Lib;
using System.Collections.Generic;
using System.IO;
namespace Scribbles;

public class LoadBinary : IFileReader {
   public LoadBinary (BinaryReader br) => mBR = br;
   BinaryReader mBR;
   public Drawing ReadDrawing () {
      Drawing dwg = new ();
      string version;
      string firstLine = mBR.ReadString ();
      var split = firstLine.Split (':');
      if (split[0] == "Version") version = split[1].ToString ()!;
      else mBR.BaseStream.Position -= firstLine.Length + 1;
      while (mBR.BaseStream.Position != mBR.BaseStream.Length) {
         dwg.Shapes.Add (ReadPLine ());
         mBR.ReadChar (); // removing \u000f
      }
      return dwg;
   }

   public PLine ReadPLine () {
      if (mBR.ReadBoolean ()) { // has curves - Ellipse / Circle / Arcs
         return mBR.ReadInt32 () == 1 ?
         PLine.CreateEllipse (ReadPoint(), ReadPoint(), ReadPoint ()) :
         PLine.CreateArc ();
      } else if (mBR.ReadInt32 () == 0) { // open straight shapes - Line, CLine
         List<Point> points = new ();
         while (mBR.PeekChar () != '\n')
            points.Add (ReadPoint ());
         return PLine.CreateLine (points);
      } else return PLine.CreateRect (ReadPoint (), ReadPoint ());
   }

   public Point ReadPoint ()
      => new (mBR.ReadDouble (), mBR.ReadDouble ());
}
