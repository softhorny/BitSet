public class BitSet
{
    private const int LOG2_UINT32_SIZE = 5;

    public BitSet(int length)
    {
        if(length < 0)
            throw new ArgumentOutOfRangeException();

        _bits = new uint[((length - 1) >> LOG2_UINT32_SIZE) + 1];
        
        Length = length;
    }

    public uint[] _bits;

    public int Length { get; private set; }

    public bool Get(int key) => (_bits[key >> LOG2_UINT32_SIZE] & (1u << key)) != 0;

    public void SetTrue(int key) => _bits[key >> LOG2_UINT32_SIZE] |= 1u << key;

    public void SetFalse(int key) => _bits[key >> LOG2_UINT32_SIZE] &= ~(1u << key);

    public void Set(int key, bool value)
    {
        if(value)
            SetTrue(key);
        else
            SetFalse(key);
    }

    public bool this[int key] { get => Get(key); set => Set(key, value); }
    
    /// <summary>
    /// Sets the bits in the given range from (inclusive) and to (exclusive) to true.
    /// </summary>
    public void SetTrue(int from, int to)
    {
        if(from >= to)
            return;

        int i = from >> LOG2_UINT32_SIZE;

        int length = to - 1 >> LOG2_UINT32_SIZE;

        if(i == length)
        {
            _bits[i] |= (1u << to) - 1 & uint.MaxValue << from;

            return;
        }

        _bits[i] |= uint.MaxValue << from;

        _bits[length] |= (1u << to) - 1;

        for(i++; i < length; i++)
            _bits[i] = uint.MaxValue;
    }
        
    /// <summary>
    /// Sets the bits in the given range from (inclusive) and to (exclusive) to false.
    /// </summary>
    public void SetFalse(int from, int to)
    {
        if(from >= to)
            return;

        int i = from >> LOG2_UINT32_SIZE;

        int length = to - 1 >> LOG2_UINT32_SIZE;

        if(i == length)
        {
            _bits[i] &= ~((1u << to) - 1 & uint.MaxValue << from);

            return;
        }

        _bits[i] &= ~(uint.MaxValue << from);

        _bits[length] &= ~((1u << to) - 1);

        for(i++; i < length; i++)
            _bits[i] = uint.MinValue;
    }
    
    /// <summary>
    /// Sets the bits in the given range from (inclusive) and to (exclusive) to the specified value.
    /// </summary>
    public void Set(int from, int to, bool value)
    {
        if(value)
            SetTrue(from, to);
        else
            SetFalse(from, to);
    }
    
    public void SetAll(bool value)
    {
        int length = _bits.Length;

        uint uintValue = value ? uint.MaxValue : uint.MinValue;

        for(int i = 0; i < length; i++)
            _bits[i] = uintValue;
    }
}
