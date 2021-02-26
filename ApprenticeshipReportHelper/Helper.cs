using System;
using System.IO;
using System.Linq;
using System.Diagnostics;
using System.Collections.Generic;

namespace ApprenticeshipReportHelper
{
    /// <summary>
    /// Zuständige Struktur für Hilfsmethoden.
    /// </summary>
    public struct Helper
    {
        #region Public Methods

        /// <summary>
        /// Bestimmt diverse Konsolenattribute wie etwa Name und Farbe.
        /// </summary>
        public static void SetConsoleAttributes()
        {
            // Lokale Variablen.
            Console.Title = "Apprenticeship-Report Helper";
            Console.ForegroundColor = Color.Foreground;
            Console.BackgroundColor = Color.Background;
            Console.Clear();
        }

        /// <summary>
        /// Stoppt den Programmablauf bis eine bestimmte Taste gedrückt wird.
        /// </summary>
        /// <param name="key">Die Taste, welche gedrückt werden soll.</param>
        public static void WaitForKey(ConsoleKey key)
        {
            while (Console.ReadKey(true).Key != key)
            {
                // Wartet auf einen bestimmten Input.
            }
        }

        /// <summary>
        /// Bestimmt den Pfad des ausgeführten Programms.
        /// </summary>
        /// <returns>Den Pfad der ausgeführten Datei.</returns>
        public static string GetBaseDirectory()
        {
            // Lokale Variablen.
            ProcessModule processModule = Process.GetCurrentProcess().MainModule;
            string directory = Path.GetDirectoryName(processModule?.FileName);

            // Gibt das Verzeichnis des ausgeführten Programms zurück.
            return directory;
        }

        /// <summary>
        /// Bestimmt das zuletzt geschriebene Verzeichnis in einem Ausbildungsjahresverzeichnis.
        /// </summary>
        /// <param name="yearCounter">Das aktuelle Ausbildungsjahr.</param>
        /// <returns>Der Name der neuesten Datei im vorherigen Ausbildungsjahresverzeichnis.</returns>
        public static string GetLastCreatedFileFromDirectory(int yearCounter)
        {
            // Die leere Zuweisung verhindert eine Null-Exception.
            string latestFile = "";

            // Überprüfung, ob es sich um ein weiterführendes Ausbildungsjahr handelt.
            if (yearCounter > 1)
            {
                // Liest das vorherige Ausbildungsjahresverzeichnis.
                DirectoryInfo directory = new DirectoryInfo(Path.Combine(Input.OutputRootDir + "\\" + "Ausbildungsjahr " + (yearCounter - 1)));

                // Liest den Namen des neuesten Verzeichnisses im vorherigen Ausbildungsjahresverzeichnis.
                latestFile = (from f in directory.GetDirectories()
                              orderby f.LastWriteTime descending
                              select f).First().Name;
            }

            // Gibt den Namen des neuesten Verzeichnisses im vorherigen Ausbildungsjahresverzeichnis zurück.
            return latestFile;
        }

        /// <summary>
        /// Bestimmt die Pfade aller gültiger Vorlage-Dateien im Template-Verzeichnis.
        /// </summary>
        /// <returns>Eine Liste mit den Dateipfaden gültiger Dokument-Dateien.</returns>
        public static List<string> GetTemplateFiles()
        {
            // Lokale Variablen.
            string[] allowedExtensions = { ".doc", ".docx", ".pdf", ".txt", ".ppt", ".pptx", ".htm", ".html", ".xls", ".xlsx" };
            string templatePath = Path.Combine(GetBaseDirectory() + "\\" + "template");
            List<string> templateFiles = new List<string>();

            // Wenn das Verzeichnis nicht existiert wird dieses im gleichem Verzeichnis wie das der ausführbaren Datei erstellt.
            if (!Directory.Exists(templatePath))
            {
                Console.Clear();
                Color.WriteLine($"Das Verzeichnis [{templatePath}] konnte nicht gefunden werden. Erstelle Verzeichnis...", Color.Path);

                try
                {
                    // Erstellt das Verzeichnis.
                    Directory.CreateDirectory(templatePath);
                    Color.WriteLine($"Verzeichnis wurde erstellt. Pfad: [{templatePath}]", Color.Path);
                }
                catch (Exception ex)
                {
                    // Beendet das Programm, wenn das Verzeichnis nicht erstellt werden konnte.
                    Console.Clear();
                    Color.WriteLine($"Ein Fehler ist aufgetreten während der Erstellung des Verzeichnisses! Details: \n\n[{ex.Message}]\n\n", Color.Error);
                    Color.Write("Drücken Sie die [Leertaste], um das Programm zu beenden...", Color.Key);
                    WaitForKey(ConsoleKey.Spacebar);
                    Environment.Exit(0);
                }
            }

            // Speichert alle Dateipfade aus dem Template-Verzeichnis, welche ein gültiges Dateiformat besitzen, in eine Liste.
            templateFiles = Directory
                             .EnumerateFiles(templatePath)
                             .Where(file => allowedExtensions.Any(file.ToLower().EndsWith))
                             .ToList();

            // Beendet das Programm, wenn keine gültigen Berichtsheftvorlage-Dateien gefunden werden konnten.
            if (templateFiles.Count < 1)
            {
                Console.Clear();
                Color.WriteLine($"Es konnte keine gültige Datei als Berichtshefte-Vorlage im Verzeichnis [{templatePath}] gefunden werden.\n"
                               + "Stellen Sie sicher, dass sich in diesem Pfad eine gültige Datei befindet! (siehe readme.txt)\n", Color.Error);
                Color.WriteLine($"Anzahl der Elemente in der Datei-Liste: [{templateFiles.Count}]\n", Color.Error);
                Color.Write("Drücken Sie die [Leertaste], um das Programm zu beenden...", Color.Key);
                WaitForKey(ConsoleKey.Spacebar);
                Environment.Exit(0);
            }

            // Gibt die Liste mit gültigen Dateipfaden aus dem Template-Verzeichnis zurück.
            return templateFiles;
        }

        #endregion Public Methods
    }
}