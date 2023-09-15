using Cipher;
using EmailSender.Models;
using ReportService.Core;
using ReportService.Core.Repositories;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;

namespace ReportService
{
    public partial class ReportService : ServiceBase
    {

        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        private int _sendHour;
        private int _intervalInMinutes;
        private System.Timers.Timer _timer;
        private ErrorRepository _errorRepository = new ErrorRepository();
        private ReportRepository _reportRepository = new ReportRepository();
        private Email _email;
        private GenerateHtmlEmail _htmlEmail = new GenerateHtmlEmail();
        private string _emailReceiver;
        private StringCipher _stringCipher = new StringCipher("28FFF881-600A-47F5-A4FB-F2F9D22B0904");
        private const string NotEncryptedPasswordPrefix = "encrypt:";
        private bool SendReports;

        public ReportService()
        {
            InitializeComponent();         

            try
            {
                _emailReceiver = ConfigurationManager.AppSettings["ReciverEmail"];


                _email = new Email(new EmailParams
                {
                    HostSmtp = ConfigurationManager.AppSettings["HostSmtp"],
                    Port = Convert.ToInt32(ConfigurationManager.AppSettings["Port"]),
                    EnableSsl = Convert.ToBoolean(ConfigurationManager.AppSettings["EnableSsl"]),
                    SenderName = ConfigurationManager.AppSettings["SenderName"],
                    SenderEmail = ConfigurationManager.AppSettings["SenderEmail"],
                    SenderEmailPassword = DecryptSenderEmailPassword()
                });

                SendHour = Convert.ToInt32(ConfigurationManager.AppSettings["SendHour"]);
                _intervalInMinutes = Convert.ToInt32(ConfigurationManager.AppSettings["IntervalInMinutes"]);
                SendReports = Convert.ToBoolean(ConfigurationManager.AppSettings["SendReports"]);

                _timer = new System.Timers.Timer(_intervalInMinutes * 60000);


            }
            catch (Exception ex)
            {
                Logger.Error(ex, ex.Message);
                throw new Exception(ex.Message);
            }
        }

        private string DecryptSenderEmailPassword()
        {
            var encryptedPassword = ConfigurationManager.AppSettings["SenderEmailPassword"];

            if (encryptedPassword.StartsWith(NotEncryptedPasswordPrefix))
            {
                encryptedPassword = _stringCipher.Encrypt(encryptedPassword.Replace(NotEncryptedPasswordPrefix, ""));

                var configFile = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                configFile.AppSettings.Settings["SenderEmailPassword"].Value = encryptedPassword;
                configFile.Save();
            }
            return _stringCipher.Decrypt(encryptedPassword);
        }

        protected override void OnStart(string[] args)
        {
            _timer.Elapsed += DoWork;
            _timer.Start();
            Logger.Info("Serwis started...");
        }

        private async void DoWork(object sender, ElapsedEventArgs e)
        {
            try
            {
                await SendError();
                await SendReport();
            }
            catch (Exception ex)
            {
                Logger.Error(ex, ex.Message);
                throw new Exception(ex.Message);
            }
        }

        private async Task SendError()
        {
            var errors = _errorRepository.GetLastErrors(_intervalInMinutes);

            if (errors == null || !errors.Any())
            {
                return;
            }

           await _email.Send("Błędy w aplikacji", _htmlEmail.GenerateErrors(errors, _intervalInMinutes), _emailReceiver);

            Logger.Info("Error sent...");
        }

        private async Task SendReport()
        {
            var actualHour = DateTime.Now.Hour;

            if (actualHour < SendHour || !SendReports)
                return;

            var report = _reportRepository.GetLastNotSentReport();


            if (report == null)
                return;

            await _email.Send("Raport dobowy", _htmlEmail.GenerateReport(report), _emailReceiver);

            _reportRepository.ReportSent(report);


            Logger.Info("Report sent...");

        }

        protected override void OnStop()
        {

            Logger.Info("Serwis stopped...");
        }
    }
}
