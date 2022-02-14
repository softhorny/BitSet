public class BitSet
{
    private const int UINT32_SIZE = 32;
    
    private const int LOG2_UINT32_SIZE = 5;

    public BitSet(int length)
    {
        if(length < 0)
            throw new ArgumentOutOfRangeException();

        _bits = new uint[((length - 1) >> LOG2_UINT32_SIZE) + 1];
        
        Length = length;
    }

    private uint[] _bits;

    public int Length { get; private set; }
    
#region O(1)
    
    public bool Get(int key) => (_bits[key >> LOG2_UINT32_SIZE] & (1u << key)) != uint.MinValue;

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
    
#endregion
    
#region O(n)
    
    /// <summary> Sets the bits in the given range from (inclusive) and to (exclusive) to true. </summary>
    public void SetTrue(int from, int to)
    {
        int i = from >> LOG2_UINT32_SIZE,
        length = to - 1 >> LOG2_UINT32_SIZE;

        if(i == length)
        {
            _bits[i] |= uint.MaxValue >> UINT32_SIZE - to & uint.MaxValue << from;

            return;
        }

        _bits[i] |= uint.MaxValue << from;
        _bits[length] |= uint.MaxValue >> UINT32_SIZE - to;

        for(i++; i < length; i++)
            _bits[i] = uint.MaxValue;
    }
        
    /// <summary> Sets the bits in the given range from (inclusive) and to (exclusive) to false. </summary>
    public void SetFalse(int from, int to)
    {
        int i = from >> LOG2_UINT32_SIZE,
        length = to - 1 >> LOG2_UINT32_SIZE;

        if(i == length)
        {
            _bits[i] &= ~(uint.MaxValue >> UINT32_SIZE - to & uint.MaxValue << from);

            return;
        }

        _bits[i] &= ~(uint.MaxValue << from);
        _bits[length] &= ~(uint.MaxValue >> UINT32_SIZE - to);

        for(i++; i < length; i++)
            _bits[i] = uint.MinValue;
    }
    
    /// <summary> Sets the bits in the given range from (inclusive) and to (exclusive) to the specified value. </summary>
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

        uint mask = value ? uint.MaxValue : uint.MinValue;

        for(int i = 0; i < length; i++)
            _bits[i] = mask;
    }
    
    /// <summary> Returns the population count (number of bits set) of this bitset. </summary>
    public int PopCount()
    { 
        int count = 0;

        foreach(var mask in _bits)
            count += HammingWeight(mask);

        return count;
    }
    
    /// <summary> Returns the population count (number of bits set) of this bitset in the given range from (inclusive) and to (exclusive). </summary>
    public int PopCount(int from, int to)
    {
        int i = from >> LOG2_UINT32_SIZE,
        length = to - 1 >> LOG2_UINT32_SIZE;

        if(i == length)
            return HammingWeight(_bits[i] & (uint.MaxValue << from & uint.MaxValue >> UINT32_SIZE - to));
        
        int count = HammingWeight(_bits[i] & (uint.MaxValue << from)) + HammingWeight(_bits[length] & (uint.MaxValue >> UINT32_SIZE - to));
        
        for(i++; i < length; i++)
            count += HammingWeight(_bits[i]);
        
        return count;
    }
    
#endregion
    
    private static int HammingWeight(uint mask)
    {
        if(mask == uint.MinValue)
            return 0;
                
        if(mask == uint.MaxValue)
            return UINT32_SIZE;
    
        mask -= (mask >> 1) & 0x_55555555u;
        mask = (mask & 0x_33333333u) + ((mask >> 2) & 0x_33333333u);
        mask = (((mask + (mask >> 4)) & 0x_0F0F0F0Fu) * 0x_01010101u) >> 24;

        return (int)mask;
    }
}
