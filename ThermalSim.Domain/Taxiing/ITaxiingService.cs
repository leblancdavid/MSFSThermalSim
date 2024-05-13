namespace ThermalSim.Domain.Towing
{
    public interface ITaxiingService
    {
        bool IsTaxiing { get; }
        double TaxiingSpeed { get; set; }
        bool StartTaxiing();
        bool StopTaxiing();
    }
}
