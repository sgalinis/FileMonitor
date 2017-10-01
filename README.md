# FileMonitor
File monitoring application C#, WPF

Synopsis:

The purpose of the application is to monitor a specific folder and synchronize files with the specified output folder.

Motivation:

.NET FileSystemWatcher is the class that monitors the file system and provides notification events.
However, during heavy multiple file copying a lot of duplicate events get fired and the files are still being used. This causes a lot of IO access problems trying to move/copy the files just right after notifications are received.

The idea behind the project is to use a unique queue to register files with changes and then process the queue in a separate background thread.

A custom unique queue class was added to resolve file name collisions for registering file events. Also, .NET framework does not have neither unique queue nor concurrent dictionary implementation thus is seems to be easier to use the lock on a Queue-HashSet pair.

Implementation:

The application is created in Visual Studio 2017; there might be a few C# keywords used not available in earlier versions.

Only standard .NET features were used. No third-party frameworks, libraries, or extensions like IoC included.

The application was created as a single window WPF application using MVVM pattern.
