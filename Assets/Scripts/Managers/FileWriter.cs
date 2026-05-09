using System.IO;
using UnityEngine;

public class FileWriter : MonoBehaviour
{
    // Made with claude
    public static FileWriter instance;

    private string filePath;
    private string folderPath;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

        folderPath = Path.Combine(Application.persistentDataPath, "ParticipantFiles", "ParticipantData");

        if (!Directory.Exists(folderPath)) Directory.CreateDirectory(folderPath);

        int participantNumber = 1;
        while (File.Exists(Path.Combine(folderPath, $"Participant_{participantNumber}.csv")))
        {
            participantNumber++;
        }

        filePath = Path.Combine(folderPath, $"Participant_{participantNumber}.csv");
        File.WriteAllText(filePath, "Gamebalance\n");

    }

    public void LogGameBalance()
    {
        string line = $"Game balance: {AiManager.Instance.EvaluateGameBalance()}\n";
        File.AppendAllText(filePath, line);
    }

    public void LogGameEnd(string text)
    {
        string line = $"End of game info: {text}, turns: {Gamemanager.instance.turns}, player troops: {Gamemanager.instance.playerTroopsCreated}, ai troops: {Gamemanager.instance.aiTroopsCreated}";
        File.AppendAllText(filePath, line);
    }
}
