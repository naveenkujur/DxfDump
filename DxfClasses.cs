/// <summary>DXF CLASSES section</summary>
class ClassesSection : IDxfSection {
   public ClassesSection (ReadOnlySpan<DxfGroup> groups) => Name = "CLASSES";

   public string Name { get; private set; }

   public void Dump () {
      Console.WriteLine ($"SECTION {Name}");
   }
}
