# AGENTS.md

Guidance for automated coding agents working in this repository. See `README.md`
for what the project is and how to use it, and `ROADMAP.md` for what is planned,
what is a future goal, and what is deliberately not scheduled.

## Commands

```bash
dotnet build              # whole solution
dotnet test               # whole suite, currently 440 tests
dotnet test --filter FullyQualifiedName~RotorTests
echo "AAAAA" | dotnet run --project Enigma.Cmd
dotnet run --project Enigma.Web    # the panel, on http://localhost:5298
dotnet run --project Enigma.Cmd -- --init-key-sheet my-machine.json
dotnet run --project Enigma.Cmd -- --preset barbarossa
```

There is no linter or formatter configured. Match the surrounding style.

## Layout

```
Enigma/                 The library
  Abstractions/         Interfaces and abstract bases, namespace Enigma
  Models/               Settings objects, namespace Enigma.Models
  Rotors/               RotorI..RotorVIII plus the thin Beta and Gamma
  Reflectors/           ReflectorA/B/C plus the thin B and C of the M4
  Extensions/           DI registration
Enigma.App/             Orchestration shared by the front ends, namespace Enigma.App
Enigma.Cmd/             Console app, packaged as a tool named enigma
Enigma.Web/             Blazor WebAssembly front end
  Components/           The panel: windows, lamps, keyboard
Enigma.Tests/           xUnit tests
  Reference/            An independent implementation used as a test oracle
Enigma.Web.Tests/       Component tests, on bUnit
```

Folders do not always map to namespaces: `Abstractions/` is `namespace Enigma`,
while `Models/`, `Rotors/` and `Reflectors/` carry their folder name. Follow
whichever the neighbouring files use.

## Conventions

- One public type per file, file named for the type.
- Comments are sparse and explain *why*, not *what*. Do not add narration.
- Concrete rotors and reflectors are data, not logic: three overrides over a static
  wiring table. Keep them that way.
- The bases declare their interfaces (`RotorBase : IRotor`), so the compiler checks
  the contract where the logic lives rather than at each concrete class.
- Methods on the bases are deliberately non-virtual, with one exception:
  `RotorBase.Step` is virtual so `ThinRotorBase` can refuse to turn. Variation
  otherwise belongs in the abstract data members, not in overridden behaviour.
- A machine knows its own layout (`IEnigmaMachine.Layout`), which is how a caller
  asks whether it has a plugboard at all. The drive comes from the layout rather
  than being passed beside it, so the two cannot disagree.
- The panel is a view of a machine and holds no cipher logic of its own. What it
  shows must come from the session; anything it works out for itself can disagree
  with the machine, which is the one thing a simulator's UI must never do. The
  message lives on the page for the same reason: the keyboard and the plaintext
  pane are two ways of entering one message, so neither owns it.
- A string-typed component parameter takes its attribute *literally*:
  `Plaintext="_message"` passes the text `_message`. Write `Plaintext="@_message"`.
  Parameters of any other type are expressions already.
- `Enigma.App` is UI-agnostic and must stay so: no `Console`, no `FileInfo`, no
  `System.CommandLine`. It works in strings and streams, so a front end without a
  filesystem can use it. Anything a front end alone needs belongs in that front end.
- The signal path is described once, as a `TranslationTrace`. The debug log line is
  *formatted from* that structure rather than assembled beside it, so anything that
  wants to watch the machine — a log, a display — sees the same thing. Do not grow
  a second path description.

## Key sheets

Settings are expressed as a **key sheet** (`Enigma.Models.KeySheet`), in the
notation a real one used: rotor order, Ringstellung, Grundstellung, Steckerver-
bindungen. That is the only representation — there is deliberately no second,
index-based settings format, because it expressed nothing extra and invited
zero-based mistakes. `KeySheet` holds the written strings and parses them on
demand through `Wheels()`, `Cables()` and `ReflectorName()`; `RotorPlacement` is
the parsed per-rotor result.

`KeySheets.All` holds the packaged historical sheets. Every one of them is pinned
by a test that decrypts the real intercept it came from, so do not add a key sheet
without a verified message to prove it, and do not adjust one to make a test pass.

## Parts and procedure

`IPartsCatalogue` is where rotors and reflectors come from. `BuiltInPartsCatalogue`
resolves the keyed registrations; `PartsCatalogue` lays a `PartsFile` over it and
reports any built-in name a definition replaces. The factory goes through the
catalogue and never resolves wheels from the container itself.

`ICharacterMap` is the single authority for how many contacts a machine has. Rotors
report `Contacts`, and `EnigmaMachineFactory` refuses a part whose count differs
from the alphabet, so a mismatch is named rather than surfacing as an index error.
Anything that parses a key sheet or a wiring takes the alphabet: do not reintroduce
`- 'A'` or a literal 26. `MessageText` and the QWERTZ entry wheel are deliberate
exceptions, being German and a fixed keyboard respectively.

`IPlugBoard` has both `Translate` and `TranslateReverse`. A cabled board is its
own inverse so the two agree, but `SubstitutionPlugBoard` takes any permutation and
they differ. Do not "simplify" the return leg back to `Translate`: reciprocity
needs the *reflector* to be paired, never the plugboard, and collapsing the two
would silently break any non-paired board.

`RewirableReflector` is UKW-D: thirteen pairs given on the key sheet rather than a
part looked up by name. Its pairs are plain alphabet letters; the historical
contact lettering is deliberately not applied, because there is no verified source
for it here. `EnigmaUhr` carries the forty position dial. Its table was derived from published
vectors for all forty positions, not reconstructed, and every vector is a test.
Three properties hold and are asserted rather than assumed: position 00 reproduces
plain cables, an `a` plug always reaches a `b` plug in every position, and exactly
every fourth position is reciprocal, which was the flaw in the device. Do not
regenerate the table from memory — see the Sources section of the README. An
earlier hand-written attempt was deleted for failing the position 00 test. `UhrSetting` covers only the two letter notation that carried a
dial setting, which the plates inside the Uhr's lid document in full; the plates
are stamped letters, so it is Latin by construction and does not follow a machine's
character map. Do not add either from memory; they need a reference, the way the
rotor wirings were pinned by real intercepts.

`IEntryWheel` is the stator between the plugboard and the first rotor. It is
described by its **keyboard order**: the letter at position i is the key wired to
contact i, which is the form the wiring is published in. Every machine the library
ships with uses the straight-through wheel, so it is exercised mainly through
`--parts` and the QWERTZ one; do not "simplify" it away on the grounds that it is
the identity.

`MessageText` holds the signalling conventions rather than any cryptography:
preparing text for a keyboard with no space bar and no digits, and grouping the
output. It deliberately does not rewrite "ch" to Q — that was a habit, not a rule,
and it would silently alter words.

`IIndicatorProcedure` is the operating procedure rather than the machine: it
enciphers a message key at a ground setting to give the indicator, and recovers it
again. A six letter indicator is the pre-1938 doubled form, and its halves must
agree. Note that the halves differ from each other — the rotors move between them.

## Machines and drives

`IMachineLayout` is the model: its drive, its default entry wheel, whether it has a
plugboard, and which arrangements of parts could be assembled. `ServiceLayout`
covers the Enigma I, M3 and M4; `GearLayout` the Zählwerk G-31. A key sheet names
one with `Model`. Fitment rules belong here and not in the factory, which keeps only
the check that every part has as many contacts as the alphabet.

`IStepping` is the drive. `PawlDrive` has the double step; `GearDrive` is a plain
odometer carry that ends at the reflector, because on those machines the reflector
turns too. Both the reference simulator and the Crypto Museum G-111 paper were used
to pin that, and the G output is checked against an independent implementation — do
not adjust the drive without re-running that comparison.

Note that `KeySheet.EntryWheel` defaults to empty rather than to "Standard", so the
model can supply it. Setting a literal default there silently gave the G-31 a
straight-through stator, which the differential test caught.

## Domain rules that are easy to get wrong

- **Modular arithmetic.** C#'s `%` is a remainder and keeps the sign of its left
  operand, so a bare `x % 26` yields negatives. Everything goes through the
  positive-modulo helper on `RotorBase`. This has been a real bug here.
- **Turnover position** is the letter *showing in the window* when the next
  keypress carries the rotor to its left, so `IsTurnoverPosition()` is checked
  **before** `Step()`, and against the raw `Position`, never the ring-adjusted
  offset. The notch sits on the letter ring.
- **Ring setting** subtracts from the position to form the offset the wiring is
  read at. Ring R with position P behaves like ring 0 with position P − R.
- **Rotor order** is left to right as written on a key sheet, so the *last* entry
  is the fast rotor next to the entry wheel.
- **Double step.** When the middle rotor sits on its own turnover it is driven by
  the pawl to its right and carries the left rotor with it. Reaching this state
  takes over a hundred keypresses from AAA, so short test vectors never exercise it.
- **The M4's fourth rotor.** Beta and Gamma derive from `ThinRotorBase`, which
  enforces both halves of being thin: `Step` is inert because there is no ratchet,
  and the turnover set is empty because there is no notch. `SetPosition` still
  works, because an operator set the wheel by hand. The invariant lives on the
  wheel rather than in the stepping code, so it holds whoever calls it — do not
  reintroduce a check in `EnigmaMachine`, and do not assume "it is leftmost so it
  cannot step" is enough. `IsThin` remains, but only so the factory can check
  fitment.
- **Fitment.** A thin rotor is half width and only fits in the space a thin
  reflector frees, so four wheels and a thin reflector always come together with
  the thin wheel leftmost. `EnigmaMachineFactory` rejects anything else. The
  machine class itself stays permissive — it is the mechanism, the factory is the
  gate that decides whether a machine could have been built.
- **Reflectors are paired and have no fixed point.** Build them through
  `WiringTable.FromReflectorString`, which enforces both. A reflector that maps a
  letter to itself would let a letter encipher to itself and break the machine.
- **Lifetimes.** Rotors and plugboards carry state and must stay transient in DI.
  Making them singletons lets two machines corrupt each other's positions, and the
  result is plausible-looking wrong ciphertext rather than an exception.
- **Separators in a key sheet.** Rotor names may contain a hyphen ("K-I",
  "UKW-D"), so names split on whitespace and commas only. The hyphen separates
  plugboard pairs, and only there. Splitting names on it silently turned one rotor
  into two.
- **Deferred execution.** `Translate(IEnumerable<int>)` materialises its result on
  purpose: the rotors advance per character, so lazy evaluation would make the
  output depend on when it is enumerated.

## Testing expectations

- New cipher behaviour needs a test that would fail without it. Prefer a published
  vector where one exists; otherwise extend the differential suite.
- `Enigma.Tests/Reference` deliberately duplicates the wiring tables and shares no
  code with the library. That duplication is the point — do not refactor it away,
  and do not "fix" the reference to match the library if they disagree. Work out
  which one is wrong.
- When changing the cipher, it is worth reintroducing a plausible bug and checking
  the suite fails. Tests that cannot fail are worse than no tests.

Component tests do not see a page. Twice now a green bUnit suite has sat over a
broken rendering: an unstyled loading spinner, and controls whose values matched no
option. To look at the real thing, run `dotnet run --project Enigma.Web` and drive
a headless browser at it -- `chromium --headless=new --screenshot` for a still, or
the DevTools protocol on `--remote-debugging-port` when the page has to be clicked
first. Note `--dump-dom` does not serialise form values, which are DOM properties
rather than attributes; assert those in bUnit instead.

## Traps in the build

- The console assembly cannot be named `enigma`: it would differ from the `Enigma`
  library only by case, assembly resolution is case-insensitive, and the runtime
  loads the wrong one. The command name comes from `ToolCommandName` instead.
- The host's content root is pinned to `AppContext.BaseDirectory`. Without that,
  `appsettings.json` is only found when the working directory happens to be the
  project directory.
- `dotnet test` builds the library and test project but **not** `Enigma.Cmd`, so
  `dotnet run --no-build` after a test run can execute against a stale library.
- Diagnostics go to standard error and enciphered text to standard output. Keep it
  that way; anything written to standard output corrupts piped ciphertext.
- `--init-key-sheet` reads the defaults back out of `appsettings.json` rather than
  holding its own copy, so the generated file cannot drift from what the app starts
  with. Keep it that way if you change the key sheet shape.
