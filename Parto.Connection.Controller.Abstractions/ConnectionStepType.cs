namespace Parto.Connection.Controller.Abstractions
{
    public enum ConnectionStepType
    {
        Before = 0,
        BeforeParameter = 1,
        AfterParameter = 2,
        BeforeExecute = 3,
        AfterExecute = 4,
        BeforeResult = 5,
        AfterResult = 6,
        After = 7
    }
}