using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Drawing;
using System.IO;
using System.Threading;

namespace FoxScreen
{
    public class UploadOrganizer
    {

        Thread uploadThread;
        Thread uploadCheckerThread;

        Queue<UploadThreadInfo> uploads = new Queue<UploadThreadInfo>();

        public const string MAINURL = "https://foxcav.es/";

        public UploadOrganizer()
        {


            uploadCheckerThread = new Thread(new ThreadStart(UploadCheckerThread));
            uploadCheckerThread.Start();
        }

        ~UploadOrganizer()
        {
            this.Stop();
        }

        public void Stop()
        {
            try
            {
                uploadCheckerThread.Abort();
            }
            catch { }

            try
            {
                uploadThread.Abort();
            }
            catch { }
        }

        public void AddUpload(string customname, MemoryStream mstr)
        {
            int imax = customname.Length;
            char c;
            char[] cna = customname.ToCharArray(0, imax);
            for (int i = 0; i < imax; i++)
            {
                c = cna[i];
                if (c == '<' || c == '>' || c == '\n' || c == '\t' || c == '\r' || c == '\0')
                {
                    cna[i] = '_';
                }
            }
            Console.WriteLine("AddUploadCheckpoint1, if you are reading this, then everything before the actual uploading has worked");

            customname = new String(cna);

        }

        private void UploadCheckerThread()
        {
            while (true)
            {
                do
                {
                    Thread.Sleep(100);
                } while (uploadThread != null && uploadThread.IsAlive);

                if (uploads.Count > 0)
                {
                    uploadThread = new Thread(new ParameterizedThreadStart(UploadThread));
                    uploadThread.Start(uploads.Dequeue());
                }
            }
        }

        private void UploadThread(object obj)
        {
            Console.WriteLine("AddUploadCheckpoint2, This means you are in teh same sub thing as the login details and (i think) the uploading");
            UploadThreadInfo info = (UploadThreadInfo)obj;
            string customname = info.customname;
            MemoryStream mstr = info.mstr;
            string foxscreenDir = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "/.foxScreen/";
            string tbUser;
            string tbPword;
            try
            {
                string[] lines = File.ReadAllLines("config.cfg");
                tbUser = lines[0];
                tbPword = lines[1];
            }
            catch { return; }

            try
            {
                mstr.Seek(0, SeekOrigin.Begin);
            }
            catch { }

            try
            {
                HttpWebRequest hwr = (HttpWebRequest)HttpWebRequest.Create(MAINURL + "create?" + customname);
                hwr.Method = WebRequestMethods.Http.Put;
                hwr.CachePolicy = new System.Net.Cache.RequestCachePolicy(System.Net.Cache.RequestCacheLevel.NoCacheNoStore);
                hwr.Headers.Add("X-Foxscreen-User", tbUser);
                hwr.Headers.Add("X-Foxscreen-Password", tbPword);
                hwr.Proxy = null;
                hwr.AllowWriteStreamBuffering = false;
                hwr.ServicePoint.Expect100Continue = false;
                hwr.ContentLength = mstr.Length;
                Stream str = hwr.GetRequestStream();

                byte[] buffer = new byte[256];
                int readb;
                while (mstr.CanRead)
                {
                    readb = (int)(mstr.Length - mstr.Position);
                    if (readb > 256) readb = 256;
                    readb = mstr.Read(buffer, 0, readb);
                    if (readb <= 0) break;
                    str.Write(buffer, 0, readb);
                    str.Flush();

                }
                str.Close();
                mstr.Close();

                HttpWebResponse resp = (HttpWebResponse)hwr.GetResponse();
                StreamReader respreader = new StreamReader(resp.GetResponseStream());
                customname = MAINURL + respreader.ReadToEnd();
                respreader.Close();
                resp.Close();

            }
            catch (WebException e)
            {

                HttpWebResponse resp = (HttpWebResponse)e.Response;
                StreamReader respreader = new StreamReader(resp.GetResponseStream());
                string response = respreader.ReadToEnd();
                respreader.Close();
                resp.Close();
                Console.WriteLine("Error uploading: " + response + " (" + ((int)resp.StatusCode) + ")", "foxScreen");
            }
            catch (Exception e)
            {
                Console.WriteLine("Internal error: " + e.ToString(), "foxScreen");
            }
            finally
            {

            }
        }

        private static string FixTwoChar(int num)
        {
            if (num < 10) return "0" + num.ToString();
            return num.ToString();
        }

        internal class UploadThreadInfo
        {
            public readonly string customname;
            public readonly MemoryStream mstr;


        }
    }
}