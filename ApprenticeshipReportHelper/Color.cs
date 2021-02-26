using System;
using System.Text.RegularExpressions;

namespace ApprenticeshipReportHelper
{
    /// <summary>
    /// Zuständige Klasse für die Farben der Konsolenausgaben.
    /// </summary>
    public class Color
    {
        #region Public Properties

        /// <summary>
        /// Die standardmäßige Textfarbe.
        /// </summary>
        public static ConsoleColor Foreground { get; } = ConsoleColor.White;

        /// <summary>
        /// Die standardmäßige Hintergrundfarbe.
        /// </summary>
        public static ConsoleColor Background { get; } = ConsoleColor.Black;

        /// <summary>
        /// Textfarbe für Tastatur-Tasten.
        /// </summary>
        public static ConsoleColor Key { get; } = ConsoleColor.DarkYellow;

        /// <summary>
        /// Textfarbe für Verzeichnispfade.
        /// </summary>
        public static ConsoleColor Path { get; } = ConsoleColor.DarkCyan;

        /// <summary>
        /// Textfarbe für wichtige Informationen.
        /// </summary>
        public static ConsoleColor Info { get; } = ConsoleColor.DarkGreen;

        /// <summary>
        /// Textfarbe für Fehlermeldungen.
        /// </summary>
        public static ConsoleColor Error { get; } = ConsoleColor.DarkRed;

        #endregion Public Properties

        #region Public Methods

        /// <summary>
        /// Ermöglicht Inline-Farbveränderungen für Substrings, welche in der Konsole ausgegeben werden.
        /// <br></br>
        /// Um Substrings zu färben werden diese in [] Klammern gesetzt.
        /// </summary>
        /// <param name="message">Der vollständige String.</param>
        /// <param name="color">Die Farbe für die Substrings.</param>
        public static void WriteLine(string message, ConsoleColor color)
        {
            // Regulärer Ausdruck, um den vollständigen String in Einzelteile zu zerlegen.
            string[] pieces = Regex.Split(message, @"(\[[^\]]*\])");

            /* Für jeden gefundenen Substring der mit [] umklammert ist wird die Textfarbe für diesen geändert
               und in der Konsole ausgegeben, andernfalls wird der Substring ohne Farbänderung ausgegeben. */
            for (int i = 0; i < pieces.Length; i++)
            {
                string piece = pieces[i];

                // Überprüfung, ob der Substring mit [] umklammert ist.
                if (piece.StartsWith("[") && piece.EndsWith("]"))
                {
                    Console.ForegroundColor = color;
                    piece = piece.Substring(1, piece.Length - 2);
                }

                // Der Substring wird in der Konsole ausgegeben.
                Console.Write(piece);

                // Setzt die Textfarbe wieder auf weiß.
                Console.ForegroundColor = ConsoleColor.White;
            }

            // Zeilenumbruch nach Ausgabe des vollständigen Strings.
            Console.WriteLine();
        }

        /// <summary>
        /// Ermöglicht Inline-Farbveränderungen für Substrings, welche in der Konsole ausgegeben werden.
        /// <br></br>
        /// Um Substrings zu färben werden diese in [] Klammern gesetzt.
        /// </summary>
        /// <param name="message">Der vollständige String.</param>
        /// <param name="color">Die Farbe für die Substrings.</param>
        public static void Write(string message, ConsoleColor color)
        {
            // Regulärer Ausdruck, um den vollständigen String in Einzelteile zu zerlegen.
            string[] pieces = Regex.Split(message, @"(\[[^\]]*\])");

            /* Für jeden gefundenen Substring der mit [] umklammert ist wird die Textfarbe für diesen geändert
               und in der Konsole ausgegeben, andernfalls wird der Substring ohne Farbänderung ausgegeben. */
            for (int i = 0; i < pieces.Length; i++)
            {
                string piece = pieces[i];

                // Überprüfung, ob der Substring mit [] umklammert ist.
                if (piece.StartsWith("[") && piece.EndsWith("]"))
                {
                    Console.ForegroundColor = color;
                    piece = piece.Substring(1, piece.Length - 2);
                }

                // Der Substring wird in der Konsole ausgegeben.
                Console.Write(piece);

                // Setzt die Textfarbe wieder auf weiß.
                Console.ForegroundColor = ConsoleColor.White;
            }
        }

        #endregion Public Methods
    }
}