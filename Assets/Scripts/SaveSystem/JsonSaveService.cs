using Newtonsoft.Json;
using System;
using System.IO;
using UnityEngine;

public class JsonSaveService
{
    readonly string ROOT_FOLDER = Path.Combine(Application.persistentDataPath, "Saves");
    string CombinePaths(params string[] paths) => Path.GetFullPath(Path.Combine(paths));

    /// <summary>
    /// Salva o objeto especificado em um arquivo JSON.
    /// </summary>
    /// <typeparam name="T">O Tipo do objeto a ser salvo.</typeparam>
    /// <param name="relativePath">O caminho relativo da pasta onde o arquivo será salvo.</param>
    /// <param name="fileName">O nome do arquivo a ser salvo, sem a extensãi.</param>
    /// <param name="data">O objeto a ser salvo.</param>
    /// <param name="encrypted">Se o arquivo deve ser criptografado para proteger seu conteúdo</param>
    /// <returns>True se o arquivo foi salvo com sucesso.</returns>
    public bool SaveData<T>(string relativePath, string fileName, T data, bool encrypted)
    {
        string folderPath = CombinePaths(ROOT_FOLDER, relativePath);
        string path = CombinePaths(folderPath, $"{fileName}.json");

        Debug.Log("Path: " + path);

        try
        {
            if (File.Exists(path))
            {
                Debug.Log("File exists, deleting");
                File.Delete(path);
            }
            else
                Debug.Log("Creating new file");

            Directory.CreateDirectory(folderPath);
            using FileStream fs = File.Create(path);
            fs.Close();
            string jsonData = JsonConvert.SerializeObject(data);
            File.WriteAllText(path, jsonData);
            return true;
        }
        catch (Exception e)
        {
            Debug.LogError($"Error saving file: {e.Message}/n/n/n{e.StackTrace}");
            return false;
        }
    }

    /// <summary>
    /// Carrega o arquivo JSON contendo o objeto especificado.
    /// </summary>
    /// <typeparam name="T">O Tipo do objeto a ser carregado.</typeparam>
    /// <param name="relativePath">O caminho relativo da pasta onde o arquivo será salvo.</param>
    /// <param name="fileName">O nome do arquivo a ser salvo, sem a extensãi.</param>
    /// <param name="encrypted">Se o arquivo foi criptografado quando salvo.</param>
    /// <returns>O objeto do tipo especificado, carregado do arquivo JSON.</returns>
    public T LoadData<T>(string relativePath, string fileName, bool encrypted)
    {
        string path = CombinePaths(ROOT_FOLDER, relativePath, $"{fileName}.json");

        if (!File.Exists(path))
        {
            Debug.LogError("File does not exist at path: " + path);
            return default;
        }

        try
        {
            string jsonData = File.ReadAllText(path);
            T data = JsonConvert.DeserializeObject<T>(jsonData);
            return data;
        }
        catch (Exception e)
        {
            Debug.LogError($"Error loading file: {e.Message}/n/n/n{e.StackTrace}");
            return default;
        }
    }
}
