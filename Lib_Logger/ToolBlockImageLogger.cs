using System;
using System.Drawing;
using System.IO;
using Cognex.VisionPro;
using Cognex.VisionPro.Display;
using Cognex.VisionPro.ImageFile;
using Cognex.VisionPro.ToolBlock;
using Lib_Enums;
using Utils;

namespace Lib_Logger
{
    /// <summary>
    ///     This class takes care of how CogDisplay is refreshed and saved
    /// </summary>
    public class ToolBlockImageLogger : IToolBlockLogger
    {
        private readonly CogDisplayContentBitmapConstants _bitmapStyle;
        private readonly CogRecordDisplay _display;
        private readonly string _logBaseDir;
        private readonly RunResult _logScenario;
        private readonly int _outdatedDays;
        private readonly string _subrecordPath;
        private string _logDirToday;
        private string _recentImagePath;

        public ToolBlockImageLogger(CogRecordDisplay display, string logBaseDir,
            int outdatedDays,
            CogDisplayContentBitmapConstants bitmapStyle,
            RunResult logScenario = RunResult.NG | RunResult.ProductMissing,
            string subrecordPath = "CogIPOneImageTool1.OutputImage")
        {
            _display = display;
            _logBaseDir = logBaseDir;
            _outdatedDays = outdatedDays;
            _bitmapStyle = bitmapStyle;
            _logScenario = logScenario;
            _subrecordPath = subrecordPath;

            Logged += (sender, args) => RemoveOutdatedImageDirs();
        }

        public string LogDirToday
        {
            get
            {
                if (string.IsNullOrEmpty(_logDirToday))
                    throw new InvalidDataException("LogDirToday is not available yet.");
                return _logDirToday;
            }
        }

        public string RecentImagePath
        {
            get
            {
                if (string.IsNullOrEmpty(_recentImagePath))
                    throw new InvalidDataException("RecentImagePath is not available yet.");
                return _recentImagePath;
            }
        }

        public event EventHandler Logged;

        public void Log(ICogTool tool, RunResult runResult)
        {
            LogToScreenInvoke((CogToolBlock) tool, runResult);

            if ((_logScenario & runResult) == 0) return;

            _logDirToday = Path.Combine(_logBaseDir, DateTime.Now.ToString("MM-dd"));
            if (!Directory.Exists(_logDirToday)) Directory.CreateDirectory(_logDirToday);

            _recentImagePath = Path.Combine(_logDirToday, DateTime.Now.ToString("HHmmss") + ".jpg");
            LogToDisk_ExceptionEaten();

            OnLogged();
        }

        private void LogToScreenInvoke(CogToolBlock toolBlock, RunResult runResult)
        {
            // TODO: Handle runResult
            if (_display.InvokeRequired)
                _display.Invoke(new Action(() =>
                {
                    _display.Record = toolBlock.CreateLastRunRecord().SubRecords[_subrecordPath];
                }));
            else
                _display.Record = toolBlock.CreateLastRunRecord().SubRecords[_subrecordPath];
        }

        private void RemoveOutdatedImageDirs()
        {
            FileSystem.RemoveAllThatOutdates(_logBaseDir, _outdatedDays, FileOrDirectory.Directory);
        }

        private void OnLogged()
        {
            Logged?.Invoke(this, EventArgs.Empty);
        }

        private void LogToDisk_ExceptionEaten()
        {
            try
            {
                using (var fileTool = new CogImageFileTool())
                {
                    var img = _display.CreateContentBitmap(_bitmapStyle);

                    var bmp = new Bitmap(img);
                    var cogImage = new CogImage24PlanarColor(bmp);
                    fileTool.InputImage = cogImage;

                    fileTool.Operator.Open(_recentImagePath, CogImageFileModeConstants.Write);
                    fileTool.Run();
                }
            }

            catch
            {
            }
        }
    }
}