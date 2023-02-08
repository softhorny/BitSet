using System;
using System.Runtime.CompilerServices;

namespace softh.Collections
{
    public sealed class BitSet
    {
        private const MethodImplOptions INLINE = MethodImplOptions.AggressiveInlining;

        private const int MASK_SIZE = 32;

        private const int LOG2_MASK_SIZE = 5;

        /// <summary> 
        /// Create a new bitset of the given length. All bits are initially false. 
        /// Length will be rounded up to a multiple of mask size (32). 
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

        public Bit this[ int offset ] 
        { 
            [MethodImpl( INLINE )] 
            get => Bit.GetFrom( _bits[ offset >> LOG2_MASK_SIZE ], offset );
        }

        [MethodImpl( INLINE )] 
        public void SetTrue( int offset ) => _bits[ offset >> LOG2_MASK_SIZE ] |= 1u << offset;

        [MethodImpl( INLINE )] 
        public void SetFalse( int offset ) => _bits[ offset >> LOG2_MASK_SIZE ] &= ~( 1u << offset );

        /// <summary> 
        /// Sets the bits in the given range from (inclusive) and to (exclusive) to true. 
        /// </summary>
        [MethodImpl( INLINE )] 
        public void SetTrue( int from, int to )
        {
            if( from >= to )
            {
                return;
            }

            ( int start, int end ) = ( from >> LOG2_MASK_SIZE, to - 1 >> LOG2_MASK_SIZE );
            
            to = MASK_SIZE - to;

            if( start == end )
            {
                _bits[ start ] |= ( ~0u >> to ) & ( ~0u << from );

                return;
            }

            _bits[ start ] |= ~0u << from;
            _bits[ end ] |= ~0u >> to;

            for( start++; start < end; start++ )
            {
                _bits[ start ] = ~0u;
            }
        }

        /// <summary> 
        /// Sets the bits in the given range from (inclusive) and to (exclusive) to false. 
        /// </summary>
        [MethodImpl( INLINE )] 
        public void SetFalse( int from, int to )
        {
            if( from >= to )
            {
                return;
            }

            ( int start, int end ) = ( from >> LOG2_MASK_SIZE, to - 1 >> LOG2_MASK_SIZE );
            
            to = MASK_SIZE - to;

            if( start == end )
            {
                _bits[ start ] &= ~( ( ~0u >> to ) & ( ~0u << from ) );

                return;
            }

            _bits[ start ] &= ~( ~0u << from );
            _bits[ end ] &= ~( ~0u >> to );

            for( start++; start < end; start++ )
            { 
                _bits[ start ] = 0u;
            }
        }

        /// <summary> 
        /// Sets all bits in the bitset to true.
        /// </summary>
        [MethodImpl( INLINE )] 
        public void SetTrue()
        {
            var bits = _bits;

            for( int i = 0; i < bits.Length; i++ )
            {
                bits[ i ] = ~0u;
            }
        }

        /// <summary> 
        /// Sets all bits in the bitset to false. 
        /// </summary>
        [MethodImpl( INLINE )] 
        public void SetFalse()
        {
            var bits = _bits;

            for( int i = 0; i < bits.Length; i++ )
            {
                bits[ i ] = 0u;
            }
        }

    #endregion

    #region Resize

        /// <summary> 
        /// Changes the number of bits of this bitset to the specified new size. 
        /// </summary>
        [MethodImpl( INLINE )] 
        public void Resize( int newSize )
        {
            if ( !IsValidLength( ref newSize ) )
            {
                _bits = Array.Empty<uint>();

                return;
            }

            if( newSize == _bits.Length )
            {
                return;
            }
            
            var newArray = new uint[ newSize ];
            
            Array.Copy( _bits, 0, newArray, 0, _bits.Length >= newSize ? newSize : _bits.Length );

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
            {
                return 0;
            }

            ( int start, int end ) = ( from >> LOG2_MASK_SIZE, to - 1 >> LOG2_MASK_SIZE );
            
            to = MASK_SIZE - to;

            if( start == end )
            {
                return Bit.PopCount( _bits[ start ] & ( ( ~0u << from ) & ( ~0u >> to ) ) );
            }

            int count = Bit.PopCount( ( _bits[ start ] & ( ~0u << from ) ) 
                      | ( (ulong)( _bits[ end ] & ( ~0u >> to ) ) << MASK_SIZE ) );

            for( start++; start < end; start++ )
            {
                count += Bit.PopCount( _bits[ start ] );
            }

            return count;
        }

        /// <summary> 
        /// Returns the population count (number of bits set) of this set. 
        /// </summary>
        [MethodImpl( INLINE )]
        public int PopCount()
        { 
            int count = 0;

            for ( int i = 0; i < Length; i++ )
            {
                count += Bit.PopCount( _bits[ i ] ); 
            }

            return count;
        }

    #endregion
    }
}
