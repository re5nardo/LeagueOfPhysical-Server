using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Text;

namespace LOP
{
    public class ApplicationLog
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void OnBeforeSceneLoadRuntimeMethod()
        {
            Initialize();
        }

        private static void Initialize()
        {
            string logFilePath = Path.Combine(UnityEngine.Application.persistentDataPath, $"{DateTime.Now.ToString("yyyy'-'MM'-'dd'T'HH'꞉'mm'꞉'ss")}.txt");

            UnityEngine.Application.logMessageReceived += (condition, stackTrace, type) =>
            {
                switch (type)
                {
                    case LogType.Error:
                    case LogType.Exception:

                        var stringBuilder = new StringBuilder()
                        .AppendLine()
                        .Append(condition)
                        .AppendLine()
                        .Append(stackTrace)
                        ;

                        File.AppendAllText(logFilePath, stringBuilder.ToString());
                        break;
                }
            };
        }
    }
}
