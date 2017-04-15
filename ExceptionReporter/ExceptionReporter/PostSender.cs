using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Collections.Specialized;
using System.IO;
using System.Windows.Forms;
using ExceptionReporter.Core;

namespace ExceptionReporter
{
    internal class PostSender
    {
        private NameValueCollection _params = new NameValueCollection();
        private string _fileParamName = String.Empty;
        private string _filePath = String.Empty;

        public PostSender(ExceptionReportInfo reportInfo)
        {
            AddParameter("version", reportInfo.AppVersion);
            AddParameter("assembly", reportInfo.AppAssembly.FullName);
            AddParameter("message", reportInfo.CustomMessage);
            AddParameter("user_message", reportInfo.UserExplanation);
            AddParameter("report", new ExceptionReportGenerator(reportInfo).CreateExceptionReport().ToString());
        }

        public void AddParameter(string name, string value)
        {
            _params.Add(name, value);
        }

        public void AddFile(string name, string filePath)
        {
            _fileParamName = name;
            _filePath = filePath;
        }

        private void AddParameters(Stream requestStream, byte[] boundary)
        {
            if (_params.Count == 0) return;

            string formdataTemplate = "Content-Disposition: form-data; name=\"{0}\"\r\n\r\n{1}";

            foreach (string key in _params.Keys)
            {
                requestStream.Write(boundary, 0, boundary.Length);

                var formitem = System.Text.Encoding.UTF8.GetBytes(string.Format(formdataTemplate, key, _params[key]));

                requestStream.Write(formitem, 0, formitem.Length);
            }

            requestStream.Write(boundary, 0, boundary.Length);
        }

        private string GetFileMimeType()
        {
            return "application/zip";
        }

        private void AddFile(Stream requestStream, string boundary)
        {
            if (String.IsNullOrEmpty(_fileParamName) || String.IsNullOrEmpty(_filePath)) return;

            var header = System.Text.Encoding.UTF8.GetBytes(string.Format("Content-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"\r\nContent-Type: {2}\r\n\r\n", _fileParamName, _filePath, GetFileMimeType()));
            requestStream.Write(header, 0, header.Length);

            var file = File.ReadAllBytes(_filePath);
            requestStream.Write(file, 0, file.Length);

            byte[] trailer = System.Text.Encoding.ASCII.GetBytes("\r\n--" + boundary + "--\r\n");

            requestStream.Write(trailer, 0, trailer.Length);
        }

        public void Send()
        {
            string boundary = "---------------------------" + DateTime.Now.Ticks.ToString("x");
            byte[] boundaryBytes = System.Text.Encoding.ASCII.GetBytes("\r\n--" + boundary + "\r\n");

            HttpWebRequest wr = (HttpWebRequest)WebRequest.Create("http://bug.artpoint.com.ua/index.php");
            wr.ContentType = "multipart/form-data; boundary=" + boundary;
            wr.Method = "POST";
            wr.KeepAlive = true;
            wr.Credentials = System.Net.CredentialCache.DefaultCredentials;

            using (var requestStream = wr.GetRequestStream())
            {
                AddParameters(requestStream, boundaryBytes);
                AddFile(requestStream, boundary);
            }

            WebResponse wresp = null;
            try
            {
                wresp = wr.GetResponse();

                using (var stream = wresp.GetResponseStream())
                using (var reader = new StreamReader(stream))
                {
                    var response = reader.ReadToEnd();
                }
            }
            catch
            {
                if (wresp != null)
                {
                    wresp.Close();
                    wresp = null;
                }
            }
            finally
            {
                wr = null;
            }

            MessageBox.Show("Thank you for sending report.\r\nIt is very important for us to know about problems", "Report sent", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
