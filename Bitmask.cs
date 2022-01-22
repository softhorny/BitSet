public class Bitmask
{
    public Bitmask(int length)
    {
        if (length < 0)
            throw new ArgumentOutOfRangeException();

        _length = length;

        _bits = new byte[((length - 1) >> 3) + 1];
    }

    private byte[] _bits;
    
    private int _length;

    public int Length => _length;
    
    public void Clear()
    {
        for(int i = 0; i < _bits.Length; i++)
            _bits[i] = 0;
    }

    private bool Get(int key) => (_bits[key >> 3] & (1 << key)) != 0;
    
    private void Set(int key, bool value) 
    {
        if(value) 
            _bits[key >> 3] |= (byte)(1 << key);
        else 
            _bits[key >> 3] &= (byte)~(1 << key);
    }

    public bool this[int key] { get => Get(key); set => Set(key, value); }
}
