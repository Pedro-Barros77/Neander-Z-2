using System;
using System.Runtime.InteropServices;
using UnityEngine;

public class FileDialogManager : MonoBehaviour
{
    [DllImport("__Internal")]
    private static extern void openFilePicker();

    [DllImport("__Internal")]
    private static extern void downloadFile(byte[] fileName, int fileNameLength, byte[] content, int contentLength);

    public static Action<string> OnFileSelectedEvent;

    private static bool DestroyOnReceived;

    /// <summary>
    /// Função chamada pelo JavaScript para enviar o conteúdo do arquivo selecionado
    /// </summary>
    /// <param name="fileContent"></param>
    public void OnFileSelected(string fileContent)
    {
        Debug.Log("File content received: " + fileContent);
        OnFileSelectedEvent?.Invoke(fileContent);
        if (DestroyOnReceived)
            Destroy(gameObject);
    }

    /// <summary>
    /// Solicita ao javascript para abrir a janela de seleção de arquivo.
    /// </summary>
    /// <param name="onSelectedCallback">O que fazer quando o usuário terminar de selecionar o arquivo.</param>
    /// <param name="destroyOnReceived">Se esse componente se destruir ao receber a resposta.</param>
    public static void RequestFileFromUser(Action<string> onSelectedCallback, bool destroyOnReceived = true)
    {
        OnFileSelectedEvent = onSelectedCallback;
        DestroyOnReceived = destroyOnReceived;

        var dialogManagerObj = Instantiate(Resources.Load<GameObject>("Prefabs/UI/FileDialogManager"));
        dialogManagerObj.name = "FileDialogManager";

#if UNITY_WEBGL && !UNITY_EDITOR
        openFilePicker();
        //Application.ExternalEval("openFilePicker();");
#endif
    }

    /// <summary>
    /// Solicita ao javascript para fazer o download de um arquivo.
    /// </summary>
    /// <param name="fileName">O nome do arquivo ao ser baixado.</param>
    /// <param name="content">O conteúdo do arquivo em texto.</param>
    public static void DownloadFile(string fileName, string content)
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        byte[] fileNameBytes = System.Text.Encoding.UTF8.GetBytes(fileName);
        byte[] contentBytes = System.Text.Encoding.UTF8.GetBytes(content);
        downloadFile(fileNameBytes, fileNameBytes.Length, contentBytes, contentBytes.Length);
#endif
    }
}
