public class Bitset
{
    private const int Log2_ByteSize = 3;
    
    public Bitset(int length)
    {
        if (length < 0)
            throw new ArgumentOutOfRangeException();

        _length = length;

        _bits = new byte[((length - 1) >> Log2_ByteSize) + 1];
    }

    private byte[] _bits;
    
    private int _length;

    public int Length => _length;
    
    public bool Get(int key) => (_bits[key >> Log2_ByteSize] & (1 << key)) != 0;
    
    public void SetTrue(int key) => _bits[key >> Log2_ByteSize] |= (byte)(1 << key);
    
    public void SetFalse(int key) => _bits[key >> Log2_ByteSize] &= (byte)~(1 << key);
    
    public void Set(int key, bool value)
    {
        if(value)
            SetTrue(key)
        else
            SetFalse(key);
    }
    
    public bool this[int key] { get => Get(key); set => Set(key, value); }
    
    public void SetAll(bool value)
    {
        int length = _bits.Length;
        byte byteValue = value ? byte.MaxValue : byte.MinValue;
        for(int i = 0; i < length; i++)
            _bits[i] = byteValue;
    }
}
