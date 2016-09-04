namespace BulletMLLib
{
    public class BulletValue
    {
        public readonly BLValueType ValueType;
        public readonly float Value;

        public BulletValue(BLValueType type, float value)
        {
            ValueType = type;
            Value = value;
        }

    }
}