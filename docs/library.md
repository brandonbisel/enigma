# Using the library

The `Enigma` package on its own builds and runs machines; `Enigma.App` adds the
session a front end works a machine through. Neither knows anything about a console or
a browser.

## Building a machine

```csharp
services.AddEnigmaServices();

var machine = factory.Create(keySheet);
var cipher = machine.Translate(plaintextIndices);
```

`AddEnigmaServices` registers rotors and reflectors as keyed services under the
names used on a key sheet. Rotors and plugboards are **transient** because they
carry state — two machines sharing a rotor instance would silently corrupt each
other's positions. `IEnigmaMachineFactory` builds a configured machine, which is
what selects three rotors out of eight and applies their positions and ring
settings.

To work a machine rather than build one, `Enigma.App` has a session: a machine
keyed for the day, pressed a key at a time. A key sheet it cannot be built from
comes back as a message rather than an exception, which is what a front end needs.

```csharp
var keyed = EnigmaSession.Open(factory, keySheet);

if (!keyed.Succeeded)
{
    Console.Error.WriteLine(keyed.Error);
    return;
}

var session = keyed.Session!;

session.Press('A');           // the lamp that lit, or null for a key it has not got
session.Type("ATTACKATDAWN"); // the same, for a run of text
session.WindowText;           // where the wheels stand
session.Patch('A', 'V');      // run a cable, as the board itself allows it
session.Reset();              // back to the key sheet, rebuilt rather than wound back
```

## Watching it work

`--verbose` traces each character through every component, with the rotor window
alongside. Primed names are the return leg, after the reflector.

```
A -> B   window AAB   plug A>A | Standard A>A | III A>C | II C>D | I D>F | B F>S | I' S>S | II' S>E | III' E>B | Standard' B>B | plug B>B
```

The same path is available as data rather than text. Subscribe to a machine's
`Translated` event and each keypress arrives as a `TranslationTrace`, carrying the
key, the lamp, the rotor window, and every component the current passed through.
The log line above is formatted from exactly that structure, so a trace and a
diagnostic cannot disagree. A machine nobody is watching builds nothing.

```csharp
machine.Translated += trace =>
{
    // Contacts are numbers in the machine's own alphabet, so map them to read them.
    var window = string.Concat(trace.Window.Select(machine.CharacterMap.GetCharacter));

    Console.WriteLine($"window {window}, {trace.Steps.Count} components");
};
```

## Testing

The suite covers published test vectors, the reciprocity and no-self-encipherment
invariants, rotor stepping including the double step, and the DI lifetimes.

Alongside it sits an independent implementation in `Enigma.Tests/Reference`, written
from the mechanical description of the machine rather than ported from the library,
and anchored on the published vectors. Randomised configurations are run through
both and compared, which covers ground fixed vectors cannot: a bug that reads the
notch from the ring-adjusted offset rather than the window position, for instance,
is invisible to every vector taken at ring setting zero.

## See also

- [AGENTS.md](../AGENTS.md) for the conventions this library is written to, and the
  domain rules that are easy to get wrong.
- [The machines](machines.md) for what a layout decides, and [parts and
  alphabets](parts-and-alphabets.md) for what the catalogue resolves.
