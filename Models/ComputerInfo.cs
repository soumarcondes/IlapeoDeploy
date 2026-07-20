namespace ILAPEODeploy.Models;

public sealed class ComputerInfo
{
    public string ComputerName { get; set; } = "";
    public string UserName { get; set; } = "";
    public string Domain { get; set; } = "";
    public string WindowsVersion { get; set; } = "";
    public string Manufacturer { get; set; } = "";
    public string Model { get; set; } = "";
    public string SerialNumber { get; set; } = "";
    public string Processor { get; set; } = "";
    public string Ram { get; set; } = "";
    public string Disk { get; set; } = "";
    public string IPv4 { get; set; } = "";
    public string MacAddress { get; set; } = "";
}
