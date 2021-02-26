using System;
using System.IO;

namespace ApprenticeshipReportHelper
{
    /// <summary>
    /// Zuständige Klasse, um die Nutzereingaben zu verarbeiten und zu überprüfen.
    /// </summary>
    public class Input
    {
        #region Public Properties

        /// <summary>
        /// Das Startdatum.
        /// </summary>
        public static DateTime DateStart { get; set; }

        /// <summary>
        /// Die Ausdauer der Ausbildung in Jahren. (Halbjahre mit X,5 sind zulässig)
        /// </summary>
        public static double ApprenticeDuration { get; set; }

        /// <summary>
        /// Die Anzahl der Arbeitstage in einer Ausbildungswoche.
        /// </summary>
        public static int WorkDaysPerWeek { get; set; }

        /// <summary>
        /// Das übergeordnete Verzeichnis für die Ausgabe der Verzeichnisse und Dokument-Dateien.
        /// </summary>
        public static string OutputRootDir { get; set; }

        #endregion Public Properties

        #region Private Methods

        /// <summary>
        /// Überprüft, ob es sich bei der Eingabe des Nutzers um ein gültiges Datum handelt.
        /// </summary>
        /// <param name="input">Die Eingabe des Nutzers.</param>
        /// <returns>Das Startdatum der Ausbildung.</returns>
        private static DateTime VerifyDateTimeInput(string input)
        {
            // Lokale Variablen.
            DateTime dateStart;
            DateTime dateMin = DateTime.Now.AddYears(-100);
            DateTime dateMax = DateTime.Now.AddYears(100);
            bool convertionSuccess;
            bool validInput;
            string displayRequest = "Bitte geben Sie das Startdatum der Ausbildung ein: ";
            string displayInvalid = "[Ungültige Eingabe!] Bitte nutzen sie folgendes Format: [YYYY-MM-DD] oder ein Äquivalent.\n"
                                  + "Der Beginn kann außerdem maximal nur [100 Jahre] in der Zukunft oder der Vergangenheit liegen.\n";

            // Die Eingabe ist gültig, wenn diese in ein DateTime-Objekt konvertiert werden kann und im gültigen Bereich liegt.
            convertionSuccess = DateTime.TryParse(input, out dateStart);
            validInput = convertionSuccess && (dateStart > dateMin && dateStart < dateMax);

            // Wiederholung, solange die Eingabe ungültig ist.
            while (!validInput)
            {
                Console.Clear();
                Color.WriteLine(displayInvalid, Color.Error);
                Console.Write(displayRequest);

                input = Console.ReadLine();

                convertionSuccess = DateTime.TryParse(input, out dateStart);
                validInput = convertionSuccess && (dateStart > dateMin && dateStart < dateMax);
            }

            // Sobald die Eingabe verifiziert wurde, wird diese zurückgegeben.
            return dateStart;
        }

        /// <summary>
        /// Überprüft, ob es sich bei der Eingabe des Nutzers um eine gültige Zahl für die Dauer der Ausbildung handelt.
        /// </summary>
        /// <param name="input">Die Eingabe des Nutzers.</param>
        /// <returns>Die Dauer der Ausbildung in Jahren.</returns>
        private static double VerifyApprenticeDurationInput(string input)
        {
            // Lokale Variablen.
            double duration;
            double decimalPlace;
            int durationMin = 1;
            int durationMax = 10;
            bool convertionSuccess;
            bool validInput;
            string displayRequest = "Bitte geben Sie die Dauer der Ausbildung in Jahren ein: ";
            string displayInvalid = "[Ungültige Eingabe!] Bitte nutzen sie folgendes Format: [Ganzzahl] (z.B. [3])\noder [Kommazahl] mit X,5 für ein Halbjahr (z.B. [2,5]).\n"
                                  + "Die Dauer muss mindestens [1 Jahr] und darf maximal [10 Jahre] betragen.\n";

            // Die Eingabe ist gültig, wenn diese in eine gültige FKZ konvertiert werden kann, zwischen 1-10 liegt, und die Nachkommastelle darf nur 0 oder 5 betragen.
            convertionSuccess = Double.TryParse(input, out duration);
            decimalPlace = duration - Math.Truncate(duration);
            validInput = convertionSuccess && (duration >= durationMin && duration <= durationMax) && (decimalPlace == 0.5 || decimalPlace == 0.0);

            // Wiederholung, solange die Eingabe ungültig ist.
            while (!validInput)
            {
                Console.Clear();
                Color.WriteLine(displayInvalid, Color.Error);
                Console.Write(displayRequest);

                input = Console.ReadLine();

                convertionSuccess = Double.TryParse(input, out duration);
                decimalPlace = duration - Math.Truncate(duration);
                validInput = convertionSuccess && (duration >= durationMin && duration <= durationMax) && (decimalPlace == 0.5 || decimalPlace == 0.0);
            }

            // Sobald die Eingabe verifiziert wurde, wird diese zurückgegeben.
            return duration;
        }

        /// <summary>
        /// Überprüft, ob es sich bei der Eingabe des Nutzers um eine gültige Zahl für die Arbeitstage in einer Woche handelt.
        /// </summary>
        /// <param name="input">Die Eingabe des Nutzers.</param>
        /// <returns>Die Arbeitstage in einer Woche.</returns>
        private static int VerifyWorkDaysInput(string input)
        {
            // Lokale Variablen.
            int workDaysPerWeek;
            int workDaysMin = 1;
            int workDaysMax = 7;
            bool convertionSuccess;
            bool validInput;
            string displayRequest = "Bitte geben Sie die Arbeitstage in einer Woche ein: ";
            string displayInvalid = "[Ungültige Eingabe!] Bitte nutzen sie folgendes Format: [Ganzzahl] (z.B. [3]).\n"
                                  + "Die Anzahl der Tage muss mindestens [1] und darf maximal [7] betragen.\n";

            // Die Eingabe ist gültig, wenn diese in eine gültige GZ konvertiert werden kann und zwischen 1-7 liegt.
            convertionSuccess = Int32.TryParse(input, out workDaysPerWeek);
            validInput = convertionSuccess && (workDaysPerWeek >= workDaysMin && workDaysPerWeek <= workDaysMax);

            // Wiederholung, solange die Eingabe ungültig ist.
            while (!validInput)
            {
                Console.Clear();
                Color.WriteLine(displayInvalid, Color.Error);
                Console.Write(displayRequest);

                input = Console.ReadLine();

                convertionSuccess = Int32.TryParse(input, out workDaysPerWeek);
                validInput = convertionSuccess && (workDaysPerWeek >= workDaysMin && workDaysPerWeek <= workDaysMax);
            }

            // Sobald die Eingabe verifiziert wurde, wird diese zurückgegeben.
            return workDaysPerWeek;
        }

        /// <summary>
        /// Überprüft, ob es sich bei der Eingabe des Nutzers um einen existierenden Pfad handelt.
        /// </summary>
        /// <param name="input">Die Eingabe des Nutzers.</param>
        /// <returns>Der Ausgabepfad.</returns>
        private static string VerifyPathExistance(string input)
        {
            // Lokale Variablen.
            string directory;
            bool validInput;
            string displayRequest = "Bitte geben sie den gewünschten Ausgabepfad ein: ";
            string displayInvalid = "[Ungültige Eingabe!] Bitte geben Sie einen [gültigen Pfad] ein.\n"
                                  + "Beispiel: [X:\\VerzeichnisName1\\VerzeichnisName2].";

            // Die Eingabe ist gültig, wenn der Pfad gefunden werden konnte.
            validInput = Directory.Exists(input);

            // Wiederholung, solange der Pfad nicht gefunden werden konnte.
            while (!validInput)
            {
                Console.Clear();
                Color.WriteLine(displayInvalid, Color.Error);
                Console.Write(displayRequest);

                input = Console.ReadLine();

                validInput = Directory.Exists(input);
            }

            // Wenn die Eingabe die Validierung besteht, wird das Verzeichnis dieser gleichgesetzt.
            directory = input;

            // Sobald die Eingabe verifiziert wurde, wird diese zurückgegeben.
            return directory;
        }

        #endregion Private Methods

        #region Public Methods

        /// <summary>
        /// Fordert den Nutzer zur Eingabe der benötigten Daten auf.
        /// </summary>
        public static void RequestInputs()
        {
            // Nutzereingabe für das Startdatum.
            Console.Write("Bitte geben Sie das Startdatum der Ausbildung ein (Format: gültiges Datum (z.B. YYYY-MM-DD)): ");
            DateStart = VerifyDateTimeInput(Console.ReadLine());

            // Nutzereingabe für die Dauer der Ausbildung.
            Console.Write("Bitte geben Sie die Dauer der Ausbildung in Jahren ein (Format: Ganz- oder Kommazahl): ");
            ApprenticeDuration = VerifyApprenticeDurationInput(Console.ReadLine());

            // Nutzereingabe für die Arbeitstage in einer Woche.
            Console.Write("Bitte geben Sie die Arbeitstage in einer Woche ein (Format: Ganzzahl): ");
            WorkDaysPerWeek = VerifyWorkDaysInput(Console.ReadLine());

            // Nutzereingabe für den gewünschten Ausgabepfad.
            Console.Write("Bitte geben sie den gewünschten Ausgabepfad ein (Format: X:\\...\\...): ");
            OutputRootDir = VerifyPathExistance(Console.ReadLine());
        }

        #endregion Public Methods
    }
}