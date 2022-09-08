using System;
using System.ComponentModel;
using System.Linq;

namespace HelperScripts
{
	public static class EnumHelpers
	{
		public static T GetMaxEnum<T>(params T[] enums) where T : struct, IConvertible
		{
			if (enums.Length < 2)
			{
				throw new InvalidEnumArgumentException();
			}
			return enums.Max();
		}
	}
}