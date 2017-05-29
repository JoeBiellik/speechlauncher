using System.Collections.Generic;
using YamlDotNet.Serialization;

namespace SpeechLauncher
{
	internal class Settings : YamlSettings<Settings>
	{
		public string Locale { get; protected set; } = "en-GB";
		public ushort Confidence { get; protected set; } = 40;
		public string WakeWord { get; protected set; } = "okay computer";
		public List<Object> Objects { get; protected set; } = new List<Object>();

		public override Settings Initialize()
		{
			return new Settings
			{
				Objects = new List<Object>
				{
					new Object
					{
						Name = "Test",
						Words = new List<string>
						{
							"test"
						},
						Actions = new List<Action>
						{
							new Action
							{
								Name = "Echo",
								Words = new List<string>
								{
									"echo", "message"
								},
								Command = "cmd",
								Arguments = "/C \"echo Hello World && pause\"",
								Visible = true
							}
						}
					},
					new Object
					{
						Name = "Question",
						Words = new List<string>
						{
							"what"
						},
						Actions = new List<Action>
						{
							new Action
							{
								Name = "Current Time",
								Words = new List<string>
								{
									"time is it", "is the time"
								},
								Command = "https://time.is/"
							}
						}
					}
				}
			};
		}

		internal class Object
		{
			public string Name { get; set; }

			public List<Action> Actions { get; set; } = new List<Action>();

			public List<string> Words { get; set; } = new List<string>();
		}

		internal class Action
		{
			public string Name { get; set; }

			[YamlMember(Alias = "dir")]
			public string WorkingDirectory { get; set; }

			[YamlMember(Alias = "cmd")]
			public string Command { get; set; }

			public string Arguments { get; set; }

			public bool Visible { get; set; }

			public List<string> Words { get; set; } = new List<string>();
		}
	}
}
