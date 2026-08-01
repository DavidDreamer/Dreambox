using System.Runtime.CompilerServices;

namespace Dreambox.Math
{
	public static class IntUtils
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int ClosestOdd(this int value) => value | 1;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int ClosestEven(this int value) => value & ~1;
	}
}
