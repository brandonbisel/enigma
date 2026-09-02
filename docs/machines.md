# The machines

Every model this library builds, what separates it from the others, and how to name it
on a key sheet. A model is *parts plus a layout*: `IMachineLayout` says what drive it
runs, what stator it takes, whether it has a plugboard at all, and which arrangements
of parts could actually have been assembled. A key sheet names one with `Model`.

| Model | `Model` | Wheels | Drive | Plugboard |
|---|---|---|---|---|
| Enigma I, M3 | *(default)* | `I`–`V`, plus `VI`–`VIII` for the Navy | pawls, double stepping | yes |
| Naval M4 | *(default)* | three plus `Beta` or `Gamma`, thin reflector | pawls; the thin wheel never turns | yes |
| Norenigma | *(default)* | `N-I`–`N-V`, reflector `N` | pawls, double stepping | yes |
| Enigma D, K | `Commercial` | `K-I`–`K-III`, reflector `G` | pawls, double stepping | no |
| Swiss K | `Commercial` | `SK-I`–`SK-III`, reflector `G` | pawls, double stepping | no |
| Railway | `Commercial` | `R-I`–`R-III`, reflector `R` | pawls, double stepping | no |
| Tirpitz | `Commercial` | `T-I`–`T-VIII`, reflector `T` | pawls, double stepping | no |
| Enigma KD | `KD` | `KD-I`–`KD-III`, a rewired UKW-D | pawls, double stepping | no |
| Zählwerk G-31 | `G-31` | `G-I`–`G-III` or `G312-I`–`G312-III` | cogwheels, no double step | no |
| Enigma Z30 | `Z30` | `Z-I`–`Z-III`, ten contacts | pawls, the reflector driven | no |

The service machines are the default and need no `Model`. What each of the others
needs is below. The short version is in [the README](../README.md#the-machines).

## The service machines

The Enigma I and the M3 are the same machine as far as the cipher goes — three wheels
driven by pawls, a straight-through stator, a reflector that does not move, and the
plugboard the Army reserved to itself. The Navy's difference is its extra wheels,
`VI` to `VIII`, which carry two notches apiece rather than one.

The M4 is that machine with a thin reflector fitted, which frees the width for a
fourth wheel. `Beta` and `Gamma` have no ratchet and no notch, so the fourth wheel
never steps and never drives its neighbour: it is set once and stays there for the
whole message. That pairing is the only one that fits, and
[key sheets](key-sheets.md#the-four-rotor-naval-m4) covers what the factory will and
will not assemble.

The **Norenigma** is a service machine too, and so needs no `Model` named either;
[below](#two-machines-that-are-other-machines-rewired) covers what was changed in it.

## The Zählwerk Enigma (G-31)

The Abwehr's Enigma G is the one machine here that differs in *mechanism* rather
than in wiring. Name it as the model:

```json
{
  "Model": "G-31",
  "Reflector": "G",
  "Rotors": "G-I G-II G-III",
  "RingSettings": "AAA",
  "Positions": "AAA",
  "ReflectorPosition": "A",
  "ReflectorRingSetting": "A"
}
```

Four things set it apart:

- **Cogwheels rather than pawls.** The wheels turn as a plain odometer, so there is
  **no double step** — the middle wheel advances only when the wheel to its right
  passes a notch, never twice in succession.
- **The reflector turns.** It is set to a starting position like a wheel, and is
  driven round by the leftmost wheel during encipherment, so it takes an active part
  in the cipher.
- **Many notches.** Seventeen, fifteen and eleven on wheels I, II and III, numbers
  chosen because they are relatively prime to twenty six, which stretches the
  machine's period enormously.
- **No plugboard.** The Steckerbrett was reserved for the Army, and a key sheet that
  gives cables to a G-31 is refused.

The entry wheel is the keyboard-wired one, and a key sheet need not say so: leave
`EntryWheel` unset and the model supplies it. The commercial A28/G31 wheels are
`G-I`…`G-III` with reflector `G`; the Abwehr set from the Bletchley Park machine is
`G312-I`…`G312-III` with reflector `G312`.

Because of all this a Zählwerk machine cannot exchange messages with an Enigma I,
which is exactly what its makers intended.

## The commercial machines: Enigma D and K

The Enigma D of 1926 and the Enigma K that followed it were sold to whoever would
buy them, and are the ancestors of everything else here. Three wheels driven by
pawls, a keyboard-order entry wheel, a reflector set to a position but never driven,
and no plugboard — the Steckerbrett was an Army fitting.

```json
{
  "Model": "Commercial",
  "Reflector": "G",
  "Rotors": "K-III K-II K-I",
  "RingSettings": "AAA",
  "Positions": "AAA",
  "ReflectorPosition": "A"
}
```

The D, the K and the Zählwerk A28/G31 were fitted with **the same three wheel
wirings and the same reflector**, which is why they share them here too. What
separates them is entirely in the notches:

| | Notches | Cut into | Driven by |
|---|---|---|---|
| Enigma D | Z, Z, Z | the rotor body | pawls, double stepping |
| Enigma K | Y, E, N | the letter ring | pawls, double stepping |
| A28 / G31 | 17, 15, 11 | the letter ring | cogwheels, no double step |

The D's notch placement has a consequence its makers may not have intended: since
the notch keeps its place against the wiring, moving the ring moves the turnover
with it, and a Ringstellung is only ever a different starting position. As the
Crypto Museum puts it, "the cryptographic effect of the Ringstellung is null. It
does not enhance the machine's key space." A test proves it by setting ring and
position together and getting the same ciphertext back.

### The same pattern, other wheels

Three more machines are the commercial arrangement with different parts, and need
no new mechanism at all:

| | Wheels | Reflector | Stator | Notes |
|---|---|---|---|---|
| Swiss K | `SK-I`…`SK-III` | `G` | QWERTZ | the Swiss rewired the wheels and left the rest |
| Railway | `R-I`…`R-III` | `R` | QWERTZ | the wiring found in machine K438 |
| Tirpitz | `T-I`…`T-VIII` | `T` | its own | eight wheels, five notches each |

The Railway Enigma has two published wirings: Bletchley Park's wartime
reconstruction and the original, recovered from machine K438 in 2023. They are
equivalent up to ring settings, but BP's has the turnovers of wheels I and III
swapped through a misidentification. The original is what ships.

The Enigma T is the one machine here whose entry wheel is neither straight through
nor the keyboard order, and forgetting it would encipher perfectly well and
wrongly — so its model supplies it, and a key sheet need not say so. Its eight
wheels carry five notches apiece, which is what stretches its period.

### Two machines that are other machines rewired

The **Norenigma** is an Enigma I. The Norwegian police security service kept theirs
after the war and gave it new wheels (`N-I`…`N-V`) and a new reflector (`N`),
leaving the plugboard, the straight-through stator and the pawls exactly as they
were — so it is a service machine and needs no model named at all. Its notches sit
where the service wheels of the same numbers had them, wheel I still carrying at Q.

The **Enigma KD** is a commercial K with a rewirable UKW-D in place of its
reflector. That is the one thing that separates it: a UKW-D has no position to set,
so it needs its own model, `KD`, which is the commercial one with that requirement
relaxed. Its three wheels carry nine notches apiece.

```json
{
  "Model": "KD",
  "Reflector": "D",
  "ReflectorPairs": "AK BO CT DV EP FN GL HM IJ QW RY SX UZ",
  "Rotors": "KD-III KD-II KD-I"
}
```

Those pairs are the wiring of the KD machine held by the FRA in Sweden, written as
the plain letter pairs this library takes rather than in the printed UKW-D notation,
which uses the wheel's own contact lettering and is not applied here.

## The Enigma Z, a machine of figures

The Z30 has no letters at all: ten contacts per wheel, a keyboard of a single row
of figures, and no plugboard. It was built for traffic that was numeric to begin
with, such as weather reports.

```json
{
  "Model": "Z30",
  "CharacterMap": "Digits",
  "Reflector": "Z",
  "Rotors": "Z-III Z-II Z-I",
  "RingSettings": "000",
  "Positions": "000",
  "ReflectorPosition": "0"
}
```

Its reflector is driven, as the Zählwerk machines' is, but by pawls rather than
cogs: there is one pawl more than there are wheels, and the extra one rides the
leftmost wheel's notch ring. That gives the leftmost wheel a double step of its
own, for the same reason the middle wheel has one on an Enigma I — and it is
exactly the pawl a service machine does not have.

The wheel wirings are published by the Crypto Museum indexed 1 to 0 and by Daniel
Palloks' simulator indexed 0 to 9; converting between the two makes them identical.
The vectors in the test suite come from that simulator.

The machine reached the Spanish Foreign Ministry in 1931 as a 600-Reichsmark offer
alongside the commercial A27 and the printing H29, and the papers that survive there
are most of what is known about it. Its key had two parts and no plugboard: an
*inner* key naming the wheels and their order, and an *outer* key of four figures —
three wheels and the reflector — written like `III I II 5 2 8 1`.

Those papers also describe a machine that counts. The wheels and the reflector are
"coupled to each other in the manner of a normal decade counter", so the windows
tell the operator how many figures have been enciphered, and the brochure claims a
period of 10,000: every wheel back where it started after ten thousand keystrokes.

That is an odometer, and it is a claim worth testing. This library's `GearDrive`
has a period of exactly 10,000 on four ten-position wheels. The pawl drive of the
Mk I shipped here never returns to its starting window at all — a double step makes
the stepping non-injective, so some windows can never be reached and all zeros is
one of them. So the brochure describes the geared Mk II rather than this machine,
and both tests are in the suite.

Whether that Mk II was ever built is itself uncertain, and only the Mk I survives —
one machine, Z103, held by the FRA in Sweden. Its wheels are what this library
carries; the geared drive is here because the Zählwerk machines need it anyway.

Its notch sits somewhere unusual, and it matters. On an Enigma I or K the notch is
cut into the index ring, so a wheel carries its neighbour at a fixed letter in the
window whatever the Ringstellung — rotor I always at Q. On the Z30, as on the older
Enigma D, it is cut into the rotor body instead, so it keeps its place against the
wiring and setting the ring carries the turnover with it. `RotorBase` models both,
and every wheel is of the first kind unless it says otherwise.

That is not a detail one can reason out from first principles: the same corpus that
settles it for the service machines says nothing about this one. It is settled here
because the Crypto Museum states it outright, and because the simulator's vectors
with ring settings only match once the notch is in the right place.

## See also

- [Key sheets](key-sheets.md) for the rest of the file these fields sit in.
- [Parts and alphabets](parts-and-alphabets.md) for the entry wheel, the character
  map a ten-contact machine needs, and defining wheels of your own.
- [The plugboard, the Uhr and UKW-D](plugboard.md) for the field-rewired reflector the
  KD takes.
- [Sources](sources.md) for where every wiring and notch here comes from.
