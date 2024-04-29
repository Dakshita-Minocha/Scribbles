using Microsoft.Win32;
using System.Windows;
using System;
using System.IO;
using System.ComponentModel;
using Lib;
namespace Scribbles;

public class DocManager : INotifyPropertyChanged {
   #region Constructors ---------------------------------------------
   public DocManager (Drawing dwg) => mDwg = dwg;
   Drawing mDwg;
   #endregion

   #region Properties -----------------------------------------------
   public bool IsModified {
      get => mModified;
      set {
         if (mModified == value) return;
         mModified = value;
         if (value) FileName += "*";
         else FileName = FileName.TrimEnd ('*');
      }
   }
   bool mModified = false;

   public string FileName {
      get => mFileName;
      set {
         mFileName = value;
         PropertyChanged?.Invoke (this, new PropertyChangedEventArgs (nameof (FileName)));
      }
   }
   string mFileName = "Untitled";

   public event PropertyChangedEventHandler? PropertyChanged;
   #endregion

   #region Methods --------------------------------------------------
   public void Save () {
      SaveFileDialog saveFile = new () {
         CheckPathExists = true, InitialDirectory = "C:\\etc",
         Filter = $"BinaryFiles |*.bin", AddExtension = true, DefaultExt = ".bin"
      };
      if (saveFile.ShowDialog () == true)
         try {
            mFilePath = saveFile.FileName;
            SaveAs ();
            FileName = mFilePath[(mFilePath.LastIndexOfAny (mSlash) + 1)..];
         } catch (NotImplementedException) { MessageBox.Show ("File not saved."); }
      IsModified = false;
   }
   readonly static char[] mSlash = { '/', '\\' };

   public void LoadNew () {

   }

   public void Load () {
      if (!IsModified) return;
      OpenFileDialog openFile = new () {
         CheckFileExists = true, CheckPathExists = true, Multiselect = false,
         InitialDirectory = "C:\\etc", Filter = $"BinaryFiles |*.bin", DefaultExt = ".bin"
      };
      if (openFile.ShowDialog () == true)
         try {
            mDwg.Shapes.Add ((Drawing)Drawing.LoadBinary (new (new FileStream (openFile.FileName, FileMode.Open)), "0"));
            FileName = openFile.FileName[(mFilePath.LastIndexOfAny (mSlash) + 1)..];
         } catch (Exception) { MessageBox.Show ("Couldn't Open file. Unknown FileFormat."); }
      IsModified = false;
   }

   public void SaveAs () {
      mDwg.SaveBinary (new (new FileStream (mFilePath, FileMode.OpenOrCreate, FileAccess.Write)));
   }
   #endregion

   #region Private --------------------------------------------------
   string mFilePath = "";
   #endregion
}
