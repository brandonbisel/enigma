# Sources

The wirings, key sheets and test vectors here are taken from published sources
rather than reconstructed, and each is pinned by a test.

## The standard everything here is held to

One rule governs what may be added: **no cipher data ships unless it is pinned to a
published source or a published vector.**

This is not a slogan. A hand-written Uhr table was produced during development, failed
its first test, and was deleted rather than patched into agreement — the feature waited
until published forty-position vectors turned up. The G-31's wheels were withheld until
its stepping rule was confirmed by two independent sources. When a mechanism is
understood but its wirings are not, the right move is to ship the mechanism and say so.

It applies to a plan exactly as it applies to a commit, and to a table of language
statistics exactly as it applies to a wiring. Everything below earned its place under
it, and nothing joins the list without one.

## The plugboard and the Uhr

- **[Crypto Museum](https://www.cryptomuseum.com/crypto/enigma/uhr/index.htm)** —
  the Enigma Uhr, including the photograph of the plates inside the lid from which
  the two letter setting notation is read, and the cable ordering `1a-1b` through
  `10a-10b`.
- **[John Savard, quadibloc](http://www.quadibloc.com/crypto/ro020402.htm)** — the
  Uhr's construction: eighty contacts in two rings, the `a` wires in order and the
  `b` wires scrambled, and the reason every fourth position is reciprocal. Savard
  credits Frode Weierud for the facts behind it, and cites a paper in *Cryptologia*,
  July 1999, for the authoritative account.
- **[Arduino Enigma](https://arduinoenigma.blogspot.com/2020/03/enigma-uhr-switch-test-vectors.html)**
  — test vectors for all forty dial positions, generated with Daniel Palloks'
  Universal Enigma and partly hand verified. The Uhr table in this repository is
  derived from those vectors, and every one of them is a test. A
  [companion post](https://arduinoenigma.blogspot.com/2020/05/designing-uhr-switch-for-arduino-based.html)
  gives an independent vector with a different plug set, which the same table
  reproduces.

## The machines

- **[Palloks, *Universal Enigma*](https://palloks.2ix.de/enigma/index_en.html)** —
  the geared drive. Its `engage_gear` gives the carry chain the Zählwerk machines
  use and where it ends, and its `etq` table independently confirms the direction of
  the keyboard-wired entry wheel used here.
- **[Crypto Museum, *Enigma D*](https://www.cryptomuseum.com/crypto/enigma/d/index.htm)**
  and **[*Enigma K*](https://www.cryptomuseum.com/crypto/enigma/k/index.htm)** — the
  commercial wirings the D, the K and the A28/G31 share, their notch positions, and
  the difference that matters between them: the D's "notch ring is attached to the
  body of the rotor (rather than to the letter ring)", while the K's is "attached to
  the letter ring rather than rotor body".
- **Crypto Museum, [*Swiss K*](https://www.cryptomuseum.com/crypto/enigma/k/swiss.htm),
  [*Railway Enigma*](https://www.cryptomuseum.com/crypto/enigma/k/railway.htm) and
  [*Enigma T*](https://www.cryptomuseum.com/crypto/enigma/t/index.htm)** — their
  wheels, notches, reflectors and, for the T, its own entry wheel. The Railway page
  also records that its two published wirings differ by a misidentification, "with
  turnover positions of rotors I and III swapped" in Bletchley Park's version, and
  that Friedman's report of a moving reflector on that machine was mistaken.
- **Palloks, *Universal Enigma*, model tables** — the Norenigma's five wheels and
  reflector, the KD's nine-notch wheels, and the UKW-D wiring of the KD machine
  held by the FRA in Sweden, which is the one the packaged key sheet carries.
- **Reuvers & Simons, *Enigma G-111: A rare version of Zählwerk Enigma G31***
  (Crypto Museum, 2013) — the mechanism in the machine's own terms: cogwheels rather
  than pawls and levers, no double stepping anomaly, notches attached to the letter
  ring, and the statement that the UKW "can be moved by wheel 3", which is what puts
  the reflector at the end of the carry chain.
- **[Crypto Museum, *Enigma Z*](https://www.cryptomuseum.com/crypto/enigma/z/index.htm)**
  — the Z30's wheel and reflector wirings, indexed 1 to 0, and where its notch is
  cut: "The notch is attached to the rotor body, which means that altering the
  Ringstellung does not alter its position with respect to the wiring... different
  from the rotors of later machines like Enigma K and Enigma I where the notch is
  attached to the index ring."
- **Arturo Quirantes, *Model Z: a numbers-only Enigma version*** — the paper that
  found the machine in the Spanish Foreign Affairs Ministry archives (reference
  007459-4-R), with the 1931 offer, the two-part key, and the brochure's claim of "a
  period of 10,000... thereby suggesting a non-Enigma (odometer) stepping". Its
  worked example, 25183 91467 enciphering to 38760 15924, cannot be used as a vector
  here: as the paper says, no machine settings are given with it.
- **The Z30 instruction manual (Spanish) and brochure (German)**, both from those
  archives — the decade-counter description of the drive, and the key count of
  1,200,000,000 with six cylinders fitted three at a time, which is exactly 120 wheel
  orders times ten thousand window positions times a thousand ring settings.
- **[Palloks, *Enigma Z*](https://palloks.2ix.de/enigma/index_en.html)** — the same
  wirings indexed 0 to 9, which agree exactly once converted; the pawl chain that
  reaches the reflector; and the vectors the Z30 tests are built on, taken from it
  before any of this was written.

## The naval indicator procedure and its tables

- **[Dirk Rijmenants, *Enigma Procedures*](https://ciphermachinesandcryptology.com/en/enigmaproc.htm)**
  — the Kriegsmarine indicator procedure step by step, and the worked example the
  tests reproduce: groups HLG and KQK with fillers A and Z becoming BDBJ EMEJ under
  bigram table B. Four entries of that table are published with it, and those four
  are all this library claims to know.
- **[Michael Hörenberg, *The Kenngruppen System*](https://enigma.hoerenberg.com/index.php?cat=The+U534+messages&page=The+Kenngruppen+System)**
  — message P1030690 from U-534 worked right through: the transmitted indicator, the
  four entries of "Quelle" Tafel A it needs, the day's key, and the message key
  `ODFF` that comes out. The naval procedure here is pinned by that, and it is what
  showed the M4's fourth wheel is set by the filler.
- **[Crypto Museum, *Bigram tables*](https://www.cryptomuseum.com/crypto/codebook/bigram.htm)**
  — what the tables were and how they were used. The recovered ones are published
  there as photographs of the originals rather than as data, which is what the
  transcription pipeline in `tools/transcribe` exists for: "Quelle", "Meer" and
  "Flußlauf" are read off those scans.

## The intercepts

- **[Michael Hörenberg, *Graf Spee* (the Norrköping intercepts)](https://enigma.hoerenberg.com/index.php?cat=Norrk%C3%B6ping%20messages&page=PAGE_69_OWLS%20Graf%20Spee)**
  — a signal from the Seekriegsleitung to the Admiral Graf Spee on 12 December 1939,
  taken by the Swedish signals station at Norrköping and photographed from its
  archive: the settings, 272 letters of ciphertext and the plaintext. It is cabled
  with eight plugboard pairs rather than ten, which is what the Kriegsmarine was
  doing in 1939. Its setting is given two ways, rings AHX at EKD and rings AUX at
  EXD, and that is one machine rather than two readings of a smudged sheet: ring and
  position are shifted together by thirteen on the middle wheel, and that wheel is
  VI, whose two notches are themselves thirteen apart, so the shift maps the notch
  set onto itself and nothing the machine does can tell the settings apart.
- **[Michael Hörenberg, *P1030681, the Karl Dönitz message*](https://enigma.hoerenberg.com/index.php?cat=The%20U534%20messages&page=P1030681)**
  — the signal announcing that Dönitz had been named Hitler's successor, photographed
  among U-534's papers: the settings, 372 letters of ciphertext and the plaintext
  with its garbles. It is the only message here on the thin UKW-C, and so the only
  thing that pins that reflector's wiring to traffic rather than to a table. The page
  gives the settings twice, as rings EPEL at CDSZ and as AAEL at YOSZ; the second is
  the same machine written another way, and a test says so.
- **[Dan Girard, *Solution of the last of the "H.M.S. Hurricane" intercepts*](https://enigma.hoerenberg.com/index.php?cat=M4%20Project%202006&page=Rasch%20Message)**
  (with Michael Hörenberg) — the break of the first of the three signals Ralph
  Erskine published in *Cryptologia* in 1996 as a challenge, unbroken for twenty
  years after the M4 Project took the other two. It gives the settings, the corrected
  ciphertext and the plaintext, which is the whole of what the `rasch` key sheet and
  its test need. A second Ringstellung and Grundstellung are published with it; they
  read this message identically and are not the same machine.

The historical messages — Barbarossa, Scharnhorst, Graf Spee, U-264, U-106, the
Dönitz succession signal and the 1930 instruction manual — are published intercepts,
and each is decrypted in the test suite with its own key sheet.

## Cryptanalysis

- **James J. Gillogly, "Ciphertext-Only Cryptanalysis of Enigma"**, *Cryptologia*
  19(4), 1995 — the decomposition `RotorSearch` follows: sweep the wheel order and
  the starting positions with the board empty and score by the index of coincidence,
  then place the turnovers by sweeping the ring settings. What is here is that method
  and the measure it starts from, not the plugboard hill-climb it goes on to, because
  that needs statistics this repository has no source for.

