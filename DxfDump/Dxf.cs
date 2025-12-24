
static class Util {
   public static IEnumerable<DxfGroup> Groups (string filename) {
      var itr = File.ReadLines (filename).GetEnumerator ();
      while (itr.MoveNext ()) {
         int groupCode = int.Parse (itr.Current);
         MustMove (itr);
         yield return new DxfGroup (groupCode, itr.Current.Trim ());
      }
   }

   public static void DumpAll (string filename) {
      foreach (var section in ExtractSections (filename)) {
         section.Dump ();
         Console.WriteLine ();
      }
   }

   public static IEnumerable<IDxfSection> ExtractSections (string filename) {
      var itr = Groups (filename).GetEnumerator ();
      List<DxfGroup> groups = [];
      while (itr.MoveNext ()) {
         var g = itr.Current;
         if (g.IsEof) yield break;
         if (!g.IsStartSection) continue;
         MustMove (itr);
         g = itr.Current;
         Check (g.Code == 2, "Expect SECTION name");
         var sectionName = g.Value;
         MustMove (itr);
         groups.Clear ();
         while (!itr.Current.IsEndSection) {
            groups.Add (itr.Current);
            MustMove (itr);
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
         MustMove (itr);
         g = itr.Current;
         Check (g.Code == 2, "Expect SECTION name");
         Console.WriteLine (g.Value);
         MustMove (itr);
         while (!itr.Current.IsEndSection) 
            MustMove (itr);
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
         _ => new IgnoredSection (name, groups),
      };
   }

   public static bool MustMove<T> (IEnumerator<T> itr) 
      => itr.MoveNext () ? true : throw new FileLoadException ("Itr failed to move!");

   public static void Check (bool predicate, string message) {
      if (!predicate) throw new FileLoadException (message);
   }   
}

/// <summary>DXF group is simply code + value pair</summary>
readonly record struct DxfGroup (int Code, string Value) {
   public override string ToString () => $"{Code}:{Value}";
   public bool IsStartSection => Code == 0 && Value == "SECTION";
   public bool IsEndSection => Code == 0 && Value == "ENDSEC";
   public bool IsEof => Code == 0 && Value == "EOF";
}

interface IDxfSection {
   string Name { get; }
   void Dump ();
}

/// <summary>DXF "IGNORED" section</summary>
class IgnoredSection : IDxfSection {
   public IgnoredSection (string name, ReadOnlySpan<DxfGroup> groups) => Name = name;
   
   public string Name { get; private set; }

   public void Dump () {
      Console.WriteLine ($"SECTION {Name}");
   }
}
