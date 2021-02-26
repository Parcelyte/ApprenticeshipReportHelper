using System;

namespace ApprenticeshipReportHelper
{
    /// <summary>
    /// Zuständige Struktur für Berechnungen.
    /// </summary>
    public struct Calc
    {
        #region Public Methods

        /// <summary>
        /// Berechnet das Enddatum der Ausbildung.
        /// </summary>
        /// <param name="dateStart">Das Startdatum der Ausbildung.</param>
        /// <param name="duration">Die Dauer der Ausbildung in Jahren.</param>
        /// <returns>Das Enddatum der Ausbildung.</returns>
        public static DateTime EndDate(DateTime dateStart, double duration)
        {
            // Lokale Variablen.
            DateTime dateEnd;
            double decimalPlace = duration - Math.Truncate(duration);

            // Entfernt die Nachkommastelle der Ausbildungsdauer und fügt diese dem Startdatum hinzu, um das Enddatum zu berechnen.
            duration = Math.Floor(duration);
            dateEnd = dateStart.AddYears(Convert.ToInt32(duration));

            // Durch die Input-Validierung kann es hier nur 3 Fälle geben: Keine Nachkommastelle, .0 oder .5. Die ersten beiden Fälle sind hier nicht zu berücksichtigen.
            if (decimalPlace == 0.5)
            {
                dateEnd = dateEnd.AddMonths(6);
            }

            // Gibt das Enddatum der Ausbildung zurück.
            return dateEnd;
        }

        /// <summary>
        /// Gibt den letzten Arbeitstag der Woche zurück, basierend auf den Arbeitstagen pro Woche.
        /// </summary>
        /// <param name="workdays">Die Anzahl der Arbeitstage in einer Woche.</param>
        /// <returns>Den Wochentag des letzten Arbeitstages in einer Arbeitswoche.</returns>
        public static DayOfWeek EndOfWeek(int workdays)
        {
            DayOfWeek weekEndDay = default;

            // Setzt den Endtag der Arbeitswoche auf den Wochentag, basierend auf der Anzahl der Arbeitstage in einer Woche.
            switch (workdays)
            {
                case 1:
                    weekEndDay = DayOfWeek.Monday;
                    break;
                case 2:
                    weekEndDay = DayOfWeek.Tuesday;
                    break;
                case 3:
                    weekEndDay = DayOfWeek.Wednesday;
                    break;
                case 4:
                    weekEndDay = DayOfWeek.Thursday;
                    break;
                case 5:
                    weekEndDay = DayOfWeek.Friday;
                    break;
                case 6:
                    weekEndDay = DayOfWeek.Saturday;
                    break;
                case 7:
                    weekEndDay = DayOfWeek.Sunday;
                    break;
                default:
                    // Sollte nicht erreichbar sein.
                    break;
            }

            // Gibt den Wochentag des Endtags der Arbeitwoche zurück.
            return weekEndDay;
        }

        /// <summary>
        /// Berechnet die Anzahl der Tage von einem Wochentag zu einem anderen.
        /// </summary>
        /// <param name="currentDay">Der derzeitige Wochentag.</param>
        /// <param name="desiredDay">Der angepeilte Wochentag.</param>
        /// <returns>Die Anzahl der Tage, welche bis zu dem gewünschten Wochentag noch fehlen.</returns>
        public static int DayOffset(DayOfWeek currentDay, DayOfWeek desiredDay)
        {
            // Lokale Variablen.
            int current = (int)currentDay;
            int desired = (int)desiredDay;
            int offset = (7 - current + desired) % 7;

            // Gibt die Anzahl der Tage von dem ersten übergegebenen Wochentag zu dem gewünschten Wochentag zurück.
            return offset == 0 ? 7 : offset;
        }

        /// <summary>
        /// Entscheidet, wann der Wochenzähler erhöht werden soll.
        /// </summary>
        /// <param name="yearCounter">Der Zähler für das Ausbildungsjahr.</param>
        /// <param name="weekCounter">Der Zähler für die Ausbildungswoche.</param>
        /// <param name="incrementDays">Die Anzahl der Tage, welche dem Datum hinzugefügt werden soll.</param>
        /// <returns>Der Wochenzähler.</returns>
        public static int IncrementWeek(int yearCounter, int weekCounter, int incrementDays)
        {
            /* Überprüft, ob die derzeitige Woche mit dem nächsten Ausbildungsjahr überlappt, 
               ansonsten wird der Wochenzähler normal erhöht. */
            if (Creator.DateCurrent.AddDays(incrementDays) > Input.DateStart.AddYears(yearCounter))
            {
                if (Input.WorkDaysPerWeek == 1)
                {
                    // Da es nur 1 Tag pro Woche ist, kann es keine Überlappung geben.
                    weekCounter++;
                }
                else if (DayOffset(Creator.DateCurrent.DayOfWeek, (DayOfWeek)Input.WorkDaysPerWeek) < 7)
                {
                    // Wenn der Offset kleiner als 7 ist, bedeutet das, dass die Woche noch nicht vorbei ist.
                    Creator.NoWeekIncrementCounter++;
                }
                else
                {
                    // Erhöht den Wochenzähler, wenn der letzte Tag der Woche erreicht ist.
                    weekCounter++;
                }
            }
            else if (Creator.DateCurrent.AddDays(incrementDays) < Creator.DateEnd)
            {
                // Erhöht den Wochenzähler, wenn der letzte Tag der Woche erreicht ist.
                weekCounter++;
            }
            else
            {
                weekCounter++;
                // Erhöht zum letzten mal den Wochenzähler. Dieser Fall sollte nur eintreten, nachdem alle Verzeichnisse erstellt wurden bzw. bereits vorhanden sind.
            }

            // Gibt den Wochenzähler zurück.
            return weekCounter;
        }

        #endregion Public Methods
    }
}