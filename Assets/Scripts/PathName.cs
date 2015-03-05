using UnityEngine;
using System.Collections;

public class PathName {
	/// <summary>
	/// Prints the full transform path of the target object
	/// </summary>
	/// <param name="recurse">Recurse >= 0 specifies a maximum number of recursions</param>
	public static string Path (GameObject target, int recurse = -1) {
		string path = target.name;
		if (recurse != 0 &&
			target.transform.parent != null) {
			path = Path(target.transform.parent.gameObject) + "/" + path;
		}
		return path;
	}
}
