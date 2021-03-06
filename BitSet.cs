namespace softh.BitSet
{
    using System;
    using System.Runtime.CompilerServices;
    
    // https://github.com/softhorny/BitSet
    public partial class BitSet
    {
        private const MethodImplOptions INLINE = MethodImplOptions.AggressiveInlining;

        private const int MASK_SIZE = 32;

        private const int LOG2_MASK_SIZE = 5;

        /// <summary> 
        /// Create a new bitset of the given length. All bits are initially false. 
        /// Length will be rounded up to a multiple of 32. 
        /// </summary>
        public BitSet( int length ) => _bits = IsValidLength( ref length ) ? new uint[ length ] : Array.Empty<uint>();

        private uint[] _bits;

        /// <summary> 
        /// Returns the number of bits actually used by this bitset. 
        /// </summary>
        public int Length 
        { 
            [MethodImpl( INLINE )] 
            get => _bits.Length << LOG2_MASK_SIZE; 
        }

        [MethodImpl( INLINE )] 
        private static bool IsValidLength( ref int length ) => ( length = ( ( length - 1 ) >> LOG2_MASK_SIZE ) + 1 ) > 0;

    #region Get/Set

        [MethodImpl( INLINE )] 
        public bool Get( int key ) => ( _bits[ key >> LOG2_MASK_SIZE ] & ( 1u << key ) ) != uint.MinValue;
        
        [MethodImpl( INLINE )] 
        public int GetInt( int key ) => (int)( _bits[ key >> LOG2_MASK_SIZE ] >> key & 1u );

        [MethodImpl( INLINE )] 
        public void SetTrue( int key ) => _bits[ key >> LOG2_MASK_SIZE ] |= 1u << key;

        [MethodImpl( INLINE )] 
        public void SetFalse( int key ) => _bits[ key >> LOG2_MASK_SIZE ] &= ~( 1u << key );

        [MethodImpl( INLINE )] 
        public void Set( int key, bool value )
        {
            if( value )
                SetTrue( key );
            else
                SetFalse( key );
        }

        public bool this[ int key ] 
        { 
            [MethodImpl( INLINE )] 
            get => Get( key );
            [MethodImpl( INLINE )] 
            set => Set( key, value ); 
        }

        /// <summary> 
        /// Sets the bits in the given range from (inclusive) and to (exclusive) to true. 
        /// </summary>
        [MethodImpl( INLINE )] 
        public void SetTrue( int from, int to )
        {
            if( from >= to )
                return;

            int i = from >> LOG2_MASK_SIZE, last = to - 1 >> LOG2_MASK_SIZE;
            
            to = MASK_SIZE - to;

            if( i == last )
            {
                _bits[i] |= ( uint.MaxValue >> to ) & ( uint.MaxValue << from );
                return;
            }

            _bits[ i ] |= uint.MaxValue << from;
            _bits[ last ] |= uint.MaxValue >> to;

            for( i++; i < last; i++ )
                _bits[ i ] = uint.MaxValue;
        }

        /// <summary> 
        /// Sets the bits in the given range from (inclusive) and to (exclusive) to false. 
        /// </summary>
        [MethodImpl( INLINE )] 
        public void SetFalse( int from, int to )
        {
            if( from >= to )
                return;

            int i = from >> LOG2_MASK_SIZE, last = to - 1 >> LOG2_MASK_SIZE;
            
            to = MASK_SIZE - to;

            if( i == last )
            {
                _bits[ i ] &= ~( ( uint.MaxValue >> to ) & ( uint.MaxValue << from ) );
                return;
            }

            _bits[ i ] &= ~( uint.MaxValue << from );
            _bits[ last ] &= ~( uint.MaxValue >> to );

            for( i++; i < last; i++ )
                _bits[ i ] = uint.MinValue;
        }

        /// <summary> 
        /// Sets the bits in the given range from (inclusive) and to (exclusive) to the specified value. 
        /// </summary>
        [MethodImpl( INLINE )] 
        public void Set( int from, int to, bool value )
        {
            if( value )
                SetTrue( from, to );
            else
                SetFalse( from, to );
        }

        /// <summary> 
        /// Sets all bits in the bitset to the specified value. 
        /// </summary>
        [MethodImpl( INLINE )] 
        public void SetAll( bool value )
        {
            uint mask = value ? uint.MaxValue : uint.MinValue;

            uint[] bits = _bits;

            for( int i = 0; i < bits.Length; i++ )
                bits[ i ] = mask;
        }

    #endregion

    #region Resize

        /// <summary> 
        /// Changes the number of bits of this bitset to the specified new length. 
        /// </summary>
        [MethodImpl( INLINE )] 
        public void Resize( int length )
        {
            if ( !IsValidLength( ref length ) )
            {
                _bits = Array.Empty<uint>();
                return;
            }

            if( length == _bits.Length )
                return;
            
            var newArray = new uint[ length ];
            
            Array.Copy( _bits, 0, newArray, 0, _bits.Length >= length ? length : _bits.Length );

            _bits = newArray;
        }

    #endregion

    #region PopCount

        /// <summary> 
        /// Returns the population count (number of bits set) in the given range from (inclusive) and to (exclusive). 
        /// </summary>
        [MethodImpl( INLINE )] 
        public int PopCount( int from, int to )
        {
            if( from >= to )
                return 0;

            int i = from >> LOG2_MASK_SIZE, last = to - 1 >> LOG2_MASK_SIZE;
            
            to = MASK_SIZE - to;

            if( i == last )
                return HammingWeight( _bits[ i ] & ( ( uint.MaxValue << from ) & ( uint.MaxValue >> to ) ) );

            int count = HammingWeight( ( _bits[ i ] & ( uint.MaxValue << from ) ) | 
                                       ( (ulong)( _bits[ last ] & ( uint.MaxValue >> to ) ) << MASK_SIZE ) );

            for( i++; i < last; i++ )
                count += HammingWeight( _bits[ i ] );

            return count;
        }

        /// <summary> 
        /// Returns the population count (number of bits set) of this set. 
        /// </summary>
        [MethodImpl( INLINE )]
        public int PopCount()
        { 
            int count = 0;

            foreach( var mask in _bits )
                count += HammingWeight( mask );

            return count;
        }

        [MethodImpl( INLINE )] 
        private static int HammingWeight( uint mask )
        {
            mask -= ( mask >> 1 ) & 0x_55555555u;
            mask = ( mask & 0x_33333333u ) + ( ( mask >> 2 ) & 0x_33333333u );
            mask = ( ( ( mask + ( mask >> 4 ) ) & 0x_0F0F0F0Fu ) * 0x_01010101u ) >> 24;

            return (int)mask;
        }

        [MethodImpl( INLINE )] 
        private static int HammingWeight( ulong mask )
        {
            mask -= ( mask >> 1 ) & 0x_55555555_55555555ul;
            mask = ( mask & 0x_33333333_33333333ul ) + ( ( mask >> 2 ) & 0x_33333333_33333333ul );
            mask = ( ( ( mask + ( mask >> 4 ) ) & 0x_0F0F0F0F_0F0F0F0Ful ) * 0x_01010101_01010101ul ) >> 56;

            return (int)mask;
        }

    #endregion
    }
}
