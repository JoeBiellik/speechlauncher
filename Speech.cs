using System;
using System.Diagnostics;
using System.Speech.Recognition;
using System.Globalization;
using System.Linq;

namespace SpeechLauncher
{
	internal class Speech : IDisposable
	{
		private readonly SpeechRecognitionEngine recognizer;

		internal Speech()
		{
			this.recognizer = new SpeechRecognitionEngine(new CultureInfo(Settings.Instance.Locale));
			this.recognizer.SetInputToDefaultAudioDevice();
			this.recognizer.UnloadAllGrammars();

			foreach (var configObject in Settings.Instance.Objects)
			{
				this.recognizer.LoadGrammar(CreateGrammar(configObject));
			}

			this.recognizer.SpeechHypothesized += recognizer_SpeechHypothesized;
			this.recognizer.SpeechRecognized += recognizer_SpeechRecognized;
			this.recognizer.RecognizeAsync(RecognizeMode.Multiple);
		}

		internal static void Run(Settings.Action action)
		{
			if (action == null) return;

			Process.Start(new ProcessStartInfo
			{
				WorkingDirectory = Environment.ExpandEnvironmentVariables(action.WorkingDirectory ?? ""),
				FileName = Environment.ExpandEnvironmentVariables(action.Command ?? ""),
				Arguments = Environment.ExpandEnvironmentVariables(action.Arguments ?? ""),
				WindowStyle = action.Visible ? ProcessWindowStyle.Normal : ProcessWindowStyle.Hidden
			});
		}

		private static Grammar CreateGrammar(Settings.Object config)
		{
			var objectChoices = new Choices();
			foreach (var word in config.Words)
			{
				objectChoices.Add(new SemanticResultValue(word, config.Name));
			}
			var objectSemKey = new SemanticResultKey("object", objectChoices);
			var objectGrammar = new GrammarBuilder();
			objectGrammar.Append(objectSemKey);

			var actionChoices = new Choices();
			foreach (var action in config.Actions)
			{
				foreach (var word in action.Words)
				{
					actionChoices.Add(new SemanticResultValue(word, action.Name));
				}
			}
			var actionSemKey = new SemanticResultKey("action", actionChoices);
			var actionGrammar = new GrammarBuilder();
			actionGrammar.Append(actionSemKey);

			var wakeGrammar = new GrammarBuilder();
			wakeGrammar.Append(new Choices(Settings.Instance.WakeWord));

			objectGrammar.Append(actionGrammar);
			wakeGrammar.Append(objectGrammar);

			return new Grammar(wakeGrammar);
		}

		private static void recognizer_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
		{
			var result = e.Result;
			var objectMatch = (string)result.Semantics["object"].Value;
			var actionMatch = (string)result.Semantics["action"].Value;
			var confidence = result.Confidence * 100;

			Console.WriteLine();
			Console.WriteLine($"\"{objectMatch}\" => \"{actionMatch}\" ({confidence:N0}%)");

			if (confidence < Settings.Instance.Confidence)
			{
				Console.WriteLine("Confidence low, ignoring");
				return;
			}

			Program.Tray.Popup(objectMatch, $"Action: {actionMatch}{Environment.NewLine}Confidence: {confidence:N0}%");

			var action = Settings.Instance.Objects.First(o => o.Name == objectMatch)?.Actions.First(a => a.Name == actionMatch);

			Run(action);
		}

		private static void recognizer_SpeechHypothesized(object sender, SpeechHypothesizedEventArgs e)
		{
			Console.Write(".");
		}

		public void Dispose()
		{
			this.recognizer?.RecognizeAsyncStop();
			this.recognizer?.Dispose();
		}
	}
}
