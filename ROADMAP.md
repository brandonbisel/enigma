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
| Indicator procedure | Army version complete, doubled and single, in both front ends |
| Message formatting | Complete — preparation and five-letter groups |
| Key sheets | Complete, in the notation a real key sheet used |
| Enigma D and K | Complete — the commercial pattern, shared wheels, notches apart |
| Swiss K, Railway, Tirpitz | Complete — the same pattern with other wheels |
| Norenigma and KD | Complete — an Enigma I rewired, and a K with a UKW-D |
| Enigma Z30 | Complete — ten contacts, figures, a pawl-driven reflector, a rotor-body notch |
| Front ends | A command line tool and a browser panel, over a shared session layer |
| The panel | Windows, lamps, keyboard, signal path, message, key sheet, plugboard |

528 tests, including four historical messages — Barbarossa, Scharnhorst, U-264 and
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

- **Enigma Z Mk II.** The gear-driven version of the Z30, whose mechanism is the
  `GearDrive` already written for the Zählwerk machines. Its wiring is not
  published — the one simulator that offers the model says so plainly and
  substitutes the Mk I wheels — so the mechanism is there and the wheels wait for
  a source, as the Uhr's dial did.

  The Mk I is done. It needed a character map, a set of wheels and one new drive:
  making the alphabet first-class did carry it, and what it did *not* carry was the
  stepping, because the Z30 has a pawl the service machines do not.

### The naval indicator procedure

`IndicatorProcedure` implements the army method: encipher the message key at the
ground setting and send the ground setting in clear. The Kriegsmarine did not send
it in clear. It disguised the indicator through the
**Doppelbuchstabentauschtafel** — bigram substitution tables — with the key groups
drawn from the **Kenngruppenbuch**. Until those are in, the naval machines are
modelled but the naval *procedure* is not, which is a gap in a simulator that
otherwise takes procedure seriously.

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
