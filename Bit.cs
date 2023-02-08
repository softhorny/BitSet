using System.Runtime.CompilerServices;

namespace softh.Collections
{
    public readonly struct Bit
    {   
        private const MethodImplOptions INLINE = MethodImplOptions.AggressiveInlining;

        private readonly byte _value;

        private Bit( byte value ) => _value = value;

        public static implicit operator bool( Bit bit ) => bit._value != 0;
        public static implicit operator int( Bit bit ) => bit._value;

        [MethodImpl( INLINE )] 
        public static Bit GetFrom( uint value, int offset ) => new Bit( (byte)( value >> offset & 1 ) );

        [MethodImpl( INLINE )] 
        public static int PopCount( uint value )
        {
            const uint k1 = ~0u / 3;
            const uint k2 = ~0u / 5;
            const uint k4 = ~0u / 17;
            const uint kf = ~0u / 255;

            value -= ( value >> 1 ) & k1;
            value = ( value & k2 ) + ( ( value >> 2 ) & k2 );
            value = ( ( ( value + ( value >> 4 ) ) & k4 ) * kf ) >> 24;

            return (int)value;
        }

        [MethodImpl( INLINE )] 
        public static int PopCount( ulong value )
        {
            const ulong k1 = ~0ul / 3;
            const ulong k2 = ~0ul / 5;
            const ulong k4 = ~0ul / 17;
            const ulong kf = ~0ul / 255;

            value -= ( value >> 1 ) & k1;
            value = ( value & k2 ) + ( ( value >> 2 ) & k2 );
            value = ( ( ( value + ( value >> 4 ) ) & k4 ) * kf ) >> 56;

            return (int)value;
        }
    }
}
