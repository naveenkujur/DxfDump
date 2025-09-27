/// <summary>DXF ENTITIES section</summary>
class EntitiesSection : IDxfSection {
   public EntitiesSection (ReadOnlySpan<DxfGroup> groups) => Name = "ENTITIES";

   public string Name { get; private set; }

   public void Dump () {
      Console.WriteLine ($"SECTION {Name}");
   }
}
