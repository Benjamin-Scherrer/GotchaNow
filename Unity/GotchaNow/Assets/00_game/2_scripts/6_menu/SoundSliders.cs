using System;
using UnityEngine;
using UnityEngine.UI;

namespace GotchaNow
{
	public class SoundSliders : MonoBehaviour
	{
		[Serializable]
		public class VolumeSaveData
		{
			public float SFXVolume;
			public float MusicVolume;
		}


		[Header("References")]
		[SerializeField] private Slider SFXSlider;
		[SerializeField] private Slider MusicSlider;
		[SerializeField] private VolumeControl VolumeControl;

		private void Awake()
		{
			LoadVolumeSettings();

			if (SFXSlider == null)
			{
				Debug.LogError("SoundSliders: SFXSlider reference is missing.");
			}

			if (MusicSlider == null)
			{
				Debug.LogError("SoundSliders: MusicSlider reference is missing.");
			}

			if (VolumeControl == null)
			{
				Debug.LogError("SoundSliders: VolumeControl reference is missing.");
			}

			SFXSlider.value = VolumeControl.SFXVolume;
			MusicSlider.value = VolumeControl.MusicVolume;
		}

		private void OnApplicationQuit()
		{
			SaveVolumeSettings();
			Debug.Log("SoundSliders: OnDestroy called, volume settings saved.");
		}

        private void OnDestroy()
        {
            SaveVolumeSettings();
			Debug.Log("SoundSliders: OnDestroy called, volume settings saved.");
        }

        private void OnEnable()
		{
			SFXSlider.value = VolumeControl.SFXVolume;
			MusicSlider.value = VolumeControl.MusicVolume;

			SFXSlider.onValueChanged.AddListener(OnSFXSliderChanged);
			MusicSlider.onValueChanged.AddListener(OnMusicSliderChanged);
		}

		private void OnDisable()
		{
			SFXSlider.onValueChanged.RemoveListener(OnSFXSliderChanged);
			MusicSlider.onValueChanged.RemoveListener(OnMusicSliderChanged);
		}

		private void OnSFXSliderChanged(float value)
		{
			VolumeControl.SFXVolume = value;
		}

		private void OnMusicSliderChanged(float value)
		{
			VolumeControl.MusicVolume = value;
		}

		private void SaveVolumeSettings()
		{
			float sfxVolume = VolumeControl.SFXVolume;
			float musicVolume = VolumeControl.MusicVolume;

            VolumeSaveData volumeSave = new()
            {
                SFXVolume = sfxVolume,
                MusicVolume = musicVolume
            };

            String SaveFilePath = Application.persistentDataPath + "/QiAffinitySaveData";
			String SaveFileText = JsonUtility.ToJson(volumeSave);
			Debug.Log("Saved Save Data: " + SaveFileText);
			System.IO.File.WriteAllText(SaveFilePath, SaveFileText);
		}

		private void LoadVolumeSettings()
		{
			String SaveFilePath = Application.persistentDataPath + "/QiAffinitySaveData";
			if (!System.IO.File.Exists(SaveFilePath)) return;
			String SaveFileText = System.IO.File.ReadAllText(SaveFilePath);
			if (String.IsNullOrEmpty(SaveFileText))
			{
				Debug.LogWarning("SoundSliders: Save file is empty.");
				return;
			}
			Debug.Log("Loaded Save Data: " + SaveFileText);
			VolumeSaveData volumeSave = null;
			try
			{
				volumeSave = JsonUtility.FromJson<VolumeSaveData>(SaveFileText);
				Debug.Log("Deserialized Volume Data: SFXVolume = " + volumeSave.SFXVolume + ", MusicVolume = " + volumeSave.MusicVolume);
			}
			catch (Exception e)
			{
				Debug.LogWarning("SoundSliders: Failed to load volume settings. Exception: " + e.Message);
			}
			if (volumeSave != null)
			{
				VolumeControl.SFXVolume = volumeSave.SFXVolume;
				VolumeControl.MusicVolume = volumeSave.MusicVolume;
				Debug.Log("Volumes loaded: SFXVolume = " + volumeSave.SFXVolume + ", MusicVolume = " + volumeSave.MusicVolume);
			}
            else
            {
                VolumeControl.SFXVolume = 1f;
				VolumeControl.MusicVolume = 0.8f;
				Debug.Log("Volumes set to default values.");
            }	
		}
	}
}
