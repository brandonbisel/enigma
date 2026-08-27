"""Independent Zaehlwerk (G-31) reference.

Stepping transcribed from the semantics of Palloks' engage_gear: a plain nested
carry, each wheel read for a notch BEFORE it steps, the chain ending at the UKW —
which Crypto Museum's G-111 paper states is "moved by wheel 3", the leftmost.
The entry wheel is the commercial QWERTZ stator; its forward mapping is the
published 'jwulcmnohpqzyxiradkegvbtsf'.
"""
A = "ABCDEFGHIJKLMNOPQRSTUVWXYZ"
ETQ = "JWULCMNOHPQZYXIRADKEGVBTSF"          # entry wheel, forward (key -> contact)

WIRING = {
    "G-I":   ("LPGSZMHAEOQKVXRFYBUTNICJDW", "SUVWZABCEFGIKLOPQ"),
    "G-II":  ("SLVGBTFXJQOHEWIRZYAMKPCNDU", "STVYZACDFGHKMNQ"),
    "G-III": ("CJGDPSHKTURAWZXFMYNQOBVLIE", "UWXAEFHKMNR"),
}
UKW = {"G": "IMETCGFRAYSQBZXWLHKDVUPOJN"}


class Wheel:
    def __init__(self, spec, pos=0, ring=0):
        wiring, notches = spec
        self.map = [A.index(c) for c in wiring]
        self.inv = [0] * 26
        for i, o in enumerate(self.map):
            self.inv[o] = i
        self.notches = {A.index(c) for c in notches}
        self.pos, self.ring = pos, ring

    @property
    def off(self):
        return (self.pos - self.ring) % 26

    def at_notch(self):
        return self.pos in self.notches

    def step(self):
        self.pos = (self.pos + 1) % 26

    def fwd(self, c):
        return (self.map[(c + self.off) % 26] - self.off) % 26

    def bwd(self, c):
        return (self.inv[(c + self.off) % 26] - self.off) % 26


class Reflector(Wheel):
    def __init__(self, wiring, pos=0, ring=0):
        super().__init__((wiring, ""), pos, ring)


class G31:
    def __init__(self, order, ukw="G", positions="AAA", rings="AAA", ukw_pos="A", ukw_ring="A"):
        # order is left to right; the last is the fast wheel
        self.wheels = [Wheel(WIRING[n], A.index(p), A.index(r))
                       for n, p, r in zip(order, positions, rings)]
        self.ukw = Reflector(UKW[ukw], A.index(ukw_pos), A.index(ukw_ring))

    def _step(self):
        for wheel in reversed(self.wheels):
            carries = wheel.at_notch()
            wheel.step()
            if not carries:
                return
        self.ukw.step()

    def window(self):
        return A[self.ukw.pos] + "".join(A[w.pos] for w in self.wheels)

    def press(self, ch):
        self._step()
        c = A.index(ETQ[A.index(ch)])
        for w in reversed(self.wheels):
            c = w.fwd(c)
        c = self.ukw.fwd(c)
        for w in self.wheels:
            c = w.bwd(c)
        return A[ETQ.index(A[c])]

    def encrypt(self, text):
        return "".join(self.press(c) for c in text)
