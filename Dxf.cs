
static class Util {
   public static IEnumerable<DxfGroup> Groups (string filename) {
      var itr = File.ReadLines (filename).GetEnumerator ();
      while (itr.MoveNext ()) {
         int groupCode = int.Parse (itr.Current);
         MoveItr (itr);
         yield return new DxfGroup (groupCode, itr.Current.Trim ());
      }
   }

   public static void DumpAll (string filename) {
      foreach (var section in ExtractSections (filename)) {
         section.Dump ();
      }
   }

   public static IEnumerable<IDxfSection> ExtractSections (string filename) {
      var itr = Groups (filename).GetEnumerator ();
      List<DxfGroup> groups = [];
      while (itr.MoveNext ()) {
         var g = itr.Current;
         if (g.IsEof) yield break;
         if (!g.IsStartSection) continue;
         MoveItr (itr);
         g = itr.Current;
         Check (g.Code == 2, "Expect SECTION name");
         var sectionName = g.Value;
         MoveItr (itr);
         groups.Clear ();
         while (!itr.Current.IsEndSection) {
            groups.Add (itr.Current);
            MoveItr (itr);
         }
         yield return CreateSection (sectionName, [.. groups]);
      }
      Check (false, "Corrupt DXF file");

   }

   public static void DumpSectionNames (string filename) {
      var itr = Groups (filename).GetEnumerator ();
      while (itr.MoveNext ()) {
         var g = itr.Current;
         if (g.IsEof) return;
         if (!g.IsStartSection) continue;
         MoveItr (itr);
         g = itr.Current;
         Check (g.Code == 2, "Expect SECTION name");
         Console.WriteLine (g.Value);
         MoveItr (itr);
         while (!itr.Current.IsEndSection) 
            MoveItr (itr);
      }
      Check (false, "Corrupt DXF file");
   }

   /// <summary>Dump all groups having group code 0 (which marks beginning of some section/block)</summary>
   public static void DumpGroupCode0Freq (string filename) {
      Dictionary<string, int> refCounts = [];
      foreach (var g in Util.Groups (filename)) {
         if (g.Code != 0) continue;
         if (!refCounts.TryGetValue (g.Value, out int count)) {
            refCounts.Add (g.Value, 1);
         } else refCounts[g.Value]++;
      }
      foreach (var rc in refCounts)
         Console.WriteLine (rc);
   }

   static IDxfSection CreateSection (string name, ReadOnlySpan<DxfGroup> groups) {
      return name switch {
         "HEADER" => new HeaderSection (groups),
         "TABLES" => new TablesSection (groups),
         "ENTITIES" => new EntitiesSection (groups),
         "BLOCKS" => new BlocksSection (groups),
         "CLASSES" => new ClassesSection (groups),
         "OBJECTS" => new ObjectsSection (groups),
         _ => throw new IndexOutOfRangeException ($"Unknown SECTION name {name}"),
      };
   }

   public static void MoveItr<T> (IEnumerator<T> itr) {
      if (!itr.MoveNext()) throw new FileLoadException ();
   }

   public static void Check (bool predicate, string message) {
      if (!predicate) throw new FileLoadException (message);
   }   
}
readonly record struct DxfGroup (int Code, string Value) {
   public bool IsStartSection => Code == 0 && Value == "SECTION";
   public bool IsEndSection => Code == 0 && Value == "ENDSEC";
   public bool IsEof => Code == 0 && Value == "EOF";
}

interface IDxfSection {
   string Name { get; }
   void Dump ();
}
