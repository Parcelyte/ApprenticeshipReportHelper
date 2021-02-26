# ApprenticeshipReportHelper

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
	  insofern das Format unterstützt wird. Der Pfad darf nicht länger als 255 Zeichen sein.

Derzeit werden folgende Datei-Formate unterstützt:
  - .doc, .docx, .pdf, .txt, .ppt, .pptx, .htm, .html, .xls, .xslx
  - Mehrere Dateien sind möglich.
_________________________________________________________________________________________________________________________________________________

Informationen zur Version 1.0:

- Erstellung von Verzeichnissen. ((Halb-)Jahre und Wochen)
- Kopieren und Umbenennen von Vorlage-Dokument-Dateien. (Diese müssen sich im template-Verzeichnis befinden und das Format muss unterstützt werden)
- Ausgabe von Informationen wie etwa die Anzahl der erstellten Verzeichnisse.
- Überprüfung des Starttags der Ausbildung, falls die Ausbildung mitten in der Woche beginnt.
- Das Programm geht davon aus, dass jede Woche (bis auf die Startwoche) mit einem Montag beginnt.
- Sollte sich eine Woche zweier Ausbildungsjahre überlappen wird dies erkannt.
- Nutzereingaben werden validiert um falsche Eingaben zu verhindern.
