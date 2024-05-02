namespace ThermalSim.Domain.Thermals
{
    public abstract class ValueRange<T>
    {
        public ValueRange(T min, T max)
        {
            Min = min; Max = max;
        }

        public T Min { get; set; }
        public T Max { get; set; }

        public abstract T GetRandomValue(Random random);
    }

    public class ValueRangeDouble : ValueRange<double>
    {
        public ValueRangeDouble(double min, double max) : base(min, max)
        {
        }

        public override double GetRandomValue(Random random)
        {
            var diff = Min - Max;
            return random.NextDouble() * diff + Min;
        }
    }

    public class ValueRangeInt : ValueRange<int>
    {
        public ValueRangeInt(int min, int max) : base(min, max)
        {
        }

        public override int GetRandomValue(Random random)
        {
            var diff = Min - Max;
            return (int)(random.NextDouble() * (double)diff) + Min;
        }
    }
}
