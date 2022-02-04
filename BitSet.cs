public class BitSet
{
    private const int UINT32_SIZE = 32;
    
    private const int LOG2_UINT32_SIZE = 5;

    public BitSet(int length)
    {
        if (length < 0)
            throw new ArgumentOutOfRangeException();

        _bits = new uint[((length - 1) >> LOG2_UINT32_SIZE) + 1];
        
        Length = length;
    }

    public uint[] _bits;

    public int Length { get; private set; }

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
    
    public void SetTrue(int from, int to)
    {
        if(from == to)
        {
            SetTrue(from);

            return;
        }

        int i = from >> LOG2_UINT32_SIZE;

        int left = from;

        from = UINT32_SIZE * (i + 1);

        bool b = to < from;

        int right = b ? UINT32_SIZE - to - 1 : 0;

        _bits[i] |= uint.MaxValue >> right & uint.MaxValue << left;

        if(!b)
            SetTrue(from, to);
    }

    public void SetAll(bool value)
    {
        int length = _bits.Length;

        uint uintValue = value ? uint.MaxValue : uint.MinValue;

        for(int i = 0; i < length; i++)
            _bits[i] = uintValue;
    }
}
