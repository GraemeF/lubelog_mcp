namespace LubeLogMCP.Models
{
    public enum ImportMode
    {
        ServiceRecord = 0,
        RepairRecord = 1,
        GasRecord = 2,
        TaxRecord = 3,
        UpgradeRecord = 4,
        ReminderRecord = 5,
        NoteRecord = 6,
        SupplyRecord = 7,
        Dashboard = 8,
        PlanRecord = 9,
        OdometerRecord = 10,
        VehicleRecord = 11,
        InspectionRecord = 12
    }
    public class ExtraFieldsVM
    {
        public string RecordType { get; set; }
        public List<ExtraFieldVM> ExtraFields { get; set; }
    }
    public class ExtraFieldVM
    {
        public string Name { get; set; }
        public string IsRequired { get; set; }
        public string FieldType { get; set; }
    }
}