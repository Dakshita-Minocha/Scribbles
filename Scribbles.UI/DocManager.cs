namespace Scribbles;
public class DocManager {
   #region Constructors ---------------------------------------------
   public DocManager () { }
   #endregion

   #region Properties -----------------------------------------------
   public bool IsModified { get; set; } = false;
   #endregion

   #region Methods --------------------------------------------------
   public void Save (string fileName) {
      IsModified = false;
   }

   public void Load (string fileName) {
      IsModified = false;
   }

   public void SaveAs (string fileName) { }
   #endregion

   #region Private --------------------------------------------------
   #endregion
}
