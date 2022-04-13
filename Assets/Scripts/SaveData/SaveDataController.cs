using System;
using System.IO;
using System.Xml.Serialization;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class SaveDataController
{
	private static readonly string SAVE_PATH = Application.persistentDataPath + "/SaveData.xml";

	public void Save(SaveData saveData)
	{
		XmlSerializer writer = new XmlSerializer(typeof(SaveData));

		FileStream file = File.Create(SAVE_PATH);

		writer.Serialize(file, saveData);
		file.Close();
	}

	public SaveData Load()
	{
		SaveData saveData = null;

		try
		{
			XmlSerializer reader = new XmlSerializer(typeof(SaveData));
			StreamReader file = new StreamReader(SAVE_PATH);
			saveData = (SaveData)reader.Deserialize(file);
			file.Close();
		}
		catch
		{
			saveData = new SaveData();
			Save(saveData);
		}

		return saveData;
	}

	public void UpdateScore(SaveData saveData, int levelID, float score)
	{
		foreach(SaveData.Level level in saveData.levels)
		{
			if(level.ID == levelID)
			{
				if(score < level.score)
				{
					level.score = score;
				}
				return;
			}
		}

		SaveData.Level newLevel = new SaveData.Level()
		{
			ID = levelID,
			score = score
		};
		saveData.levels.Add(newLevel);
	}
}
