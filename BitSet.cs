public class BitSet
{
    private const int UINT32_SIZE = 32;
    private const int LOG2_UINT32_SIZE = 5;

    /// <summary> 
    /// Create a new bitset of the given length. All bits are initially false.
    /// Length will be rounded up to a multiple of 32. 
    /// </summary>
    public BitSet(int length)
    {
        EnsureLength(ref length);

        _bits = length > 0 ? new uint[length] : Array.Empty<uint>();
    }

    private uint[] _bits;

    public int Length => _bits.Length * UINT32_SIZE;

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

    /// <summary> 
    /// Sets the bits in the given range from (inclusive) and to (exclusive) to true. 
    /// </summary>
    public void SetTrue(int from, int to)
    {
        if(from >= to)
            return;

        int i = from >> LOG2_UINT32_SIZE,
        last = to - 1 >> LOG2_UINT32_SIZE;
        to = UINT32_SIZE - to;

        if(i == last)
        {
            _bits[i] |= uint.MaxValue >> to & uint.MaxValue << from;
            return;
        }

        _bits[i] |= uint.MaxValue << from;
        _bits[last] |= uint.MaxValue >> to;

        for(i++; i < last; i++)
            _bits[i] = uint.MaxValue;
    }

    /// <summary> 
    /// Sets the bits in the given range from (inclusive) and to (exclusive) to false. 
    /// </summary>
    public void SetFalse(int from, int to)
    {
        if(from >= to)
            return;

        int i = from >> LOG2_UINT32_SIZE,
        last = to - 1 >> LOG2_UINT32_SIZE;
        to = UINT32_SIZE - to;

        if(i == last)
        {
            _bits[i] &= ~(uint.MaxValue >> to & uint.MaxValue << from);
            return;
        }

        _bits[i] &= ~(uint.MaxValue << from);
        _bits[last] &= ~(uint.MaxValue >> to);

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

        uint mask = value ? uint.MaxValue : uint.MinValue;

        for(int i = 0; i < length; i++)
            _bits[i] = mask;
    }

    public void Resize(int length)
    {
        EnsureLength(ref length);

        if(length == _bits.Length)
            return;

        if(length == 0)
        {
            _bits = Array.Empty<uint>();
            return;
        }
        
        var newArray = new uint[length];
        
        Array.Copy(_bits, 0, newArray, 0,  _bits.Length > length ? length : _bits.Length);
        
        _bits = newArray;
    }
    
    private static void EnsureLength(ref int length)
    {
        if(length < 0)
            throw new ArgumentOutOfRangeException();

        length = ((length - 1) >> LOG2_UINT32_SIZE) + 1;
    }

    /// <summary> 
    /// Returns the population count (number of bits set) of this bitset. 
    /// </summary>
    public int PopCount()
    { 
        int count = 0;

        foreach(var mask in _bits)
            count += HammingWeight(mask);

        return count;
    }

    /// <summary> 
    /// Returns the population count (number of bits set) in the given range from (inclusive) and to (exclusive). 
    /// </summary>
    public int PopCount(int from, int to)
    {
        if(from >= to)
            return 0;

        int i = from >> LOG2_UINT32_SIZE,
        last = to - 1 >> LOG2_UINT32_SIZE;
        to = UINT32_SIZE - to;

        if(i == last)
            return HammingWeight(_bits[i] & (uint.MaxValue << from & uint.MaxValue >> to));

        int count = HammingWeight((_bits[i] & (uint.MaxValue << from)) | (ulong)(_bits[last] & (uint.MaxValue >> to)) << UINT32_SIZE);

        for(i++; i < last; i++)
            count += HammingWeight(_bits[i]);

        return count;
    }

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

    private static int HammingWeight(ulong mask)
    {
        const int UINT64_SIZE = 64;

        if(mask == ulong.MinValue)
            return 0;

        if(mask == ulong.MaxValue)
            return UINT64_SIZE;

        mask -= (mask >> 1) & 0x_55555555_55555555ul;
        mask = (mask & 0x_33333333_33333333ul) + ((mask >> 2) & 0x_33333333_33333333ul);
        mask = (((mask + (mask >> 4)) & 0x_0F0F0F0F_0F0F0F0Ful) * 0x_01010101_01010101ul) >> 56;

        return (int)mask;
    }
}