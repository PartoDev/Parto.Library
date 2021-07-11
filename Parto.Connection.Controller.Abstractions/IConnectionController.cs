namespace Parto.Connection.Controller.Abstractions
{
    public interface IConnectionController
    {
        IConnectionControllerContext Context { get; set; }
    }
}