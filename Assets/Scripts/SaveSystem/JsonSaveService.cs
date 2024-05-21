using Newtonsoft.Json;
using System;
using System.IO;
using UnityEngine;

public class JsonSaveService
{
    public readonly string ROOT_FOLDER = Path.Combine(Application.persistentDataPath, "Saves");
    public string SAVE_EXTENSION => save_extension;
    const string save_extension = "nzsave";
    public string CombinePaths(params string[] paths) => Path.GetFullPath(Path.Combine(paths));

    /// <summary>
    /// Salva o objeto especificado em um arquivo JSON.
    /// </summary>
    /// <typeparam name="T">O Tipo do objeto a ser salvo.</typeparam>
    /// <param name="relativePath">O caminho relativo da pasta onde o arquivo será salvo.</param>
    /// <param name="fileName">O nome do arquivo a ser salvo, sem a extensãi.</param>
    /// <param name="data">O objeto a ser salvo.</param>
    /// <param name="encrypted">Se o arquivo deve ser criptografado para proteger seu conteúdo</param>
    /// <returns>True se o arquivo foi salvo com sucesso.</returns>
    public bool SaveData<T>(string relativePath, string fileName, T data, bool encrypted, string extension = save_extension)
    {
        string folderPath = CombinePaths(ROOT_FOLDER, relativePath);
        string path = CombinePaths(folderPath, $"{fileName}.{extension}");

        try
        {
            if (File.Exists(path))
                File.Delete(path);

            Directory.CreateDirectory(folderPath);
            using FileStream fs = File.Create(path);
            fs.Close();
            string jsonData = JsonConvert.SerializeObject(data, Constants.FormatSaveFiles ? Formatting.Indented : Formatting.None);
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
    /// Baixa o arquivo no caminho especificado para a pasta downloads.
    /// </summary>
    /// <param name="fullFolderPath">O caminho completo da pasta.</param>
    /// <param name="fileName">O nome do arquivo.</param>
    /// <param name="extension">A extensão do arquivo.</param>
    /// <returns>True se foi baixado com sucesso.</returns>
    public bool DownloadData(string fullFolderPath, string fileName, string extension = save_extension)
    {
        string sourcePath = CombinePaths(fullFolderPath, $"{fileName}.{extension}");

        string downloadsFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads");
        string destinationPath = Path.Combine(downloadsFolder, $"{fileName}.{extension}");

        try
        {
            if (!File.Exists(sourcePath))
                return false;

            if (!Directory.Exists(downloadsFolder))
                Directory.CreateDirectory(downloadsFolder);

            File.Copy(sourcePath, destinationPath, true);

            return true;
        }
        catch (Exception e)
        {
            Debug.LogError($"Error downloading file: {e.Message}/n/n/n{e.StackTrace}");
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
    public T LoadData<T>(string relativePath, string fileName, bool encrypted, string extension = save_extension)
    {
        string path = CombinePaths(ROOT_FOLDER, relativePath, $"{fileName}.{extension}");

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
