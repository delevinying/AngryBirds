using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TraceLog{
	public static void traceLog(string className,string funName,string str){
		Debug.Log (className + "--" + funName + "-->" + str);
	}
}
