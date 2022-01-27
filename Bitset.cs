public class Bitset
{
    public Bitset(int length)
    {
        if (length < 0)
            throw new ArgumentOutOfRangeException();

        _length = length;

        _bits = new byte[((length - 1) >> 3) + 1];
    }

    private byte[] _bits;
    
    private int _length;

    public int Length => _length;
    
    public bool this[int key] { get => Get(key); set => Set(key, value);
    
    public bool Get(int key) => (_bits[key >> 3] & (1 << key)) != 0;
    
    public void Set(int key, bool value) 
    {
        if(value) 
            _bits[key >> 3] |= (byte)(1 << key);
        else 
            _bits[key >> 3] &= (byte)~(1 << key);
    }
    
    public void SetAll(bool value)
    {
        int length = _bits.Length;
        byte byteValue = value ? byte.MaxValue : byte.MinValue;
        for(int i = 0; i < length; i++)
            _bits[i] = byteValue;
    }
}
