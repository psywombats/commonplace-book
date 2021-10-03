using Newtonsoft.Json;
using System;
using System.Collections;
using System.IO;
using UnityEngine;

public class SerializationManager : SingletonBehavior {

    public const int SaveSlotCount = 5;

    private const int CurrentSaveVersion = 1;
    private const string SystemMemoryName = "system.sav";
    private const string SaveGameSuffix = ".sav";

    public GameData Data { get; private set; }

    private SystemData systemData;
    public SystemData SystemData {
        get {
            if (systemData == null) {
                LoadOrCreateSystemMemory();
            }
            return systemData;
        }
    }

    private JsonSerializer serializer;
    private JsonSerializer Serializer  {
        get {
            if (serializer == null) {
                var settings = new JsonSerializerSettings();
                serializer = JsonSerializer.Create(settings);
            }
            return serializer;
        }
    }

    public void Awake() {
        ResetData();
    }
    
    public override void GameReset() {
        base.GameReset();
        ResetData();
    }

    public void ResetData() {
        Data = new GameData(this);
    }

    public void SaveToSlot(int slot) {
        Data.SaveVersion = CurrentSaveVersion;
        Data.SavedAt = UIUtils.CurrentTimestamp();
        WriteJsonToFile(Data, FilePathForSlot(slot));

        SystemData.LastSaveSlot = slot;
        SystemData.SaveInfo[slot] = new SaveInfoData(Data);
        SaveSystemMemory();
    }

    public GameData LoadGameDataForSlot(int slot) {
        SystemData.LastSaveSlot = slot;
        var fileName = FilePathForSlot(slot);
        if (File.Exists(fileName)) {
            return ReadJsonFromFile<GameData>(fileName);
        } else {
            return null;
        }
    }

    public bool DoSavedGamesExist() {
        return SystemData.LastSaveSlot > -1;
    }

    public void SaveSystemMemory() {
        WriteJsonToFile(SystemData, GetSystemMemoryFilepath());
    }

    public T ReadJsonFromString<T>(string str) {
        return Serializer.Deserialize<T>(new JsonTextReader(new StringReader(str)));
    }

    private T ReadJsonFromFile<T>(string fileName) {
        var json = File.ReadAllText(fileName);
        return ReadJsonFromString<T>(json);
    }
    
    private void WriteJsonToFile(object toSerialize, string fileName) {
        FileStream file = File.Open(fileName, FileMode.Create);
        StreamWriter writer = new StreamWriter(file);
        Serializer.Serialize(writer, toSerialize);
        writer.Flush();
        writer.Close();
    }

    private string GetSystemMemoryFilepath() {
        return Application.persistentDataPath + "/" + SystemMemoryName;
    }

    private string FilePathForSlot(int slot) {
        string fileName = Application.persistentDataPath + "/";
        fileName += slot.ToString();
        fileName += SaveGameSuffix;
        return fileName;
    }

    private void LoadOrCreateSystemMemory() {
        string path = GetSystemMemoryFilepath();
        if (File.Exists(path)) {
            systemData = ReadJsonFromFile<SystemData>(path);

        } else {
            systemData = new SystemData();
        }
    }
}
