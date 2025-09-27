
static class Util {
   public static IEnumerable<DxfGroup> Groups (string filename) {
      var itr = File.ReadLines (filename).GetEnumerator ();
      while (itr.MoveNext ()) {
         int groupCode = int.Parse (itr.Current);
         MoveItr (itr);
         yield return new DxfGroup (groupCode, itr.Current);
      }
   }

   public static void DumpSections (string filename) {
      var itr = Groups (filename).GetEnumerator ();
      while (itr.MoveNext ()) {
         var g = itr.Current;
         if (!g.IsStartSection) continue;
         MoveItr (itr);
         g = itr.Current;
         Check (g.Code == 2);
         Console.WriteLine (g.Value);
         MoveItr (itr);
         while (!itr.Current.IsEndSection) 
            MoveItr (itr);
      }
   }

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

   static void MoveItr<T> (IEnumerator<T> itr) {
      if (!itr.MoveNext()) throw new FileLoadException ();
   }

   static void Check (bool predicate) {
      if (!predicate) throw new FileLoadException ("");
   }   
}
readonly record struct DxfGroup (int Code, string Value) {
   public bool IsStartSection => Code == 0 && Value == "SECTION";
   public bool IsEndSection => Code == 0 && Value == "ENDSEC";

}

class Section { // Starts with 0:SECTION, ends with 0:ENDSEC, and has name 2:<NAME>,
   public Section () {

   }
}