
namespace Domain.Helpers
{
    public static class TipoModeloRequest
    {
        public static int OutboundLoadsExport                   { get; } = 1;
        public static int InboundShipmentVerificationsExport    { get; } = 2;
        public static int InventoryHistoryExport                { get; } = 3;
        public static int OrderVerificationExport               { get; } = 4;
    }
}
