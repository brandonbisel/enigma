"""Independent Enigma reference, written from the mechanical description:
each rotor is a permutation on a ring that can be rotated (position) and whose
ring can be slipped relative to the wiring (Ringstellung); the pawls step the
rotors before the current flows."""

WIRING = {
    "I":    ("EKMFLGDQVZNTOWYHXUSPAIBRCJ", "Q"),
    "II":   ("AJDKSIRUXBLHWTMCQGZNPYFVOE", "E"),
    "III":  ("BDFHJLCPRTXVZNYEIWGAKMUSQO", "V"),
    "IV":   ("ESOVPZJAYQUIRHXLNFTGKDCMWB", "J"),
    "V":    ("VZBRGITYUPSDNHLXAWMJQOFECK", "Z"),
}
REFLECTORS = {"B": "YRUHQSLDPXNGOKMIEBFZCWVJAT", "C": "FVPJIAOYEDRZXWGCTKUQSBNMHL"}

N = 26
idx = lambda c: ord(c) - 65
chr_ = lambda i: chr(i + 65)


class Rotor:
    def __init__(self, name, position=0, ring=0):
        spec, notch = WIRING[name]
        self.name = name
        self.map = [idx(c) for c in spec]
        self.inv = [0] * N
        for contact, out in enumerate(self.map):
            self.inv[out] = contact
        self.notch = idx(notch)
        self.position = position
        self.ring = ring

    @property
    def offset(self):
        return (self.position - self.ring) % N

    def at_notch(self):
        return self.position == self.notch

    def step(self):
        self.position = (self.position + 1) % N

    def forward(self, c):
        return (self.map[(c + self.offset) % N] - self.offset) % N

    def backward(self, c):
        return (self.inv[(c + self.offset) % N] - self.offset) % N


class Machine:
    def __init__(self, rotor_names, reflector="B", positions=(0, 0, 0), rings=(0, 0, 0), plugs=()):
        # rotor_names given left to right, as written on a key sheet.
        self.rotors = [Rotor(n, p, r) for n, p, r in zip(rotor_names, positions, rings)]
        self.reflector = [idx(c) for c in REFLECTORS[reflector]]
        self.plug = list(range(N))
        for a, b in plugs:
            self.plug[idx(a)], self.plug[idx(b)] = idx(b), idx(a)

    def _step(self):
        left, middle, right = self.rotors
        if middle.at_notch():
            middle.step()
            left.step()
        elif right.at_notch():
            middle.step()
        right.step()

    def window(self):
        return "".join(chr_(r.position) for r in self.rotors)

    def press(self, c):
        self._step()
        v = self.plug[idx(c)]
        for rotor in reversed(self.rotors):
            v = rotor.forward(v)
        v = self.reflector[v]
        for rotor in self.rotors:
            v = rotor.backward(v)
        return chr_(self.plug[v])

    def encrypt(self, text):
        return "".join(self.press(c) for c in text)
