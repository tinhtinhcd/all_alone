using Godot;
using System;
using System.Collections;
using System.Collections.Generic;


namespace ZombieHoardGame
{
    public partial class SavedData : Node
    {
        
        private const String LevelBestsFilePath = "user://level_bests.cfg";

        public int LevelHighestRound(String levelName)
        {
            ConfigFile levelBestsFile = new ConfigFile();
            Error err = levelBestsFile.Load(LevelBestsFilePath);
            if (err != Error.Ok)
            {
                return 0;
            }
            else
            {
                return (int)levelBestsFile.GetValue(levelName, "round", 0);
            }
        }

        public void UpdateLevelHighestRound(String levelName, int newBest)
        {
            FileAccess fileChecker = new FileAccess();
            if (!fileChecker.FileExists(LevelBestsFilePath))
            {
                // Create new file
                fileChecker.Open(LevelBestsFilePath, FileAccess.ModeFlags.Write);
            }
            fileChecker.Close();

            ConfigFile levelBestsFile = new ConfigFile();
            Error err = levelBestsFile.Load(LevelBestsFilePath);
            if (err == Error.Ok)
            {
                levelBestsFile.SetValue(levelName, "round", newBest);
                levelBestsFile.Save(LevelBestsFilePath);
            }
        }
    }
}

