using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JsonParser<T>
{
	public static string ToJson(T obj)
	{
		return JsonUtility.ToJson(obj);
	}

	public static T FromJson(string json)
	{
		T obj = default(T);
		try
		{
			obj = JsonUtility.FromJson<T>(json);
		}
		catch
		{
			Debug.LogError("Invalid json format!");
			return obj;
		}
		return obj;
	}
}
