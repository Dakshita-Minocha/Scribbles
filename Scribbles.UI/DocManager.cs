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
         mFilePath = value;
         mFileName = mFilePath[(mFilePath.LastIndexOfAny (mSlash) + 1)..];
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
            FileName = saveFile.FileName;
            SaveFile ();
         } catch (NotImplementedException) { MessageBox.Show ("File not saved."); }
   }
   readonly static char[] mSlash = { '/', '\\' };

   public void Load () {
      if (!IsModified ||
         MessageBox.Show ("Any unsaved changes will be lost. Do you wish to continue?", "Open", MessageBoxButton.YesNo) == MessageBoxResult.Yes) {
         OpenFileDialog openFile = new () {
            CheckFileExists = true, CheckPathExists = true, Multiselect = false,
            InitialDirectory = "C:\\etc", Filter = $"BinaryFiles |*.bin", DefaultExt = ".bin"
         };
         if (openFile.ShowDialog () == true)
            try {
               FileName = openFile.FileName;
               LoadFile ();
            } catch (Exception) { MessageBox.Show ("Couldn't open file. Unknown file format."); }
      }
   }

   public void LoadFile () {
      var dwg = (Drawing)Drawing.LoadBinary (new (new FileStream (mFilePath, FileMode.Open)), "0");
      mDwg.Shapes.Clear ();
      mDwg.Shapes.Add (dwg);
      IsModified = false;
   }

   public void SaveFile () {
      mDwg.SaveBinary (new (new FileStream (mFilePath, FileMode.Create, FileAccess.Write)));
      IsModified = false;
   }
   #endregion

   #region Private --------------------------------------------------
   string mFilePath = "";
   #endregion
}
