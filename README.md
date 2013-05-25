dff.Extensions
================================
Nützliche Snippets für das täglich Programmierer-Brot
-----------------------------------------------------

Kein Bock mehr auf sowas hier:

```c#
if (lieferauftraege.EndsWith(", "))
     lieferauftraege = lieferauftraege.Substring(0, lieferauftraege.Length - 2);
```
 
Dann nimm einfach:

```c#
lieferauftraege = lieferauftraege.RemoveLastSeperator();
```