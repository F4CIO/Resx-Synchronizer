using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using System.IO;

namespace CraftSynth.ResxSynchronizer
{
	static class Program
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static int Main(string[] args)
		{
			try
			{
				if (args.Length == 0)
				{
					Application.EnableVisualStyles();
					Application.SetCompatibleTextRenderingDefault(false);
					Application.Run(new Form1());
					return 1;
				}
				else
				{
					string log = string.Empty;
					bool hasErrors = MainInner(args, ref log);
					Console.Write(log);
					return hasErrors ? 1 : -1;
				}
				
			}
			catch (Exception exception)
			{
				try
				{
					string exeFolderPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().CodeBase).Replace("file:\\", "");
					string content = exception.Message + "\r\n\r\n" + exception.StackTrace;
					File.WriteAllText(exeFolderPath + "\\LastError.txt", content);
				}
				catch (Exception)
				{
				}
				throw exception;
			}
		}

		public static bool MainInner(string[] args, ref string log)
		{
			if (args.Length != 2 && args.Length!=3)
			{
				throw new Exception("App must be run with two arguments (folder 1 and folder 2 with .resx files) or with three arguments (where third argument is folder for backup files and log).");
			}

			string folder1Path = args[0];
			folder1Path = folder1Path.TrimEnd('\\');
			if (!Directory.Exists(folder1Path))
			{
				throw new Exception("Folder not found:" + folder1Path);
			}

			string folder2Path = args[1];
			folder2Path = folder2Path.TrimEnd('\\');
			if (!Directory.Exists(folder2Path))
			{
				throw new Exception("Folder not found:" + folder2Path);
			}

			string folderBackupPath = args[2];
			folderBackupPath = folderBackupPath.TrimEnd('\\');
			if (!Directory.Exists(folderBackupPath))
			{
				Directory.CreateDirectory(folderBackupPath);
			}

			List<string> resxFiles1Paths = GetFilePaths(folder1Path, "*.resx");
			if (resxFiles1Paths == null || resxFiles1Paths.Count == 0)
			{
				throw new Exception("No .resx files in folder:" + folder1Path);
			}

			List<string> resxFiles2Paths = GetFilePaths(folder2Path, "*.resx");
			if (resxFiles2Paths == null || resxFiles2Paths.Count == 0)
			{
				throw new Exception("No .resx files in folder:" + folder2Path);
			}

			bool hasErrors = SynchronizeFolders(resxFiles1Paths, resxFiles2Paths, folderBackupPath, ref log);
			return hasErrors;
		}

		private static bool SynchronizeFolders(List<string> resxFiles1Paths, List<string> resxFiles2Paths, string folderBackupPath, ref string log)
		{
			bool hasErrors = false;
			bool hasConflicts = false;
			CustomTraceLog customTraceLog = new CustomTraceLog(log);

			#region prepare backup folder
			//make structure:
			//..folderBackupPath\2012.12.12_12:12:00\1\*.resx
			//..folderBackupPath\2012.12.12_12:12:00\2\*.resx
			//..folderBackupPath\2012.12.12_12:12:00\log.txt
			customTraceLog.AddLine("Creating backup folder structure in:'"+folderBackupPath+"' ...");

			folderBackupPath = folderBackupPath.TrimEnd('\\');
			if (!Directory.Exists(folderBackupPath))
			{
				Directory.CreateDirectory(folderBackupPath);
			}
			DateTime now =DateTime.UtcNow;
			string moment = string.Format("{0}.{1:00}.{2:00}._{3:00}-{4:00}-{5:00}", now.Year, now.Month, now.Day, now.Hour, now.Minute, now.Second);
			string folderBackupPathWithMoment = folderBackupPath + "\\" + moment;
			Directory.CreateDirectory(folderBackupPathWithMoment);
			string folderBackupPathWithMomentForFolder1 = folderBackupPathWithMoment + "\\1";
			Directory.CreateDirectory(folderBackupPathWithMomentForFolder1);
			string folderBackupPathWithMomentForFolder2 = folderBackupPathWithMoment + "\\2";
			Directory.CreateDirectory(folderBackupPathWithMomentForFolder2);
			foreach (var resxFiles1Path in resxFiles1Paths)
			{
				string fileName =Path.GetFileName(resxFiles1Path);
				string destFilePath = Path.Combine(folderBackupPathWithMomentForFolder1, fileName);
				File.Copy(resxFiles1Path, destFilePath );
			}
			foreach (var resxFiles2Path in resxFiles2Paths)
			{
				string fileName = Path.GetFileName(resxFiles2Path);
				string destFilePath = Path.Combine(folderBackupPathWithMomentForFolder2, fileName);
				File.Copy(resxFiles2Path, destFilePath);
			}
			customTraceLog.AddLine("Finished creating backup folder structure.");
			#endregion

			#region find pairs and log non paired
			customTraceLog.AddLine("Finding pairs in two folders...");
			List<KeyValuePair<string ,string >> pairs = new List<KeyValuePair<string, string>>();
			for (int f1 = 0; f1 < resxFiles1Paths.Count; f1++)
			{
			    string pairFile = resxFiles2Paths.SingleOrDefault(f=> string.Compare(Path.GetFileName(resxFiles1Paths[f1]),Path.GetFileName(f), StringComparison.OrdinalIgnoreCase)==0);
				if (pairFile == null)
				{
					hasErrors = true;
					customTraceLog.AddLine(string.Format("Error: No pair file for: {0}", resxFiles1Paths[f1]));
				}
				else
				{
					pairs.Add(new KeyValuePair<string, string>(resxFiles1Paths[f1], pairFile));
				}
			}

			for (int f2 = 0; f2 < resxFiles1Paths.Count; f2++)
			{
				bool allreadyPaired = pairs.Where(p => string.Compare(resxFiles2Paths[f2], p.Value, StringComparison.OrdinalIgnoreCase)==0).Count()>0;
				if(!allreadyPaired){
					hasErrors = true;
					customTraceLog.AddLine(string.Format("Error: No pair file for: {0}", resxFiles2Paths[f2]));
				}
			}
			customTraceLog.AddLine("Finished finding pairs in two folders.");
			#endregion

			foreach (var pair in pairs)
			{
				bool hasC;
				bool hasE = Synchronizer.SynchronizeResxFiles(pair.Key, pair.Value, ref customTraceLog, now, out hasC);
				hasErrors = hasErrors || hasE;
				hasConflicts = hasConflicts || hasC;
			}
			customTraceLog.AddLine(string.Format("Finished. Has errors:{0}, Has conflicts:{1}", hasErrors, hasConflicts));

			

			log = customTraceLog.ToString();
			string logFilePath = Path.Combine(folderBackupPathWithMoment, "Log.txt");
			File.WriteAllText(logFilePath, log);
			return hasErrors;
		}

		public static bool RestoreLastBackup(string backupFolderPath, string folder1Path, string folder2Path)
		{
			bool hasErrors = false;

			backupFolderPath = backupFolderPath.TrimEnd('\\');
			if (!Directory.Exists(backupFolderPath))
			{
				throw new Exception("Folder not found:" + backupFolderPath);
			}

			var allSubFolders = GetFolderPaths(backupFolderPath);
			if (allSubFolders == null || allSubFolders.Count == 0)
			{
				throw new Exception("No subfolders/backups in:" + backupFolderPath);
			}

			allSubFolders.Sort();

			var lastSubfolder = allSubFolders.Last();

			folder1Path = folder1Path.TrimEnd('\\');
			if (!Directory.Exists(folder1Path))
			{
				Directory.CreateDirectory(folder1Path);
			}

			folder2Path = folder2Path.TrimEnd('\\');
			if (!Directory.Exists(folder2Path))
			{
				Directory.CreateDirectory(folder2Path);
			}


			string folderBackupWithMomentForFolder1 = Path.Combine(lastSubfolder, "1");
			string folderBackupWithMomentForFolder2 = Path.Combine(lastSubfolder, "2");
			if (!Directory.Exists(folderBackupWithMomentForFolder1))
			{
				throw new Exception("Folder not found:" + folderBackupWithMomentForFolder1);
			}
			if (!Directory.Exists(folderBackupWithMomentForFolder2))
			{
				throw new Exception("Folder not found:" + folderBackupWithMomentForFolder2);
			}

			var resxesForFolder1 = GetFilePaths(folderBackupWithMomentForFolder1, "*.resx");
			if (resxesForFolder1 == null || resxesForFolder1.Count == 0)
			{
				throw new Exception("No .resx files in folder:" + folderBackupWithMomentForFolder1);
			}

			var resxesForFolder2 = GetFilePaths(folderBackupWithMomentForFolder2, "*.resx");
			if (resxesForFolder2 == null || resxesForFolder2.Count == 0)
			{
				throw new Exception("No .resx files in folder:" + folderBackupWithMomentForFolder2);
			}

			foreach (var resxFiles1Path in resxesForFolder1)
			{
				string fileName = Path.GetFileName(resxFiles1Path);
				string destFilePath = Path.Combine(folder1Path, fileName);
				File.Copy(resxFiles1Path, destFilePath, true);
			}
			foreach (var resxFiles2Path in resxesForFolder2)
			{
				string fileName = Path.GetFileName(resxFiles2Path);
				string destFilePath = Path.Combine(folder2Path, fileName);
				File.Copy(resxFiles2Path, destFilePath, true);
			}

			return hasErrors;
		}

		public static string GetLastLogFilePath(string backupFolderPath)
		{
			backupFolderPath = backupFolderPath.TrimEnd('\\');
			if (!Directory.Exists(backupFolderPath))
			{
				throw new Exception("Folder not found:" + backupFolderPath);
			}

			var allSubFolders = GetFolderPaths(backupFolderPath);
			if (allSubFolders == null || allSubFolders.Count == 0)
			{
				throw new Exception("No subfolders/backups in:" + backupFolderPath);
			}

			allSubFolders.Sort();

			var lastSubfolder = allSubFolders.Last();
			var result = Path.Combine(lastSubfolder, "log.txt");
			return result;
		}

		#region Helper methods
		/// <summary>
		/// Gets list of strings where each is full path to file including filename (for example: <example>c:\dir\filename.ext</example>.
		/// </summary>
		/// <param name="folder">Full path of folder that should be searched. For example: <example>c:\dir</example>.</param>
		/// <param name="searchPatern">Filter that should be used. For example: <example>*.txt</example></param>
		/// <returns></returns>
		/// <exception cref="ArgumentException">Thrown when parameter is null or empty.</exception>
		public static List<string> GetFilePaths(string folderPath, string searchPatern)
		{
			if (string.IsNullOrEmpty(folderPath)) throw new ArgumentException("Value must be non-empty string.", "folderPath");
			if (string.IsNullOrEmpty(searchPatern)) throw new ArgumentException("Value must be non-empty string.", "searchPatern");

			List<string> filePaths = new List<string>();
			string[] filePathStrings = Directory.GetFiles(folderPath, searchPatern);
			if (filePathStrings != null)
			{
				filePaths.AddRange(filePathStrings);
			}

			return filePaths;
		}

		/// <summary>
		/// Gets list of strings where each is full path to folder (for example FolderA) including foldername (for example: <example>c:\dir\FolderA</example>.
		/// </summary>
		/// <param name="folder">Full path of folder that should be searched. For example: <example>c:\dir</example>.</param>
		/// <returns></returns>
		/// <exception cref="ArgumentException">Thrown when parameter is null or empty.</exception>
		public static List<string> GetFolderPaths(string folderPath)
		{
			if (string.IsNullOrEmpty(folderPath)) throw new ArgumentException("Value must be non-empty string.", "folderPath");

			List<string> folderPaths = new List<string>();
			string[] folderPathStrings = Directory.GetDirectories(folderPath);
			if (folderPathStrings != null)
			{
				folderPaths.AddRange(folderPathStrings);
			}

			return folderPaths;
		}

		/// <summary>
		/// Opens file with associated external application.
		/// </summary>
		/// <param name="filePath">Full path with filename of file to open.</param>
		/// <param name="filePath">Parameters to add. Pass null for no parameters.</param>
		/// <exception cref="ArgumentException">Thrown when parameter is null or empty.</exception>
		public static void OpenFile(string filePath, string parameters)
		{
			if (string.IsNullOrEmpty(filePath)) throw new ArgumentException("Value must be non-empty string.", "filePath");

			Process process = new Process();
			process.StartInfo.UseShellExecute = true;
			process.StartInfo.FileName = filePath;
			if (!string.IsNullOrEmpty(parameters))
			{
				process.StartInfo.Arguments = parameters;
			}
			process.Start();
		}

		/// <summary>
		/// Opens file with associated external application.
		/// </summary>
		/// <param name="filePath">Full path with filename of file to open.</param>
		/// <exception cref="ArgumentException">Thrown when parameter is null or empty.</exception>
		public static void OpenFile(string filePath)
		{
			OpenFile(filePath, null);
		}
		#endregion
	}
}
