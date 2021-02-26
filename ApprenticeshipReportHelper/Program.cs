using System;

/* 
Das Programm erstellt eine Verzeichnisstruktur mit Dokument-Vorlagen für Berichtshefte basierend auf den Nutzereingaben.
Die Verzeichnisse sind benannt nach der Zeitspanne der Ausbildungswoche (ISO 8601, YYYY-MM-DD) und der Ausbildungswoche selbst.
(Woche 1, 2, 3, ...)

Folgende Inputs werden vom Nutzer benötigt:

	- Start-Tag der Ausbildung.
		- Gültige Werte: Alle gültigen Datumsformate, wie etwa DD.MM.YYYY, YYYY-MM-DD, MM/DD/YYYY, etc.),
		  welche 100 Jahre in der Zukunft oder der Vergangenheit liegen.
	- Zeitspanne der Ausbildung.
		- Gültige Werte: Ganzzahlen von 1-10 und Kommazahlen mit X,5 für Halbjahre.
	- Arbeitstage pro Woche.
		- Gültige Werte: Ganzzahlen von 1-7.
	- Ausgabepfad der Verzeichnisstruktur.
		- Gültige Werte: Ein auf dem Computer existierender Pfad, z.B. X:\...\...\...

	- Der Nutzer hat die Möglichkeit, die im Verzeichnis "template" vorhandene Vorlage gegen eine eigene auszutauschen,
	  insofern das Format unterstützt wird.

Derzeit werden folgende Datei-Formate unterstützt:
	- .doc, .docx, .pdf, .txt, .ppt, .pptx, .htm, .html, .xls, .xslx
 */
namespace ApprenticeshipReportHelper
{
    class Program
    {
        static void Main(string[] args)
        {
            // Lokale Variablen
            string outputYearDir;
            string outputWeekDir;
            int yearCounter;
            int weekCounter = 1;
            int filesCopied = 0;
            int incrementDays;

            // Bestimmt Konsoleneigenschaften.
            Helper.SetConsoleAttributes();

            Color.WriteLine("Dieses Programm hilft, automatisch eine Verzeichnisstruktur mit Dokument-Vorlagen\nfür die Berichtshefte der Ausbildung zu generieren.\n\n"
                      + "Dafür benötigt dieses Programm ein paar Daten:\n\n"
                      + "\t- [Datum des Ausbildungsstarts]\n"
                      + "\t- [Dauer der Ausbildung]\n"
                      + "\t- [Arbeitstage pro Woche]\n"
                      + "\t- [Ausgabepfad der Verzeichnisstruktur]\n", Color.Info);

            // Aufforderung zur Nutzereingabe und Verarbeitung dieser.
            Input.RequestInputs();

            // Schleife, welche für jedes Ausbildungsjahr durchlaufen wird.
            for (yearCounter = 1; yearCounter <= Math.Ceiling(Input.ApprenticeDuration); yearCounter++)
            {
                // Erstellt das jeweilige Jahresverzeichnis.
                outputYearDir = Creator.YearDirectory(yearCounter);

                // Schleife, welche für jede Woche in einem Ausbildungsjahr durchlaufen wird.
                for (Creator.DateCurrent = Input.DateStart.AddYears(yearCounter - 1);
                     Creator.DateCurrent < Creator.DateEnd;
                     Creator.DateCurrent = Creator.DateCurrent.AddDays(incrementDays))
                {
                    /* Überprüft, wieviele Tage bis zum nächsten Montag fehlen, um diese 
                       Tage dem aktuellen Datum nach Abschluss der Schleife hinzuzufügen. */
                    incrementDays = Calc.DayOffset(Creator.DateCurrent.DayOfWeek, DayOfWeek.Monday);

                    // Erstellt das jeweilige Wochenverzeichnis.
                    outputWeekDir = Creator.WeekDirectory(outputYearDir, yearCounter, weekCounter);

                    // Kopiert die Vorlagen-Dateien aus dem Template-Verzeichnis und benennt diese um.
                    filesCopied += Creator.CopyAndRenameTemplateFiles(outputWeekDir);

                    /* Der Wochenzähler wird erhöht bei Abschluss der Schleife, außer wenn 
                       ein neues Ausbildungsjahr mitten in einer Ausbildungswoche beginnt. */
                    weekCounter = Calc.IncrementWeek(yearCounter, weekCounter, incrementDays);
                }
            }

            // Ausgabe von Informationen zur Erstellung nach erfolgreichem Programmablauf.
            Creator.Details(yearCounter, weekCounter, filesCopied);

            // Warten auf spezifische Tasteneingabe, um das Programm zu beenden...
            Color.Write("\nDrücken Sie die [Leertaste], um das Programm zu beenden...", Color.Key);
            Helper.WaitForKey(ConsoleKey.Spacebar);
        }
    }
}