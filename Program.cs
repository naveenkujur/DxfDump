class Program {
   static void Main (string[] args) {
      //var file = @"N:\TData\IO\DXF\3Horns.dxf";
      //Util.DumpAll ("C:\\Incoming\\DirtyDwgs\\Drawing data\\Open contour\\P1303846.DXF");

      SearchDxfs ("C:\\Incoming\\DirtyDwgs\\Drawing data");
      //SearchDxfs (@"X:\TData");
   }

   static void SearchDxfs (string searchRoot) {
      var len = searchRoot.Length;
      foreach (var file in Directory.GetFiles (searchRoot, "*.dxf", SearchOption.AllDirectories)) {
         Console.WriteLine (file.Substring (len + 1));
         try {
            DumpHeaderInfo (file, "$ACADVER|$DWGCODEPAGE");
         } catch (Exception e) {
            Console.WriteLine (e);
            Console.WriteLine ();
            continue;
         }
         Console.WriteLine ();
      }
   }

   static void DumpHeaderInfo (string file, string keys) {
      HeaderSection? h = null;
      foreach (var s in Util.ExtractSections (file)) {
         Console.WriteLine (s.Name);
         if (s.Name == "HEADER") h = (HeaderSection)s;
      }
      if (h is null) return;
      foreach (var k in keys.Split ('|')) {
         if (h.KVs.TryGetValue (k, out var v)) {
            Console.WriteLine ($"{k} {v}");
         }
      }
   }
}