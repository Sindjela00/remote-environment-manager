namespace Diplomski.Models
{
    public class LabsDTO
    {
        public string Name { get; set; }
        public int? NumberOfMachines { get; set; }
        public int Rows { get; set; }
        public int Columns { get; set; }
        public List<Machine>? Machines { get; set; }
    }
}
