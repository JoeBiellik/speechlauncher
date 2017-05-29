# Speech Launcher
[![License](https://img.shields.io/github/license/JoeBiellik/speechlauncher.svg)](LICENSE.md)
[![Release Version](https://img.shields.io/github/release/JoeBiellik/speechlauncher.svg)](https://github.com/JoeBiellik/speechlauncher/releases)

When you say something, run something.

Very simple, yet functional voice activated launcher. Configuration allows you to launch any program when a voice trigger is heard. All voice detection is local using the built in [Windows SpeechRecognitionEngine](https://msdn.microsoft.com/en-us/library/system.speech.recognition.speechrecognitionengine.aspx) which means no internet connection is required.

## Configuration
Settings are stored in the [YAML](http://yaml.org/) file ``settings.yml`` next to the application.

The settings contain a list of one or more ``objects`` which define the first voice keyword. Each ``object`` contains one or more ``actions`` which define the second voice keyword. Each ``object`` and ``action`` can have multiple ``words`` which you can say to trigger them.

The following example would open ``http://google.com`` in your browser when you say the phrase ``"computer browse google"``:
```yml
wake_word: computer

objects:
- name: Browse
  words:
  - browse

  actions:
  - name: Google
    words:
    - google
    cmd: http://google.com
```

For better accuracy you can specify your [locale](https://msdn.microsoft.com/en-us/library/cc233982.aspx) and the speech engine minimum confidence percentage. You can also change the wake word from the default ``okay computer``, however it is recommended you make the wake word quite long to avoid false positives.

## Download
Download the [latest release](https://github.com/JoeBiellik/speechlauncher/releases/latest). The application is portable and does not require installation. Requires .NET Framework 4.6.
