using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Utils
{

	public static class Utils
	{

		public static float ConvertToNewRange(float OldMin, float OldMax, float NewMin, float NewMax, float OldValue)
		{
			OldValue = Mathf.Clamp(OldValue, OldMin, OldMax);
			float OldRange = (OldMax - OldMin);
			float NewRange = (NewMax - NewMin);
			return (((OldValue - OldMin) * NewRange) / OldRange) + NewMin;
		}

	}

	public class MyKeyValuePair<T, K>
	{
		public MyKeyValuePair(T key, K val) { this.key = key; this.value = val; }
		public T key;
		public K value;
	}

}