using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Cognex.VisionPro;
using Cognex.VisionPro.ToolBlock;
using Lib_Enums;
using Utils;

namespace Lib_VP.Logger
{
    public class ToolBlockDataLogger : IToolBlockLogger
    {
        public ToolBlockDataLogger(string logDir, int outdatedDays = 10)
        {
            LogDir = logDir;
            _outdatedDays = outdatedDays;

            Logged += RemoveOutdatedFiles;
        }

        public event EventHandler Logged;

        public void Log(ICogTool tool, RunResult runResult)
        {
            var toolBlock = (CogToolBlock) tool;
            if (string.IsNullOrEmpty(_header))
            {
                _outputNames = ExtractOutputNamesFromBlock(toolBlock);
                _header = FormatOutputNames(_outputNames);
            }

            var outputValues = ExtractOutputValuesFromBlock(toolBlock);
            var line = runResult == RunResult.OK || runResult == RunResult.NG ? FormatOutputValues(outputValues) :
                runResult == RunResult.ProductMissing ? "Null" : "Error";

            _logFile = WriteLine(line);

            OnLogged();
        }

        private void RemoveOutdatedFiles(object sender, EventArgs e)
        {
            FileSystem.RemoveAllThatOutdates(LogDir, _outdatedDays, FileOrDirectory.File);
        }

        private void OnLogged()
        {
            Logged?.Invoke(this, EventArgs.Empty);
        }

        private string WriteLine(string line)
        {
            if (!Directory.Exists(LogDir)) Directory.CreateDirectory(LogDir);
            var logFile = Path.Combine(LogDir, DateTime.Now.ToString("yyyy-MM-dd") + ".csv");

            if (!File.Exists(logFile))
                using (var ss = new StreamWriter(logFile, true))
                {
                    ss.WriteLine(_header);
                    ss.WriteLine(line);
                }
            else
                using (var ss = new StreamWriter(logFile, true))
                {
                    ss.WriteLine(line);
                }

            return logFile;
        }

        private string FormatOutputValues(List<double> outputValues)
        {
            var ret = DateTime.Now.ToString("HH:mm:ss") + ",";

            return ret + string.Join(",", outputValues.Select(a => a.ToString("f3")));
        }

        private List<double> ExtractOutputValuesFromBlock(CogToolBlock toolBlock)
        {
            var ret = new List<double>();
            for (var i = 0; i < toolBlock.Outputs.Count; i++) ret.Add((double) toolBlock.Outputs[i].Value);

            return ret;
        }

        private string FormatOutputNames(List<string> outputNames)
        {
            outputNames.Insert(0, "DataTime");

            return string.Join(",", outputNames);
        }

        private List<string> ExtractOutputNamesFromBlock(CogToolBlock toolBlock)
        {
            var ret = new List<string>();

            for (var i = 0; i < toolBlock.Outputs.Count; i++) ret.Add(toolBlock.Outputs[i].Name);

            return ret;
        }

        #region fields

        public string LogDir { get; }
        private string _header = string.Empty;
        private string _logFile = string.Empty;
        private List<string> _outputNames;
        private readonly int _outdatedDays;


        public List<string> OuputNames
        {
            get
            {
                if (_outputNames == null) throw new InvalidDataException("OutputNames is not available yet");
                return _outputNames;
            }
        }

        public string LogFile
        {
            get
            {
                if (string.IsNullOrEmpty(_logFile)) throw new InvalidDataException("LogFile is not available yet.");

                return _logFile;
            }
        }

        #endregion
    }
}