/// <summary>DXF HEADER section</summary>
class HeaderSection : IDxfSection {
   public HeaderSection (ReadOnlySpan<DxfGroup> groups) {
      Name = "HEADER";
      // Collect all the HEADER value groups (Group value could be multi-valued)
      var itr = groups.GetEnumerator ();
      Util.Check (itr.MoveNext (), "Expect HEADER variable name");
      var g = itr.Current;
      Util.Check (g.Code == 9, "Ensure HEADER variable name");
      var hName = g.Value;
      Util.Check (itr.MoveNext (), "Expect HEADER variable value");
      List<DxfGroup> hGroups = [];
      List<string> strs = [];
      while (true) {
         g = itr.Current;
         if (g.Code == 9) {
            strs.Clear ();
            foreach (var group in hGroups)
               strs.Add (group.ToString ());
            mKVs.Add (hName, string.Join ("|", strs));
            hName = g.Value;
            hGroups.Clear ();
         } else 
            hGroups.Add (g);
         if (!itr.MoveNext ()) break;
      }
   }

   public string Name { get; private set; }

   public void Dump () {
      Console.WriteLine ($"SECTION {Name}");
      foreach (var kv in mKVs) {
         Console.WriteLine ($"{kv.Key} = {kv.Value}");
      }
   }

   readonly Dictionary<string, string> mKVs = [];
}

/// <summary>DXF HEADER entry</summary>
readonly record struct HEntry (string Name, string Value);
