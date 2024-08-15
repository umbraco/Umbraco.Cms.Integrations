namespace Umbraco.Cms.Integrations.Crm.Dynamics.Models
{
    [Flags]
    public enum DynamicsModule
    {
        Outbound = 1,
        RealTime = 2,
        Both = Outbound | RealTime
    }
}
