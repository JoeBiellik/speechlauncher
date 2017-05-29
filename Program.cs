using System;
using System.Threading;
using System.Windows.Forms;
using JoeBiellik.Utils;

namespace SpeechLauncher
{
	internal static class Program
	{
		internal static readonly string Name = "Speech Launcher";
		internal static TrayApplication Tray;

		[STAThread]
		internal static void Main()
		{
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			Application.ThreadException += Application_ThreadException;

			try
			{
				if (!Settings.FileExists) Settings.Save();

				using (Tray = new TrayApplication())
				{
					SingleInstance.Run(Tray);
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show($"A fatel error has orccured, application will now exit:{Environment.NewLine}{ex.Message}", Name, MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private static void Application_ThreadException(object sender, ThreadExceptionEventArgs e)
		{
			MessageBox.Show($"A fatel thread error has orccured, application will now exit:{Environment.NewLine}{e.Exception.Message}", Name, MessageBoxButtons.OK, MessageBoxIcon.Error);
		}
	}
}
