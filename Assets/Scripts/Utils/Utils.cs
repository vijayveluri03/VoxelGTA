using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Utils
{

	public static class Utils
	{

	}

	public class MyKeyValuePair<T, K>
	{
		public MyKeyValuePair(T key, K val) { this.key = key; this.value = val; }
		public T key;
		public K value;
	}
}