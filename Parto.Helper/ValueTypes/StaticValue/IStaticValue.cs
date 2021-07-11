namespace Parto.Helper.ValueTypes.StaticValue
{
    public interface IStaticValue<out TValue>
    {
        TValue Value { get; }
    }
}