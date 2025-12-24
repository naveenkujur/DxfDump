/// <summary>DXF BLOCKS section</summary>
class BlocksSection : IDxfSection {
   public BlocksSection (ReadOnlySpan<DxfGroup> groups) => Name = "BLOCKS";

   public string Name { get; private set; }

   public void Dump () {
      Console.WriteLine ($"SECTION {Name}");
   }
}
