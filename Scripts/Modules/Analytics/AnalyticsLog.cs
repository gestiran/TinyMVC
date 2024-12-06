using System;
using System.IO;
using System.Text;
using TinyMVC.Modules.Saving;
using UnityEngine;

namespace TinyMVC.Modules.Analytics {
    public sealed class AnalyticsLog {
        private static readonly string _pathToLog;
        
        private const string _LOG_FILE_NAME = "Analytics.log";
        
        static AnalyticsLog() => _pathToLog = Path.Combine(Application.persistentDataPath, _LOG_FILE_NAME);
        
        public AnalyticsLog() {
            try {
                string startLog = $"[{GetTime()}]=[App Start]\n";
                
                if (File.Exists(_pathToLog)) {
                    WriteLog($"\n{startLog}");
                } else {
                    WriteLog($"{startLog}", FileMode.OpenOrCreate);
                }
            } catch (Exception exception) {
                Debug.LogError(exception);
            }
        }
        
        public void LogEvent(AnalyticsEvent data) {
            try {
                StringBuilder logResult = new StringBuilder();
                logResult.Append("[");
                logResult.Append(GetTime());
                logResult.Append("]=");
                
                logResult.Append("[");
                logResult.Append(data.eventName);
                logResult.Append("]=");
                
                for (int parameterId = 0; parameterId < data.parameters.Length; parameterId++) {
                    logResult.Append("[");
                    logResult.Append(data.parameters[parameterId]);
                    logResult.Append("]");
                }
                
                logResult.AppendLine();
                
                WriteLog(logResult.ToString());
            } catch (Exception exception) {
                Debug.LogError(exception);
            }
        }
        
        private static void WriteLog(string data, FileMode mode = FileMode.Append) {
            byte[] bytes = Encoding.UTF8.GetBytes(data);
            using FileStream fileStream = new FileStream(_pathToLog, mode, FileAccess.Write, FileShare.Write, 4096, FileOptions.None);
            fileStream.Write(bytes, 0, bytes.Length);
            fileStream.Close();
        }
        
        private static string GetTime() {
            DateTime now = DateTime.Now;
            
            return $"{now.DayOfYear:000}::{now.Hour:00}:{now.Minute:00}";
        }
        
        #if UNITY_EDITOR
        
        [UnityEditor.InitializeOnLoadMethod]
        private static void ClearDataInitialization() => SaveService.onDataClearEditor += ClearData;
        
        private static void ClearData() {
            if (File.Exists(_pathToLog)) {
                File.Delete(_pathToLog);
            }
        }
        
        [UnityEditor.MenuItem("File/Open Analytics.log", false, 195)]
        private static void OpenLog() {
            if (File.Exists(_pathToLog)) {
                System.Diagnostics.Process.Start($"{_pathToLog}");
            } else {
                Debug.LogError("Not have Analytics.log file!");
            }
        }
        
        #endif
    }
}