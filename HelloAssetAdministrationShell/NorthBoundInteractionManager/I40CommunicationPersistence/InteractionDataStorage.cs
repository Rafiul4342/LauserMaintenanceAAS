
using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace HelloAssetAdministrationShell.NorthBoundInteractionManager.I40CommunicationPersistence
{
    public static class InteractionDataStorage
    {
        private const string MaintenanceCounterFilePath = "maintenanceCounter.json";
        private const string ConversationTrackerFilePath = "conversationTracker.json";

        public static void SaveMaintenanceCounter(Dictionary<string, int> maintenanceCounter)
        {
            string jsonData = JsonConvert.SerializeObject(maintenanceCounter);
            File.WriteAllText(MaintenanceCounterFilePath, jsonData);
        }

        public static Dictionary<string, int> LoadMaintenanceCounter()
        {
            try
            {
                if (File.Exists(MaintenanceCounterFilePath))
                {
                    string jsonData = File.ReadAllText(MaintenanceCounterFilePath);

                    if (!string.IsNullOrEmpty(jsonData))
                    {
                        return JsonConvert.DeserializeObject<Dictionary<string, int>>(jsonData);
                    }
                }

                return new Dictionary<string, int>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading maintenance counter: {ex.Message}");
                // You may want to log the exception or take other appropriate actions.
                return new Dictionary<string, int>();
            }
        }
        
        public static void SaveConversationTracker(Dictionary<string, ConversationInfo> conversationTracker)
        {
            string jsonData = JsonConvert.SerializeObject(conversationTracker);
            File.WriteAllText(ConversationTrackerFilePath, jsonData);
        }

        public static Dictionary<string, ConversationInfo> LoadConversationTracker()
        {
            try
            {
                if (File.Exists(ConversationTrackerFilePath))
                {
                    string jsonData = File.ReadAllText(ConversationTrackerFilePath);

                    if (!string.IsNullOrEmpty(jsonData))
                    {
                        return JsonConvert.DeserializeObject<Dictionary<string, ConversationInfo>>(jsonData);
                    }
                }

                return new Dictionary<string, ConversationInfo>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading conversation tracker: {ex.Message}");
                // You may want to log the exception or take other appropriate actions.
                return new Dictionary<string, ConversationInfo>();
            }
        }
    }
        // Add other methods if needed
}
