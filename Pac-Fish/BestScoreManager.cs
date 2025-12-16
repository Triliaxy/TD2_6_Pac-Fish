using System;
using System.IO;
using System.Text.Json;

namespace Pac_Fish
{
    public class BestScoreContainer //definition du format
    {
        public int HighScoreValue { get; set; } 
    }

    public static class BestScoreManager
    {
        private static readonly string BestScoreFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "BestScore.json"); //AppDomain.CurrentDomain.BaseDirectory est le répertoire de l'application

        public static int LoadBestScore()
        {
            try
            {
                if (File.Exists(BestScoreFilePath))
                {
                    string jsonContent = File.ReadAllText(BestScoreFilePath);
                    var scoreContainer = JsonSerializer.Deserialize<BestScoreContainer>(jsonContent); //JsonSerializer.Deserialize permet d’utiliser le contenu du json comme un objet | <BestScoreContainer> type de l’objet json

                    if (scoreContainer != null)
                    {
                        return scoreContainer.HighScoreValue;
                    }
                }
            }
            catch{ }//debbug : attrape toutes les erreurs
            return 0;//retourne 0 en cas d’erreur ( par exemple si le fichier est vide )
        }

        public static void SaveBestScore(int score)
        {
            try
            {
                var scoreContainer = new BestScoreContainer { HighScoreValue = score };
                string jsonContent = JsonSerializer.Serialize(scoreContainer);
                File.WriteAllText(BestScoreFilePath, jsonContent);
            }
            catch (Exception) { }
        }
    }
}
