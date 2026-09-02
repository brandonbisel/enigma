# The command line

The console app, `Enigma.Cmd`, installable as a tool named `enigma`. It reads a
message, runs it through the machine a key sheet describes, and writes the result.
Because the machine is reciprocal, the same settings both encipher and decipher. The
short version is in [the README](../README.md#the-command-line).

## Every option

**Choosing the machine**

| Option | What it does |
|---|---|
| `-k, --key-sheet <file>` | Key sheet file to use. Defaults to the `KeySheet` section of `appsettings.json`. Also accepted as `--settings` |
| `-p, --preset <name>` | Use a packaged key sheet by name. See `--list-presets` |
| `--list-presets` | List the packaged key sheets and exit |
| `--parts <file>` | File defining extra rotors and reflectors, or replacing built-in ones |
| `--init-key-sheet <file>` | Write a key sheet holding the default machine to this path, then exit. Also accepted as `--init-settings` |

**The message**

| Option | What it does |
|---|---|
| `-i, --input <file>` | Read the message from this file instead of standard input |
| `-o, --output <file>` | Write the result to this file instead of standard output |
| `--prepare` | Fit the text to the keyboard first: umlauts expanded, digits spelled out, spaces as X |
| `--groups <n>` | Write the result in groups of this many letters, as a signaller would. Try 5 |

**The indicator procedure**

| Option | What it does |
|---|---|
| `--message-key <letters>` | Encipher with this message key, using the key sheet positions as the ground setting |
| `--indicator <letters>` | Recover the message key from this indicator, then decipher with it |
| `--doubled` | Send the message key twice, as the procedure required until 1938 |
| `--set <name>` | Naval: which set of tables to use. See `--list-sets`. Defaults to Quelle |
| `--list-sets` | List the bigram table sets that ship and exit |
| `--tafel <letter>` | Naval: the Doppelbuchstabentauschtafel to use, named by its letter |
| `--kennziffer <n>` | Naval: take the table from the Tauschtafelplan instead, using this column. Needs `--monatstag` |
| `--monatstag <n>` | Naval: the day of the month to read the Tauschtafelplan at (1–31) |
| `--kenngruppen <six letters>` | Naval: send with these two trigrams, Schlüsselkenngruppe then Verfahrenkenngruppe |
| `--fillers <two letters>` | Naval: the two padding letters, first and last. Defaults to XX |

**Breaking a message**

| Option | What it does |
|---|---|
| `--recover` | Attack the input instead of enciphering it: search for the wheels and where they were set |
| `--wheels <names>` | The wheels that could have been in the machine, such as `"I II III IV V"`. Defaults to the ones the key sheet names |
| `--fitted <n>` | How many wheels the machine carries. Defaults to the number on the key sheet |
| `--reflectors <names>` | The reflectors to try, such as `"B C"`. Defaults to the one on the key sheet |
| `--candidates <n>` | How many settings to report. Defaults to five |

**Watching it work**

| Option | What it does |
|---|---|
| `-v, --verbose` | Trace every character through the plugboard, rotors and reflector |
| `--log-file <file>` | Also write the diagnostic log to this file |
| `-?, -h, --help` | Show help |

## Input and output

With no `--input`, it reads standard input line by line. The rotors keep stepping
across lines, exactly as the real machine does — running the program again is what
returns them to their configured starting position. So enciphering and deciphering
are two separate runs with the same settings:

```bash
echo "ATTACKATDAWN" | dotnet run --project Enigma.Cmd   # BZHGNOCRRTCM
echo "BZHGNOCRRTCM" | dotnet run --project Enigma.Cmd   # ATTACKATDAWN
```

Diagnostics go to standard error and enciphered text to standard output, so piping
and redirection stay clean.

## Overriding one setting

Individual settings can be overridden with configuration keys, which compose with
the named options:

```bash
echo "AAAAA" | dotnet run --project Enigma.Cmd -- --KeySheet:Reflector=C
```

## See also

- [Key sheets](key-sheets.md) for the file the machine is built from, and the
  packaged historical sheets `--preset` names.
- [Indicator procedures](indicator-procedures.md) for what `--message-key`,
  `--indicator`, `--tafel` and `--kenngruppen` are doing, worked through both ways.
- [Bigram tables](bigram-tables.md) for what `--set` is choosing between.
- [Breaking a message](cryptanalysis.md) for what `--recover` costs and what it can
  and cannot break.
- [Parts and alphabets](parts-and-alphabets.md) for the file `--parts` reads.
