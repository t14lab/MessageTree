using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;

namespace Addison_Wesley.Codebook.WPF
{
   /// <summary>
   /// Klasse mit statischen Utility-Methoden zur Arbeit mit WPF-Fenstern
   /// </summary>
   public static partial class WindowUtils
   {
      #region Deklaration der API-Struktur und -Funktionen

      struct MARGINS
      {
         public int Left;
         public int Right;
         public int Top;
         public int Bottom;
      }

      [DllImport("dwmapi.dll", PreserveSig = false)]
      static extern void DwmExtendFrameIntoClientArea(IntPtr hwnd, ref MARGINS margins);

      [DllImport("dwmapi.dll", PreserveSig = false)]
      static extern bool DwmIsCompositionEnabled();

      #endregion

      /// <summary>
      /// Erweitert den Rahmen eines Fensters unter Windows im Aero-Thema nach innen
      /// </summary>
      /// <param name="window">Das Fenster</param>
      /// <returns>Gibt true zurück wenn das Erweitern möglich war</returns>
      public static bool ExtendWindowFrame(Window window)
      {
         return ExtendWindowFrame(window, new Thickness(-1));
      }

      /// <summary>
      /// Erweitert den Rahmen eines Fensters unter Windows im Aero-Thema nach innen
      /// </summary>
      /// <param name="window">Das Fenster</param>
      /// <param name="margins">Gibt an, um wie viele Einheiten der Rahmen
      /// nach innen an jeder Seite vergrößert werden soll</param>
      /// <returns>Gibt true zurück wenn das Erweitern möglich war</returns>
      public static bool ExtendWindowFrame(Window window, Thickness margins)
      {
         try
         {
            // Überprüfen, ob die notwendige Desktopgestaltung aktiviert ist
            if (DwmIsCompositionEnabled() == false)
            {
               return false;
            }
         }
         catch (DllNotFoundException)
         {
            // Die DLL dwmapi.dll ist nicht verfügbar. 
            // Wahrscheinlich läuft die Anwendung unter
            // einer älteren Windows-Version
            return false;
         }

         // Den Handle des Fensters ermitteln
         IntPtr hwnd = new WindowInteropHelper(window).Handle;
         if (hwnd == IntPtr.Zero)
         {
            throw new InvalidOperationException(
               "Das Fenster muss bereits angezeigt werden, " +
               "damit der Glasrahmen nach innen verbreitert " +
               "werden kann.");
         }

         // Den Hintergrund (für WPF und Windows) transparent schalten
         window.Background = Brushes.Transparent;
         HwndSource.FromHwnd(hwnd).CompositionTarget.BackgroundColor = Colors.Transparent;

         // Den Faktor für die Umrechnung der geräteunabhängigen Einheit in
         // Pixel ermitteln
         double pixelCalculationFactorX = 1;
         double pixelCalculationFactorY = 1;
         PresentationSource source = PresentationSource.FromVisual(window);
         if (source != null)
         {
            pixelCalculationFactorX = source.CompositionTarget.TransformToDevice.M11;
            pixelCalculationFactorY = source.CompositionTarget.TransformToDevice.M22;
         }

         // Den Glasrahmen nach innen verbreitern
         MARGINS apiMargins = new MARGINS();
         apiMargins.Left = (int)(margins.Left * pixelCalculationFactorX);
         apiMargins.Top = (int)(margins.Top * pixelCalculationFactorY);
         apiMargins.Right = (int)(margins.Right * pixelCalculationFactorX);
         apiMargins.Bottom = (int)(margins.Bottom * pixelCalculationFactorX);
         DwmExtendFrameIntoClientArea(hwnd, ref apiMargins);

         return true;
      }
   }
}
