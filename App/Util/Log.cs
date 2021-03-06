﻿using System;
using System.Drawing;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace App
{
    class Log
    {
        private static Regex escape = new Regex(@"\{(.+?)\}");
        internal static MainForm Form { get; set; }

        private static void Write(Color color, string format, params object[] args)
        {
            var datetime = DateTime.Now.ToString("HH:mm:ss");
            var formatted = string.Format(format, args);
            var message = string.Format("[{0}] {1}{2}", datetime, formatted, Environment.NewLine);

            Form.Invoke(() => 
            {
                Form.richTextBox_Log.SelectionStart = Form.richTextBox_Log.TextLength;
                Form.richTextBox_Log.SelectionLength = 0;

                Form.richTextBox_Log.SelectionColor = color;
                Form.richTextBox_Log.AppendText(message);
                Form.richTextBox_Log.SelectionColor = Form.richTextBox_Log.ForeColor;
            });
        }

        internal static void S(string format, params object[] args)
        {
            Write(Color.Green, format, args);
        }

        internal static void I(string format, params object[] args)
        {
            Write(Color.Black, format, args);
        }

        internal static void E(string format, params object[] args)
        {
            Write(Color.Red, format, args);
        }

        internal static void Ex(Exception ex, string format, params object[] args)
        {
#if DEBUG
            var message = ex.ToString();
#else
            var message = ex.Message;
#endif
            message = Escape(message);
            E(string.Format("{0}: {1}", format, message), args);

            Sentry.ReportAsync(ex, new { LogMessage = string.Format(format, args) });
        }

        internal static void D(string format, params object[] args)
        {
#if DEBUG
            Write(Color.Gray, format, args);
#endif
        }

        internal static void B(byte[] buffer)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine();

            for (int i = 0; i < buffer.Length; i++)
            {
                if (i != 0)
                {
                    if (i % 16 == 0)
                    {
                        sb.AppendLine();
                    }
                    else if (i % 8 == 0)
                    {
                        sb.Append(' ', 2);
                    }
                    else
                    {
                        sb.Append(' ');
                    }
                }

                sb.Append(buffer[i].ToString("X2"));
            }

            D(sb.ToString());
        }

        private static string Escape(string line)
        {
            return escape.Replace(line, "{{$1}}");
        }
    }
}
