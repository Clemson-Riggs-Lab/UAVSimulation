using System.Collections.Generic;
using System.Linq;

namespace HelperScripts
{
	public static class NatoAlphabetConverter
	{
		static Dictionary<char, string> dDict = new Dictionary<char, string>()
		{
			{ 'a', "Alfa" }, { 'b', "Bravo" }, { 'c', "Charlie" },
			{ 'd', "Delta" }, { 'e', "Echo" }, { 'f', "Frank" },
			{ 'g', "Golf" }, { 'h', "Hotel" }, { 'i', "India" },
			{ 'j', "John" }, { 'k', "Kilo" }, { 'l', "Lima" },
			{ 'm', "Mike" }, { 'n', "Nectar" }, { 'o', "Oscar" },
			{ 'p', "Papa" }, { 'q', "Quebec" }, { 'r', "Romeo" },
			{ 's', "Sierra" }, { 't', "Tango" }, { 'u', "Uniform" },
			{ 'v', "Victor" }, { 'w', "Whiskey" }, { 'x', "X-Ray" },
			{ 'y', "Yankee" }, { 'z', "Zulu" }
		};

		public static string LettersToName(string abbrvName)
		{
			string result = string.Join(" ", abbrvName.Select(x => dDict[char.ToLower(x)]));
			return result;
		}

		/// <summary>
		/// This method returns the alpha numeric representation of a number
		/// e.i., 1=A, 2=B, ...
		/// If a number is greater than 26, the representation is by two letters
		/// i.e., 27=AA, 28=AB, ...
		/// The offsetZero specifies if the id-char pairing should be shifted by one to account for zero indexing
		/// i.e., 0=A, 1=B, ...
		/// offsetZero is defaulted to true.
		/// </summary>
		/// <param name="id"></param>
		/// <param name="offsetZero"></param>
		/// <returns></returns>
		public static string IntToLetters(int id, bool offsetZero = true)
		{
			if (offsetZero is true)
			{
				id += 1;
			}

			var result = string.Empty;
			while (--id >= 0)
			{
				result = (char)('A' + id % 26) + result;
				id /= 26;
			}

			return result;
		}
	}
}