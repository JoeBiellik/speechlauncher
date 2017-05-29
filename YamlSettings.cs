using System;
using System.IO;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace SpeechLauncher
{
	/// <summary>
	/// Singleton lazy loaded settings serialized to YAML.
	/// </summary>
	/// <typeparam name="T">Settings class derived from <see cref="YamlSettings{T}"/> to instantiate.</typeparam>
	/// <example>
	/// <code>
	/// public class MySettings : YamlSettings&lt;MySettings&gt;
	///	{
	///		public bool Disabled { get; set; } = false;
	///	}
	///	
	///	MySettings.FileName = "mysettings.yml";
	///	
	///	if (MySettings.Instance.Disabled) {
	///		...
	///	}
	///	
	///	MySettings.Instance.Disabled = false;
	///	MySettings.Save();
	/// </code>
	/// </example>
	public abstract class YamlSettings<T> : ISettings<T> where T : class, ISettings<T>, new()
	{
		// ReSharper disable StaticMemberInGenericType

		private static string directory = AppDomain.CurrentDomain.BaseDirectory;
		private static T instance;

		/// <summary>
		/// Gets or sets the name of the settings file.
		/// 
		/// Defaults to "settings.yml".
		/// </summary>
		/// <seealso cref="Directory"/>
		/// <seealso cref="Path"/>
		public static string FileName { get; set; } = "settings.yml";

		/// <summary>
		/// Gets or sets the directory location of the settings file.
		/// 
		/// Defaults to the current <see cref="AppDomain"/> base directory.
		/// </summary>
		/// <exception cref="DirectoryNotFoundException">Throw if the target directory does not exist.</exception>
		/// <seealso cref="FileName"/>
		/// <seealso cref="Path"/>
		public static string Directory
		{
			get { return directory; }
			set
			{
				if (!System.IO.Directory.Exists(value)) throw new DirectoryNotFoundException();

				directory = value;
			}
		}

		/// <summary>
		/// The full path to the settings file. Computed from <see cref="FileName"/> and <see cref="Directory"/>.
		/// </summary>
		public static string Path => System.IO.Path.Combine(Directory, FileName);

		/// <summary>
		/// Checks if the settings file exists on disk.
		/// </summary>
		/// <seealso cref="Path"/>
		public static bool FileExists => File.Exists(Path);

		/// <summary>
		/// Gets the singleton lazy loaded settings instace.
		/// Loads the settings file on first access, then maintains settings in memory.
		/// If the settings do not exist on disk, <see cref="Initialize"/> is called and the settings are saved to disk.
		/// </summary>
		public static T Instance
		{
			get
			{
				if (instance != null) return instance;

				try
				{
					instance = new DeserializerBuilder().WithNamingConvention(new UnderscoredNamingConvention()).Build().Deserialize<T>(File.ReadAllText(Path));

					if (instance == null) throw new NullReferenceException("Empty YAML file");
				}
				catch (Exception)
				{
					instance = new T();
					instance = instance.Initialize();
					Save();
				}

				return instance;
			}
		}

		/// <summary>
		/// Virtual constructor to return a new instance of the settings type.
		/// 
		/// Allows for setting defaults when creating a new settings file.
		/// </summary>
		/// <returns>New instance of its own type with starting defaults.</returns>
		/// <example>
		/// <code>
		/// public class MySettings : YamlSettings&lt;MySettings&gt;
		///	{
		///		public List&lt;ShoppingItems&gt; Items { get; set; };
		///
		///		public override MySettings Initialize()
		///		{
		///			return new MySettings
		///			{
		///				Items = new List&lt;ShoppingItems&gt;
		///				{
		///					new ShoppingItems("Apples", 5),
		///					new ShoppingItems("Oranges", 3)
		///				}
		///			};
		///		}
		///	}
		/// </code>
		/// </example>
		public virtual T Initialize()
		{
			instance = new T();

			return instance;
		}

		/// <summary>
		/// Saves the current settings instance to disk.
		/// </summary>
		/// <seealso cref="Path"/>
		public static void Save()
		{
			File.WriteAllText(Path, new SerializerBuilder().WithNamingConvention(new UnderscoredNamingConvention()).Build().Serialize(Instance));
		}
	}

	/// <summary>
	/// Base settings interface exposing virtual constructor.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public interface ISettings<out T>
	{
		/// <summary>
		/// Virtual constructor to return a new instance of the settings type.
		/// </summary>
		/// <returns></returns>
		T Initialize();
	}
}
