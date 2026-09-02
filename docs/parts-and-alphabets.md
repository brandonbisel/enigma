# Parts and alphabets

Everything the machine is assembled from — the stator, the wheels, the reflector and
the alphabet they are all wired in — can come from a file rather than from the
library. `--parts` on the command line reads one; the panel picks its lists out of the
same catalogue, so a wheel defined in a file appears there without the page knowing it
exists.

## The entry wheel

The entry wheel (*Eintrittswalze*) is the fixed stator between the plugboard and
the first rotor, deciding which contact each key is wired to. Every service Enigma
wired it straight through in alphabet order, so it changes nothing and needs no
mention. The commercial and railway machines wired it in **keyboard** order
instead, which is why their traffic could not be read on a service machine even
with identical wheels.

Name it on the key sheet with `"EntryWheel": "QWERTZ"`, or define your own in a
parts file. A definition gives the keyboard in the order its keys are wired to
contacts, which is the form the wiring is published in:

```json
{ "EntryWheels": [ { "Name": "Railway", "Keyboard": "QWERTZUIOASDFGHJKPYXCVBNML" } ] }
```

So `Q` is wired to the first contact, `W` to the second, and a wheel reading
`ABCDEFGHIJKLMNOPQRSTUVWXYZ` is wired straight through.

## Custom alphabets

Every service Enigma worked in the twenty six capital letters, but nothing in the
mechanism requires that. A parts file can define an alphabet, and the machine is
then built entirely in it — the wiring, the notches, the ring settings, the
plugboard pairs and the message itself:

```json
{
  "CharacterMaps": [ { "Name": "Digits", "Characters": "0123456789" } ],
  "Rotors": [
    { "Name": "D-I",   "Wiring": "1357902468", "Notches": "4" },
    { "Name": "D-II",  "Wiring": "2468013579", "Notches": "2" },
    { "Name": "D-III", "Wiring": "3052749618", "Notches": "7" }
  ],
  "Reflectors": [ { "Name": "D-UKW", "Wiring": "5678901234" } ]
}
```

Name it from the key sheet with `"CharacterMap": "Digits"`. Parts in the file take
that alphabet automatically when the file defines exactly one; otherwise each names
its own.

The alphabet is the **single authority** for how many contacts a machine has. A
wheel wired for a different one is refused by name rather than failing later with
an index error:

```
'I' has 26 contacts, but the Digits alphabet has 10 characters.
```

Ring settings and positions are numbered within the alphabet, so a ten character
machine numbers its wheels 01 to 10. A reflector needs an even number of
characters, since it wires them in pairs.

Two things stay Latin. `--prepare` expands German umlauts and spells out German
numerals, which only means anything in that alphabet. And the `QWERTZ` entry wheel
is a fixed twenty six key layout; the straight-through one exists for any alphabet,
being the identity.

## Custom rotors and reflectors

A parts file defines wheels and reflectors the library does not ship with, which
is enough to run machines it knows nothing about:

```json
{
  "Rotors": [
    { "Name": "K-I",  "Wiring": "PEZUOHXSCVFMTBGLRINQJWAYDK", "Notches": "Y" },
    { "Name": "K-II", "Wiring": "ZOUESYDKFWPCIQXHMVBLGNJRAT", "Notches": "E" }
  ],
  "Reflectors": [
    { "Name": "UKW-K", "Wiring": "IMETCGFRAYSQBZXWLHKDVUPOJN" }
  ]
}
```

`Notches` lists the letters showing in the window when the wheel turns the one to
its left. Add `"Thin": true` for a half-width wheel, which then takes no notches.
Definitions are checked when the file is read, so a wiring that is not a
permutation, or a reflector that is not paired, is reported before any enciphering
starts.

The file is laid over the built-in parts rather than replacing them, so it need
only define what is missing. A definition whose name matches a built-in part
**takes its place**, which is what makes it possible to model a machine whose
wheels happen to share our names — the substitution is reported as a warning,
because ciphertext produced that way cannot be reproduced without the same file.

## See also

- [The machines](machines.md) for the models these parts are fitted to, and for the
  Z30, which is the library's own machine in an alphabet of ten figures.
- [Key sheets](key-sheets.md) for naming a part once it is defined.
