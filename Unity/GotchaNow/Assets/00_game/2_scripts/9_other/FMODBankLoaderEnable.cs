using UnityEngine;
using FMODUnity;
using System.Collections.Generic;
using System;

namespace GotchaNow
{
	[DefaultExecutionOrder(-1000)]
    [RequireComponent(typeof(StudioBankLoader))]
    public class FMODBankLoaderEnable : MonoBehaviour
	{
		private StudioBankLoader bankLoader;

		private void Start()
		{
			bankLoader = GetComponent<StudioBankLoader>();

			List<String> banks =  bankLoader.Banks;

			bool unloadedBanks = false;
			foreach (String bank in banks)
			{
                if (RuntimeManager.HasBankLoaded(bank)) continue;
				unloadedBanks = true;
				break;
			}
			if (!unloadedBanks) return;
			bankLoader.enabled = true;
		}
	}
}
