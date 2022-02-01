public class BitSet
{
    private const int LOG2_UINT32_SIZE = 5;

    public BitSet(int length)
    {
        if (length < 0)
            throw new ArgumentOutOfRangeException();

        _length = length;

        _bits = new uint[((length - 1) >> LOG2_UINT32_SIZE) + 1];
    }

    public uint[] _bits;

    private int _length;

    public int Length => _length;

    public bool Get(int key) => (_bits[key >> LOG2_UINT32_SIZE] & (1 << key)) != 0;

    public void SetTrue(int key) => _bits[key >> LOG2_UINT32_SIZE] |= (uint)(1 << key);

    public void SetFalse(int key) => _bits[key >> LOG2_UINT32_SIZE] &= (uint)~(1 << key);

    public void Set(int key, bool value)
    {
        if(value)
            SetTrue(key);
        else
            SetFalse(key);
    }

    public bool this[int key] { get => Get(key); set => Set(key, value); }

    public void SetAll(bool value)
    {
        int length = _bits.Length;

        uint uintValue = value ? uint.MaxValue : uint.MinValue;

        for(int i = 0; i < length; i++)
            _bits[i] = uintValue;
    }
}
