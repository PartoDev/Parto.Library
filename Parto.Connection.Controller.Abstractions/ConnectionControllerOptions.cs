namespace Parto.Connection.Controller.Abstractions
{
    public class ConnectionControllerOptions
    {
        public string? ReceiveBlockNullErrorPath { get; set; }
        public string? ReceiveLocationNUllErrorPath { get; set; }
        public string? ReceiveControllerNullErrorPath { get; set; }
    }
}