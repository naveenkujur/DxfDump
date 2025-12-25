// DXF encoding study
using System.Text;

namespace DxfEncoding;

class DxfDecoder {
   public static void Playground () {
      Console.OutputEncoding = Encoding.UTF8;
      //CheckEncoding ("X:/TData");
      //CheckEncoding ("A:/TData");
      CheckEncoding ("N:/TData");
   }

   public static void CheckEncoding (string dirPath) {
      foreach (var fn in Directory.GetFiles (dirPath, "*.dxf", SearchOption.AllDirectories)) {
         Console.ForegroundColor = ConsoleColor.Yellow;
         Console.WriteLine (fn);

         // Output preferred encoding info, if any
         try {
            Dxf.ExtractEncoding (fn, out string acadVer, out string acadEncoding);
            if (acadVer.Length > 0 || acadEncoding.Length > 0) {
               Console.ForegroundColor = ConsoleColor.White;
               Console.WriteLine ("acadver: {0}, dwgcodepage: {1}", acadVer, acadEncoding);
            }
         } catch (FormatException) { // File: X:\TData\IO\DXF\mac.dxf [uses CRCRLF new-line encoding format]
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine ("FORMAT ERROR : Possibly corrupt file");
         }
         Console.ResetColor ();

         DumpNonAscii (fn);
      }
   }

   // Detects and dumps non-ascii lines!
   static void DumpNonAscii (string fnDxf) => DumpNonAscii (File.ReadAllBytes (fnDxf));

   // Detects and dumps non-ascii lines!
   static void DumpNonAscii (ReadOnlySpan<byte> bytes) {
      int currentIdx = 0;
      int nLine = 0;
      while (currentIdx < bytes.Length) {
         var line = NextLineCRLF (bytes, ref currentIdx);
         nLine++;
         if (IsAsciiEncoded (line)) continue;
         Console.ForegroundColor = ConsoleColor.Cyan;
         Console.Write ("{0}: ", nLine); Console.ResetColor ();
         PrintLn (line);
      }
   }

   // expects CR (carriage return : 13, '\r') and LF (line feed : 10, '\n') characters in the same order
   static ReadOnlySpan<byte> NextLineCRLF (ReadOnlySpan<byte> bytes, ref int startIdx) {
      // Note: We are generating single LF character (for new line)!
      //    Other files have CRLF new lines
      // Trim out any CR characters
      int nextIdx = -1; // Beginning of next line
      for (int i = startIdx; i < bytes.Length; i++) {
         var b = bytes[i];
         if (b == '\n') {
            nextIdx = i + 1;
            break;
         }
      }
      var line = nextIdx == -1 ? bytes[startIdx..] : bytes[startIdx..(nextIdx - 1)];
      if (nextIdx == -1) {
         startIdx = bytes.Length;
         return TrimmedCR (line);
      }
      startIdx = nextIdx;
      return TrimmedCR (line);

      static ReadOnlySpan<byte> TrimmedCR (ReadOnlySpan<byte> bytes) {
         if (bytes.Length == 0) return bytes;
         (int s, int e) = (0, bytes.Length);
         if (bytes[0] == '\r') s++;
         if (bytes[^1] == '\r') e--;
         return s <= e ? bytes[s..e] : bytes;
      }
   }

   static void Print (ReadOnlySpan<byte> bytes) {
      foreach (var b in bytes) {
         if (!char.IsAscii ((char)b))
            Console.ForegroundColor = ConsoleColor.Green;
         Console.Write ("{0}", (char)b);
         Console.ResetColor ();
      }
   }

   static void PrintLn (ReadOnlySpan<byte> bytes) {
      Print (bytes); Console.WriteLine ();
   }

   static bool IsAsciiEncoded (ReadOnlySpan<byte> bytes) {
      foreach (var b in bytes)
         if (!char.IsAscii ((char)b)) return false;
      return true;
   }
}
