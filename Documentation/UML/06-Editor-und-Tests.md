# Editor-Werkzeug und Testklasse

Stand: 16. November 2026. Getrennt vom Spielablauf: statische Projektordner-Färbung und der bestehende TwoPlusTwo-Test. Die Testklasse prüft keine Gameplay-Funktion.

[Legende und Einordnung](README.md) · [SVG öffnen](06-Editor-und-Tests.svg) · [Offline-Ansicht](UML.html)

```mermaid
classDiagram
direction LR
class ProjectFolderColors {
  <<static>>
  -Dictionary ColorCache$
  -KeyValuePair[] Rules$
  -ProjectFolderColors()$
  -Rule(string path, Color color) KeyValuePair$
  -ClearCache() void$
  -GetFolderColor(string guid) Color?$
  -DrawFolder(string guid, Rect selectionRect) void$
}
class EditorApplication {
  <<external>>
}
class NewTestScript {
  +TwoPlusTwo() void
}
class Assert {
  <<external>>
}
ProjectFolderColors ..> EditorApplication : abonniert Editor-Callbacks
NewTestScript ..> Assert : AreEqual
```
