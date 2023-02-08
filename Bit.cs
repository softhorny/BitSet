using System.Runtime.CompilerServices;

namespace softh.Collections
{
    public readonly struct Bit
    {   
        private readonly byte _value;
        
        private Bit( byte value ) => _value = value;

        [MethodImpl( MethodImplOptions.AggressiveInlining )] 
        public static Bit GetFromInt( int value, int offset ) => new Bit( (byte)( value >> offset & 1 ) );

        public static implicit operator bool( Bit bit ) => bit._value != 0;
        public static implicit operator int( Bit bit ) => bit._value;
    }
}
