using System;
using System.Collections.Generic;
using System.Linq;

namespace HelperScripts
{
	public static class NatoAlphabetConverter
	{
		static Dictionary<char, string> _dDict = new Dictionary<char, string>()
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
		
		private static HashSet<string> _uniqueNames = new HashSet<string>();

		private static int randomSeed = -999;
		private static Random random = new Random();
		public static string LettersToName(string abbrvName)
		{
			string result = string.Join(" ", abbrvName.Select(x => _dDict[char.ToLower(x)]));
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
		public static string IntToLetters(int id, bool offsetZero = true,bool resetAfter26 = false)
		{
			var result = string.Empty;
			
			if (offsetZero is true)
				id += 1;

			if (resetAfter26)
				id = id % 26;
			
			if (id == 0)
				result = "A";
			
			while (--id >= 0)
			{
				result = (char)('A' + id % 26) + result;
				id /= 26;
			}

			return result;
		}
		
		public static string LettersToUniqueName(string input)
		{
			var firstLetter = char.ToUpper(input[0]);
			var firstName=LettersToName(firstLetter.ToString());
			var name = "";
	
			//set up random seed (if not already set)
			if (randomSeed == -999)
			{
				randomSeed = GameManager.Instance.settingsDatabase.randomSeed;
				//create new instance of system random
				random = new Random(randomSeed);
			}
			
			do
			{
				var randomNumber = random.Next(1, GameManager.Instance.settingsDatabase.uavSettings.maxUavNumberAfterName);
				name = $"{firstName} {randomNumber}";
			} while (_uniqueNames.Contains(name));

			_uniqueNames.Add(name);
			return name;
		}
	}
}