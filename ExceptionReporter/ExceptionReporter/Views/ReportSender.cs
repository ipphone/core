using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Drawing.Imaging;
using System.Threading;
using ContactPoint.Common;

namespace ExceptionReporter.Views
{
    public partial class ReportSender : Form
    {
        private ExceptionReportPresenter _report;

        public ReportSender(ExceptionReportPresenter report)
        {
            this._report = report;

            InitializeComponent();
        }

        private void ReportSender_Load(object sender, EventArgs e)
        {
            (new Thread(new ThreadStart(SendProcess))).Start();
        }

        private void SendProcess()
        {
            var sender = new PostSender(_report.ReportInfo);

            try
            {
                string dirName = Guid.NewGuid().ToString();
                string tempPath = Path.GetTempPath();
                string usedPath = tempPath + dirName;
                Directory.CreateDirectory(usedPath);

                try
                {
                    if (this._report.ReportInfo.ScreenshotImage != null)
                    {
                        using (var stream = File.OpenWrite(usedPath + Path.DirectorySeparatorChar + "screenshot.jpg"))
                        {
                            this._report.ReportInfo.ScreenshotImage.Save(stream, ImageFormat.Jpeg);

                            stream.Close();
                        }

                    }
                }
                catch { }

                try
                {
                    File.Copy(
                        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "callservice.settings.xml"),
                        usedPath + Path.DirectorySeparatorChar + "settings.xml");
                }
                catch { }

                try
                {
                    File.Copy("pjsip.log", usedPath + Path.DirectorySeparatorChar + "pjsip.log");
                }
                catch { }

                try
                {
                    this._report.SaveReportToFile(usedPath + Path.DirectorySeparatorChar + "report.txt");
                }
                catch { }

                try
                {
                    var sb = new StringBuilder();

                    foreach (var item in Logger.Log)
                        sb.AppendLine(item.ToString());

                    File.WriteAllText(usedPath + Path.DirectorySeparatorChar + "log.txt", sb.ToString());
                }
                catch { }

                ZipUtil.ZipFiles(usedPath, usedPath + ".zip", "");
                sender.AddFile("log_files", usedPath + ".zip");

                sender.Send();

                this.CrossThreadClose(DialogResult.OK);
            }
            catch (Exception e)
            {
                MessageBox.Show("Не удалось отправить отчет!\r\n" + e.Message, "Ошибка при отправке отчета", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                this.CrossThreadClose(DialogResult.Cancel);
            }
            finally
            {
                sender = null;
            }
        }

        private void CrossThreadClose(DialogResult result)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new Action<DialogResult>(CrossThreadClose), new object[] { result });
                return;
            }

            this.DialogResult = result;

            this.Close();
        }
    }
}
