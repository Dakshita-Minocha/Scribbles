using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scribbles;
public class DocManager {
   #region Constructors ---------------------------------------------
   public DocManager () { }
   #endregion

   #region Properties -----------------------------------------------
   public bool IsModified { get; set; } = false;
   #endregion

   #region Methods --------------------------------------------------
   public void Save () {
      IsModified = false;
   }

   public void Load () {
      IsModified = false;
   }

   public void SaveAs () { }
   #endregion

   #region Private --------------------------------------------------
   #endregion
}
