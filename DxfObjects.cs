
/// <summary>DXF OBJECTS section</summary>
class ObjectsSection : IDxfSection {
   public ObjectsSection (ReadOnlySpan<DxfGroup> groups) => Name = "OBJECTS";

   public string Name { get; private set; }

   public void Dump () {
      Console.WriteLine ($"SECTION {Name}");
   }
}
