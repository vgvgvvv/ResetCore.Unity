using UnityEngine;
using System.Collections;
using System;

public class GCHelper {

	public static void GCHandle()
    {
        GC.Collect();
    }
}
