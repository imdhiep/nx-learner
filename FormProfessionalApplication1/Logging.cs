using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.IO;
using System.Drawing;
using NXOpen;

namespace PartList
{
    #region File: Logging.cs
    // Interface Logging
    public interface ILoggingService
    {
        void Log(string message);
    }

    // Logging Service (write Console or Status Label)
    public class LoggingService : ILoggingService
    {
        private Label _statusLabel;

        public LoggingService(Label statusLabel)
        {
            _statusLabel = statusLabel;
        }

        public void Log(string message)
        {
            if (_statusLabel != null)
            {
                _statusLabel.Text = message;
            }
            else
            {
                Console.WriteLine(message);
            }
        }
    }
    #endregion

}

