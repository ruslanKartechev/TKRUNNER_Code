using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BzKovSoft.CharacterSlicer;
using BzKovSoft.ObjectSlicer;
using System.Diagnostics;
using UnityEngine.Profiling;
using BzKovSoft.CharacterSlicerSamples;
using System;


namespace TKRunner
{

	public class DummySlicer : CharacterSlicerSampleFast
	{

		public void SliceFrameDelayed(Plane plane, int sliceId, Action<BzSliceTryResult> callBack)
		{
			StartCoroutine(SliceWithDelay(plane,sliceId,callBack));
		}
		private IEnumerator SliceWithDelay(Plane plane, int sliceId, Action<BzSliceTryResult> callBack)
        {
			yield return null;
			Slice(plane, 1, callBack);
		}

	}
}