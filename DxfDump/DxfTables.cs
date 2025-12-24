/// <summary>DXF TABLES section</summary>
class TablesSection : IDxfSection {
   public TablesSection (ReadOnlySpan<DxfGroup> groups) => Name = "TABLES";

   public string Name { get; private set; }

   public void Dump () {
      Console.WriteLine ($"SECTION {Name}");
   }
}
