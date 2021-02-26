using System;
using System.IO;
using System.Collections.Generic;

namespace ApprenticeshipReportHelper
{
    /// <summary>
    /// Zuständige Klasse für die Erstellung der Verzeichnisse und Umbenennung der zu kopierenden Dokumentenvorlagen.
    /// </summary>
    public class Creator
    {
        #region Public Properties

        /// <summary>
        /// Das derzeitige Datum für die Schleife und Verzeichnisnamen.
        /// </summary>
        public static DateTime DateCurrent { get; set; }

        /// <summary>
        /// Das Enddatum der Ausbildung.
        /// </summary>
        public static DateTime DateEnd { get; set; } = Calc.EndDate(Input.DateStart, Input.ApprenticeDuration);

        /// <summary>
        /// Teilzähler für die überlappenden Wochen zwischen den Ausbildungsjahren.
        /// </summary>
        public static int OverlapWeekParts { get; set; }

        /// <summary>
        /// Zählt die Anzahl der erstellten Verzeichnisse, für die nichts auf den Wochenzähler addiert wurde.
        /// </summary>
        public static int NoWeekIncrementCounter { get; set; }

        /// <summary>
        /// Wert zur Überprüfung, ob bereits ein Ausbildungsjahr durchlaufen wurde.
        /// </summary>
        public static bool HasInitialYearPassed { get; set; } = false;

        #endregion Public Properties

        #region Public Methods

        /// <summary>
        /// Setzt das Verzeichnis für das Ausbildungsjahr und erstellt dieses.
        /// </summary>
        /// <param name="yearCounter">Das Ausbildungsjahr.</param>
        /// <returns>Das Verzeichnis für das Ausbildungsjahr.</returns>
        static public string YearDirectory(int yearCounter)
        {
            // Lokale Variablen
            string folderNameYear = "Ausbildungsjahr ";
            string outputYearDir;

            // Legt den Namen für das Verzeichnis der Ausbildungsjahre fest.
            outputYearDir = Path.Combine(Input.OutputRootDir, folderNameYear + $"{yearCounter}");

            // Erstellt die Verzeichnisse für die Ausbildungsjahre, wenn das Verzeichnis nicht bereits existiert.
            if (!Directory.Exists(outputYearDir))
            {
                try
                {
                    Directory.CreateDirectory(outputYearDir);
                    Color.WriteLine($"\nVerzeichnis [{outputYearDir}] erstellt!\n", Color.Path);
                }
                catch (Exception ex)
                {
                    Color.WriteLine($"Verzeichnis [{outputYearDir}] konnte nicht erstellt werden. Details: \n\n[{ex.Message}]\n\n", Color.Error);
                    Color.Write("Drücken Sie die [Leertaste], um das Programm zu beenden...", Color.Key);
                    Helper.WaitForKey(ConsoleKey.Spacebar);
                    Environment.Exit(0);
                }
            }
            else
            {
                Color.WriteLine($"Das Verzeichnis mit dem Namen [{outputYearDir}] existiert bereits im Verzeichnis!", Color.Path);
            }

            // Wenn der Jahreszähler größer als 1 ist, wird der Wert für die Überprüfung des Initialjahres auf wahr gesetzt.
            if (yearCounter > 1)
            {
                HasInitialYearPassed = true;
            }

            // Gibt den Pfad des Jahresverzeichnisses zurück.
            return outputYearDir;
        }

        /// <summary>
        /// Setzt das Verzeichnis für die Ausbildungswoche und erstellt dieses.
        /// </summary>
        /// <param name="outputYearDir">Das Jahresverzeichnis.</param>
        /// <param name="yearCounter">Das Ausbildungsjahr.</param>
        /// <param name="weekCounter">Die Ausbildungswoche.</param>
        /// <returns>Das Verzeichnis für die Ausbildungswoche.</returns>
        static public string WeekDirectory(string outputYearDir, int yearCounter, int weekCounter)
        {
            // Lokale Variablen
            string weekTimeSpan = default;
            string outputWeekDir;
            string latestDirInPreviousYear;
            int dayOffset;

            /* Setzt den Wert des Strings, welcher für den Namen der Verzeichnisse verantwortlich ist, wenn
               es sich um den Beginn eines Ausbildungsjahres handelt. */
            if (DateCurrent == Input.DateStart.AddYears(yearCounter - 1))
            {
                int nextMonday = Calc.DayOffset(DateCurrent.DayOfWeek, DayOfWeek.Monday);

                switch (Input.WorkDaysPerWeek)
                {
                    case 1:
                        // Nur Fall 1 wird unterschieden, da es hier keine Datumsspanne gibt.
                        weekTimeSpan = DateCurrent.ToString("yyyy-MM-dd")
                                     + " - " + $"Woche {weekCounter}";
                        break;
                    case 2:
                    case 3:
                    case 4:
                    case 5:
                    case 6:
                    case 7:
                        dayOffset = Calc.DayOffset(DateCurrent.DayOfWeek, (DayOfWeek)Input.WorkDaysPerWeek);
                        latestDirInPreviousYear = Helper.GetLastCreatedFileFromDirectory(yearCounter);

                        // Wird benötigt, da Sonntag 0 und nicht 7 ist.
                        if (dayOffset == 7)
                        {
                            dayOffset = 0;
                        }

                        /* Wenn der Tages-Offset kleiner ist als der Offset zum nächsten Montag ist,
                           wird hier der Verzeichnisname bestimmt. */
                        if (nextMonday > dayOffset)
                        {
                            // Überprüfung, ob die zuletzt erstellte Datei im vorherigen Jahresverzeichnis auf "Teil 1" endet.
                            if (latestDirInPreviousYear.EndsWith("Teil 1"))
                            {
                                /* Erhöht den Zähler für die zwischen 2 Ausbildungsjahren überlappende Woche zurück, da die zuletzt erstellte Datei
                                       nur auf "Teil 1" endet, wenn die Woche überlappt. */
                                OverlapWeekParts++;

                                weekTimeSpan = DateCurrent.ToString("yyyy-MM-dd") + " - "
                                             + DateCurrent.AddDays(dayOffset).ToString("yyyy-MM-dd")
                                             + " - " + $"Woche {weekCounter} - Teil {OverlapWeekParts}";

                                // Setzt den Zähler für die zwischen 2 Ausbildungsjahren überlappende Woche zurück, da dieser maximal 2 betragen kann.
                                OverlapWeekParts = 0;
                            }
                            else
                            {
                                weekTimeSpan = DateCurrent.ToString("yyyy-MM-dd") + " - "
                                             + DateCurrent.AddDays(dayOffset).ToString("yyyy-MM-dd")
                                             + " - " + $"Woche {weekCounter}";
                            }
                        }
                        /* Wenn der Tages-Offset größer ist als der Offset zum nächsten Montag ist,
                           wird hier der Verzeichnisname bestimmt. */
                        else
                        {
                            /* Setzt den Namen für die Wochenverzeichnisse, wenn der Wert durch das Hinzufügen des Montags-Offsets
                              zum Startdatum kleiner oder gleich dem Wert ist, welcher sich durch dem Hinzufügen der Arbeitstage auf
                              das derzeitige Datum ergibt. */
                            if (Input.DateStart.AddYears(yearCounter - 1).AddDays(nextMonday) <= DateCurrent.AddDays(Input.WorkDaysPerWeek))
                            {
                                // Fügt dem aktuellen Datum die Anzahl der Tage bis zum nächsten Montag hinzu.
                                DateCurrent = DateCurrent.AddDays(nextMonday);

                                // Überprüfung, ob die zuletzt erstellte Datei im vorherigen Jahresverzeichnis auf "Teil 1" endet.
                                if (latestDirInPreviousYear.EndsWith("Teil 1"))
                                {
                                    /* Erhöht den Zähler für die zwischen 2 Ausbildungsjahren überlappende Woche zurück, da die zuletzt erstellte Datei
                                       nur auf "Teil 1" endet, wenn die Woche überlappt. */
                                    OverlapWeekParts++;

                                    weekTimeSpan = DateCurrent.ToString("yyyy-MM-dd") + " - "
                                                 + DateCurrent.AddDays(Input.WorkDaysPerWeek - 1).ToString("yyyy-MM-dd")
                                                 + " - " + $"Woche {weekCounter} - Teil {OverlapWeekParts}";

                                    // Setzt den Zähler für die zwischen 2 Ausbildungsjahren überlappende Woche zurück, da dieser maximal 2 betragen kann.
                                    OverlapWeekParts = 0;
                                }
                                else
                                {
                                    weekTimeSpan = DateCurrent.ToString("yyyy-MM-dd") + " - "
                                                 + DateCurrent.AddDays(Input.WorkDaysPerWeek - 1).ToString("yyyy-MM-dd")
                                                 + " - " + $"Woche {weekCounter}";
                                }

                                // Kalkuliert die Tage bis zum nächsten Montag und fügt diese dem aktuellen Datum hinzu.
                                nextMonday = Calc.DayOffset(DateCurrent.DayOfWeek, DayOfWeek.Monday);
                                DateCurrent = DateCurrent.AddDays(nextMonday - 1);
                            }
                            else
                            {
                                // Überprüfung, ob die zuletzt erstellte Datei im vorherigen Jahresverzeichnis auf "Teil 1" endet.
                                if (latestDirInPreviousYear.EndsWith("Teil 1"))
                                {
                                    /* Erhöht den Zähler für die zwischen 2 Ausbildungsjahren überlappende Woche zurück, da die zuletzt erstellte Datei
                                       nur auf "Teil 1" endet, wenn die Woche überlappt. */
                                    OverlapWeekParts++;

                                    weekTimeSpan = DateCurrent.ToString("yyyy-MM-dd") + " - "
                                                 + DateCurrent.AddDays(Input.WorkDaysPerWeek - 1).ToString("yyyy-MM-dd")
                                                 + " - " + $"Woche {weekCounter}";

                                    // Setzt den Zähler für die zwischen 2 Ausbildungsjahren überlappende Woche zurück, da dieser maximal 2 betragen kann.
                                    OverlapWeekParts = 0;
                                }
                                else
                                {
                                    weekTimeSpan = DateCurrent.ToString("yyyy-MM-dd") + " - "
                                                 + DateCurrent.AddDays(Input.WorkDaysPerWeek - 1).ToString("yyyy-MM-dd")
                                                 + " - " + $"Woche {weekCounter}";
                                }
                            }
                        }
                        break;
                    default:
                        // Dies sollte unter keinen Umständen möglich sein, da diese Variable nur 1-7 sein kann.
                        Environment.Exit(0);
                        break;
                }
            }
            // Falls es sich nicht um den Anfang eines Ausbildungsjahres handelt, wird hier der Verzeichnisname bestimmt.
            else
            {
                /* Falls der Wert, welcher sich aus dem Hinzufügen des Offsets aus dem aktuellen Tag und dem nächsten Montag zum aktuellen Datum ergibt,
                   größer ist, als der Wert, welcher sich aus dem Hinzufügen des Ausbildungsjahres zum Startdatum ergibt,
                   wird hier der Verzeichnisname bestimmt. */
                if (DateCurrent.AddDays(Calc.DayOffset(DateCurrent.DayOfWeek, DayOfWeek.Monday)) > Input.DateStart.AddYears(yearCounter))
                {
                    // Fall 1 unterscheidet sich wieder, da hier keine Datumsspanne benötigt wird.
                    if (Input.WorkDaysPerWeek == 1)
                    {
                        weekTimeSpan = DateCurrent.ToString("yyyy-MM-dd")
                                     + " - " + $"Woche {weekCounter}";
                    }
                    else
                    {
                        /* Bestimmt den Verzeichnisnamen, wenn der Wochentag, welcher sich aus dem Hinzufügen des Ausbildungsjahres zum 
                           Startdatum und der Subtraktion eines Tages ergibt, ungleich dem letzten Tag in einer Arbeitswoche ist und der
                           Jahreszähler nicht der Zahl der Ausbildungsdauer entspricht. */
                        if (DateCurrent.AddDays(Input.WorkDaysPerWeek - 1) >= Input.DateStart.AddYears(yearCounter)
                            && (yearCounter != Input.ApprenticeDuration))
                        {
                            OverlapWeekParts++;

                            /* Bestimmt den Verzeichnisnamen, wenn durch das Hinzufügen der Anzahl der Arbeitstage in einer Woche 
                               zu dem aktuellen Datum ein größerer oder gleicher Wert entsteht, als der Beginn des nächsten Ausbildungsjahres. */
                            if (DateCurrent.AddDays(Input.WorkDaysPerWeek - 1) >= Input.DateStart.AddYears(yearCounter))
                            {
                                weekTimeSpan = DateCurrent.ToString("yyyy-MM-dd") + " - "
                                             + Input.DateStart.AddYears(yearCounter).AddDays(-1).ToString("yyyy-MM-dd")
                                             + " - " + $"Woche {weekCounter} - Teil {OverlapWeekParts}";
                            }
                            else
                            {
                                weekTimeSpan = DateCurrent.ToString("yyyy-MM-dd") + " - "
                                             + DateCurrent.AddDays(Input.WorkDaysPerWeek - 1).ToString("yyyy-MM-dd")
                                             + " - " + $"Woche {weekCounter} - Teil {OverlapWeekParts}";
                            }
                        }
                        else
                        {
                            /* Bestimmt den Verzeichnisnamen, wenn durch das Hinzufügen der Anzahl der Arbeitstage in einer Woche 
                               zu dem aktuellen Datum ein größerer oder gleicher Wert entsteht, als der Beginn des nächsten Ausbildungsjahres. */
                            if (DateCurrent.AddDays(Input.WorkDaysPerWeek - 1) >= Input.DateStart.AddYears(yearCounter))
                            {
                                weekTimeSpan = DateCurrent.ToString("yyyy-MM-dd") + " - "
                                             + Input.DateStart.AddYears(yearCounter).AddDays(-1).ToString("yyyy-MM-dd")
                                             + " - " + $"Woche {weekCounter}";
                            }
                            else
                            {
                                weekTimeSpan = DateCurrent.ToString("yyyy-MM-dd") + " - "
                                             + DateCurrent.AddDays(Input.WorkDaysPerWeek - 1).ToString("yyyy-MM-dd")
                                             + " - " + $"Woche {weekCounter}";
                            }
                        }
                    }
                }
                else
                {
                    // Fall 1 unterscheidet sich wieder, da hier keine Datumsspanne benötigt wird.
                    if (Input.WorkDaysPerWeek == 1)
                    {
                        weekTimeSpan = DateCurrent.ToString("yyyy-MM-dd")
                                     + " - " + $"Woche {weekCounter}";
                    }
                    else
                    {
                        weekTimeSpan = DateCurrent.ToString("yyyy-MM-dd") + " - "
                                     + DateCurrent.AddDays(Input.WorkDaysPerWeek - 1).ToString("yyyy-MM-dd")
                                     + " - " + $"Woche {weekCounter}";
                    }
                }
            }

            // Setzt den Verzeichnispfad für die Wochen.
            outputWeekDir = Path.Combine(outputYearDir + "\\" + weekTimeSpan);

            // Erstellt die Verzeichnisse für die Ausbildungswochen, wenn das Verzeichnis nicht bereits existiert.
            if (!Directory.Exists(outputWeekDir) && (DateCurrent < DateEnd))
            {
                try
                {
                    Directory.CreateDirectory(outputWeekDir);
                    Color.WriteLine($"Verzeichnis [{outputWeekDir}] erstellt!", Color.Path);
                }
                catch (Exception ex)
                {
                    Color.WriteLine($"Verzeichnis [{outputWeekDir}] konnte nicht erstellt werden. Details: \n\n[{ex.Message}]\n\n", Color.Error);
                    Color.Write("Drücken Sie die [Leertaste], um das Programm zu beenden...", Color.Key);
                    Helper.WaitForKey(ConsoleKey.Spacebar);
                    Environment.Exit(0);
                }
            }
            else
            {
                if (Directory.Exists(outputWeekDir))
                {
                    Color.WriteLine($"Das Verzeichnis mit dem Namen [{outputWeekDir}] existiert bereits im Verzeichnis!", Color.Path);
                }
                else if (DateCurrent > DateEnd)
                {
                    // Nichts passiert. Dieser Fall sollte nur eintreten, wenn alle Verzeichnisse erstellt wurden bzw. bereits vorhanden sind.
                }
            }

            // Gibt den Pfad des Wochenverzeichnisses zurück.
            return outputWeekDir;
        }

        /// <summary>
        /// Setzt den Pfad für die Kopie der Vorlagen-Dateien, kopiert und benennt diese um.
        /// </summary>
        /// <param name="outputWeekDir">Das Wochenverzeichnis.</param>
        /// <returns>Die Anzahl der Dateien, welche kopiert wurden.</returns>
        static public int CopyAndRenameTemplateFiles(string outputWeekDir)
        {
            // Lokale Variablen
            List<string> templateFiles = Helper.GetTemplateFiles();
            int filesCopied = 0;
            int filesCounter = 1;

            try
            {
                // Jedes unterstützte Dokument, welches sich im "template" Verzeichnis befindet wird kopiert und umbenannt, falls dieses nicht bereits im Output-Verzeichnis existiert.
                foreach (string sourceFilePath in templateFiles)
                {
                    // Dateiname wird dem Verzeichnisname der Woche gleichgesetzt.
                    string fileName = new DirectoryInfo(outputWeekDir).Name;

                    FileInfo currentFile = new FileInfo(sourceFilePath);
                    string outputWeekDirTemp = Path.Combine(outputWeekDir + "\\" + fileName + currentFile.Extension);

                    // Kopiere die Vorlagen-Datei, wenn die Datei im Zielverzeichnis nicht existiert.
                    if (!File.Exists(outputWeekDirTemp) && (DateCurrent < DateEnd))
                    {
                        currentFile.CopyTo(outputWeekDirTemp);

                        Color.WriteLine($"Vorlage-Datei [{currentFile.Name}] wurde nach [{outputWeekDirTemp}] kopiert und zu [{fileName}] umbenannt!\n", Color.Path);

                        filesCopied++;
                        filesCounter++;
                    }
                    else
                    {
                        /* Falls die Datei bereits im Zielverzeichnis existiert, wird die nächste Datei umbenannt und kopiert, 
                           insofern das aktuelle Datum nicht das Enddatum überschreitet. */
                        if (File.Exists(outputWeekDirTemp) && (DateCurrent < DateEnd))
                        {
                            fileName += $" ({filesCounter})";
                            outputWeekDirTemp = Path.Combine(outputWeekDir + "\\" + fileName + currentFile.Extension);
                            currentFile.CopyTo(outputWeekDirTemp);

                            Color.WriteLine($"Vorlage-Datei [{currentFile.Name}] wurde nach [{outputWeekDirTemp}] kopiert und zu [{fileName}] umbenannt!\n", Color.Path);

                            filesCopied++;
                            filesCounter++;
                        }
                        else
                        {
                            // Nichts passiert. Dieser Fall sollte nur eintreten, wenn alle Verzeichnisse erstellt wurden bzw. bereits vorhanden sind.
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Color.WriteLine($"Dateien konnten nicht kopiert und/oder umbenannt werden! Details: \n\n[{ex.Message}]\n\n", Color.Error);
                Color.Write("Drücken Sie die [Leertaste], um das Programm zu beenden...", Color.Key);
                Helper.WaitForKey(ConsoleKey.Spacebar);
                Environment.Exit(0);
            }

            // Gibt die Anzahl der kopierten Dateien zurück.
            return filesCopied;
        }

        /// <summary>
        /// Gibt dem Nutzer Informationen über die Anzahl der erstellten Verzeichnisse und Kopien der Vorlagedateien.
        /// </summary>
        /// <param name="yearCounter">Das Ausbildungsjahr.</param>
        /// <param name="weekCounter">Die Ausbildungswoche.</param>
        /// <param name="filesCopied">Die Anzahl der kopierten Dateien.</param>
        static public void Details(int yearCounter, int weekCounter, int filesCopied)
        {
            Color.WriteLine("Erstellung der Verzeichnisse und Kopieren der Vorlagen-Dateien erledigt.\n\n"
                      + "Informationen:\n"
                      + $"\n\t- [Jahresverzeichnisse erstellt: \t{(yearCounter - 1),5}]"
                      + $"\n\t- [Wochenverzeichnisse erstellt: \t{(weekCounter - 1 + NoWeekIncrementCounter),5}]"
                      + $"\n\t- [Dateien kopiert und umbenannt: \t{filesCopied,5}]", Color.Info);
        }

        #endregion Public Methods
    }
}