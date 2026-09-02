# Writing a message out

The signaller's conventions rather than any cryptography: fitting text to a keyboard
with no space bar and no digits, and writing the ciphertext out in groups.
`MessageText` holds both, `--prepare` and `--groups` reach them from the command
line, and the panel offers them as toggles.

An Enigma has twenty six letter keys, no space bar and no digits, so a signaller
fitted his text to the machine before typing it. `--prepare` does the same:
umlauts expanded, the sharp s doubled, digits spelled out in German, spaces
written as `X`, and everything else dropped. `--groups` breaks the result into the
groups it was transmitted in, five letters at a time by convention, so that a
miscount shows up at the far end.

```bash
$ echo "Angriff um 06:30 Uhr, Größe 12 Männer" | enigma --prepare --groups 5
BQIUD QDAPV HMVYV DTYJT FQTUS HMOEC QJPTL BSJZT AURHZ ISDUH RHJCY H
```

Deciphering ignores the grouping, and gives back the prepared text:

```
ANGRIFFXUMXNULLSECHSDREINULLXUHRXGROESSEXEINSZWOXMAENNER
```

Operators also wrote *ch* as `Q`, which is why the intercepts read `BEOBAQTET` and
`RIQTUNG`. That is deliberately **not** applied: it was a habit rather than a rule,
it was not universal, and it would quietly rewrite any word containing those two
letters.

Grouping is how a message was written out rather than how it was enciphered, so it
moves no wheels. On the panel that is visible: turn grouping on while a key is held
and the lamp stays lit.

## See also

- [The command line](command-line.md) for `--prepare` and `--groups`.
- [The panel](panel.md) for the same two as toggles under the message.
