# The panel

How the browser front end works, part by part. What it is and how to start it is in
[the README](../README.md#the-panel).

```bash
dotnet run --project Enigma.Web
```

The machine as an operator faced it: the wheel windows above, the lamps below them
and the keys below those. Press a key and the wheels turn *before* the lamp
lights, which is the order the machine works in. A lamp stays lit only while its
key is held, and the keyboard takes one key at a time, as the real one did. The
letter that lights, pressed back on a machine returned to the same setting, gives
the original — reciprocity, which is the first thing anyone tries.

The panel is built from whatever machine the key sheet names. An Enigma I shows
three windows and an M4 four, for its fourth wheel; a three-wheel Zählwerk machine
shows four as well, but the extra one is its reflector, which turns and so is part
of the setting. The keyboard is laid out from the machine's own alphabet rather
than from a constant, so a machine that does not work in letters gets its own keys.

The panel is labelled as the machine was — Walzenlage, Ringstellung, Steckerbrett —
with the English alongside each term, so the words can be picked up rather than
looked up.

Under *Spruchschlüssel* the panel offers both indicator procedures, because the two
services worked out where the rotors start in quite different ways. Choose **Heer /
Luftwaffe** and the message key is enciphered at the ground setting and shown in
clear, as it travelled. Choose **Kriegsmarine** and nothing goes in clear at all: the
two Kenngruppen and their padding letters when sending, the eight transmitted letters
when receiving, and both hidden under a Doppelbuchstabentauschtafel.

Which **Satz** — which booklet you are holding — comes first, because a set of tables
and the calendar issued with it are useless apart. Choosing one changes what the rest
of the panel offers: Quelle has eight Tafeln and six Kennziffer columns, Meer nine and
twelve, Flußlauf fifteen and twelve.

Which table is then a choice the operator did not make. Give the panel a Kennziffer column
— the one his cipher net was allotted — and a day of the month, and the
Tauschtafelplan names the table, exactly as it did aboard. Pick nothing and any of the
set's shipped tables can be chosen by hand instead. A day that falls on Quelle's Tafel
J, the one never recovered, says so rather than failing quietly.

U-534's real message runs through it: load the *U-534, 1 May 1945* key sheet, choose
Kriegsmarine receiving, Kennziffer six on the first of the month, and type the
transmitted `FNHC GVET`. The windows come up `ODFF` — the setting the operator wrote
on his own message sheet.

Under the machine, the current can be followed. Tick **Follow the current** and
each keypress is set out step by step: in through the board and the stator, right
to left across the wheels, back off the reflector — marked, because it is the
turning point — and out the way it came. Every letter shown comes from the
`TranslationTrace` the machine itself reports, which is the same structure the
diagnostic log is written from, so the view is the machine's own account rather
than a retelling.

It is asked for rather than assumed: a machine nobody is watching builds no trace,
which is what keeps a long message cheap.

Below the machine is the message. The keyboard and the plaintext pane are two ways
of entering one message, not two messages: type on the keys and the text grows a
letter at a time, or write the whole thing and it is keyed from the start. Either
way the ciphertext is the same, because the machine is the only thing deciding it.

Two conventions of the signaller sit behind toggles. **Fit to the keyboard** applies
`MessageText.Prepare`, expanding umlauts, spelling digits out and writing spaces as
X, so ordinary German can be keyed on a machine that has none of those. **Groups of**
breaks the ciphertext into fives, as it was transmitted, so a miscount showed up at
the far end. Grouping is how the message is written out rather than how it was
enciphered, so it moves no wheels — a lamp still held stays lit.

Changing any setting keys the same message again, which is the point of being able
to change them: the same text, keyed a different way. Clearing the message is what
puts the wheels back; there is no separate reset, because a machine with nothing
typed on it is a machine at its start.

Under the message is the indicator procedure, if one is in use. Sending, you choose
a message key and the page works out the indicator to transmit with it; the wheels
move to your key rather than to the sheet's Grundstellung, which becomes the ground
setting. Receiving, you enter the indicator and the message key is worked back.
Either way an indicator that will not work is reported and the wheels are left
where they were, because mistyping one is an ordinary thing to do.

Below that is its key sheet, set by hand. The wheels, reflector, entry
wheel and alphabet are chosen from the parts catalogue, so a wheel defined in a
parts file appears in the list without the page knowing it exists. Adding a
fourth wheel offers a thin one, because that is the only kind that fits beside a
thin reflector. Settings that will not build a machine are reported, and the
machine already in use is left alone — being midway through setting up is not the
same as holding a broken machine.

The Steckerbrett is below that: click a letter to take up a plug, click another to
run the cable, and click a cabled letter to pull it out. A machine built without a
board — a Zählwerk Enigma — is offered none at all rather than one whose cables
would be refused.

It is a WebAssembly page with no server behind it: the library runs in the browser
unchanged, and nothing typed into it is transmitted anywhere. `dotnet publish`
produces a folder of static files — about 2.1 MB over the wire once compressed —
that can be served from anywhere. Hosting it under a subpath means changing
`<base href="/" />` in `wwwroot/index.html` to match.

