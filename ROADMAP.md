# Roadmap

Where this simulator has got to, what is planned, and what is deliberately left
alone. For how to work in the repository see [AGENTS.md](AGENTS.md); for how to use
it, [README.md](README.md).

## The standard everything here is held to

One rule governs what may be added: **no cipher data ships unless it is pinned to a
published source or a published vector.**

This is not a slogan. A hand-written Uhr table was produced during development,
failed its first test, and was deleted rather than patched into agreement — the
feature waited until published forty-position vectors turned up. The G-31's wheels
were withheld until its stepping rule was confirmed by two independent sources.
When a mechanism is understood but its wirings are not, the right move is to ship
the mechanism and say so.

Everything below inherits that rule. A machine on the "next" list is not done when
it runs; it is done when something published says it runs correctly.

## Where things stand

| Area | State |
|---|---|
| Enigma I, M3 | Complete, with the pawl drive and its double-step anomaly |
| Naval M4 | Complete, including the thin rotors that no pawl can drive |
| Abwehr G-31 (Zählwerk) | Complete: gear drive, no double step, turning reflector |
| Plugboard | Complete, including the Uhr and its dial notation |
| UKW-D | Complete — the reflector rewired in the field |
| Entry wheel | Complete, straight-through and QWERTZ |
| Custom alphabets | Complete — the alphabet is a first-class part of the machine |
| Custom parts files | Complete — rotors, reflectors and alphabets from user files |
| Indicator procedure | Army version in both front ends; the naval one in the library |
| Message formatting | Complete — preparation and five-letter groups |
| Key sheets | Complete, in the notation a real key sheet used |
| Enigma D and K | Complete — the commercial pattern, shared wheels, notches apart |
| Swiss K, Railway, Tirpitz | Complete — the same pattern with other wheels |
| Norenigma and KD | Complete — an Enigma I rewired, and a K with a UKW-D |
| Enigma Z30 | Complete — ten contacts, figures, a pawl-driven reflector, a rotor-body notch |
| Front ends | A command line tool and a browser panel, over a shared session layer |
| The panel | Windows, lamps, keyboard, signal path, message, key sheet, plugboard |

558 tests, including four historical messages — Barbarossa, Scharnhorst, U-264 and
the 1930 instruction manual — each verified against its published ciphertext, and a
differential suite comparing against an independently written oracle.

The web front end is complete. It is a Blazor WebAssembly page with the verified
library running unchanged in the browser: no server, so nothing typed into it is
transmitted anywhere.

## Next

### More machines

`IMachineLayout` and `IStepping` were introduced for the G-31, and they reduced a
new model to *parts plus a layout*. That makes the remaining machines mostly a
matter of sourcing wirings rather than writing mechanism:

- **Enigma Z Mk II** — *not planned, and probably not possible.* The gear-driven
  version of the Z30 may never have been built: "it is uncertain whether this
  machine was actually built", though a brochure photograph shows a serial number in
  the range its production would have used. Only one Z30 of any mark survives, the
  Mk I Z103 held by the FRA in Sweden. No wiring is published, and there is no
  machine to recover one from, so this is listed to record that it was considered
  rather than as work waiting to be done.

  Its mechanism is nonetheless here already, and tested: `GearDrive` has the period
  of 10,000 that the 1931 brochure advertised, which the Mk I's pawls cannot give.

  The Mk I is done. It needed a character map, a set of wheels and one new drive:
  making the alphabet first-class did carry it, and what it did *not* carry was the
  stepping, because the Z30 has a pawl the service machines do not.

### The naval indicator procedure

The procedure itself is done: `NavalIndicatorProcedure` and `BigramTable` carry the
Kenngruppenbuch trigrams, the fillers, the column pairing and the
Doppelbuchstabentauschtafel, pinned by the published worked example.

It is pinned by a real message: U-534's P1030690 of 1 May 1945, read from its
transmitted indicator through to the message key its operator wrote down.

**The source has been found and checked.** The Crypto Museum publishes the whole
"Quelle" booklet — *Doppelbuchstabentauschtafeln für Kenngruppen*, serial 2499 — as
a scan: a cover, the Tauschtafelplan, and nine tables lettered A to H and J, each
across two pages of thirteen columns by twenty-six rows. It is the same set U-534
was using. All four of the entries that message needed read off the scan exactly:

| | from the message | from the booklet |
|---|---|---|
| `FN` | `KY` | `KY` |
| `HC` | `DM` | `DM` |
| `GV` | `UU` | `UU` |
| `ET` | `ZZ` | `ZZ` |

**Tafels A and B are transcribed and shipped.** All 676 entries, every mirror agreeing, no
bigram enciphering to itself, and the four cells known from real traffic correct. One
cell needed adjudicating — a speck under an O in `DS` reads as a Q's tail — and three
other entries settled it. U-534's message now runs end to end through the real table.

Two things remain. **The other seven tables of the set** are still only scans; OCR
manages perhaps three quarters and confuses `G` with `C`, `U` with `L`, and the dotted
`İ` the typeface uses, so they have to be read by eye as Tafel A was. And the
procedure is **library only**: neither front end offers it yet, where the army one is
in both.

## Future

**Cryptanalysis** — bombe simulation, index-of-coincidence rotor search, and
ciphertext-only attack. This is deliberately placed after the simulator is
complete rather than alongside it: an attack is only worth writing against a
machine already known to be right, and a wrong machine would make a broken attack
look successful.

## Not scheduled

- **Packaging and continuous integration.** The tool builds and installs; nothing
  publishes it to NuGet and no CI runs the suite. Worth doing, not yet planned.
- **Mechanical detail with no effect on the cipher.** The G-31's hand crank and
  backward stepping, its four-digit letter counter, the 52-tooth cogwheel. These
  are properties of the object rather than of the cipher, and a simulator that
  reproduced them would not encipher a single letter differently.
