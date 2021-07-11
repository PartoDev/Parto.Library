namespace Parto.Helper.ValueTypes.StaticValue
{
    public class StaticValue<TValue> : IStaticValue<TValue>
    {
        protected StaticValue(TValue value) => Value = value;

        public TValue Value { get; }
    }
}