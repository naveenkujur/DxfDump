namespace DxfEncoding;

static class Dxf {
   public static void ExtractEncoding (string fnDxf, out string acadVer, out string codePage) {
      acadVer = codePage = string.Empty;
      var itr = ExtractCodes (fnDxf).GetEnumerator ();
      // Quick 'n dirty implementation
      // HEADER section must appear first (so we simply assume we are in HEADER section already)
      while (true) {
         if (!itr.MoveNext ()) throw new Exception ("Unexpected EOF");
         (int code, string value) = itr.Current;
         if (code == 0 && (value is "ENDSEC" or "EOF")) return;
         // Note: The values of interest are fortunately single-valued
         if (code != 9) continue;
         if (value == "$ACADVER") {
            if (!itr.MoveNext ()) throw new Exception ("Unexpected EOF");
            (code, value) = itr.Current;
            if (code != 1) throw new Exception ("Unexpected Code");
            acadVer = value;
         } else if (value == "$DWGCODEPAGE") {
            if (!itr.MoveNext ()) throw new Exception ("Unexpected EOF");
            (code, value) = itr.Current;
            if (code != 3) throw new Exception ("Unexpected Code");
            codePage = value;
         }
      }
   }

   static IEnumerable<(int Code, string Value)> ExtractCodes (string fnDxf) {
      var itr = File.ReadLines (fnDxf).GetEnumerator ();
      bool done = false;
      while (!done) {
         (int code, string value) = (NextInt (itr), NextStr (itr));
         if (code == 0 && value == "EOF") done = true;
         yield return (code, value);
      }

      static int NextInt (IEnumerator<string> itr) {
         if (!itr.MoveNext ()) throw new Exception ("Next Int");
         return int.Parse (itr.Current);
      }

      static string NextStr (IEnumerator<string> itr) {
         if (!itr.MoveNext ()) throw new Exception ("Next String");
         return itr.Current;
      }
   }
}