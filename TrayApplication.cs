using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

namespace SpeechLauncher
{
	internal class TrayApplication : ApplicationContext
	{
		private readonly IContainer components = new Container();
		private readonly NotifyIcon icon;
		private Speech speech;

		[SuppressMessage("ReSharper", "AssignNullToNotNullAttribute")]
		internal TrayApplication()
		{
			this.icon = new NotifyIcon(this.components)
			{
				ContextMenuStrip = new ContextMenuStrip(),
				Icon = new Icon(Assembly.GetExecutingAssembly().GetManifestResourceStream("SpeechLauncher.Resources.tray.ico")),
				Text = Program.Name,
				Visible = true
			};

			this.icon.ContextMenuStrip.Opening += this.ContextMenuStrip_Opening;
			this.icon.MouseDown += this.icon_MouseDown;

			this.speech = new Speech();
		}

		internal void Popup(string title, string msg)
		{
			this.icon.ShowBalloonTip(1000, title, msg, ToolTipIcon.Info);
		}

		private void ContextMenuStrip_Opening(object sender, CancelEventArgs e)
		{
			e.Cancel = false;

			this.icon.ContextMenuStrip.Items.Clear();

			foreach (var configObject in Settings.Instance.Objects)
			{
				var items = new List<ToolStripMenuItem>();

				foreach (var configAction in configObject.Actions)
				{
					items.Add(new ToolStripMenuItem(configAction.Name, null, (s, a) =>
					{
						Speech.Run(configAction);
					}));
				}

				this.icon.ContextMenuStrip.Items.Add(new ToolStripMenuItem(configObject.Name, null, items.Cast<ToolStripItem>().ToArray()));
			}

			this.icon.ContextMenuStrip.Items.Add(new ToolStripSeparator());
			this.icon.ContextMenuStrip.Items.Add(new ToolStripMenuItem("&Exit", null, (s, a) => { Application.Exit(); }));
		}

		private void icon_MouseDown(object sender, MouseEventArgs e)
		{
			if (e.Button != MouseButtons.Left) return;

			typeof(NotifyIcon).GetMethod("ShowContextMenu", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(this.icon, null);
		}

		protected override void Dispose(bool disposing)
		{
			if (!disposing) return;

			this.speech?.Dispose();
			this.speech = null;
		}
	}
}
