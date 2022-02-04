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

        int right = 0, left = from;

        from = UINT32_SIZE * (i + 1);

        if(to < from)
            right = UINT32_SIZE - to - 1;
        else 
            SetTrue(from, to);

        _bits[i] |= uint.MaxValue >> right & uint.MaxValue << left;
    }
    
    public void SetFalse(int from, int to)
    {
        if(from == to)
        {
            SetFalse(from);

            return;
        }

        int i = from >> LOG2_UINT32_SIZE;

        int right = 0, left = from;

        from = UINT32_SIZE * (i + 1);

        if(to < from)
            right = UINT32_SIZE - to - 1;
        else 
            SetFalse(from, to);

        _bits[i] &= ~(uint.MaxValue >> right & uint.MaxValue << left);
    }

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
