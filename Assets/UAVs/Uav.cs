using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;

namespace UAVs
{
	public class Uav : MonoBehaviour
	{
		private int _id;

		[NonSerialized] public Navigator Navigator;
		[NonSerialized] public string CodeName;
		[NonSerialized] public string AbbrvName;
		[NonSerialized] public List<Path> Paths;
		[NonSerialized] public Path CurrentPath;

		public int ID
		{
			get => _id;
			set => SetIDandNames(value);
		}

		public event EventHandler UAVIdisSet;

		public Uav(int id = 999)
		{
			ID = id;
		}

		void Start()
		{
		}

		private void SetIDandNames(int value)
		{
			_id = value;
			AbbrvName = NatoAlphabetConverter.IntToLetters(value);
			CodeName = NatoAlphabetConverter.LettersToName(AbbrvName);
		}

		private void Navigate()
		{
		}
	}
}