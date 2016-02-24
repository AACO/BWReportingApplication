using log4net;
using log4net.Config;

using MySql.Data.MySqlClient;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

using BWServerLogger.Model;
using BWServerLogger.Service;
using BWServerLogger.Util;


namespace BWServerLogger
{
    static class LoggerUI
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(LoggerUI));

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            XmlConfigurator.Configure(new FileInfo("log-config.xml"));

            try
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new MainWindow());
            }
            catch (Exception e)
            {
                logger.Error("Reporting failed", e);
            }

            // This will shutdown the log4net system
            LogManager.Shutdown();
        }
    }
}
