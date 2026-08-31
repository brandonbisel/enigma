namespace Enigma;

/// <summary>
/// The bigram tables this library ships with, under the names their booklets used.
///
/// A set was named by a codeword and held nine tables lettered A to H and J, with a
/// Tauschtafelplan saying which applied on which day. Only tables transcribed from a
/// published scan of an original appear here, and each is checked before it ships:
/// every one of the 676 bigrams present, every entry paired with its mirror, and no
/// bigram enciphering to itself.
///
/// These are from the set "Quelle", booklet Prüf-Nr. 2499, reproduced by the Crypto
/// Museum. It is the set U-534 was using on 1 May 1945.
/// </summary>
public static class BigramTables
{
    /// <summary>
    /// Tafel A of "Quelle". The four entries message P1030690 needed — FN, HC, GV
    /// and ET — are exactly those published with the working of that message.
    /// </summary>
    public static BigramTable QuelleA { get; } = BigramTable.Parse(QuelleAEntries);

    /// <summary>Tafel B of "Quelle".</summary>
    public static BigramTable QuelleB { get; } = BigramTable.Parse(QuelleBEntries);

    /// <summary>Tafel C of "Quelle".</summary>
    public static BigramTable QuelleC { get; } = BigramTable.Parse(QuelleCEntries);

    /// <summary>Tafel D of "Quelle".</summary>
    public static BigramTable QuelleD { get; } = BigramTable.Parse(QuelleDEntries);

    /// <summary>
    /// Tafel E of "Quelle". Six cells of its first two rows are covered by later
    /// hand-written ink on the scan and were recovered from their mirrors rather
    /// than read: on an involution the mirror is the same fact written twice.
    /// </summary>
    public static BigramTable QuelleE { get; } = BigramTable.Parse(QuelleEEntries);

    /// <summary>Tafel F of "Quelle".</summary>
    public static BigramTable QuelleF { get; } = BigramTable.Parse(QuelleFEntries);

    /// <summary>
    /// Tafel G of "Quelle". Its two pages are the poorest scan in the booklet;
    /// eight cells were too blurred to read off and were settled by the rest of
    /// the table, then checked against the glyph at magnification.
    /// </summary>
    public static BigramTable QuelleG { get; } = BigramTable.Parse(QuelleGEntries);

    /// <summary>
    /// Tafel H of "Quelle", the last table the Crypto Museum scan contains. The
    /// booklet's own cover says the edition holds nine tables, "A bis J ohne I";
    /// the scan reproduces eight, A to H, and neither Tafel J nor the two
    /// Tauschtafelplan sheets it also lists.
    /// </summary>
    public static BigramTable QuelleH { get; } = BigramTable.Parse(QuelleHEntries);

    /// <summary>
    /// Tafel A of "Meer", booklet Prüf-Nr. 3733. A different set from "Quelle" and a
    /// markedly better scan: almost every cell agreed with its own mirror on a first
    /// reading, where Quelle's poorer pages needed the involution to arbitrate.
    /// </summary>
    public static BigramTable MeerA { get; } = BigramTable.Parse(MeerAEntries);

    /// <summary>Tafel B of "Meer".</summary>
    public static BigramTable MeerB { get; } = BigramTable.Parse(MeerBEntries);

    /// <summary>Tafel C of "Meer".</summary>
    public static BigramTable MeerC { get; } = BigramTable.Parse(MeerCEntries);

    /// <summary>Tafel D of "Meer".</summary>
    public static BigramTable MeerD { get; } = BigramTable.Parse(MeerDEntries);

    /// <summary>Tafel E of "Meer".</summary>
    public static BigramTable MeerE { get; } = BigramTable.Parse(MeerEEntries);

    /// <summary>Tafel F of "Meer".</summary>
    public static BigramTable MeerF { get; } = BigramTable.Parse(MeerFEntries);

    /// <summary>Tafel G of "Meer".</summary>
    public static BigramTable MeerG { get; } = BigramTable.Parse(MeerGEntries);

    /// <summary>Tafel H of "Meer".</summary>
    public static BigramTable MeerH { get; } = BigramTable.Parse(MeerHEntries);

    /// <summary>
    /// Tafel J of "Meer" — the ninth and last of the edition, and the only Tafel J
    /// in this library. The Quelle scan stops at H, so before this no set here was
    /// complete.
    /// </summary>
    public static BigramTable MeerJ { get; } = BigramTable.Parse(MeerJEntries);

    /// <summary>
    /// Tafel A of "Flußlauf", booklet Prüf-Nr. 3633 — the first table of a set that
    /// runs to fifteen rather than nine.
    ///
    /// Its scan is 150 ppi against Meer's 300, so it is read from the native image and
    /// not from an upsampled render, which is what the calendar sheet established was
    /// necessary. Cut on the printed rules and magnified from native pixels it reads
    /// better than its resolution suggests: all 676 entries came through with every
    /// mirror agreeing, no bigram enciphering to itself, and nothing to adjudicate.
    /// </summary>
    public static BigramTable FlusslaufA { get; } = BigramTable.Parse(FlusslaufAEntries);

    /// <summary>Tafel B of "Flußlauf". Printed on pink stock, as every second table
    /// of the booklet is; red ink on pink is what the green channel is for.</summary>
    public static BigramTable FlusslaufB { get; } = BigramTable.Parse(FlusslaufBEntries);

    /// <summary>
    /// Tafel C of "Flußlauf". One cell needed adjudicating: AT read WO, but WO pairs
    /// with XG both ways and WD reads AT, so the value is WD — an ink blot where the
    /// D's stem meets its bowl closes the letter into an O at page scale.
    /// </summary>
    public static BigramTable FlusslaufC { get; } = BigramTable.Parse(FlusslaufCEntries);

    /// <summary>Tafel D of "Flußlauf". Clean on a first reading, like A and B.</summary>
    public static BigramTable FlusslaufD { get; } = BigramTable.Parse(FlusslaufDEntries);

    /// <summary>Tafel E of "Flußlauf". Clean on a first reading.</summary>
    public static BigramTable FlusslaufE { get; } = BigramTable.Parse(FlusslaufEEntries);

    /// <summary>
    /// Tafel F of "Flußlauf". Clean on a first reading, and the densest page in the
    /// booklet for the dotted İ the typeface uses to keep I apart from J — every one
    /// of them confirmed by its own mirror rather than by the reading alone.
    /// </summary>
    public static BigramTable FlusslaufF { get; } = BigramTable.Parse(FlusslaufFEntries);

    /// <summary>
    /// Tafel G of "Flußlauf", the faintest pair of pages in the booklet. Clean on a
    /// first reading all the same, once the page was cut correctly: the right-hand
    /// edge of its reverse is unruled, and the column fit had to be measured from the
    /// rules that are printed rather than extrapolated to one that is not.
    /// </summary>
    public static BigramTable FlusslaufG { get; } = BigramTable.Parse(FlusslaufGEntries);

    /// <summary>
    /// Tafel H of "Flußlauf". One cell to adjudicate: EZ read BP where it is EP, the
    /// B-for-E confusion this typeface invites at page scale. The mirror settled it --
    /// BP is spoken for by XH, so the reading left without a partner was the wrong one.
    /// </summary>
    public static BigramTable FlusslaufH { get; } = BigramTable.Parse(FlusslaufHEntries);

    /// <summary>
    /// Tafel J of "Flußlauf". The set skips I, as these booklets do throughout, so
    /// this is the ninth table and not the tenth. Clean on a first reading.
    /// </summary>
    public static BigramTable FlusslaufJ { get; } = BigramTable.Parse(FlusslaufJEntries);

    /// <summary>
    /// Tafel K of "Flußlauf". The set skips I, so this is the tenth table and not the
    /// eleventh. Clean on a first reading.
    /// </summary>
    public static BigramTable FlusslaufK { get; } = BigramTable.Parse(FlusslaufKEntries);

    /// <summary>
    /// Tafel L of "Flußlauf". The set skips I, so this is the eleventh table and not
    /// the twelfth. Clean on a first reading.
    /// </summary>
    public static BigramTable FlusslaufL { get; } = BigramTable.Parse(FlusslaufLEntries);

    /// <summary>
    /// Tafel M of "Flußlauf". The set skips I, so this is the twelfth table and not
    /// the thirteenth. Clean on a first reading.
    /// </summary>
    public static BigramTable FlusslaufM { get; } = BigramTable.Parse(FlusslaufMEntries);

    /// <summary>
    /// Tafel N of "Flußlauf". The set skips I, so this is the thirteenth table and
    /// not the fourteenth. Clean on a first reading.
    /// </summary>
    public static BigramTable FlusslaufN { get; } = BigramTable.Parse(FlusslaufNEntries);

    /// <summary>
    /// Tafel O of "Flußlauf". The set skips I, so this is the fourteenth table and
    /// not the fifteenth. One cell, AT, was settled by the involution rather than
    /// read: an ink blot closed the gap between its letters, and QI was the only
    /// value left unclaimed.
    /// </summary>
    public static BigramTable FlusslaufO { get; } = BigramTable.Parse(FlusslaufOEntries);

    /// <summary>
    /// The tables of "Meer" by their letter: all nine, A to J without I. Unlike
    /// "Quelle", whose scan stops at H, this set is complete — every day of its
    /// Tauschtafelplan leads to a table that is actually here.
    /// </summary>
    public static IReadOnlyDictionary<char, BigramTable> Meer { get; } =
        new Dictionary<char, BigramTable>
        {
            ['A'] = MeerA,
            ['B'] = MeerB,
            ['C'] = MeerC,
            ['D'] = MeerD,
            ['E'] = MeerE,
            ['F'] = MeerF,
            ['G'] = MeerG,
            ['H'] = MeerH,
            ['J'] = MeerJ,
        };

    /// <summary>
    /// The tables of "Flußlauf" by the letter its Tauschtafelplan names them by. The
    /// plan names fifteen, A to P without I; fourteen are transcribed so far.
    /// </summary>
    public static IReadOnlyDictionary<char, BigramTable> Flusslauf { get; } =
        new Dictionary<char, BigramTable>
        {
            ['A'] = FlusslaufA,
            ['B'] = FlusslaufB,
            ['C'] = FlusslaufC,
            ['D'] = FlusslaufD,
            ['E'] = FlusslaufE,
            ['F'] = FlusslaufF,
            ['G'] = FlusslaufG,
            ['H'] = FlusslaufH,
            ['J'] = FlusslaufJ,
            ['K'] = FlusslaufK,
            ['L'] = FlusslaufL,
            ['M'] = FlusslaufM,
            ['N'] = FlusslaufN,
            ['O'] = FlusslaufO,
        };

    /// <summary>
    /// The tables of "Quelle" by the letter the Tauschtafelplan names them by, so a
    /// day on the calendar leads straight to a table.
    ///
    /// Eight of the edition's nine are here. Tafel J is absent from the Crypto
    /// Museum scan and from every other source found, so a plan that calls for it
    /// finds nothing — which is the honest answer rather than a substitute.
    /// </summary>
    public static IReadOnlyDictionary<char, BigramTable> Quelle { get; } =
        new Dictionary<char, BigramTable>
        {
            ['A'] = QuelleA,
            ['B'] = QuelleB,
            ['C'] = QuelleC,
            ['D'] = QuelleD,
            ['E'] = QuelleE,
            ['F'] = QuelleF,
            ['G'] = QuelleG,
            ['H'] = QuelleH,
        };

    // Written once through: each entry implies its reverse, and BigramTable builds
    // the other direction itself.

    private const string QuelleAEntries =
        "AA=QG AB=CH AC=NF AD=WT AE=OL AF=ER AG=JE AH=LD AI=ZS AJ=HH AK=PZ AL=UX AM=DB AN=RT " +
        "AO=BY AP=SK AQ=TV AR=YI AS=VF AT=MM AU=FO AV=XN AW=IP AX=NQ AY=KL AZ=GG BA=FS BB=PH " +
        "BC=DW BD=SR BE=GT BF=RA BG=UQ BH=CU BI=VO BJ=MC BK=QX BL=TM BM=VC BN=EX BO=YU BP=JM " +
        "BQ=EG BR=IS BS=OV BT=LJ BU=XR BV=FL BW=IK BX=UK BZ=WN CA=JH CB=OR CC=VJ CD=NM CE=HF " +
        "CF=XV CG=DO CI=UD CJ=ZP CK=XI CL=GE CM=RL CN=TR CO=ZB CP=MJ CQ=YO CR=KQ CS=FE CT=IU " +
        "CV=LT CW=QS CX=SZ CY=NU CZ=IN DA=EJ DC=NH DD=QK DE=SG DF=LQ DG=IF DH=UG DI=HO DJ=OA " +
        "DK=FW DL=JK DM=HC DN=FQ DP=YL DQ=RH DR=PM DS=ON DT=RO DU=GQ DV=TJ DX=WP DY=ZG DZ=KN " +
        "EA=RJ EB=GI EC=PD ED=MH EE=TD EF=OQ EH=LF EI=HR EK=YR EL=QV EM=KH EN=SF EO=NT EP=JV " +
        "EQ=FG ES=VR ET=ZZ EU=LW EV=ID EW=UM EY=WF EZ=LO FA=PS FB=LM FC=NZ FD=JP FF=HL FH=LB " +
        "FI=RE FJ=UA FK=ZL FM=HX FN=KY FP=ME FR=XX FT=TH FU=PV FV=KG FX=VT FY=GL FZ=IX GA=JS " +
        "GB=QD GC=HZ GD=RF GF=TP GH=YC GJ=KD GK=ZU GM=II GN=LV GO=PP GP=JY GR=NC GS=ST GU=MT " +
        "GV=UU GW=XK GX=TB GY=HS GZ=WJ HA=QN HB=TG HD=IZ HE=VL HG=ZX HI=SM HJ=MP HK=OG HM=KA " +
        "HN=WY HP=NO HQ=XE HT=JC HU=YF HV=US HW=LL HY=RX IA=KV IB=OP IC=TF IE=WW IG=YH IH=VZ " +
        "IJ=MG IL=XT IM=SC IO=LE IQ=WH IR=YY IT=JJ IV=RQ IW=ZN IY=TA JA=MR JB=OC JD=SP JF=SS " +
        "JG=YM JI=NX JL=UH JN=ZQ JO=LH JQ=KK JR=OF JT=ZE JU=LN JW=VV JX=SA JZ=WL KB=RC KC=NL " +
        "KE=ZI KF=PX KI=VH KJ=UF KM=YN KO=VA KP=OT KR=SI KS=XZ KT=MF KU=QP KW=TY KX=WC KZ=LI " +
        "LA=PB LC=MX LG=ZK LK=OH LP=RD LR=SE LS=XG LU=YT LX=NJ LY=UO LZ=TN MA=ZO MB=OJ MD=RN " +
        "MI=SH MK=TI ML=YK MN=QI MO=NA MQ=WR MS=QQ MU=TT MV=XP MW=PR MY=VK MZ=UI NB=TL ND=OM " +
        "NE=PJ NG=ZJ NI=YG NK=QO NN=XC NP=QF NR=WE NS=RV NV=SJ NW=VP NY=UW OB=YQ OD=PU OE=ZW " +
        "OI=QC OK=WQ OO=SN OS=XH OU=RZ OW=UC OX=XM OY=WK OZ=TS PA=RR PC=YW PE=TX PF=ZD PG=QA " +
        "PI=XS PK=TW PL=VX PN=UL PO=WG PQ=SO PT=VM PW=YE PY=RM QB=VW QE=RP QH=YJ QJ=TZ QL=YP " +
        "QM=UV QR=UJ QT=ZR QU=SW QW=RI QY=WM QZ=XJ RB=WU RG=TQ RK=ZT RS=WA RU=VI RW=YS RY=SL " +
        "SB=YV SD=WX SQ=ZM SU=TC SV=WO SX=UN SY=XD TE=XO TK=ZV TO=UB TU=XL UE=ZC UP=YA UR=VE " +
        "UT=XW UY=WD UZ=YX VB=WS VD=YZ VG=XU VN=ZH VQ=WB VS=XQ VU=YD VY=XF WI=ZA WV=XB WZ=YB " +
        "XA=ZY XY=ZF";

    private const string QuelleBEntries =
        "AA=HN AB=PK AC=SB AD=CJ AE=VU AF=YO AG=RV AH=BE AI=XG AJ=QL AK=LH AL=EC AM=TQ AN=ZS " +
        "AO=DP AP=GG AQ=JO AR=NH AS=KX AT=MZ AU=OW AV=UT AW=FE AX=WJ AY=CS AZ=VH BA=SF BB=ER " +
        "BC=YK BD=QG BF=DH BG=VN BH=ZW BI=GP BJ=KQ BK=NA BL=PZ BM=UH BN=IS BO=XD BP=JL BQ=FI " +
        "BR=OC BS=MT BT=RO BU=TY BV=WU BW=HQ BX=EN BY=LR BZ=XR CA=IN CB=KS CC=DL CD=VB CE=OM " +
        "CF=EJ CG=UQ CH=ZY CI=LM CK=NP CL=HS CM=FD CN=ST CO=ZU CP=RH CQ=GM CR=JR CT=ML CU=PQ " +
        "CV=TK CW=SM CX=KJ CY=QP CZ=TH DA=VX DB=HJ DC=NW DD=RL DE=VS DF=YH DG=FX DI=QU DJ=HW " +
        "DK=MG DM=EZ DN=HF DO=UE DQ=PO DR=LT DS=GJ DT=IF DU=WQ DV=KM DW=OR DX=TN DY=XS DZ=SX " +
        "EA=HL EB=NK ED=JF EE=SO EF=WD EG=RX EH=ZO EI=FA EK=MP EL=TV EM=YC EO=OT EP=KD EQ=XW " +
        "ES=UB ET=VZ EU=GN EV=LD EW=PT EX=IP EY=HC FB=PG FC=HI FF=YS FG=OH FH=KB FJ=RT FK=VP " +
        "FL=ZD FM=GE FN=NB FO=UN FP=SI FQ=MK FR=QB FS=IH FT=XM FU=LK FV=NU FW=MS FY=PW FZ=JJ " +
        "GA=WH GB=JN GC=QD GD=MR GF=IV GH=SK GI=NN GK=LP GL=VJ GO=KN GQ=XO GR=OE GS=YA GT=RR " +
        "GU=JH GV=YI GW=HH GX=ZF GY=TD GZ=LF HA=MI HB=OD HD=JZ HE=RD HG=UL HK=PD HM=WO HO=JA " +
        "HP=NR HR=TI HT=IB HU=VL HV=QJ HX=XI HY=LV HZ=KO IA=QN IC=TP ID=MW IE=SA IG=WN II=RK " +
        "IJ=XJ IK=ZL IL=ND IM=KP IO=VF IQ=SR IR=YE IT=UZ IU=LN IW=JC IX=KK IY=VW IZ=UX JB=MU " +
        "JD=KG JE=OF JG=LL JI=ZQ JK=OI JM=SH JP=XE JQ=OK JS=RA JT=WL JU=PR JV=TS JW=UG JX=WZ " +
        "JY=VD KA=YQ KC=NM KE=ZI KF=UA KH=NF KI=XV KL=SS KR=VA KT=QR KU=TB KV=WT KW=PI KY=PM " +
        "KZ=YM LA=SD LB=MO LC=YG LE=NT LG=XB LI=OG LJ=ZZ LO=PA LQ=US LS=XF LU=TF LW=VQ LX=NE " +
        "LY=RM LZ=WR MA=TG MB=OP MC=ZK MD=NG ME=YP MF=SQ MH=PY MJ=QC MM=WX MN=TZ MQ=PU MV=SE " +
        "MX=YW MY=RI NC=ZR NI=RZ NJ=TO NL=VG NO=YL NQ=OY NS=RJ NV=QK NX=UV NY=ZT NZ=VM OA=QW " +
        "OB=SP OJ=ZV OL=UJ ON=PS OO=UD OQ=WF OS=ZA OU=YB OV=RP OX=TM OZ=XU PB=UI PC=QX PE=ZX " +
        "PF=RE PH=TA PJ=UO PL=XA PN=SC PP=YN PV=QF PX=ZG QA=WM QE=ZN QH=SL QI=VT QM=WA QO=XK " +
        "QQ=RB QS=YZ QT=RN QV=TE QY=WI QZ=TT RC=UK RF=WB RG=ZP RQ=SG RS=VV RU=XH RW=YU RY=XQ " +
        "SJ=VC SN=ZC SU=TC SV=XY SW=YY SY=UF SZ=WP TJ=XL TL=UC TR=ZE TU=VE TW=YX TX=UP UM=VI " +
        "UR=ZB UU=VK UW=XZ UY=YD VO=XC VR=XX VY=ZH WC=YV WE=XT WG=YR WK=YJ WS=XN WV=YF WW=ZJ " +
        "WY=XP YT=ZM";

    private const string QuelleCEntries =
        "AA=UM AB=KH AC=OP AD=NE AE=TJ AF=IN AG=ZL AH=RV AI=FS AJ=CH AK=JZ AL=MS AM=YK AN=GB " +
        "AO=QZ AP=XI AQ=LW AR=PC AS=WX AT=JN AU=VE AV=DO AW=BF AX=SQ AY=HM AZ=EB BA=WO BB=PN " +
        "BC=LF BD=FA BE=RI BG=ME BH=SZ BI=OG BJ=KT BK=ID BL=ZX BM=CT BN=EQ BO=DZ BP=YT BQ=JA " +
        "BR=VO BS=NW BT=GF BU=DB BV=XX BW=QI BX=TA BY=UT BZ=HC CA=LM CB=WD CC=NM CD=IV CE=PV " +
        "CF=QA CG=OZ CI=VV CJ=HT CK=GQ CL=ZR CM=RO CN=JF CO=SB CP=ZG CQ=YA CR=FI CS=MK CU=XN " +
        "CV=EL CW=TS CX=UD CY=DE CZ=KN DA=LQ DC=WT DD=PG DF=OA DG=VI DH=ZO DI=QS DJ=KC DK=NA " +
        "DL=SI DM=YE DN=RC DP=XB DQ=JV DR=MX DS=FE DT=UB DU=TY DV=IH DW=EG DX=GK DY=HJ EA=XL " +
        "EC=NR ED=TN EE=ZC EF=HA EH=YO EI=PQ EJ=OV EK=IR EM=KK EN=LV EO=XS EP=WI ER=GW ES=RZ " +
        "ET=QM EU=SL EV=FY EW=JR EX=VA EY=UH EZ=LI FB=PJ FC=UQ FD=VU FF=GI FG=NI FH=QD FJ=WL " +
        "FK=ZH FL=SU FM=YQ FN=RA FO=XE FP=OK FQ=HZ FR=MB FT=IB FU=TD FV=LA FW=VC FX=KZ FZ=JK " +
        "GA=UO GC=MO GD=RR GE=LT GG=ZZ GH=SE GJ=RY GL=YZ GM=QW GN=OD GO=HP GP=NC GR=PS GS=KA " +
        "GT=IY GU=XV GV=TQ GX=WB GY=XP GZ=JD HB=QO HD=NK HE=MA HF=KQ HG=OX HH=SV HI=ZU HK=PA " +
        "HL=TV HN=YW HO=RL HQ=WF HR=UX HS=VR HU=SA HV=LL HW=XK HX=JY HY=IK IA=NP IC=YU IE=RF " +
        "IF=UK IG=ZN II=QU IJ=OM IL=MH IM=XG IO=YH IP=SN IQ=PX IS=LC IT=JH IU=VZ IW=TG IX=WQ " +
        "IZ=KW JB=RK JC=NU JE=UZ JG=OB JI=SW JJ=QF JL=PE JM=ZA JO=XY JP=MV JQ=KF JS=TL JT=WV " +
        "JU=YC JW=VL JX=LO KB=ZY KD=LB KE=VX KG=OS KI=NY KJ=SS KL=YI KM=RX KO=QK KP=UA KR=PP " +
        "KS=LZ KU=WM KV=TC KX=MQ KY=TT LD=NT LE=RT LG=ZJ LH=QB LJ=YX LK=SG LN=OI LP=WZ LR=UV " +
        "LS=MM LU=XC LX=VG LY=QQ MC=PB MD=VT MF=OR MG=ZE MI=UR MJ=QY ML=TZ MN=WA MP=RE MR=YV " +
        "MT=SX MU=NG MW=ZP MY=XA MZ=QC NB=QG ND=SY NF=WK NH=ZW NJ=UF NL=XU NN=PZ NO=TP NQ=VM " +
        "NS=YM NV=OO NX=RB NZ=QV OC=UY OE=WR OF=ZS OH=RG OJ=TB OL=QX ON=UJ OQ=XZ OT=PY OU=SD " +
        "OW=YD OY=VB PD=SO PF=XM PH=ZT PI=UN PK=WG PL=YR PM=QN PO=TW PR=VQ PT=US PU=RM PW=SC " +
        "QE=TU QH=UP QJ=WP QL=ZM QP=SR QR=VK QT=RD RH=WW RJ=YG RN=ZB RP=VF RQ=TX RS=UE RU=SJ " +
        "RW=XQ SF=WN SH=UI SK=XW SM=ZQ SP=TF ST=YB TE=VY TH=ZV TI=WE TK=UW TM=XR TO=YJ TR=VD " +
        "UC=VW UG=WH UL=XJ UU=YS VH=XT VJ=ZD VN=YL VP=WY VS=XD WC=YN WJ=XO WS=ZF WU=YY XF=YP " +
        "XH=ZK YF=ZI";

    private const string QuelleDEntries =
        "AA=TN AB=KH AC=OQ AD=IN AE=PB AF=NV AG=RJ AH=JG AI=UO AJ=ZR AK=QX AL=MP AM=GD AN=DZ " +
        "AO=HC AP=YI AQ=VV AR=SE AS=WJ AT=XY AU=CL AV=JR AW=LI AX=BK AY=EF AZ=FT BA=KN BB=RR " +
        "BC=WU BD=SQ BE=JD BF=OA BG=HS BH=EV BI=GP BJ=ME BL=QH BM=PT BN=VR BO=ZF BP=YX BQ=TV " +
        "BR=LS BS=DA BT=NC BU=CZ BV=XE BW=UG BX=VA BY=FH BZ=ID CA=ZK CB=PJ CC=IT CD=LA CE=TB " +
        "CF=MX CG=JW CH=DI CI=HZ CJ=NK CK=EM CM=KX CN=GV CO=YL CP=FB CQ=OI CR=QN CS=UW CT=VK " +
        "CU=YA CV=RB CW=SX CX=WE CY=XL DB=IH DC=GJ DD=TQ DE=ZX DF=JA DG=HK DH=VQ DJ=LM DK=YJ " +
        "DL=SA DM=PO DN=WB DO=FM DP=NA DQ=OX DR=MC DS=KE DT=XS DU=RV DV=YQ DW=EA DX=QC DY=UB " +
        "EB=TH EC=OM ED=IK EE=QY EG=MA EH=NP EI=TZ EJ=PF EK=SJ EL=US EN=LX EO=ZA EP=HG EQ=JK " +
        "ER=GM ES=RN ET=YD EU=XB EW=KR EX=WP EY=VD EZ=FA FC=PA FD=MJ FE=QR FF=KB FG=WA FI=JN " +
        "FJ=HW FK=NG FL=UP FN=LD FO=IA FP=RE FQ=SM FR=ZU FS=GA FU=OD FV=YM FW=TE FX=VN FY=UD " +
        "FZ=XH GB=ZD GC=WY GE=MT GF=PL GG=VI GH=ZO GI=LP GK=TX GL=JH GN=UK GO=YT GQ=IX GR=OT " +
        "GS=XO GT=SU GU=HA GW=NZ GX=QB GY=RH GZ=KK HB=TK HD=ZC HE=MM HF=QK HH=JP HI=SG HJ=XV " +
        "HL=RZ HM=IQ HN=PX HO=WM HP=YF HQ=LF HR=ST HT=XJ HU=OG HV=KA HX=NM HY=TT IB=OO IC=VG " +
        "IE=ZH IF=RX IG=NS II=LU IJ=KT IL=JU IM=TA IO=YV IP=SC IR=MH IS=PZ IU=QE IV=UY IW=XQ " +
        "IY=VT IZ=WR JB=NQ JC=SO JE=NI JF=UM JI=WZ JJ=QU JL=OK JM=ZB JO=MV JQ=RL JS=KD JT=YY " +
        "JV=PR JX=LK JY=VB JZ=XA KC=UE KF=NX KG=VY KI=OZ KJ=QA KL=PV KM=ZQ KO=RP KP=MZ KQ=OV " +
        "KS=XC KU=TJ KV=WH KW=YB KY=MW KZ=LB LC=QP LE=UA LG=RT LH=ZM LJ=YK LL=OS LN=TP LO=NE " +
        "LQ=WO LR=XX LT=PD LV=MR LW=SZ LY=UQ LZ=VP MB=ZE MD=NO MF=TG MG=YO MI=OB MK=SS ML=XZ " +
        "MN=QJ MO=WW MQ=VL MS=PH MU=RA MY=UI NB=ZS ND=TL NF=VZ NH=XG NJ=SB NL=PM NN=UU NR=OY " +
        "NT=WG NU=YZ NW=QT NY=RC OC=UN OE=ZZ OF=SK OH=WK OJ=QS OL=RG ON=PY OP=VX OR=XU OU=YG " +
        "OW=TY PC=UZ PE=WD PG=VU PI=ZY PK=RW PN=TC PP=UR PQ=SW PS=QF PU=ZW PW=XM QD=TW QG=ZJ " +
        "QI=UV QL=SY QM=RY QO=YW QQ=XD QV=WT QW=VC QZ=TD RD=WN RF=SI RI=ZV RK=XP RM=YC RO=VE " +
        "RQ=UX RS=TM RU=WC SD=ZL SF=TU SH=VO SL=UT SN=XW SP=YR SR=WX SV=TF TI=YS TO=UC TR=WV " +
        "TS=XR UF=ZT UH=VW UJ=XT UL=YP VF=WS VH=ZG VJ=XN VM=YE VS=WF WI=XF WL=ZP WQ=YU XI=ZN " +
        "XK=YN YH=ZI";

    private const string QuelleEEntries =
        "AA=HE AB=SR AC=PO AD=FJ AE=TL AF=IM AG=RO AH=BD AI=LJ AJ=DK AK=UN AL=CB AM=JQ AN=QV " +
        "AO=ER AP=NS AQ=WT AR=KU AS=VL AT=ZB AU=OJ AV=YX AW=MI AX=GF AY=XU AZ=BP BA=RZ BB=QT " +
        "BC=HA BE=CD BF=NQ BG=TJ BH=DI BI=SP BJ=EU BK=ZD BL=MG BM=GH BN=PR BO=VJ BQ=XW BR=OG " +
        "BS=IP BT=UL BU=WX BV=YZ BW=FM BX=JT BY=LF BZ=KY CA=NO CC=DG CE=OD CF=RM CG=FH CH=UJ " +
        "CI=JO CJ=EP CK=ZF CL=GA CM=LB CN=ME CO=PT CP=YU CQ=ST CR=KW CS=QR CT=VH CU=XZ CV=HG " +
        "CW=IR CX=TO CY=WQ CZ=DP DA=VF DB=EC DC=UH DD=FE DE=TG DF=GK DH=SM DJ=HI DL=RR DM=IJ " +
        "DN=QP DO=JL DQ=PK DR=KQ DS=OM DT=LL DU=NV DV=MK DW=ZH DX=WN DY=XS DZ=YR EA=NL EB=MM " +
        "ED=OO EE=LN EF=PI EG=KN EH=QX EI=JJ EJ=RV EK=IH EL=SW EM=HO EN=TR EO=GP EQ=UF ES=FP " +
        "ET=VO EV=ZJ EW=YP EX=WK EY=XQ EZ=FX FA=GC FB=NI FC=HK FD=OQ FF=IE FG=PV FI=JG FK=QM " +
        "FL=KK FN=RJ FO=LP FQ=SH FR=MB FS=TU FT=ZM FU=YL FV=UQ FW=XN FY=WZ FZ=VR GB=HC GD=ZP " +
        "GE=IB GG=YI GI=JE GJ=XK GL=KH GM=WI GN=LS GO=VT GQ=MP GR=UU GS=NF GT=SD GU=TB GV=OA " +
        "GW=RC GX=PE GY=QI GZ=HW HB=PC HD=OT HF=QE HH=NY HJ=RG HL=MT HM=SY HN=LV HP=TX HQ=KF " +
        "HR=UC HS=JB HT=VZ HU=IA HV=WF HX=ZU HY=XI HZ=YF IC=JA ID=ZZ IF=KC IG=YD II=KS IK=XE " +
        "IL=MW IN=LD IO=WV IQ=NB IS=VW IT=OX IU=PY IV=US IW=QK IX=TE IY=SF IZ=RX JC=QZ JD=PB " +
        "JF=RT JH=OE JI=SA JK=ND JM=TZ JN=MY JP=UW JR=LH JS=VC JU=KA JV=WB JW=ZX JX=YG JY=XA " +
        "JZ=KX KB=SJ KD=RE KE=TV KG=QB KI=UA KJ=PM KL=VY KM=OL KO=WP KP=NW KR=XC KT=MR KV=ZA " +
        "KZ=YB LA=MC LC=ZR LE=NJ LG=YN LI=OH LK=XG LM=PH LO=WD LQ=RA LR=QD LT=VE LU=SC LW=UY " +
        "LX=TT LY=NU LZ=YW MA=SV MD=RF MF=TC MH=QG MJ=UP ML=PF MN=VA MO=NC MQ=OB MS=WH MU=ZT " +
        "MV=YA MX=NZ MZ=XD NA=OZ NE=PA NG=QJ NH=RD NK=SX NM=TF NN=UE NP=VI NR=WG NT=XL NX=YJ " +
        "OC=ZE OF=PG OI=QA OK=RI ON=SK OP=TD OR=UI OS=VX OU=WA OV=XB OW=YE OY=ZW PD=ZG PJ=YC " +
        "PL=XF PN=WE PP=VV PQ=UD PS=TH PU=SZ PW=RK PX=QF PZ=RY QC=RB QH=SE QL=TK QN=UB QO=VD " +
        "QQ=WY QS=XH QU=YH QW=ZC QY=RP RH=ZN RL=YK RN=XJ RQ=WL RS=VG RU=TI RW=UK SB=TM SG=ZK " +
        "SI=UG SL=YM SN=VB SO=XM SQ=WC SS=UZ SU=VK TA=UM TN=VQ TP=WJ TQ=XO TS=YO TW=ZL TY=UX " +
        "UO=VM UR=ZO UT=YS UV=XR VN=WM VP=XT VS=YY VU=ZI WO=XY WR=YQ WS=ZV WU=ZY WW=XP XV=YT " +
        "XX=ZS YV=ZQ";

    private const string QuelleFEntries =
        "AA=WH AB=QN AC=HL AD=SI AE=BG AF=MN AG=DJ AH=UR AI=GP AJ=CQ AK=EO AL=KM AM=VU AN=ZY " +
        "AO=JW AP=IT AQ=TD AR=XC AS=FB AT=YY AU=NV AV=RP AW=OG AX=PH AY=LU AZ=BS BA=RN BB=IQ " +
        "BC=DG BD=LR BE=QQ BF=EK BH=MJ BI=CN BJ=GS BK=SF BL=XG BM=HH BN=TA BO=NZ BP=JT BQ=UO " +
        "BR=OD BT=YV BU=PJ BV=WC BW=KR BX=VY BY=ZW BZ=FD CA=VR CB=HE CC=JY CD=QK CE=DD CF=RS " +
        "CG=WJ CH=EI CI=NS CJ=IO CK=SB CL=GM CM=TF CO=PE CP=MH CR=FF CS=UL CT=XI CU=KV CV=YT " +
        "CW=ZU CX=LP CY=OB CZ=DX DA=JQ DB=TI DC=KK DE=PM DF=EG DH=GV DI=SL DK=FH DL=MR DM=HB " +
        "DN=UW DO=ZS DP=IX DQ=LY DR=NO DS=YZ DT=RV DU=XA DV=WM DW=VO DY=QT DZ=OI EA=KH EB=FJ " +
        "EC=LD ED=GF EE=MC EF=JO EH=IB EJ=HP EL=NM EM=ZQ EN=OL EP=YR EQ=PP ER=XL ES=QH ET=WF " +
        "EU=RY EV=SO EW=VZ EX=TK EY=UU EZ=FA FC=GB FE=NJ FG=HS FI=OO FK=IL FL=PS FM=JK FN=QW " +
        "FO=KZ FP=RJ FQ=LM FR=SQ FS=MV FT=UI FU=TN FV=ZO FW=VA FX=YP FY=XO FZ=WP GA=OQ GC=NH " +
        "GD=PV GE=MA GG=QD GH=LI GI=RF GJ=KC GK=ST GL=JF GN=TQ GO=IH GQ=UB GR=HW GT=VJ GU=ZM " +
        "GW=YN GX=WS GY=XQ GZ=HA HC=ZK HD=NE HF=YK HG=OT HI=XS HJ=PB HK=WV HM=QY HN=VG HO=RD " +
        "HQ=UF HR=TU HT=SW HU=IZ HV=MW HX=JI HY=LW HZ=KT IA=JC IC=ZH ID=KE IE=YH IF=LA IG=XX " +
        "II=MT IJ=WY IK=NC IM=VD IN=OW IP=UZ IR=PY IS=TW IU=QB IV=SD IW=RH IY=JZ JA=QF JB=PA " +
        "JD=RC JE=OZ JG=SY JH=NQ JJ=TS JL=ME JM=UD JN=LF JP=VL JR=KP JS=WZ JU=ZF JV=XE JX=YF " +
        "KA=NF KB=LC KD=MF KF=OR KG=PD KI=ZB KJ=QZ KL=YC KN=RA KO=XU KQ=SM KS=WR KU=TY KW=VF " +
        "KX=UJ KY=LZ LB=ZD LE=MP LG=NK LH=YA LJ=OX LK=XZ LL=WB LN=PF LO=VC LQ=RK LS=QI LT=UG " +
        "LV=SH LX=TG MB=SN MD=RX MG=TB MI=QC MK=UA ML=PK MM=VX MO=WD MQ=RE MS=QP MU=XF MX=ZZ " +
        "MY=NX MZ=YL NA=ZV NB=YD ND=OE NG=PI NI=XH NL=QG NN=WQ NP=RL NR=VK NT=SA NU=TR NW=UM " +
        "NY=OV OA=PL OC=QE OF=RI OH=SE OJ=TZ OK=US OM=VH ON=WA OP=XY OS=YQ OU=ZI OY=PZ PC=TE " +
        "PG=SJ PN=UH PO=RG PQ=VB PR=QL PT=WK PU=XK PW=YI PX=ZN QA=ZT QJ=RO QM=YS QO=SP QR=WE " +
        "QS=XB QU=TH QV=UK QX=VP RB=SZ RM=TT RQ=UV RR=VV RT=WX RU=ZL RW=YU RZ=XT SC=ZA SG=YJ " +
        "SK=XJ SR=WN SS=VM SU=TV SV=UN SX=TC TJ=YO TL=XM TM=ZP TO=VN TP=WO TX=UQ UC=VW UE=WT " +
        "UP=XR UT=YW UX=ZR UY=VE VI=ZJ VQ=YM VS=XP VT=WU WG=XV WI=ZG WL=YX WW=XD XN=ZC XW=YG " +
        "YB=ZE YE=ZX";

    private const string QuelleGEntries =
        "AA=HW AB=WH AC=NL AD=BM AE=UP AF=GJ AG=DQ AH=ZI AI=MO AJ=CB AK=ID AL=QF AM=KU AN=ST " +
        "AO=YW AP=JY AQ=OZ AR=EV AS=FX AT=RP AU=XJ AV=LL AW=VN AX=PT AY=TO AZ=BB BA=RM BC=DO " +
        "BD=WE BE=GH BF=PQ BG=KR BH=SW BI=HT BJ=MM BK=QI BL=FU BN=ZG BO=JV BP=OW BQ=IG BR=TL " +
        "BS=LP BT=NR BU=YT BV=UM BW=XF BX=ES BY=VR BZ=CM CA=PO CC=MJ CD=OT CE=DM CF=LJ CG=EP " +
        "CH=NU CI=VL CJ=FS CK=QM CL=KO CN=GE CO=JT CP=RR CQ=ZD CR=TI CS=TY CT=WB CU=YR CV=SX " +
        "CW=UJ CX=IH CY=XD CZ=HV DA=NB DB=EA DC=KE DD=FP DE=LH DF=QC DG=OQ DH=SR DI=GL DJ=MH " +
        "DK=HR DL=JZ DN=PM DP=ZK DR=TG DS=YO DT=RK DU=IL DV=US DW=VU DX=WJ DY=XM DZ=EY EB=OO " +
        "EC=MQ ED=FC EE=LF EF=HP EG=PV EH=ZM EI=NW EJ=YL EK=QO EL=XP EM=KJ EN=RT EO=VX EQ=SP " +
        "ER=GN ET=IB EU=JR EW=TS EX=UV EZ=WM FA=MS FB=QK FD=LR FE=GP FF=ZA FG=HM FH=KL FI=NZ " +
        "FJ=YY FK=JO FL=OM FM=IN FN=XR FO=PX FQ=WP FR=TW FT=UY FV=RI FW=VZ FY=SN FZ=GA GB=OK " +
        "GC=NJ GD=PI GF=MU GG=QQ GI=LT GK=RV GM=KA GO=SI GQ=JH GR=TU GS=IJ GT=UH GU=ZP GV=YJ " +
        "GW=VI GX=WR GY=XT GZ=HJ HA=MF HB=IP HC=LN HD=JL HE=KG HF=NO HG=ZR HH=OV HI=YG HK=XG " +
        "HL=PK HN=QG HO=RF HQ=WU HS=TE HU=SK HX=UO HY=VT HZ=IX IA=RD IC=QE IE=SG IF=PC II=TD " +
        "IK=OJ IM=UF IO=NH IQ=VG IR=MW IS=WO IT=LV IU=XC IV=KQ IW=YM IY=ZO IZ=JX JA=KC JB=ZF " +
        "JC=LD JD=YE JE=MA JF=XI JG=NE JI=WF JJ=OG JK=VW JM=PF JN=UD JP=QA JQ=TB JS=RB JU=TC " +
        "JW=SD KB=SC KD=RE KF=UZ KH=TF KI=QD KK=PD KM=VV KN=OE KP=WY KS=NQ KT=XB KV=MZ KW=LX " +
        "KX=ZW KY=LW KZ=YX LA=MC LB=ZZ LC=YD LE=NX LG=XK LI=OB LK=WW LM=PR LO=VP LQ=RH LS=QT " +
        "LU=TR LY=UU LZ=VS MB=SF MD=RG ME=TK MG=QJ MI=UX MK=PL ML=VE MN=OR MP=WC MR=ND MT=XX " +
        "MV=ZN MX=YV MY=NV NA=OY NC=ZV NF=PH NG=YP NI=QH NK=XY NM=RW NN=WL NP=SO NS=VK NT=UQ " +
        "NY=TT OA=TH OC=SJ OD=UL OF=RN OH=VM OI=QZ OL=WK ON=PU OP=XS OS=ZU OU=YN OX=WS PA=ZT " +
        "PB=XA PE=YI PG=WI PJ=QL PN=VJ PP=RL PS=TV PW=SY PY=UW PZ=XW QB=RZ QN=ZY QP=SV QR=YU " +
        "QS=TP QU=XO QV=UN QW=RA QX=VY QY=ZB RC=SE RJ=YQ RO=ZX RQ=TM RS=XN RU=VQ RX=UI RY=WT " +
        "SA=TZ SB=ZQ SH=YK SL=XZ SM=WQ SQ=VA SS=UT SU=TX SZ=YA TA=UR TJ=VH TN=WZ TQ=XU UA=WG " +
        "UB=VC UC=XH UE=YH UG=ZL UK=VO VB=ZS VD=WA VF=YC WD=XE WN=YS WV=ZJ WX=YZ XL=ZH XQ=YF " +
        "XV=ZE YB=ZC";

    private const string QuelleHEntries =
        "AA=NU AB=HO AC=SJ AD=KE AE=QR AF=IT AG=OB AH=MV AI=GC AJ=RW AK=YM AL=DQ AM=ZF AN=BX " +
        "AO=PY AP=LS AQ=TZ AR=FN AS=UP AT=JL AU=VK AV=CG AW=XH AX=WI AY=ED AZ=OS BA=TH BB=LP " +
        "BC=NY BD=JM BE=OD BF=MI BG=RN BH=FT BI=KQ BJ=PG BK=ES BL=QO BM=GU BN=SX BO=IZ BP=UC " +
        "BQ=HA BR=ZV BS=CW BT=VR BU=DE BV=YJ BW=XL BY=WF BZ=UT CA=WJ CB=MP CC=QT CD=VU CE=JH " +
        "CF=IG CH=RX CI=DZ CJ=ZB CK=YD CL=EO CM=XR CN=LI CO=FM CP=OE CQ=PK CR=KV CS=NF CT=GL " +
        "CU=HN CV=SQ CX=UW CY=TA CZ=VF DA=RI DB=GS DC=UL DD=MT DF=EV DG=NN DH=FB DI=ZJ DJ=OY " +
        "DK=PU DL=QP DM=LG DN=HW DO=IR DP=SZ DR=KA DS=YC DT=XE DU=JO DV=TX DW=VH DX=KY DY=WQ " +
        "EA=JF EB=RK EC=MX EE=VI EF=HU EG=NJ EH=PP EI=LN EJ=UD EK=FO EL=OH EM=QL EN=KS EP=ZC " +
        "EQ=YY ER=XZ ET=GA EU=SG EW=TO EX=II EY=MB EZ=WA FA=XK FC=NK FD=IV FE=QG FF=GW FG=SD " +
        "FH=KZ FI=VT FJ=ZP FK=HH FL=OU FP=JB FQ=LC FR=YO FS=MY FU=PN FV=RR FW=SY FX=TM FY=US " +
        "FZ=WL GB=VC GD=WH GE=JS GF=LX GG=ZM GH=RY GI=HQ GJ=ND GK=IA GM=UN GN=XY GO=YZ GP=ML " +
        "GQ=OJ GR=KI GT=PB GV=QE GX=RF GY=SO GZ=TK HB=NQ HC=KW HD=UE HE=ZT HF=PM HG=IP HI=RC " +
        "HJ=YU HK=JV HL=OA HM=QI HP=LF HR=XS HS=WG HT=VZ HV=TB HX=SL HY=OV HZ=MO IB=YS IC=UB " +
        "ID=ZW IE=NR IF=JT IH=OC IJ=PQ IK=ME IL=QX IM=XD IN=KF IO=LK IQ=WX IS=RZ IU=SA IW=TV " +
        "IX=VN IY=WM JA=QB JC=KU JD=RS JE=LO JG=TW JI=YF JJ=NZ JK=ZH JN=OQ JP=MC JQ=PD JR=ST " +
        "JU=QZ JW=UY JX=WV JY=VP JZ=XG KB=ZI KC=LT KD=RO KG=MZ KH=VJ KJ=WR KK=NC KL=YV KM=OR " +
        "KN=UJ KO=TE KP=SB KR=WS KT=XA KX=MD LA=ZE LB=UH LD=QY LE=SI LH=YR LJ=MN LL=NB LM=WK " +
        "LQ=XO LR=OZ LU=PT LV=RA LW=YQ LY=TQ LZ=VW MA=VL MF=PR MG=NX MH=OO MJ=UI MK=ZY MM=SF " +
        "MQ=YH MR=PX MS=XP MU=TJ MW=QD NA=WO NE=QV NG=YT NH=ZD NI=OM NL=PC NM=RD NO=TL NP=XX " +
        "NS=VX NT=SK NV=UG NW=VB OF=YP OG=SU OI=QC OK=XM OL=ZR ON=PE OP=RG OT=WD OW=UA OX=TF " +
        "PA=SM PF=WW PH=YB PI=QN PJ=ZG PL=RE PO=SW PS=TI PV=UK PW=VY PZ=XC QA=VG QF=XT QH=ZN " +
        "QJ=RB QK=SV QM=TS QQ=UO QS=WZ QU=YK QW=TY RH=SS RJ=YE RL=ZX RM=TD RP=UZ RQ=VO RT=XV " +
        "RU=WC RV=UX SC=XJ SE=ZL SH=YW SN=TU SP=UR SR=VD TC=UU TG=ZQ TN=YX TP=XF TR=VS TT=WP " +
        "UF=VM UM=XQ UQ=YL UV=ZA VA=ZK VE=WT VQ=XB VV=YG WB=YN WE=XU WN=ZO WU=XW WY=ZZ XI=ZS " +
        "XN=YA YI=ZU";

    private const string MeerAEntries =
        "AA=SZ AB=CF AC=NO AD=UI AE=GP AF=PG AG=IU AH=ON AI=FC AJ=ZS AK=HO AL=RH AM=JP AN=VL " +
        "AO=EX AP=DK AQ=WY AR=BM AS=MB AT=YW AU=KD AV=WE AW=LV AX=PA AY=HJ AZ=QR BA=EG BB=VU " +
        "BC=JY BD=RQ BE=DT BF=WN BG=MK BH=TJ BI=YF BJ=KM BK=XS BL=PP BN=LB BO=HV BP=YR BQ=ZB " +
        "BR=FL BS=OW BT=ID BU=GY BV=UR BW=NX BX=CO BY=SH BZ=XB CA=MT CB=YO CC=KV CD=XK CE=LF " +
        "CG=PY CH=HE CI=SR CJ=CV CK=QA CL=UZ CM=NG CN=GH CP=IM CQ=OF CR=ZK CS=FU CT=RZ CU=JI " +
        "CW=VD CX=EP CY=TS CZ=DC DA=YJ DB=KQ DD=PL DE=XO DF=QI DG=TB DH=ZZ DI=FG DJ=LN DL=OS " +
        "DM=IR DN=GA DO=NJ DP=EK DQ=UD DR=JC DS=WT DU=SM DV=MG DW=TY DX=RL DY=VH DZ=LA EA=UM " +
        "EB=HZ EC=NT ED=YS EE=KH EF=OK EH=QY EI=WA EJ=IG EL=XF EM=KY EN=FX EO=ZE EQ=SB ER=MP " +
        "ES=PI ET=JM EU=LR EV=GD EW=RC EY=QU EZ=VQ FA=RU FB=JT FD=TN FE=VZ FF=MW FH=WJ FI=KO " +
        "FJ=PD FK=HR FM=LP FN=YC FO=QM FP=TF FQ=NA FR=XW FS=UG FT=GL FV=IX FW=ZH FY=OB FZ=SV " +
        "GB=ZP GC=HA GE=PU GF=VJ GG=KZ GI=RE GJ=MI GK=YQ GM=XD GN=SO GO=WW GQ=IP GR=QF GS=NV " +
        "GT=UK GU=ZM GV=LJ GW=OD GX=TR GZ=JJ HB=QK HC=SX HD=NM HF=LZ HG=HY HH=XU HI=MM HK=UV " +
        "HL=YH HM=IJ HN=OQ HP=ZW HQ=RN HS=JE HT=VO HU=WC HW=KA HX=TV IA=VF IB=RS IC=IV IE=YY " +
        "IF=LX IH=SE II=OU IK=XH IL=MD IN=PW IO=TL IQ=JV IS=KT IT=WQ IW=QO IY=NR IZ=UX JA=PS " +
        "JB=XM JD=VW JF=QC JG=UT JH=MZ JK=RJ JL=ZU JN=TH JO=LL JQ=YE JR=OI JS=WG JU=ND JW=SJ " +
        "JX=VS JZ=KF KB=MR KC=YM KE=LQ KG=NI KI=UP KJ=OY KK=TD KL=RW KN=XQ KP=ST KR=WL KS=PN " +
        "KU=QW KW=LC KX=ZD LD=ZR LE=WO LG=QQ LH=NP LI=VM LK=YK LM=RY LO=SG LS=OO LT=MU LU=XJ " +
        "LW=UB LY=PQ MA=UN MC=TO ME=QS MF=VB MH=ZY MJ=SC ML=YU MN=WV MO=NB MQ=PF MS=RG MV=TW " +
        "MX=OL MY=XX NC=WI NE=RP NF=UE NH=XZ NK=QG NL=VY NN=YA NQ=SK NS=PB NU=ZF NW=TT NY=XC " +
        "NZ=OA OC=RR OE=TZ OG=QB OH=UY OJ=XA OM=SP OP=ZI OR=VK OT=PJ OV=WR OX=TP OZ=YN PC=TU " +
        "PE=ZO PH=QX PK=UC PM=XI PO=VE PR=YL PT=ZC PV=RA PX=WF PZ=SD QD=VX QE=SY QH=WU QJ=UH " +
        "QL=YG QN=ZJ QP=VA QT=RB QV=XL QZ=VC RD=SU RF=VV RI=WZ RK=TA RM=ZV RO=YB RT=XN RV=UW " +
        "RX=VG SA=UU SF=ZX SI=TQ SL=UF SN=XY SQ=ZA SS=YZ SW=VI TC=YX TE=WS TG=XE TI=ZT TK=VR " +
        "TM=WB TX=UA UJ=ZG UL=WD UO=XV UQ=VN US=YI VP=WX VT=WH WK=ZQ WM=YD WP=XG XP=YV XR=ZL " +
        "XT=YP YT=ZN";

    private const string MeerBEntries =
        "AA=QM AB=ZV AC=FC AD=UG AE=LQ AF=XA AG=MS AH=BO AI=YD AJ=TO AK=KF AL=PP AM=NL AN=OU " +
        "AO=WV AP=GU AQ=VY AR=CV AS=TF AT=EW AU=JA AV=HK AW=RN AX=UB AY=SO AZ=IQ BA=DZ BB=KM " +
        "BC=RX BD=YL BE=HR BF=OB BG=VH BH=DN BI=LC BJ=SW BK=ZF BL=IJ BM=PG BN=WK BP=FI BQ=MD " +
        "BR=TZ BS=CC BT=JE BU=QA BV=XL BW=GO BX=NY BY=US BZ=EV CA=VO CB=MP CD=TJ CE=DT CF=KS " +
        "CG=QV CH=XU CI=HB CJ=FQ CK=ON CL=JW CM=UZ CN=EC CO=LI CP=SF CQ=ZR CR=IK CS=PL CT=HW " +
        "CU=GY CW=YW CX=RD CY=NG CZ=MA DA=PU DB=WO DC=HF DD=OI DE=VV DF=GD DG=NA DH=UM DI=FZ " +
        "DJ=MH DK=TR DL=EJ DM=LN DO=SA DP=ZC DQ=KY DR=RS DS=JI DU=JK DV=QP DW=XG DX=IA DY=ZZ " +
        "EA=UJ EB=IW ED=RH EE=FU EF=LZ EG=HO EH=QG EI=ZK EK=NT EL=WS EM=KA EN=TV EO=PY EP=YH " +
        "EQ=VC ER=JP ES=SJ ET=GG EU=XD EX=ML EY=WC EZ=OF FA=XZ FB=KI FD=SS FE=MX FF=TA FG=GR " +
        "FH=OY FJ=VK FK=IE FL=QD FM=RF FN=YT FO=FW FP=PB FR=HX FS=LU FT=XP FV=WG FX=UW FY=NQ " +
        "GA=JT GB=PN GC=TL GE=OQ GF=XX GH=MJ GI=SU GJ=IZ GK=WM GL=KO GM=UD GN=ZX GP=NE GQ=YP " +
        "GS=OW GT=VF GV=RK GW=HH GX=QI GZ=LS HA=RU HC=WQ HD=HU HE=YZ HG=NI HI=TD HJ=PS HL=MF " +
        "HM=ZA HN=IT HP=KD HQ=OK HS=JH HT=QX HV=UP HY=LF HZ=XJ IB=VS IC=NV ID=KQ IF=RW IG=YF " +
        "IH=JR II=QT IL=XS IM=WY IN=PD IO=MN IP=PJ IR=VB IS=ZO IU=UI IV=LK IX=SQ IY=TH JB=LX " +
        "JC=JY JD=OS JF=WE JG=RB JJ=UU JL=TX JM=YJ JN=MV JO=QK JQ=SY JS=VM JU=PW JV=ZT JX=NC " +
        "JZ=KK KB=TT KC=VZ KE=QR KG=XN KH=SC KJ=NN KL=ZI KN=RQ KP=WA KR=PF KT=MR KU=YX KV=OD " +
        "KW=LM KX=YI KZ=UF LA=ZW LB=OG LD=LT LE=XH LG=TP LH=RO LJ=WI LL=UR LO=VQ LP=ME LR=SH " +
        "LV=YA LW=NK LY=PH MB=NR MC=YM MG=WU MI=RY MK=VW MM=QN MO=SK MQ=XV MT=UL MU=ZM MW=TB " +
        "MY=OO MZ=ZN NB=SE ND=QZ NF=UN NH=ZD NJ=XQ NM=OL NO=RI NP=YR NS=WW NU=TG NW=QB NX=VI " +
        "NZ=PQ OA=OR OC=YU OE=VD OH=XC OJ=ZG OM=SN OP=RL OT=SZ OV=PI OX=WH OZ=QE PA=WX PC=SL " +
        "PE=UA PK=ZY PM=XF PO=TN PR=VU PT=YB PV=QJ PX=ZP PZ=RA QC=RT QF=YO QH=VX QL=SB QO=WL " +
        "QQ=ZE QS=UT QU=RG QW=XK QY=TI RC=VP RE=XY RJ=UC RM=WZ RP=YK RR=SM RV=ZJ RZ=TQ SD=XW " +
        "SG=VJ SI=ZU SP=XE SR=TK ST=YN SV=WF SX=UO TC=XR TE=VT TM=WJ TS=YQ TU=UK TW=ZH TY=VA " +
        "UE=YY UH=WT UQ=YG UV=XM UX=VE UY=ZL VG=ZQ VL=XI VN=XT VR=WN WB=ZS WD=XO WP=YE WR=ZB " +
        "XB=YV YC=YS";

    private const string MeerCEntries =
        "AA=RJ AB=XA AC=UQ AD=NS AE=QR AF=VK AG=QD AH=YM AI=ZT AJ=CC AK=OX AL=FP AM=GE AN=HI " +
        "AO=MZ AP=KW AQ=BD AR=JQ AS=WW AT=PV AU=TB AV=SZ AW=EO AX=BP AY=IF AZ=LK BA=DG BB=ZM " +
        "BC=MG BE=XM BF=CU BG=SA BH=YA BI=GT BJ=MX BK=HQ BL=JH BM=FI BN=NJ BO=VQ BQ=LY BR=QL " +
        "BS=UI BT=XG BU=VW BV=RW BW=PN BX=UD BY=TX BZ=FY CA=RA CB=VG CD=TR CE=WD CF=ZE CG=YT " +
        "CH=OA CI=DP CJ=MK CK=GP CL=JC CM=NM CN=SI CO=VA CP=NC CQ=EI CR=PD CS=TM CT=WK CV=GJ " +
        "CW=DA CX=OL CY=XX CZ=SU DB=GC DC=OP DD=ZI DE=ER DF=HT DH=SD DI=YJ DJ=SL DK=GG DL=HO " +
        "DM=HG DN=OT DO=ZZ DQ=NA DR=HB DS=WU DT=WE DU=EW DV=MC DW=EE DX=XP DY=PH DZ=KA EA=UB " +
        "EB=OJ EC=QA ED=UM EF=EY EG=JV EH=MM EJ=RQ EK=ZB EL=HR EM=QF EN=IP EP=SJ EQ=PF ES=XS " +
        "ET=FU EU=JI EV=OF EX=HL EZ=LD FA=RD FB=HH FC=MA FD=NL FE=SB FF=MQ FG=JT FH=GW FJ=SP " +
        "FK=LQ FL=JB FM=OH FN=JA FO=JK FQ=RN FR=KI FS=XT FT=JZ FV=MS FW=TU FX=UR FZ=MI GA=KF " +
        "GB=JE GD=OR GF=RC GH=TI GI=KU GK=QO GL=ZQ GM=PA GN=HC GO=OD GQ=XI GR=TO GS=LG GU=ZW " +
        "GV=VU GX=MP GY=IM GZ=IA HA=IS HD=KD HE=MF HF=IL HJ=IZ HK=QJ HM=KP HN=QC HP=LN HS=QI " +
        "HU=WO HV=ZC HW=KJ HX=PB HY=VC HZ=YL IB=JM IC=MD ID=OC IE=TH IG=XV IH=NZ II=WN IJ=ZG " +
        "IK=KY IN=MN IO=QH IQ=JG IR=ZP IT=NV IU=JD IV=ST IW=YX IX=XQ IY=JX JF=KC JJ=PK JL=LI " +
        "JN=RX JO=VL JP=PM JR=LR JS=RK JU=SO JW=TK JY=KS KB=LA KE=PC KG=NN KH=LJ KK=RB KL=LM " +
        "KM=NP KN=QB KO=UE KQ=PP KR=LO KT=RG KV=VJ KX=PI KZ=LZ LB=ML LC=WA LE=UG LF=TC LH=NX " +
        "LL=ZH LP=SS LS=MU LT=TF LU=PJ LV=QQ LW=UC LX=MO MB=ZU ME=QG MH=UA MJ=UK MR=RI MT=TQ " +
        "MV=WH MW=OI MY=PO NB=SE ND=QE NE=TJ NF=SC NG=VH NH=WB NI=QK NK=UL NO=TG NQ=QM NR=SH " +
        "NT=VO NU=TE NW=UJ NY=YO OB=TA OE=WF OG=YC OK=VF OM=YQ ON=VP OO=UH OQ=XC OS=PT OU=TP " +
        "OV=XL OW=UY OY=SR OZ=UT PE=VB PG=YN PL=SN PQ=TD PR=QU PS=SG PU=WQ PW=QN PX=YK PY=TL " +
        "PZ=YZ QP=VE QS=WC QT=YD QV=ZJ QW=VD QX=SF QY=ZL QZ=VN RE=XB RF=VI RH=WJ RL=UF RM=YB " +
        "RO=ZA RP=WI RR=RY RS=UV RT=VM RU=XE RV=ZD RZ=VZ SK=WG SM=UO SQ=WT SV=YI SW=WL SX=YE " +
        "SY=ZO TN=UN TS=WP TT=XF TV=YG TW=XD TY=YF TZ=XZ UP=VS US=XJ UU=ZS UW=WY UX=VY UZ=XH " +
        "VR=ZF VT=XN VV=XK VX=YY WM=YH WR=YR WS=XO WV=YW WX=XR WZ=ZK XU=ZV XW=YP XY=ZX YS=ZN " +
        "YU=ZY YV=ZR";

    private const string MeerDEntries =
        "AA=SY AB=OO AC=NA AD=CQ AE=ZA AF=BR AG=DK AH=EM AI=FK AJ=GT AK=PY AL=QD AM=RK AN=HV " +
        "AO=TN AP=YJ AQ=IO AR=JR AS=KG AT=UB AU=XQ AV=ML AW=VS AX=VM AY=LR AZ=WU BA=MT BB=PH " +
        "BC=DA BD=RN BE=OS BF=VE BG=HI BH=KP BI=SL BJ=JJ BK=YT BL=FQ BM=UK BN=CL BO=WG BP=NO " +
        "BQ=XK BS=LM BT=ZE BU=IS BV=TQ BW=GH BX=WM BY=QM BZ=ED CA=EQ CB=NT CC=ZO CD=FF CE=LF " +
        "CF=TU CG=QR CH=DZ CI=XE CJ=KT CK=WY CM=UQ CN=HM CO=YN CP=MD CR=RQ CS=II CT=PN CU=SE " +
        "CV=JN CW=ZY CX=VA CY=OF CZ=GL DB=EY DC=NW DD=GP DE=PR DF=MP DG=RF DH=IV DI=VJ DJ=SO " +
        "DL=XT DM=JF DN=VY DO=WQ DP=UG DQ=LU DR=YF DS=ZJ DT=KK DU=TG DV=HC DW=OI DX=FB DY=QH " +
        "EA=FU EB=NH EC=SH EE=UV EF=GD EG=WI EH=HP EI=QV EJ=XN EK=KD EL=RT EN=YH EO=IZ EP=ZS " +
        "ER=VG ES=LX ET=OK EU=XY EV=JT EW=TJ EX=PK EZ=MH FA=GX FC=OW FD=XG FE=PC FG=ZB FH=MC " +
        "FI=QJ FJ=IE FL=TD FM=HE FN=SS FO=UN FP=YQ FR=LJ FS=YX FT=VP FV=WD FW=NK FX=KW FY=RW " +
        "FZ=JB GA=LC GB=TA GC=QO GE=NQ GF=ZL GG=KM GI=US GJ=RA GK=WW GM=VV GN=HY GO=XC GQ=PT " +
        "GR=JV GS=VC GU=MW GV=SV GW=YB GY=OC GZ=IL HA=IX HB=PE HD=ND HF=JP HG=ZQ HH=UE HJ=TS " +
        "HK=KY HL=QX HN=VL HO=RI HQ=MJ HR=WK HS=LP HT=XZ HU=TW HW=SC HX=XI HZ=OA IA=JX IB=KH " +
        "IC=WB ID=OG IF=YL IG=MR IH=UC IJ=PP IK=SA IM=RY IN=XV IP=ZV IQ=NZ IR=WO IT=QB IU=VR " +
        "IW=TL IY=LL JA=NF JC=RD JD=KB JE=ZG JG=LH JH=QT JI=VH JK=UO JL=OY JM=WP JO=PV JQ=XR " +
        "JS=UI JU=YC JW=TH JY=SK JZ=MN KA=NU KC=LZ KE=VW KF=YO KI=MA KJ=SB KL=TE KN=WA KO=ZM " +
        "KQ=XL KR=WS KS=OM KU=PA KV=UL KX=QL KZ=RR LA=YG LB=MZ LD=ZU LE=VT LG=XA LI=NB LK=OQ " +
        "LN=PF LO=WF LQ=TB LS=UX LT=QF LV=RM LW=SQ LY=VD MB=NY ME=QK MF=SG MG=TY MI=VN MK=WJ " +
        "MM=OE MO=RG MQ=TO MS=XH MU=YS MV=UA MX=ZZ MY=PD NC=OZ NE=TZ NG=YR NI=UF NJ=WL NL=RB " +
        "NM=XB NN=SI NP=VI NR=PS NS=QP NV=TC NX=OB OD=WC OH=TR OJ=RP OL=XJ ON=UM OP=QN OR=YU " +
        "OT=SW OU=VZ OV=PU OX=ZC PB=UD PG=TK PI=VB PJ=ST PL=SJ PM=ZN PO=WN PQ=RJ PW=XD PX=YA " +
        "PZ=QS QA=TP QC=XP QE=ZP QG=YM QI=RU QQ=UP QU=UZ QW=VK QY=WE QZ=SN RC=XW RE=ZI RH=YE " +
        "RL=VO RO=UH RS=TM RV=WX RX=XF RZ=SD SF=UJ SM=UT SP=VF SR=TV SU=ZD SX=WR SZ=YI TF=XU " +
        "TI=VQ TT=UY TX=WH UR=ZT UU=YD UW=XS VU=ZW VX=YW WT=XM WV=YK WZ=ZH XO=YZ XX=ZK YP=ZR " +
        "YV=ZX YY=ZF";

    private const string MeerEEntries =
        "AA=PJ AB=TB AC=WE AD=NX AE=ZK AF=UP AG=VZ AH=KI AI=HF AJ=CT AK=HQ AL=BR AM=FA AN=EO " +
        "AO=GW AP=MU AQ=XR AR=NO AS=YI AT=CG AU=FS AV=WB AW=JK AX=BE AY=VM AZ=EI BA=FJ BB=QN " +
        "BC=LJ BD=XI BF=IG BG=QC BH=JA BI=MZ BJ=YE BK=HA BL=UX BM=JO BN=UV BO=CO BP=LQ BQ=IR " +
        "BS=NG BT=CA BU=VQ BV=DM BW=OP BX=SK BY=TT BZ=MI CB=CK CC=LA CD=RK CE=SS CF=XA CH=EU " +
        "CI=LL CJ=SB CL=IN CM=UH CN=GI CP=RO CQ=II CR=TH CS=VK CU=MQ CV=QA CW=ZR CX=LD CY=NT " +
        "CZ=JZ DA=DZ DB=PG DC=KE DD=IW DE=NR DF=GO DG=ON DH=MW DI=QT DJ=SH DK=KM DL=UA DN=LX " +
        "DO=SG DP=YG DQ=EE DR=LT DS=NJ DT=FW DU=HM DV=PD DW=UI DX=ZW DY=HZ EA=JF EB=RD EC=VT " +
        "ED=JS EF=QK EG=FP EH=KT EJ=SQ EK=WX EL=EW EM=OT EN=TK EP=YU EQ=ZC ER=ID ES=TV ET=YC " +
        "EV=FD EX=KQ EY=WI EZ=RY FB=IF FC=QJ FE=WG FF=PR FG=JX FH=VP FI=GU FK=MM FL=NQ FM=VS " +
        "FN=YM FO=ZF FQ=GE FR=OI FT=GQ FU=XP FV=KY FX=PI FY=RR FZ=XW GA=HJ GB=SE GC=LN GD=UF " +
        "GF=XK GG=YZ GH=JJ GJ=TO GK=JU GL=VW GM=HU GN=OE GP=HS GR=RI GS=KO GT=HH GV=WT GX=QQ " +
        "GY=JP GZ=XU HB=OK HC=IH HD=TE HE=KK HG=VI HI=XS HK=OR HL=TF HN=YP HO=PU HP=JV HR=MB " +
        "HT=VF HV=XD HW=VA HX=IS HY=LO IA=UD IB=NL IC=KS IE=OQ IJ=XH IK=QV IL=TA IM=KU IO=SM " +
        "IP=MK IQ=ZY IT=OA IU=QM IV=YX IX=JC IY=UJ IZ=ZH JB=MR JD=SF JE=ME JG=KV JH=TC JI=ZM " +
        "JL=LB JM=RP JN=UO JQ=PA JR=UM JT=KF JW=YQ JY=VC KA=NC KB=MO KC=UB KD=US KG=YA KH=RA " +
        "KJ=LZ KL=SU KN=QF KP=LM KR=ZP KW=RF KX=MX KZ=RL LC=WA LE=PW LF=WC LG=ZZ LH=MV LI=PN " +
        "LK=XL LP=MF LR=NZ LS=YS LU=OL LV=VR LW=QY LY=UK MA=OO MC=VB MD=YT MG=PS MH=TI MJ=YD " +
        "ML=NV MN=RT MP=VL MS=PQ MT=VG MY=ZU NA=RW NB=SJ ND=XF NE=ST NF=VV NH=RG NI=TS NK=VO " +
        "NM=YH NN=YB NP=OC NS=ZX NU=WZ NW=QO NY=WF OB=RH OD=TG OF=VD OG=ZA OH=TM OJ=SA OM=XG " +
        "OS=ZJ OU=SC OV=XE OW=SZ OX=UC OY=PB OZ=XQ PC=UE PE=XB PF=TR PH=WU PK=WD PL=ZD PM=RC " +
        "PO=UT PP=UG PT=QB PV=XY PX=ZI PY=SD PZ=RM QD=XC QE=ZN QG=TJ QH=QS QI=RB QL=ZB QP=WQ " +
        "QR=UY QU=YV QW=RZ QX=VE QZ=YW RE=YO RJ=VH RN=YY RQ=WH RS=ZE RU=SX RV=VJ RX=TW SI=XJ " +
        "SL=ZL SN=TD SO=UU SP=TL SR=YK SV=VN SW=VY SY=TZ TN=WK TP=XN TQ=UW TU=YF TX=ZS TY=WN " +
        "UL=YJ UN=WW UQ=YL UR=WJ UZ=XZ VU=WL VX=ZG WM=YR WO=ZO WP=XM WR=WY WS=ZQ WV=XT XO=ZT " +
        "XV=YN XX=ZV";

    private const string MeerFEntries =
        "AA=EM AB=NJ AC=BR AD=OI AE=LG AF=RM AG=DS AH=SP AI=CZ AJ=ZQ AK=KT AL=TW AM=GU AN=WS " +
        "AO=FF AP=QR AQ=IX AR=PY AS=HL AT=UH AU=JN AV=XK AW=MK AX=VQ AY=BU AZ=NY BA=PN BB=CM " +
        "BC=TR BD=LK BE=YQ BF=JP BG=ZH BH=GK BI=WJ BJ=DY BK=OS BL=EE BM=QF BN=FU BO=YE BP=MV " +
        "BQ=XV BS=VD BT=HF BV=RH BW=UN BX=KI BY=SC BZ=IP CA=FQ CB=NQ CC=DJ CD=OE CE=MR CF=ZZ " +
        "CG=EU CH=PK CI=GG CJ=VL CK=LS CL=SK CN=UU CO=HT CP=QZ CQ=KF CR=WO CS=JB CT=YM CU=II " +
        "CV=XE CW=MD CX=TJ CY=RU DA=ZB DB=EQ DC=NF DD=GO DE=PQ DF=MN DG=OW DH=FK DI=QL DK=RA " +
        "DL=LC DM=SX DN=HA DO=VX DP=JW DQ=YB DR=IB DT=XR DU=UE DV=KL DW=TB DX=VI DZ=WG EA=MH " +
        "EB=NU EC=FN ED=QX EF=ZS EG=HQ EH=OA EI=LX EJ=RW EK=JJ EL=ST EN=YG EO=GD EP=XC ER=PB " +
        "ES=IU ET=TH EV=WU EW=KQ EX=UL EY=LO EZ=VS FA=QD FB=GS FC=PU FD=MP FE=YS FG=OY FH=JL " +
        "FI=XM FJ=IK FL=KV FM=UX FO=RC FP=LU FR=SE FS=VB FT=HH FV=ZU FW=WA FX=JR FY=TL FZ=LI " +
        "GA=KD GB=NW GC=HN GE=ZJ GF=OK GH=RS GI=JY GJ=PD GL=QH GM=LA GN=SM GP=YI GQ=ID GR=TD " +
        "GT=UP GV=XT GW=MX GX=VN GY=HD GZ=WC HB=IR HC=OM HE=PF HG=RJ HI=QB HJ=KO HK=YU HM=SG " +
        "HO=ZD HP=IF HR=XX HS=WQ HU=VF HV=JT HW=TN HX=MB HY=NB HZ=JF IA=NM IC=OP IE=QV IG=RE " +
        "IH=KX IJ=ZL IL=YY IM=UC IN=JD IO=SR IQ=UJ IS=VV IT=WW IV=XH IW=OC IY=PS IZ=KB JA=LQ " +
        "JC=ND JE=YW JG=PH JH=MF JI=OG JK=QN JM=XP JO=US JQ=RP JS=WL JU=KZ JV=SA JX=JZ KA=ZN " +
        "KC=QT KE=SI KG=UA KH=RY KJ=MT KK=WE KM=XZ KN=OU KP=YO KR=LZ KS=TU KU=VZ KW=QJ KY=NO " +
        "LB=TF LD=WY LE=MZ LF=ZX LH=TY LJ=NH LL=QP LM=TP LN=OQ LP=RN LR=SV LT=ZF LV=YK LW=XA " +
        "LY=UG MA=NS MC=PL ME=ZV MG=TS MI=UQ MJ=RK ML=SZ MM=VJ MO=YC MQ=UZ MS=TA MU=QA MW=XI " +
        "MY=WH NA=PW NC=ZO NE=YL NG=XO NI=SO NK=QE NL=TI NN=RF NP=OH NR=WM NT=UV NV=SF NX=VH " +
        "NZ=YX OB=PP OD=QM OF=RR OJ=SB OL=TK ON=UK OO=VP OR=WD OT=XF OV=YP OX=ZK OZ=TE PA=ZG " +
        "PC=YJ PE=XN PG=WK PI=VT PJ=UF PM=TQ PO=SU PR=QI PT=RD PV=ZY PX=WT PZ=TX QC=RO QG=SW " +
        "QK=TZ QO=UI QQ=VK QS=WN QU=XJ QW=YD QY=ZC RB=SN RG=ZM RI=YV RL=WX RQ=VE RT=UW RV=TG " +
        "RX=SJ RZ=VU SD=TM SH=UB SL=VO SQ=WI SS=XD SY=ZA TC=UO TO=VR TT=WP TV=YF UD=ZR UM=YT " +
        "UR=XW UT=WF UY=VC VA=WZ VG=XS VM=YN VW=ZP VY=XG WB=XU WR=YA WV=ZW XB=ZI XL=YR XQ=ZE " +
        "XY=YH YZ=ZT";

    private const string MeerGEntries =
        "AA=HV AB=FJ AC=LU AD=MG AE=DK AF=ID AG=QM AH=PR AI=SF AJ=NE AK=JO AL=XQ AM=HS AN=KQ " +
        "AO=BN AP=FA AQ=GH AR=OB AS=TC AT=OO AU=HM AV=LR AW=IM AX=NU AY=RE AZ=VB BA=LN BB=YP " +
        "BC=RW BD=VI BE=KH BF=JR BG=TI BH=OP BI=SK BJ=UM BK=ZY BL=WO BM=NS BO=PX BP=XG BQ=WE " +
        "BR=HQ BS=FV BT=IU BU=MD BV=GP BW=EL BX=QJ BY=DT BZ=CA CB=GT CC=HH CD=LI CE=IP CF=KL " +
        "CG=FD CH=QL CI=TA CJ=RZ CK=JY CL=NA CM=SP CN=PZ CO=OF CP=EX CQ=UJ CR=YA CS=VG CT=ZU " +
        "CU=VR CV=DG CW=MZ CX=WK CY=XE CZ=YX DA=NY DB=QE DC=GF DD=UR DE=SI DF=JP DH=MA DI=OL " +
        "DJ=TZ DL=VX DM=RK DN=HY DO=UC DP=LA DQ=IK DR=PO DS=WB DU=ZE DV=ED DW=XZ DX=KA DY=YE " +
        "DZ=FN EA=QP EB=HN EC=KV EE=FZ EF=SN EG=JU EH=WM EI=ZB EJ=UP EK=YZ EM=MN EN=GY EO=NK " +
        "EP=TG EQ=ZI ER=PF ES=IG ET=WQ EU=YC EV=UI EW=OY EY=LF EZ=RD FB=JD FC=MJ FE=HZ FF=MB " +
        "FG=NL FH=RM FI=UB FK=WZ FL=ZA FM=SD FO=XS FP=OH FQ=JA FR=IX FS=KN FT=GD FU=LX FW=NQ " +
        "FX=PP FY=VD GA=OS GB=SZ GC=UF GE=WY GG=OW GI=LM GJ=PL GK=NX GL=QB GM=VT GN=YD GO=RU " +
        "GQ=TN GR=ZM GS=XA GU=IS GV=KD GW=MI GX=HE GZ=JK HA=RR HB=TP HC=WS HD=ZQ HF=OZ HG=PI " +
        "HI=RO HJ=YR HK=MR HL=PD HO=NR HP=OQ HR=NG HT=XK HU=QW HW=UT HX=KP IA=RG IB=NZ IC=VO " +
        "IE=ZZ IF=MP IH=KX II=JC IJ=LD IL=NV IN=UQ IO=XM IQ=VM IR=WG IT=RB IV=OJ IW=TR IY=ZN " +
        "IZ=SB JB=UV JE=QR JF=UE JG=ST JH=YM JI=QZ JJ=OU JL=OK JM=NP JN=WD JQ=TE JS=LP JT=KC " +
        "JV=RN JW=ZS JX=PV JZ=NH KB=LH KE=MU KF=NW KG=WI KI=QC KJ=XU KK=PS KM=RI KO=ZG KR=VC " +
        "KS=PM KT=WU KU=QG KW=OA KY=OD KZ=VJ LB=ON LC=TW LE=ND LG=XI LJ=VW LK=PG LL=QK LO=QY " +
        "LQ=WP LS=ZD LT=UK LV=SX LW=TO LY=OV LZ=TJ MC=XW ME=PJ MF=VN MH=QV MK=YI ML=NJ MM=SS " +
        "MO=XC MQ=XT MS=PW MT=SE MV=PA MW=RS MX=VQ MY=NT NB=WT NC=OX NF=PY NI=WR NM=ZC NN=VK " +
        "NO=TL OC=TQ OE=TB OG=QU OI=SU OM=TS OR=YG OT=SL PB=UZ PC=VU PE=WA PH=XL PK=US PN=WJ " +
        "PQ=UH PT=SG PU=WH QA=UO QD=SR QF=VP QH=TV QI=SW QN=RJ QO=SC QQ=ZW QS=UU QT=XB QX=SJ " +
        "RA=UW RC=VA RF=WN RH=ZR RL=VV RP=VZ RQ=ZT RT=XH RV=SM RX=VE RY=YL SA=WL SH=ZJ SO=UL " +
        "SQ=YJ SV=UG SY=ZH TD=VL TF=ZL TH=VS TK=UY TM=XY TT=YW TU=ZX TX=YS TY=VH UA=YO UD=ZV " +
        "UN=YK UX=WF VF=ZO VY=XN WC=XV WV=YB WW=YT WX=ZP XD=YQ XF=YY XJ=YH XO=ZK XP=YN XR=YV " +
        "XX=ZF YF=YU";

    private const string MeerHEntries =
        "AA=SQ AB=HF AC=FT AD=OG AE=ZZ AF=EH AG=QM AH=SB AI=KK AJ=XH AK=IQ AL=JN AM=NP AN=PW " +
        "AO=TA AP=WX AQ=YZ AR=LE AS=VT AT=UR AU=BA AV=EC AW=DO AX=GL AY=MD AZ=FA BB=JF BC=UM " +
        "BD=OR BE=IK BF=FG BG=EX BH=FL BI=HB BJ=PK BK=QN BL=IO BM=NG BN=GC BO=PV BP=SR BQ=OZ " +
        "BR=IH BS=WM BT=ZA BU=RI BV=YD BW=OC BX=DT BY=UU BZ=NL CA=HP CB=OJ CC=RD CD=FF CE=GM " +
        "CF=HD CG=IM CH=MA CI=HZ CJ=QR CK=QA CL=SZ CM=TK CN=LV CO=RK CP=LD CQ=NK CR=PD CS=ZP " +
        "CT=UO CU=XA CV=WU CW=RV CX=YT CY=VX CZ=LP DA=EZ DB=JQ DC=MJ DD=HO DE=RE DF=OK DG=QO " +
        "DH=OA DI=PH DJ=YH DK=QF DL=KQ DM=VQ DN=LR DP=HX DQ=MF DR=NA DS=KI DU=HI DV=MB DW=II " +
        "DX=UJ DY=ZE DZ=SL EA=JJ EB=LQ ED=KS EE=OQ EF=SN EG=ZU EI=XU EJ=YO EK=MR EL=TP EM=WD " +
        "EN=WJ EO=QH EP=RM EQ=PF ER=GF ES=ZM ET=TC EU=NZ EV=YB EW=VK EY=SH FB=UL FC=RZ FD=MP " +
        "FE=JS FH=SI FI=VG FJ=QL FK=TI FM=OI FN=PA FO=ZX FP=MH FQ=QP FR=RH FS=KA FU=VD FV=LA " +
        "FW=KU FX=JI FY=KM FZ=ML GA=XK GB=JL GD=MZ GE=LH GG=KO GH=ND GI=JU GJ=SA GK=IW GN=LL " +
        "GO=SU GP=MN GQ=WB GR=NV GS=KE GT=IE GU=ZI GV=HR GW=OM GX=RR GY=XP GZ=VO HA=KZ HC=WH " +
        "HE=KT HG=YL HH=OT HJ=QE HK=PJ HL=JE HM=NI HN=QW HQ=ZR HS=JZ HT=XS HU=RX HV=JC HW=KB " +
        "HY=IT IA=MU IB=QC IC=UP ID=NN IF=XE IG=MS IJ=LI IL=JX IN=VB IP=ZB IR=PS IS=TM IU=QK " +
        "IV=WQ IX=UC IY=UX IZ=VV JA=NF JB=ME JD=KX JG=TH JH=UE JK=SO JM=KD JO=ZW JP=RO JR=XX " +
        "JT=PP JV=YW JW=MG JY=VI KC=XB KF=LB KG=UI KH=MX KJ=VL KL=VW KN=UD KP=MQ KR=QI KV=WG " +
        "KW=NM KY=TN LC=WT LF=QJ LG=RW LJ=ZF LK=WA LM=RJ LN=RT LO=UH LS=NW LT=MC LU=YE LW=PR " +
        "LX=YV LY=TV LZ=OW MI=UQ MK=PT MM=TQ MO=NS MT=VN MV=YQ MW=WO MY=TF NB=VE NC=TW NE=SK " +
        "NH=XQ NJ=UB NO=RB NQ=PM NR=WK NT=YM NU=OS NX=ZK NY=QZ OB=UZ OD=YI OE=RG OF=WN OH=YX " +
        "OL=UV ON=TD OO=UT OP=TT OU=TJ OV=VC OX=VY OY=XT PB=XL PC=WF PE=WC PG=TO PI=UG PL=SV " +
        "PN=RU PO=XJ PQ=QU PU=ZY PX=VZ PY=VU PZ=XD QB=SD QD=WV QG=SY QQ=YJ QS=YF QT=XZ QV=SC " +
        "QX=VJ QY=XW RA=WI RC=SS RF=XG RL=ZD RN=XY RP=VA RQ=WS RS=UN RY=VP SE=TX SF=WL SG=ZH " +
        "SJ=XO SM=XR SP=ZT ST=YU SW=ZN SX=UW TB=YY TE=ZO TG=WE TL=YC TR=ZL TS=YK TU=ZS TY=ZJ " +
        "TZ=XF UA=VS UF=VR UK=VM US=XM UY=ZG VF=YN VH=WW WP=YR WR=XV WY=ZC WZ=YP XC=XN XI=ZV " +
        "YA=ZQ YG=YS";

    private const string MeerJEntries =
        "AA=YT AB=NM AC=GQ AD=MJ AE=SU AF=TP AG=XS AH=RB AI=UW AJ=PS AK=ZD AL=QO AM=FH AN=LS " +
        "AO=KM AP=OJ AQ=BJ AR=WL AS=JM AT=ID AU=HK AV=VR AW=DD AX=ET AY=UJ AZ=CK BA=UP BB=SD " +
        "BC=TJ BD=WQ BE=VK BF=OP BG=IH BH=QN BI=NR BK=ZL BL=XQ BM=RG BN=FQ BO=HT BP=IX BQ=PE " +
        "BR=YM BS=MO BT=LG BU=DW BV=JJ BW=CN BX=GB BY=KQ BZ=KG CA=WT CB=RK CC=SS CD=WF CE=PC " +
        "CF=YG CG=NH CH=TI CI=XW CJ=ZP CL=OT CM=QG CO=SL CP=UG CQ=YA CR=LJ CS=TM CT=JA CU=HD " +
        "CV=GO CW=FB CX=MG CY=EN CZ=VN DA=UM DB=OG DC=ND DE=QW DF=TN DG=RS DH=YN DI=KZ DJ=VW " +
        "DK=JH DL=FS DM=RA DN=GW DO=EZ DP=IC DQ=LX DR=MA DS=WJ DT=KF DU=EG DV=XI DX=SO DY=LP " +
        "DZ=HV EA=SJ EB=VF EC=QU ED=PZ EE=RT EF=VY EH=RQ EI=QM EJ=YR EK=NO EL=ZZ EM=RY EO=ZB " +
        "EP=SV EQ=RI ER=NK ES=LH EU=XY EV=TO EW=UZ EX=YY EY=WA FA=SZ FC=XV FD=RZ FE=OX FF=WX " +
        "FG=RU FI=UU FJ=QX FK=SG FL=TR FM=OD FN=TD FO=WC FP=RL FR=NC FT=QR FU=ZA FV=UC FW=PT " +
        "FX=UE FY=XA FZ=PN GA=TV GC=VV GD=WZ GE=OZ GF=RR GG=UT GH=SK GI=ZS GJ=RN GK=QQ GL=RP " +
        "GM=NA GN=YZ GP=YP GR=UX GS=PP GT=UB GU=WH GV=RF GX=YC GY=TA GZ=VA HA=TQ HB=SP HC=UR " +
        "HE=NQ HF=QV HG=LN HH=KS HI=MY HJ=IZ HL=NJ HM=ML HN=MQ HO=UA HP=OC HQ=VS HR=NW HS=PX " +
        "HU=MF HW=SX HX=VB HY=LB HZ=OV IA=RO IB=OQ IE=QF IF=SE IG=MU II=MB IJ=YK IK=OA IL=OK " +
        "IM=LY IN=ZU IO=MP IP=UI IQ=JG IR=PH IS=KD IT=NY IU=SQ IV=PB IW=LU IY=LD JB=LT JC=LW " +
        "JD=YH JE=LQ JF=KL JI=KO JK=LC JL=WO JN=PM JO=MN JP=VZ JQ=WD JR=OB JS=TT JT=LI JU=SA " +
        "JV=TE JW=KR JX=QH JY=MD JZ=NT KA=PO KB=QP KC=PG KE=OM KH=VM KI=LO KJ=XP KK=PQ KN=YJ " +
        "KP=NB KT=PI KU=LF KV=ZY KW=QZ KX=LL KY=SR LA=VO LE=RM LK=PA LM=QS LR=WG LV=UK LZ=SH " +
        "MC=OS ME=TW MH=SB MI=ZN MK=ZV MM=QK MR=PJ MS=ZK MT=WU MV=QT MW=NU MX=ZF MZ=UL NE=UH " +
        "NF=WS NG=VI NI=VQ NL=WK NN=XR NP=WW NS=XZ NV=RD NX=WE NZ=OE OF=XX OH=TB OI=UO OL=PY " +
        "ON=VT OO=XN OR=ZG OU=TF OW=ZQ OY=SC PD=RV PF=VU PK=YI PL=TG PR=TX PU=YL PV=TL PW=QI " +
        "QA=WN QB=VX QC=UV QD=XB QE=TU QJ=ZC QL=ZW QY=ZJ RC=UN RE=WM RH=WV RJ=TZ RW=VP RX=YW " +
        "SF=VD SI=YX SM=XK SN=WP ST=ZO SW=UY SY=XG TC=YV TH=XU TK=VJ TS=XE TY=ZR UD=XL UF=WY " +
        "UQ=ZM US=XC VC=YO VE=XO VG=XT VH=YF VL=XD WB=ZT WI=YS WR=ZH XF=YQ XH=YU XJ=ZI XM=YE " +
        "YB=ZX YD=ZE";

    private const string FlusslaufAEntries =
        "AA=BT AB=YY AC=MR AD=ZC AE=FJ AF=SI AG=PY AH=RM AI=AZ AJ=DB AK=RR AL=LA AM=CT AN=JG " +
        "AO=NU AP=MJ AQ=TT AR=MN AS=YC AT=ZP AU=EN AV=DV AW=UW AX=YQ AY=QF BA=YS BB=XN BC=DY " +
        "BD=ZU BE=OM BF=OR BG=CY BH=WL BI=CH BJ=LO BK=EV BL=EP BM=KV BN=GH BO=KS BP=EF BQ=RC " +
        "BR=SP BS=HE BU=PC BV=OW BW=DT BX=SL BY=KW BZ=EO CA=PZ CB=EH CC=ZV CD=YN CE=MA CF=WI " +
        "CG=WN CI=PO CJ=ET CK=OZ CL=LH CM=TR CN=EW CO=HY CP=ME CQ=GD CR=TN CS=YW CU=PF CV=FS " +
        "CW=IW CX=EI CZ=RG DA=XF DC=OJ DD=HT DE=XR DF=ML DG=HJ DH=DS DI=QZ DJ=KN DK=JP DL=SS " +
        "DM=OK DN=LN DO=IE DP=IY DQ=QV DR=NO DU=IO DW=YL DX=UJ DZ=YT EA=VZ EB=MB EC=YG ED=SZ " +
        "EE=PQ EG=ZN EJ=XB EK=XJ EL=KO EM=TK EQ=TC ER=SG ES=NJ EU=JN EX=GR EY=ZJ EZ=IT FA=ZI " +
        "FB=IM FC=MT FD=YE FE=YI FF=ZW FG=QX FH=VX FI=JQ FK=LQ FL=XQ FM=UU FN=PJ FO=YU FP=FU " +
        "FQ=WT FR=NY FT=ZZ FV=ON FW=XP FX=JX FY=IS FZ=TY GA=TQ GB=WH GC=WU GE=SK GF=LR GG=ZH " +
        "GI=QI GJ=WP GK=QC GL=ZK GM=WF GN=VY GO=LC GP=HW GQ=HV GS=RY GT=GV GU=OS GW=IN GX=RO " +
        "GY=YB GZ=OL HA=SJ HB=TE HC=ID HD=YR HF=RK HG=NX HH=NT HI=TI HK=UX HL=JH HM=UP HN=QB " +
        "HO=KA HP=KT HQ=RL HR=TZ HS=SF HU=SW HX=RN HZ=JK IA=VO IB=MZ IC=VA IF=US IG=RZ IH=PT " +
        "II=YM IJ=VW IK=LY IL=IQ IP=RQ IR=SA IU=VE IV=RT IX=XL IZ=SH JA=NV JB=OV JC=XY JD=TX " +
        "JE=LT JF=VG JI=WE JJ=TO JL=UZ JM=XV JO=UD JR=SC JS=XZ JT=RB JU=PX JV=UK JW=MO JY=YK " +
        "JZ=VP KB=OT KC=UG KD=OH KE=OE KF=PM KG=PN KH=VS KI=MC KJ=XH KK=NQ KL=VU KM=UA KP=QG " +
        "KQ=WK KR=ZX KU=MP KX=QU KY=NZ KZ=XE LB=NG LD=WY LE=VM LF=LZ LG=WC LI=XA LJ=SB LK=VK " +
        "LL=SN LM=UN LP=SO LS=MS LU=NF LV=ZS LW=OO LX=NN MD=TS MF=VD MG=VV MH=WR MI=PK MK=SQ " +
        "MM=RS MQ=RJ MU=RW MV=XK MW=RE MX=YH MY=UR NA=YA NB=RA NC=UY ND=UL NE=VL NH=OB NI=SY " +
        "NK=VF NL=RV NM=OP NP=VR NR=NS NW=TV OA=VN OC=PD OD=RX OF=PV OG=XD OI=RI OQ=YX OU=QS " +
        "OX=QO OY=ZR PA=YO PB=YJ PE=ZY PG=XS PH=RH PI=ZM PL=QY PP=SE PR=QA PS=XU PU=TB PW=UI " +
        "QD=ZG QE=QM QH=TF QJ=UE QK=UF QL=SD QN=SR QP=WV QQ=TG QR=TJ QT=UM QW=UQ RD=ZT RF=YD " +
        "RP=TP RU=ZQ SM=WJ ST=WS SU=XI SV=WQ SX=ZL TA=ZA TD=UO TH=XC TL=YP TM=TW TU=WW UB=WZ " +
        "UC=WO UH=YF UT=ZB UV=VQ VB=VJ VC=WB VH=XW VI=XM VT=XX WA=YZ WD=YV WG=XO WM=ZF WX=ZO " +
        "XG=XT ZD=ZE";

    private const string FlusslaufBEntries =
        "AA=IY AB=BQ AC=II AD=MW AE=MQ AF=RE AG=DO AH=DH AI=JK AJ=RF AK=WV AL=LJ AM=FV AN=PE " +
        "AO=GD AP=OQ AQ=OV AR=GB AS=CO AT=CZ AU=EV AV=YF AW=MY AX=XE AY=FJ AZ=ZD BA=WW BB=UD " +
        "BC=LK BD=WX BE=PL BF=VN BG=JV BH=CF BI=KJ BJ=HF BK=NY BL=CY BM=CK BN=NF BO=NK BP=UR " +
        "BR=EH BS=RJ BT=IS BU=NU BV=RZ BW=YN BX=YG BY=LE BZ=DP CA=UZ CB=NH CC=JO CD=JW CE=CH " +
        "CG=TL CI=DV CJ=LT CL=QB CM=GH CN=KI CP=SK CQ=SW CR=HY CS=VL CT=OM CU=XJ CV=DL CW=SA " +
        "CX=RD DA=RG DB=XY DC=FS DD=GU DE=PC DF=TP DG=MC DI=HD DJ=WY DK=UI DM=EL DN=TR DQ=DZ " +
        "DR=TE DS=UG DT=ZI DU=YT DW=TU DX=OY DY=EN EA=OU EB=FI EC=VQ ED=KH EE=RS EF=UP EG=PQ " +
        "EI=SM EJ=EQ EK=SN EM=TY EO=LY EP=YH ER=VX ES=XV ET=UN EU=SD EW=WH EX=PB EY=ZM EZ=QF " +
        "FA=XW FB=VS FC=VI FD=JE FE=KS FF=PY FG=ON FH=LH FK=SG FL=ZN FM=PF FN=HE FO=KC FP=RX " +
        "FQ=ZC FR=ZH FT=HW FU=UA FW=MS FX=OW FY=XF FZ=NQ GA=NM GC=UX GE=TM GF=GN GG=HZ GI=OE " +
        "GJ=MF GK=IF GL=RU GM=IP GO=ND GP=XH GQ=LB GR=ZW GS=OG GT=LI GV=KO GW=HT GX=VJ GY=ZB " +
        "GZ=ZK HA=XA HB=SR HC=LO HG=ZP HH=RO HI=YE HJ=NT HK=LP HL=YC HM=YZ HN=PS HO=ID HP=WZ " +
        "HQ=WP HR=LU HS=ZE HU=MA HV=NS HX=QH IA=SL IB=XO IC=SV IE=RH IG=MJ IH=OA IJ=PU IK=PN " +
        "IL=XT IM=KK IN=ZX IO=US IQ=OI IR=XX IT=SQ IU=OX IV=MM IW=LN IX=ZY IZ=NZ JA=SO JB=WR " +
        "JC=PP JD=SJ JF=UM JG=VV JH=TD JI=QK JJ=UV JL=TB JM=LV JN=KA JP=VE JQ=ZS JR=TQ JS=NB " +
        "JT=WC JU=MK JX=SU JY=MI JZ=WJ KB=OT KD=OS KE=QL KF=YB KG=ZL KL=ZO KM=YU KN=UQ KP=KY " +
        "KQ=XI KR=YQ KT=VT KU=SZ KV=OC KW=LC KX=TF KZ=YX LA=WN LD=YM LF=MZ LG=RA LL=RR LM=SI " +
        "LQ=PV LR=YY LS=VP LW=WT LX=TI LZ=QS MB=RL MD=VO ME=TX MG=NE MH=XC ML=TS MN=QN MO=SS " +
        "MP=WF MR=OR MT=QE MU=XR MV=ZU MX=WI NA=PX NC=XM NG=SH NI=WD NJ=RY NL=NO NN=UF NP=RB " +
        "NR=YA NV=ZT NW=QQ NX=VW OB=SE OD=RQ OF=QO OH=PR OJ=QR OK=XD OL=WS OO=TN OP=XZ OZ=SB " +
        "PA=PW PD=WE PG=YK PH=RK PI=ZR PJ=TO PK=QU PM=UT PO=VA PT=UE PZ=TZ QA=YR QC=RT QD=TW " +
        "QG=YW QI=VR QJ=QX QM=TA QP=SC QT=VU QV=XK QW=YL QY=RP QZ=WO RC=YS RI=TH RM=XS RN=RV " +
        "RW=ZF SF=VM SP=VC ST=XN SX=YJ SY=VB TC=ZZ TG=UK TJ=UL TK=XU TT=UH TV=XQ UB=VH UC=YP " +
        "UJ=WK UO=ZG UU=WU UW=WG UY=WM VD=WB VF=YI VG=WA VK=XG VY=ZV VZ=YO WL=YV WQ=XB XL=ZA " +
        "XP=YD ZJ=ZQ";

    private const string FlusslaufCEntries =
        "AA=IM AB=JQ AC=IE AD=LU AE=QC AF=FJ AG=NZ AH=EN AI=NI AJ=LY AK=VP AL=WA AM=IC AN=EI " +
        "AO=VI AP=HC AQ=ZM AR=TO AS=WT AT=WD AU=BE AV=NM AW=CW AX=RF AY=OU AZ=QH BA=OE BB=ZQ " +
        "BC=BI BD=NG BF=VW BG=VR BH=TZ BJ=FX BK=DZ BL=UI BM=JK BN=SH BO=SF BP=JS BQ=VZ BR=PP " +
        "BS=RY BT=YE BU=HD BV=LA BW=XV BX=HH BY=YN BZ=JD CA=FI CB=EW CC=FN CD=GT CE=XH CF=OD " +
        "CG=CV CH=LK CI=TF CJ=OG CK=SU CL=FQ CM=CQ CN=DN CO=IX CP=EH CR=GH CS=RM CT=XN CU=QD " +
        "CX=FY CY=PZ CZ=TY DA=KO DB=NN DC=FT DD=FF DE=YM DF=DI DG=ZS DH=DP DJ=OC DK=VN DL=XL " +
        "DM=MG DO=TH DQ=EF DR=GC DS=HA DT=ZU DU=KL DV=UD DW=QI DX=LM DY=GZ EA=WY EB=HG EC=VL " +
        "ED=KZ EE=SM EG=XJ EJ=WJ EK=GD EL=XS EM=XX EO=WQ EP=GM EQ=ZD ER=RC ES=JU ET=QT EU=QX " +
        "EV=TJ EX=SR EY=KH EZ=TT FA=FP FB=YA FC=UA FD=IA FE=TE FG=JE FH=RT FK=JF FL=MQ FM=LJ " +
        "FO=QU FR=OF FS=JP FU=LB FV=TQ FW=GS FZ=MX GA=WU GB=HO GE=XM GF=ZJ GG=UJ GI=TS GJ=ON " +
        "GK=GW GL=WR GN=UT GO=ZP GP=KS GQ=JY GR=WV GU=MJ GV=NT GX=RE GY=OI HB=NY HE=YS HF=KG " +
        "HI=HN HJ=KD HK=QP HL=ZL HM=TC HP=YU HQ=IS HR=XY HS=YY HT=QZ HU=RW HV=MD HW=IG HX=QR " +
        "HY=UO HZ=QG IB=VA ID=ZV IF=SG IH=LG II=IU IJ=ZY IK=SX IL=LO IN=OX IO=IT IP=MW IQ=KN " +
        "IR=XZ IV=UN IW=OQ IY=UG IZ=VJ JA=MK JB=NK JC=JJ JG=OO JH=SO JI=QE JL=WS JM=JZ JN=UQ " +
        "JO=XW JR=MM JT=RV JV=PS JW=NE JX=VY KA=OL KB=PY KC=ME KE=XU KF=UW KI=UL KJ=OA KK=YI " +
        "KM=SP KP=PU KQ=PO KR=NB KT=KU KV=MZ KW=WC KX=PV KY=ZT LC=UH LD=YK LE=PF LF=VX LH=PE " +
        "LI=PQ LL=NF LN=YW LP=QA LQ=YQ LR=SC LS=OZ LT=RK LV=NV LW=TK LX=SV LZ=OW MA=RL MB=ST " +
        "MC=PC MF=YV MH=UV MI=ZN ML=QV MN=ZB MO=UF MP=TP MR=NP MS=MT MU=TN MV=SB MY=TU NA=NJ " +
        "NC=XB ND=OP NH=WK NL=SD NO=SY NQ=TA NR=YG NS=RI NU=OH NW=XF NX=QO OB=TX OJ=XO OK=XC " +
        "OM=UC OR=PB OS=ZC OT=QQ OV=QB OY=SJ PA=QJ PD=RP PG=YL PH=ZX PI=PX PJ=UR PK=RD PL=PW " +
        "PM=VE PN=VU PR=RO PT=ZK QF=WI QK=TI QL=VS QM=WX QN=TV QS=VB QW=WM QY=YR RA=ZH RB=SE " +
        "RG=YP RH=XE RJ=ZI RN=TW RQ=WB RR=ZG RS=UZ RU=SK RX=YD RZ=YO SA=WH SI=WF SL=XI SN=UX " +
        "SQ=UB SS=WZ SW=YX SZ=US TB=WG TD=ZW TG=TL TM=VK TR=YT UE=WW UK=XR UM=ZZ UP=YB UU=YZ " +
        "UY=ZF VC=VH VD=ZA VF=XQ VG=YH VM=WE VO=YC VQ=XD VT=XP VV=XA WL=YF WN=ZR WO=XG WP=ZE " +
        "XK=YJ XT=ZO";

    private const string FlusslaufDEntries =
        "AA=RA AB=EU AC=DT AD=WV AE=SA AF=ZT AG=BZ AH=JD AI=VD AJ=QR AK=QP AL=LX AM=YQ AN=NL " +
        "AO=MJ AP=TF AQ=YZ AR=VG AS=DY AT=XM AU=FG AV=PB AW=QV AX=KL AY=EM AZ=KJ BA=ZV BB=YK " +
        "BC=FA BD=TO BE=DP BF=KD BG=PR BH=RT BI=SN BJ=JI BK=UC BL=WK BM=WN BN=KB BO=EG BP=CE " +
        "BQ=FR BR=WG BS=CO BT=OK BU=XI BV=JG BW=KK BX=GD BY=RV CA=SK CB=NQ CC=TN CD=HF CF=UF " +
        "CG=SV CH=TI CI=CW CJ=MY CK=VW CL=RN CM=UR CN=IA CP=WJ CQ=MZ CR=KS CS=PN CT=JH CU=GT " +
        "CV=NZ CX=NX CY=OU CZ=YM DA=LV DB=HP DC=OQ DD=VA DE=PL DF=MG DG=OW DH=NM DI=VQ DJ=GK " +
        "DK=IP DL=XO DM=UM DN=GE DO=EV DQ=FX DR=DX DS=RB DU=ZI DV=MR DW=WD DZ=GR EA=TM EB=MA " +
        "EC=XT ED=LZ EE=UD EF=WC EH=ZU EI=FN EJ=EL EK=PY EN=LI EO=QT EP=WB EQ=JB ER=QL ES=LT " +
        "ET=LH EW=HO EX=NJ EY=WQ EZ=UZ FB=TT FC=YV FD=RF FE=SD FF=ZN FH=QG FI=YT FJ=LU FK=NN " +
        "FL=MW FM=RY FO=UI FP=OE FQ=JQ FS=UP FT=JW FU=LW FV=OJ FW=LL FY=XX FZ=MV GA=GV GB=QZ " +
        "GC=WX GF=SX GG=NA GH=QA GI=QD GJ=VM GL=YP GM=NC GN=QF GO=PX GP=XN GQ=RX GS=HJ GU=MC " +
        "GW=IH GX=OC GY=KZ GZ=TW HA=PE HB=ZL HC=WZ HD=LK HE=VR HG=NW HH=QS HI=KG HK=SG HL=HS " +
        "HM=XP HN=QM HQ=ZA HR=IW HT=SJ HU=UQ HV=JA HW=TB HX=IC HY=IX HZ=SY IB=UA ID=SS IE=VU " +
        "IF=NR IG=VP II=KU IJ=RD IK=NK IL=ZZ IM=WU IN=KE IO=VS IQ=YF IR=SM IS=LD IT=LJ IU=JR " +
        "IV=KW IY=XC IZ=LQ JC=XU JE=VF JF=LN JJ=WS JK=MP JL=SB JM=TY JN=XQ JO=RS JP=NO JS=ZS " +
        "JT=UE JU=ZD JV=TZ JX=SR JY=YC JZ=OI KA=MB KC=XJ KF=VZ KH=UB KI=XD KM=US KN=VE KO=MX " +
        "KP=WT KQ=WH KR=YA KT=ZE KV=MI KX=WI KY=XE LA=PG LB=MQ LC=UO LE=YI LF=OO LG=PI LM=SP " +
        "LO=RE LP=ZF LR=PD LS=ZK LY=TD MD=OS ME=ZW MF=OT MH=ZM MK=RP ML=WM MM=SI MN=XG MO=VH " +
        "MS=TU MT=XL MU=NB ND=ZO NE=TJ NF=SQ NG=ZQ NH=SU NI=RJ NP=WO NS=YL NT=PT NU=ON NV=UL " +
        "NY=UJ OA=VL OB=UN OD=TP OF=WR OG=TE OH=QJ OL=PU OM=UG OP=SL OR=QE OV=WL OX=TC OY=SF " +
        "OZ=ST PA=XR PC=RC PF=TK PH=VJ PJ=XZ PK=TR PM=VX PO=PP PQ=QI PS=UY PV=VV PW=WP PZ=XY " +
        "QB=XH QC=YX QH=UT QK=VT QN=XW QO=UH QQ=WF QU=YE QW=RR QX=RZ QY=TX RG=WW RH=YG RI=RL " +
        "RK=SZ RM=VY RO=SO RQ=YY RU=UU RW=YO SC=TL SE=ZH SH=UK SW=XS TA=YR TG=XB TH=XA TQ=ZJ " +
        "TS=ZR TV=VO UV=WY UW=YJ UX=WA VB=VC VI=ZX VK=VN WE=XV XF=ZB XK=YU YB=YH YD=ZP YN=ZC " +
        "YS=ZY YW=ZG";

    private const string FlusslaufEEntries =
        "AA=ZB AB=RG AC=HI AD=FD AE=WP AF=ZN AG=WJ AH=AJ AI=KB AK=VA AL=TM AM=ZK AN=CS AO=ZP " +
        "AP=RB AQ=BH AR=TX AS=SC AT=KR AU=RP AV=LQ AW=OI AX=ZD AY=LG AZ=HH BA=QB BB=OF BC=HG " +
        "BD=TD BE=IT BF=VF BG=CK BI=JQ BJ=GI BK=NC BL=XY BM=MJ BN=UP BO=IW BP=NH BQ=LK BR=CI " +
        "BS=XP BT=MH BU=EP BV=OA BW=ZX BX=SQ BY=QL BZ=WB CA=HX CB=GE CC=YL CD=XO CE=CM CF=ZY " +
        "CG=YW CH=IR CJ=OO CL=PX CN=IF CO=XI CP=GX CQ=RW CR=WQ CT=IX CU=FJ CV=JH CW=ON CX=QH " +
        "CY=PZ CZ=NV DA=FA DB=GW DC=PA DD=SI DE=YP DF=ZJ DG=TH DH=IH DI=EH DJ=QS DK=EX DL=HA " +
        "DM=QA DN=EA DO=KT DP=EG DQ=EZ DR=HL DS=LU DT=TZ DU=FZ DV=EI DW=TE DX=UN DY=EY DZ=YC " +
        "EB=SF EC=WZ ED=HC EE=ZO EF=TQ EJ=RZ EK=QF EL=RI EM=JT EN=XM EO=HZ EQ=WM ER=GH ES=OP " +
        "ET=HR EU=RJ EV=IG EW=ZC FB=NQ FC=WC FE=KI FF=VP FG=SV FH=VM FI=IJ FK=VK FL=VB FM=GY " +
        "FN=LX FO=RT FP=UC FQ=HE FR=PH FS=NL FT=NW FU=HQ FV=ZU FW=RK FX=UJ FY=MB GA=GR GB=XD " +
        "GC=RY GD=MI GF=JL GG=XH GJ=YG GK=YT GL=WU GM=RH GN=LP GO=QT GP=LR GQ=TF GS=UE GT=VG " +
        "GU=OZ GV=ZQ GZ=QJ HB=VL HD=TY HF=ZF HJ=PS HK=TR HM=VX HN=OD HO=XG HP=XA HS=IB HT=IY " +
        "HU=ZM HV=LE HW=SY HY=MY IA=RO IC=QQ ID=XC IE=MX II=SG IK=PY IL=SA IM=RL IN=OL IO=OK " +
        "IP=SS IQ=TU IS=JI IU=LD IV=SH IZ=JS JA=VJ JB=UR JC=NJ JD=YA JE=ZZ JF=NZ JG=SR JJ=MU " +
        "JK=LJ JM=OT JN=UZ JO=VI JP=SK JR=KD JU=OH JV=WV JW=MO JX=OB JY=PU JZ=SD KA=SX KC=KN " +
        "KE=XB KF=KU KG=QI KH=XE KJ=UY KK=MK KL=NO KM=WL KO=OJ KP=LA KQ=UT KS=TT KV=TB KW=WG " +
        "KX=NU KY=RE KZ=NF LB=PQ LC=RA LF=VO LH=YJ LI=PI LL=RS LM=WX LN=TN LO=SU LS=YQ LT=ZA " +
        "LV=YX LW=YM LY=MD LZ=PE MA=RV MC=QD ME=QW MF=OG MG=UV ML=QR MM=QY MN=RU MP=PV MQ=PD " +
        "MR=UU MS=RQ MT=YF MV=UK MW=UQ MZ=VW NA=VD NB=NT ND=SZ NE=OR NG=XS NI=WI NK=SN NM=YK " +
        "NN=OM NP=ZW NR=RX NS=OX NX=WN NY=WD OC=ZG OE=XT OQ=YI OS=TC OU=QN OV=UF OW=UW OY=PM " +
        "PB=WR PC=ZT PF=YN PG=WE PJ=ST PK=RR PL=WA PN=VY PO=WT PP=UL PR=TA PT=VE PW=VC QC=YV " +
        "QE=RF QG=XK QK=UD QM=UI QO=ZI QP=SO QU=YO QV=ZR QX=TP QZ=VS RC=XF RD=RN RM=SB SE=ZV " +
        "SJ=ZS SL=VV SM=VH SP=XQ SW=VU TG=VZ TI=XW TJ=UX TK=ZL TL=TO TS=UM TV=US TW=YZ UA=VQ " +
        "UB=WS UG=XZ UH=WW UO=XX VN=WY VR=ZH VT=WF WH=XL WK=YE WO=YU XJ=YB XN=YS XR=YH XU=XV " +
        "YD=YY YR=ZE";

    private const string FlusslaufFEntries =
        "AA=XU AB=HU AC=RP AD=QS AE=XK AF=BT AG=CX AH=TZ AI=QD AJ=HK AK=LU AL=PL AM=TB AN=QQ " +
        "AO=HH AP=CG AQ=UV AR=KG AS=XD AT=KX AU=UN AV=RB AW=HM AX=YR AY=DX AZ=IS BA=JG BB=XP " +
        "BC=YA BD=IM BE=SI BF=BY BG=BO BH=DT BI=KP BJ=QW BK=YZ BL=GO BM=CU BN=YG BP=JP BQ=HW " +
        "BR=GW BS=SU BU=QG BV=EI BW=HF BX=NV BZ=NF CA=HY CB=IK CC=DL CD=CL CE=ML CF=GY CH=WA " +
        "CI=KR CJ=YK CK=ER CM=WC CN=HX CO=XM CP=OC CQ=IO CR=HS CS=RH CT=XT CV=MT CW=ZD CY=QV " +
        "CZ=KD DA=DS DB=GJ DC=XQ DD=NX DE=QU DF=NN DG=XC DH=HB DI=YS DJ=FQ DK=EC DM=TK DN=RE " +
        "DO=XO DP=ND DQ=PO DR=SW DU=SK DV=VX DW=ZS DY=VK DZ=NO EA=QJ EB=XS ED=RY EE=KZ EF=LR " +
        "EG=GV EH=GQ EJ=PH EK=HC EL=SG EM=JF EN=SD EO=FN EP=GB EQ=LF ES=MF ET=RU EU=VR EV=OI " +
        "EW=ZR EX=LV EY=WR EZ=PV FA=OG FB=HL FC=QK FD=IC FE=MD FF=TU FG=TA FH=MO FI=UU FJ=YQ " +
        "FK=GS FL=WP FM=ZE FO=IT FP=QO FR=ZH FS=UL FT=QY FU=JR FV=RS FW=WQ FX=OP FY=TS FZ=RO " +
        "GA=ZM GC=OU GD=GI GE=UI GF=TY GG=TV GH=YH GK=OZ GL=HT GM=OX GN=QF GP=HQ GR=PA GT=OS " +
        "GU=ZW GX=IZ GZ=PE HA=SX HD=ZB HE=XZ HG=JX HI=NH HJ=NJ HN=XH HO=UR HP=MN HR=PI HV=OL " +
        "HZ=RZ IA=OO IB=ZO ID=PN IE=LQ IF=VC IG=MP IH=UJ II=ZV IJ=VH IL=TC IN=KS IP=NU IQ=JO " +
        "IR=IU IV=LX IW=SB IX=XA IY=YV JA=NP JB=XJ JC=KV JD=YF JE=PX JH=RC JI=NQ JJ=WY JK=OA " +
        "JL=YY JM=PW JN=XF JQ=LB JS=MV JT=JY JU=LS JV=LM JW=SP JZ=KF KA=US KB=LK KC=PZ KE=WH " +
        "KH=YM KI=YI KJ=MA KK=RM KL=ZT KM=OE KN=VG KO=RJ KQ=NM KT=VA KU=WL KW=SM KY=LZ LA=VJ " +
        "LC=UD LD=ZQ LE=NA LG=PU LH=TL LI=RQ LJ=OK LL=VS LN=SY LO=TJ LP=YE LT=SJ LW=ZN LY=UK " +
        "MB=YT MC=SO ME=VP MG=YD MH=UW MI=PB MJ=UO MK=NK MM=RT MQ=SZ MR=OH MS=VM MU=MW MX=WU " +
        "MY=NG MZ=OY NB=ON NC=WN NE=ZF NI=RA NL=TI NR=XG NS=UA NT=QI NW=WJ NY=RK NZ=TG OB=XY " +
        "OD=YC OF=SF OJ=WS OM=UF OQ=YL OR=XE OT=YU OV=XI OW=UG PC=VN PD=QX PF=UY PG=WZ PJ=XB " +
        "PK=VE PM=PR PP=TF PQ=ZI PS=QB PT=SR PY=YW QA=VF QC=YJ QE=UE QH=XV QL=TH QM=QP QN=WV " +
        "QR=TX QT=WW QZ=YB RD=RI RF=UX RG=ZC RL=VV RN=YO RR=WE RV=TM RW=VQ RX=ZZ SA=WD SC=SS " +
        "SE=XR SH=WI SL=TP SN=UH SQ=WX ST=ZJ SV=ZY TD=ZX TE=XN TN=WT TO=VD TQ=WF TR=UM TT=UZ " +
        "TW=XL UB=VL UC=UT UP=ZU UQ=VW VB=WO VI=WG VO=ZA VT=XW VU=WB VY=YN VZ=ZP WK=XX WM=ZL " +
        "YP=YX ZG=ZK";

    private const string FlusslaufGEntries =
        "AA=EW AB=EO AC=DP AD=JY AE=DJ AF=OE AG=JL AH=RU AI=HN AJ=SO AK=DX AL=LX AM=DU AN=EZ " +
        "AO=YG AP=JH AQ=YY AR=WE AS=ZD AT=OA AU=ML AV=KP AW=JB AX=SE AY=KK AZ=JG BA=QY BB=MD " +
        "BC=QM BD=QT BE=PB BF=IE BG=QH BH=TP BI=RY BJ=DI BK=MZ BL=EV BM=MY BN=HM BO=DS BP=LO " +
        "BQ=QR BR=FZ BS=VD BT=FT BU=YX BV=WN BW=SG BX=IG BY=ZN BZ=YW CA=GN CB=KQ CC=QP CD=TB " +
        "CE=PI CF=TC CG=NP CH=HB CI=ZX CJ=TM CK=JK CL=QB CM=JM CN=VG CO=NL CP=CY CQ=XP CR=JA " +
        "CS=JT CT=MO CU=GY CV=YL CW=UJ CX=QO CZ=VF DA=DY DB=GG DC=JP DD=MF DE=ZL DF=KT DG=SR " +
        "DH=VN DK=QC DL=VA DM=WS DN=HG DO=YM DQ=JR DR=IZ DT=TQ DV=XW DW=UK DZ=XB EA=NV EB=LY " +
        "EC=XL ED=WO EE=IA EF=XC EG=TH EH=IF EI=YD EJ=ZS EK=YK EL=NB EM=QN EN=RI EP=WD EQ=QW " +
        "ER=VR ES=SA ET=ZB EU=XK EX=SQ EY=NY FA=TF FB=TT FC=QG FD=OP FE=TL FF=RX FG=WT FH=WZ " +
        "FI=UF FJ=VX FK=GK FL=QQ FM=RL FN=PJ FO=PH FP=MN FQ=XV FR=NZ FS=YQ FU=IO FV=SL FW=KN " +
        "FX=SN FY=XR GA=HC GB=LJ GC=LA GD=PM GE=PN GF=VY GH=JN GI=XJ GJ=RM GL=ZQ GM=LS GO=MH " +
        "GP=GR GQ=OH GS=YV GT=IT GU=NK GV=RF GW=PV GX=RP GZ=YZ HA=KU HD=IV HE=SM HF=XA HH=RK " +
        "HI=SY HJ=UM HK=QX HL=QD HO=VE HP=YE HQ=TU HR=NG HS=RV HT=TE HU=WR HV=SU HW=SI HX=KV " +
        "HY=RE HZ=LP IB=NS IC=UL ID=LV IH=YS II=NJ IJ=WV IK=XN IL=UN IM=RB IN=RH IP=XZ IQ=WY " +
        "IR=SC IS=OL IU=PS IW=ZM IX=OC IY=OD JC=WU JD=WF JE=KC JF=KW JI=NE JJ=OT JO=RC JQ=UW " +
        "JS=UD JU=KD JV=WH JW=PX JX=UV JZ=UQ KA=RQ KB=MW KE=ZT KF=LR KG=TS KH=LZ KI=NT KJ=YI " +
        "KL=SF KM=NC KO=ZJ KR=VH KS=LC KX=VC KY=LM KZ=VM LB=TD LD=RT LE=XM LF=NQ LG=WC LH=UG " +
        "LI=SV LK=ZV LL=XT LN=VK LQ=PG LT=TZ LU=MC LW=VB MA=VV MB=XO ME=UU MG=SX MI=VW MJ=YN " +
        "MK=YF MM=ST MP=NO MQ=WB MR=QV MS=PF MT=PR MU=WL MV=OB MX=VQ NA=OF ND=RN NF=UB NH=UZ " +
        "NI=PD NM=TJ NN=PO NR=XD NU=XF NW=SD NX=UY OG=OJ OI=QS OK=TV OM=XG ON=YT OO=RO OQ=PU " +
        "OR=XS OS=XU OU=UT OV=QF OW=VI OX=YJ OY=ZA OZ=TX PA=TK PC=ZW PE=RG PK=TR PL=UI PP=QU " +
        "PQ=UR PT=YO PW=WX PY=XQ PZ=RJ QA=SK QE=WI QI=QJ QK=ZK QL=VT QZ=XY RA=TA RD=US RR=VZ " +
        "RS=WJ RW=WQ RZ=VL SB=ZP SH=XE SJ=YR SP=VS SS=YP SW=VU SZ=WG TG=ZY TI=UE TN=TO TW=ZU " +
        "TY=UP UA=YC UC=YB UH=XH UO=ZH UX=VJ VO=ZR VP=WA WK=ZC WM=ZZ WP=ZF WW=ZE XI=ZG XX=ZI " +
        "YA=ZO YH=YU";

    private const string FlusslaufHEntries =
        "AA=IY AB=PI AC=TI AD=MN AE=CA AF=XD AG=SG AH=PG AI=PE AJ=KU AK=XB AL=UF AM=PJ AN=RB " +
        "AO=EW AP=YF AQ=TK AR=VW AS=BQ AT=KA AU=VM AV=WX AW=VX AX=UX AY=NC AZ=QM BA=FX BB=JX " +
        "BC=TU BD=HH BE=TW BF=RT BG=MI BH=XA BI=RK BJ=XS BK=MP BL=YY BM=PS BN=FI BO=OX BP=XH " +
        "BR=NO BS=RU BT=SL BU=FP BV=DN BW=VN BX=EV BY=ZC BZ=WT CB=EI CC=TX CD=XX CE=HN CF=UM " +
        "CG=XG CH=YR CI=ZS CJ=PC CK=ZH CL=ME CM=LH CN=TB CO=VO CP=MC CQ=GD CR=FQ CS=UL CT=TR " +
        "CU=MW CV=LW CW=LP CX=UQ CY=UA CZ=QJ DA=OO DB=ZG DC=WV DD=KT DE=ZB DF=YO DG=VB DH=ID " +
        "DI=DL DJ=DP DK=EJ DM=NK DO=FE DQ=QL DR=JT DS=IE DT=MS DU=VZ DV=ZA DW=SU DX=TE DY=PA " +
        "DZ=OL EA=VJ EB=JW EC=GV ED=UO EE=OD EF=NR EG=MF EH=ZZ EK=XZ EL=ER EM=JG EN=HQ EO=XJ " +
        "EP=EZ EQ=KE ES=JI ET=IM EU=XP EX=JH EY=QS FA=SQ FB=WD FC=FO FD=YZ FF=TC FG=US FH=SK " +
        "FJ=LB FK=LT FL=HK FM=SE FN=HR FR=XT FS=VD FT=YP FU=QO FV=ZP FW=YI FY=KS FZ=KI GA=PY " +
        "GB=QR GC=MJ GE=UB GF=UU GG=HA GH=TV GI=MT GJ=ZD GK=JR GL=KV GM=SD GN=PR GO=VR GP=IR " +
        "GQ=LD GR=IV GS=YC GT=LL GU=PM GW=HM GX=OP GY=JA GZ=JN HB=YL HC=NV HD=TY HE=PN HF=QN " +
        "HG=UD HI=KF HJ=IT HL=TM HO=OK HP=UE HS=PF HT=QU HU=LX HV=HX HW=WB HY=IH HZ=SH IA=ZU " +
        "IB=XE IC=NG IF=JP IG=QP II=MX IJ=VC IK=NT IL=ST IN=KH IO=KZ IP=NL IQ=PH IS=LJ IU=QX " +
        "IW=PT IX=WI IZ=OY JB=SO JC=WE JD=MK JE=VL JF=ZY JJ=MQ JK=LA JL=PW JM=PB JO=OW JQ=RN " +
        "JS=QC JU=QF JV=SM JY=RX JZ=WC KB=WR KC=TJ KD=VI KG=WZ KJ=RY KK=YE KL=NE KM=MM KN=QK " +
        "KO=KW KP=RG KQ=TO KR=SJ KX=QD KY=TF LC=LM LE=NM LF=LS LG=WM LI=ON LK=RJ LN=YS LO=VU " +
        "LQ=MO LR=PD LU=PO LV=VP LY=OB LZ=TD MA=UT MB=NJ MD=VQ MG=ZO MH=VK ML=NF MR=QQ MU=WH " +
        "MV=TA MY=SV MZ=YB NA=NQ NB=TP ND=PL NH=VY NI=TZ NN=RL NP=VA NS=TL NU=RW NW=QA NX=NY " +
        "NZ=XW OA=XL OC=UC OE=XC OF=ZW OG=VS OH=YH OI=SB OJ=YK OM=VT OQ=SW OR=YA OS=VF OT=PK " +
        "OU=RV OV=RP OZ=ZI PP=YQ PQ=QT PU=XF PV=QE PX=RI PZ=UV QB=RQ QG=ZN QH=UJ QI=YG QV=ZV " +
        "QW=XU QY=SZ QZ=ZF RA=ZL RC=RF RD=WY RE=WK RH=XO RM=ZR RO=WG RR=ZE RS=TS RZ=YN SA=SN " +
        "SC=XQ SF=YW SI=WA SP=YT SR=UP SS=UK SX=WP SY=VG TG=TT TH=VH TN=XV TQ=XR UG=VE UH=YX " +
        "UI=ZJ UN=YM UR=WQ UW=XM UY=ZM UZ=ZT VV=YD WF=WN WJ=ZX WL=WW WO=XI WS=ZQ WU=YV XK=YJ " +
        "XN=ZK XY=YU";

    private const string FlusslaufJEntries =
        "AA=DA AB=EF AC=DF AD=UC AE=CQ AF=CD AG=HX AH=RV AI=RL AJ=DN AK=KY AL=DY AM=FV AN=TH " +
        "AO=FS AP=BR AQ=BH AR=CR AS=AZ AT=FA AU=WV AV=IG AW=KT AX=OH AY=HY BA=EI BB=EB BC=JU " +
        "BD=CT BE=ZJ BF=ZL BG=LA BI=NS BJ=NT BK=FB BL=ZB BM=EH BN=OR BO=JO BP=OC BQ=KL BS=LJ " +
        "BT=QH BU=VD BV=SB BW=ZF BX=CX BY=CN BZ=SY CA=UH CB=TG CC=GW CE=XR CF=HC CG=TX CH=UQ " +
        "CI=OG CJ=RX CK=UW CL=GE CM=YG CO=PI CP=XD CS=HO CU=HS CV=IP CW=WE CY=SC CZ=SG DB=RT " +
        "DC=DP DD=EN DE=TW DG=WY DH=TO DI=SN DJ=OI DK=EL DL=HD DM=QG DO=PE DQ=PD DR=SQ DS=GH " +
        "DT=GX DU=TU DV=ER DW=RZ DX=TS DZ=QF EA=FF EC=OQ ED=VE EE=WP EG=LH EJ=TJ EK=HU EM=JQ " +
        "EO=TR EP=LG EQ=QP ES=OP ET=JX EU=HP EV=WJ EW=HZ EX=ZD EY=TP EZ=QL FC=ND FD=VG FE=XP " +
        "FG=ZZ FH=MC FI=QM FJ=TL FK=FR FL=YJ FM=XN FN=MI FO=LI FP=GR FQ=IF FT=HK FU=MU FW=NZ " +
        "FX=QJ FY=SZ FZ=IW GA=JW GB=WF GC=OW GD=RA GF=KI GG=XB GI=VT GJ=KP GK=OX GL=UB GM=OK " +
        "GN=MH GO=NL GP=LF GQ=ZR GS=YZ GT=ZH GU=VC GV=JA GY=VY GZ=ST HA=WZ HB=NX HE=XL HF=TB " +
        "HG=PZ HH=KQ HI=XI HJ=NH HL=SI HM=MP HN=KF HQ=WH HR=NK HT=QQ HV=ZK HW=YL IA=YM IB=ZO " +
        "IC=NM ID=QA IE=UX IH=XW II=SJ IJ=OZ IK=IN IL=LK IM=YD IO=OB IQ=LC IR=MF IS=PY IT=ZS " +
        "IU=RO IV=QT IX=SL IY=ZC IZ=ZX JB=PX JC=RD JD=MW JE=KW JF=NF JG=YO JH=XF JI=NC JJ=ON " +
        "JK=JZ JL=KU JM=VN JN=TK JP=WR JR=UF JS=XZ JT=YY JV=LX JY=SS KA=UI KB=NI KC=KG KD=QX " +
        "KE=OY KH=PL KJ=UD KK=UG KM=UM KN=LW KO=UY KR=PF KS=XH KV=TQ KX=YK KZ=YU LB=RF LD=LO " +
        "LE=RI LL=NB LM=TN LN=TE LP=YW LQ=MD LR=MK LS=OV LT=ZV LU=UZ LV=NU LY=PH LZ=XA MA=RB " +
        "MB=RC ME=WA MG=QV MJ=PK ML=SK MM=WW MN=QK MO=WX MQ=XC MR=XK MS=QN MT=NO MV=YC MX=PO " +
        "MY=ZA MZ=WO NA=VO NE=PP NG=XX NJ=XJ NN=OS NP=PU NQ=WC NR=WD NV=ZG NW=PC NY=QB OA=XM " +
        "OD=WG OE=SH OF=YR OJ=VR OL=YA OM=QE OO=VA OT=SR OU=SP PA=YB PB=UJ PG=PN PJ=RK PM=QI " +
        "PQ=ZM PR=XS PS=VB PT=UO PV=QD PW=WK QC=SO QO=WT QR=YS QS=YF QU=RG QW=WL QY=UL QZ=VZ " +
        "RE=RJ RH=SD RM=UR RN=WI RP=TZ RQ=VX RR=SV RS=UK RU=XO RW=WQ RY=SM SA=UN SE=YE SF=TD " +
        "SU=UE SW=US SX=TC TA=ZN TF=XQ TI=VP TM=VK TT=XG TV=ZT TY=VS UA=ZP UP=VF UT=ZY UU=WS " +
        "UV=VI VH=YN VJ=VM VL=YP VQ=VU VV=ZE VW=XY WB=ZI WM=YI WN=YQ WU=YX XE=ZQ XT=XV XU=ZU " +
        "YH=YV YT=ZW";

    private const string FlusslaufKEntries =
        "AA=TD AB=HE AC=IK AD=RD AE=PD AF=DE AG=IN AH=WW AI=TP AJ=XN AK=FQ AL=KW AM=UM AN=UN " +
        "AO=ZJ AP=DF AQ=CO AR=HF AS=SH AT=NB AU=UR AV=SF AW=PA AX=FE AY=MN AZ=HM BA=QY BB=FI " +
        "BC=YK BD=XT BE=LM BF=VP BG=YN BH=OW BI=TV BJ=XS BK=XV BL=QV BM=YV BN=WH BO=FG BP=QG " +
        "BQ=FB BR=CX BS=ES BT=YM BU=FD BV=RL BW=SB BX=FT BY=CT BZ=DC CA=LR CB=GJ CC=FJ CD=OK " +
        "CE=DH CF=IM CG=IY CH=DT CI=LC CJ=IU CK=HQ CL=XC CM=IR CN=LS CP=RR CQ=WM CR=JZ CS=SD " +
        "CU=PV CV=GE CW=NN CY=SP CZ=WZ DA=QT DB=UB DD=VM DG=MM DI=SZ DJ=GD DK=XH DL=RA DM=EX " +
        "DN=JR DO=PO DP=KX DQ=RP DR=JY DS=KJ DU=LF DV=PS DW=PN DX=XF DY=MT DZ=XG EA=FP EB=QX " +
        "EC=YC ED=PF EE=TW EF=GI EG=VC EH=LA EI=ST EJ=JW EK=OZ EL=PP EM=VQ EN=UL EO=XJ EP=QR " +
        "EQ=SA ER=QI ET=FN EU=UO EV=JV EW=TC EY=JK EZ=XW FA=LI FC=ZW FF=XM FH=PM FK=VX FL=YB " +
        "FM=OJ FO=VY FR=ZY FS=SU FU=ZX FV=IZ FW=MB FX=XK FY=HC FZ=LE GA=VH GB=ZK GC=SW GF=KA " +
        "GG=YW GH=LD GK=OC GL=MI GM=RC GN=OA GO=NX GP=IC GQ=WR GR=ZB GS=ZR GT=SV GU=PY GV=HN " +
        "GW=ZO GX=NP GY=YY GZ=HZ HA=WX HB=KP HD=KT HG=NS HH=ZG HI=SO HJ=LG HK=HU HL=OP HO=XZ " +
        "HP=RU HR=YR HS=KL HT=QC HV=RO HW=VE HX=UJ HY=KZ IA=RB IB=ZU ID=ON IE=JD IF=JH IG=WD " +
        "IH=TF II=SQ IJ=QK IL=IS IO=TU IP=VT IQ=QS IT=KV IV=ZZ IW=NY IX=TX JA=JN JB=WV JC=PZ " +
        "JE=TK JF=YO JG=RN JI=XY JJ=PQ JL=TI JM=YF JO=RG JP=QZ JQ=MO JS=OR JT=NA JU=XR JX=WC " +
        "KB=UW KC=ZT KD=NH KE=NI KF=LW KG=NO KH=RZ KI=PX KK=NJ KM=LX KN=ZA KO=RY KQ=WY KR=YZ " +
        "KS=UG KU=UP KY=YU LB=LN LH=RV LJ=TE LK=SJ LL=MR LO=WP LP=LT LQ=ZS LU=OV LV=RH LY=YA " +
        "LZ=WJ MA=OH MC=WK MD=QE ME=PU MF=WF MG=NM MH=RI MJ=RK MK=ZL ML=WQ MP=VD MQ=QL MS=XE " +
        "MU=WN MV=ZH MW=VL MX=QN MY=WG MZ=OQ NC=YE ND=QF NE=RE NF=QB NG=YL NK=TJ NL=SL NQ=YP " +
        "NR=XQ NT=RW NU=UK NV=SS NW=TB NZ=YJ OB=XU OD=VF OE=UI OF=VA OG=OM OI=YI OL=OT OO=WB " +
        "OS=VN OU=VO OX=UC OY=RX PB=VB PC=US PE=VW PG=SR PH=QJ PI=PT PJ=RT PK=SY PL=UQ PR=YD " +
        "PW=UE QA=TS QD=QM QH=UT QO=TQ QP=ZC QQ=WO QU=WE QW=VS RF=ZN RJ=UV RM=UA RQ=XA RS=XP " +
        "SC=VU SE=ZP SG=XX SI=VJ SK=TZ SM=TY SN=UH SX=WS TA=VV TG=TM TH=ZF TL=ZI TN=TR TO=UU " +
        "TT=UF UD=ZV UX=WL UY=XB UZ=VI VG=YS VK=YH VR=YG VZ=WU WA=XD WI=YQ WT=ZE XI=XO XL=YT " +
        "YX=ZM ZD=ZQ";

    private const string FlusslaufLEntries =
        "AA=RG AB=LH AC=QZ AD=ZA AE=ZZ AF=JA AG=TQ AH=RK AI=PC AJ=CW AK=NU AL=YP AM=KG AN=UW " +
        "AO=YH AP=TK AQ=KM AR=LW AS=VI AT=WJ AU=VA AV=LU AW=YN AX=XY AY=PF AZ=KN BA=VP BB=FC " +
        "BC=MB BD=TP BE=LF BF=HQ BG=IN BH=PY BI=FS BJ=UP BK=SW BL=RM BM=JK BN=PN BO=SM BP=GE " +
        "BQ=KR BR=XA BS=UO BT=JW BU=MJ BV=ZI BW=RR BX=SS BY=RV BZ=RH CA=TO CB=RI CC=IO CD=LO " +
        "CE=GH CF=IY CG=LG CH=OJ CI=TH CJ=JR CK=PQ CL=EN CM=EC CN=MQ CO=NS CP=RX CQ=GV CR=YT " +
        "CS=RN CT=XT CU=MG CV=QC CX=HG CY=WZ CZ=DG DA=RA DB=YS DC=IB DD=YZ DE=PU DF=IM DH=YV " +
        "DI=SQ DJ=MY DK=UI DL=YG DM=FD DN=EG DO=NW DP=ZK DQ=JB DR=OM DS=KP DT=GA DU=DX DV=UY " +
        "DW=RD DY=MZ DZ=GU EA=YR EB=TB ED=PT EE=VH EF=HA EH=KK EI=LR EJ=ST EK=EV EL=GD EM=FT " +
        "EO=UQ EP=JQ EQ=HX ER=HK ES=HF ET=LS EU=LY EW=YO EX=UK EY=FU EZ=PW FA=WQ FB=JM FE=GR " +
        "FF=FP FG=KJ FH=ZC FI=KL FJ=OY FK=UV FL=SU FM=WY FN=LA FO=IA FQ=JV FR=VT FV=MP FW=JF " +
        "FX=QD FY=IL FZ=OH GB=NG GC=OS GF=SX GG=SR GI=ZP GJ=VB GK=JT GL=RO GM=NR GN=OO GO=YX " +
        "GP=VZ GQ=KA GS=ZD GT=OV GW=JE GX=YI GY=QS GZ=JP HB=MU HC=RY HD=QV HE=IW HH=MS HI=OP " +
        "HJ=MN HL=QG HM=UZ HN=PV HO=ND HP=TD HR=XW HS=QN HT=TS HU=ZN HV=XS HW=WE HY=IF HZ=KS " +
        "IC=MT ID=JX IE=PR IG=YQ IH=PM II=PE IJ=QR IK=QL IP=XZ IQ=MH IR=JZ IS=MX IT=ZY IU=XM " +
        "IV=KD IX=RE IZ=TT JC=WM JD=KZ JG=WU JH=WN JI=WT JJ=UF JL=NQ JN=SE JO=VV JS=OL JU=ZF " +
        "JY=WW KB=TM KC=WX KE=ME KF=MC KH=UN KI=LQ KO=UE KQ=KV KT=LN KU=SG KW=XU KX=LD KY=PS " +
        "LB=QQ LC=YB LE=OW LI=XX LJ=QP LK=RT LL=MF LM=WB LP=WK LT=VL LV=PD LX=YL LZ=NL MA=WV " +
        "MD=ON MI=TA MK=TE ML=PI MM=VE MO=SZ MR=XP MV=RQ MW=VC NA=QA NB=SV NC=YW NE=SN NF=XE " +
        "NH=QW NI=WA NJ=OF NK=QB NM=TG NN=OB NO=UG NP=QH NT=TC NV=SB NX=YY NY=TL NZ=PZ OA=XC " +
        "OC=RB OD=WG OE=ZV OG=OX OI=XR OK=TR OQ=OT OR=TN OU=RC OZ=VQ PA=VD PB=ZS PG=SY PH=WR " +
        "PJ=XB PK=XV PL=TX PO=VU PP=RW PX=WC QE=UR QF=ZO QI=ZU QJ=TZ QK=UD QM=RS QO=WI QT=YM " +
        "QU=XI QX=TW QY=YD RF=XF RJ=XJ RL=WH RP=ZJ RU=WS RZ=VJ SA=TU SC=UA SD=SH SF=TJ SI=UH " +
        "SJ=VY SK=VW SL=UT SO=ZW SP=TF TI=TY TV=XN UB=WF UC=YC UJ=VO UL=ZE UM=VR US=YU UU=YK " +
        "UX=ZM VF=ZX VG=XQ VK=ZH VM=YA VN=ZT VS=XK VX=YE WD=XL WL=ZR WO=XD WP=ZG XG=YJ XH=ZQ " +
        "XO=ZB YF=ZL";

    private const string FlusslaufMEntries =
        "AA=XF AB=GF AC=FV AD=IF AE=VO AF=GT AG=XV AH=DF AI=BU AJ=BK AK=ER AL=GX AM=TF AN=AO " +
        "AP=MV AQ=CU AR=LI AS=VZ AT=FA AU=ES AV=UO AW=GE AX=LD AY=GG AZ=GJ BA=QB BB=RS BC=LR " +
        "BD=VH BE=CP BF=US BG=CD BH=HE BI=FE BJ=MC BL=KB BM=GN BN=LA BO=SH BP=JO BQ=PI BR=ND " +
        "BS=VI BT=TA BV=RP BW=JR BX=QG BY=MW BZ=YE CA=QP CB=LB CC=JF CE=SX CF=UM CG=VR CH=YQ " +
        "CI=IU CJ=IX CK=PX CL=DS CM=EI CN=PV CO=UH CQ=ZT CR=NC CS=IP CT=RA CV=RX CW=XC CX=IJ " +
        "CY=DP CZ=KE DA=TR DB=JI DC=SW DD=WO DE=XQ DG=WR DH=OT DI=YX DJ=LK DK=TB DL=WG DM=QC " +
        "DN=KP DO=PT DQ=ZV DR=EK DT=JC DU=KY DV=NE DW=GI DX=KN DY=WY DZ=MB EA=EN EB=KH EC=FF " +
        "ED=JJ EE=KX EF=HL EG=YK EH=TP EJ=VQ EL=FY EM=TZ EO=GB EP=FM EQ=IB ET=GW EU=NS EV=GP " +
        "EW=RE EX=MN EY=GH EZ=PY FB=GA FC=TQ FD=RL FG=XY FH=JZ FI=NX FJ=RD FK=VK FL=QU FN=NO " +
        "FO=VW FP=RU FQ=VX FR=GK FS=PH FT=LX FU=WA FW=ZU FX=ML FZ=LZ GC=TT GD=SP GL=PP GM=OI " +
        "GO=NU GQ=OV GR=MZ GS=SA GU=UV GV=KU GY=NL GZ=LU HA=HY HB=IO HC=HX HD=WM HF=VL HG=QN " +
        "HH=MP HI=XT HJ=HS HK=WF HM=MY HN=OY HO=OP HP=XN HQ=WP HR=VA HT=KL HU=YA HV=SU HW=TH " +
        "HZ=KT IA=QF IC=TY ID=NG IE=RR IG=QY IH=ON II=KW IK=UW IL=JM IM=QD IN=TS IQ=YM IR=TX " +
        "IS=YU IT=VY IV=PK IW=UZ IY=XP IZ=QO JA=ZW JB=UA JD=UX JE=YO JG=QI JH=JP JK=PA JL=PR " +
        "JN=YR JQ=OB JS=JT JU=YS JV=OF JW=LF JX=QR JY=PL KA=RJ KC=PW KD=UU KF=ZM KG=ZP KI=VG " +
        "KJ=SK KK=OR KM=SL KO=ST KQ=PN KR=RH KS=ZS KV=QL KZ=MI LC=TV LE=SO LG=TG LH=YL LJ=UQ " +
        "LL=YP LM=XL LN=ZJ LO=XU LP=ZE LQ=RZ LS=YG LT=MX LV=UI LW=MR LY=RF MA=XX MD=TW ME=OG " +
        "MF=UD MG=RT MH=ZD MJ=PD MK=NB MM=YC MO=SC MQ=NQ MS=YI MT=OQ MU=VC NA=QW NF=XJ NH=YT " +
        "NI=SE NJ=QZ NK=SF NM=ZB NN=YZ NP=VN NR=XW NT=TO NV=OJ NW=SN NY=TJ NZ=WI OA=QT OC=XI " +
        "OD=UR OE=OL OH=UG OK=TN OM=RM OO=PF OS=VS OU=RY OW=TD OX=PM OZ=SM PB=RB PC=PJ PE=UN " +
        "PG=XB PO=YN PQ=YB PS=VP PU=UY PZ=WX QA=XZ QE=XO QH=RG QJ=YJ QK=XA QM=RW QQ=RK QS=XH " +
        "QV=XS QX=ZX RC=VT RI=ZI RN=VJ RO=XR RQ=WN RV=SR SB=WT SD=VV SG=YD SI=UB SJ=WV SQ=SZ " +
        "SS=WL SV=VD SY=XD TC=ZR TE=VB TI=WD TK=WE TL=UE TM=ZG TU=ZC UC=UP UF=YH UJ=WJ UK=XG " +
        "UL=VF UT=ZA VE=XK VM=ZN VU=ZO WB=ZY WC=WZ WH=ZL WK=WS WQ=YF WU=ZH WW=ZQ XE=YV XM=ZF " +
        "YW=ZZ YY=ZK";

    private const string FlusslaufNEntries =
        "AA=FG AB=QF AC=NB AD=XC AE=ZL AF=MU AG=XS AH=CQ AI=OJ AJ=ET AK=SX AL=UI AM=MF AN=DZ " +
        "AO=WY AP=DM AQ=CV AR=TV AS=RF AT=TO AU=GS AV=FV AW=EI AX=JO AY=OT AZ=TN BA=SO BB=TA " +
        "BC=IB BD=SM BE=NU BF=BI BG=LV BH=EZ BJ=JN BK=EV BL=IT BM=WU BN=FN BO=EL BP=FL BQ=EK " +
        "BR=QG BS=LW BT=KK BU=NK BV=GX BW=JJ BX=MV BY=WV BZ=PF CA=NE CB=OR CC=DF CD=TH CE=DL " +
        "CF=MN CG=ON CH=QS CI=YI CJ=FX CK=RE CL=KU CM=HY CN=HA CO=SQ CP=YT CR=QL CS=ND CT=UA " +
        "CU=KW CW=DD CX=DS CY=MY CZ=JE DA=MS DB=PZ DC=SG DE=KQ DG=LM DH=VE DI=KG DJ=XP DK=TS " +
        "DN=JS DO=VB DP=SE DQ=PH DR=WC DT=TD DU=GU DV=LT DW=WJ DX=ZD DY=HW EA=TQ EB=NH EC=UO " +
        "ED=JU EE=WK EF=EN EG=KF EH=TW EJ=IG EM=KI EO=LA EP=OP EQ=KX ER=GC ES=JR EU=RV EW=HB " +
        "EX=QO EY=XB FA=MT FB=HH FC=VD FD=ZR FE=GL FF=YX FH=WI FI=ZP FJ=KA FK=XX FM=YW FO=TL " +
        "FP=ZM FQ=OQ FR=YG FS=LE FT=PS FU=WP FW=GJ FY=NI FZ=IP GA=VF GB=UZ GD=IS GE=PY GF=MI " +
        "GG=JT GH=PU GI=LX GK=UV GM=TR GN=JF GO=JP GP=SJ GQ=XZ GR=XH GT=IY GV=JW GW=LG GY=UP " +
        "GZ=JX HC=VS HD=LZ HE=YH HF=NT HG=WS HI=NC HJ=SF HK=JQ HL=NV HM=ZA HN=ZI HO=WA HP=LI " +
        "HQ=NJ HR=TI HS=OC HT=NY HU=YC HV=WO HX=WX HZ=OL IA=ZQ IC=JY ID=IL IE=ZG IF=UY IH=IU " +
        "II=RJ IJ=IZ IK=UJ IM=RO IN=QE IO=XW IQ=OV IR=ZS IV=WZ IW=QA IX=SK JA=US JB=PI JC=VG " +
        "JD=YL JG=SZ JH=ZT JI=TF JK=WE JL=NO JM=YY JV=YM JZ=PL KB=QX KC=UF KD=PP KE=WM KH=XF " +
        "KJ=SD KL=XG KM=WT KN=LK KO=LP KP=YA KR=LJ KS=YV KT=PC KV=UQ KY=OI KZ=YD LB=OB LC=QH " +
        "LD=SU LF=RA LH=OO LL=NS LN=YS LO=VP LQ=VH LR=LY LS=PK LU=SI MA=YR MB=TZ MC=TT MD=XY " +
        "ME=NM MG=WB MH=ZK MJ=QY MK=MO ML=XA MM=QP MP=NF MQ=PJ MR=YO MW=TJ MX=NZ MZ=QI NA=UN " +
        "NG=XO NL=PX NN=QQ NP=YK NQ=QC NR=VJ NW=OE NX=PD OA=VQ OD=TB OF=SP OG=WQ OH=QJ OK=VX " +
        "OM=WL OS=XM OU=XV OW=UU OX=YF OY=SY OZ=RT PA=UW PB=RS PE=SS PG=RC PM=QN PN=PW PO=RZ " +
        "PQ=SB PR=SN PT=WG PV=XL QB=ZV QD=ZU QK=RL QM=QU QR=SC QT=SL QV=ZE QW=VI QZ=XD RB=RG " +
        "RD=XJ RH=VN RI=SA RK=VV RM=RX RN=VK RP=TM RQ=ZC RR=SH RU=UC RW=TY RY=VO SR=VT ST=UD " +
        "SV=YE SW=XN TC=TK TE=UL TG=ZH TP=VZ TU=XU TX=WN UB=YZ UE=VA UG=UK UH=UX UM=ZO UR=XI " +
        "UT=ZF VC=ZN VL=XR VM=YQ VR=VY VU=XE VW=YJ WD=WW WF=YN WH=ZB WR=YP XK=ZW XQ=XT YB=YU " +
        "ZJ=ZX ZY=ZZ";

    private const string FlusslaufOEntries =
        "AA=TQ AB=ET AC=JX AD=SJ AE=XV AF=HB AG=VC AH=JP AI=QZ AJ=OG AK=ZW AL=IJ AM=DB AN=EF " +
        "AO=MA AP=FT AQ=IU AR=WS AS=PP AT=QI AU=ZB AV=MC AW=FO AX=ZH AY=BD AZ=KT BA=WF BB=CK " +
        "BC=EB BE=TM BF=BO BG=BM BH=OA BI=OE BJ=HK BK=CW BL=HX BN=GK BP=TB BQ=OM BR=RA BS=QU " +
        "BT=HJ BU=MW BV=PG BW=ZY BX=KY BY=PO BZ=SY CA=TO CB=UW CC=HQ CD=IN CE=XW CF=ZT CG=RG " +
        "CH=EX CI=JW CJ=LT CL=UT CM=RQ CN=WH CO=ZF CP=LU CQ=QN CR=YM CS=XZ CT=VY CU=UU CV=NA " +
        "CX=UN CY=VP CZ=VT DA=KH DC=IF DD=GZ DE=YT DF=FA DG=NJ DH=FN DI=OL DJ=RC DK=JB DL=RL " +
        "DM=PF DN=TP DO=GQ DP=XU DQ=IP DR=FY DS=IG DT=GO DU=ER DV=UF DW=MR DX=RH DY=NK DZ=YL " +
        "EA=KZ EC=VR ED=IH EE=RD EG=QX EH=VF EI=QF EJ=UA EK=FL EL=RF EM=EN EO=XB EP=ZJ EQ=NP " +
        "ES=MG EU=XE EV=YF EW=LW EY=WL EZ=ST FB=WJ FC=SE FD=KM FE=XK FF=XJ FG=WU FH=UH FI=SZ " +
        "FJ=FZ FK=GP FM=YI FP=QW FQ=JI FR=SI FS=YB FU=SM FV=UP FW=GA FX=GM GB=VB GC=QB GD=RJ " +
        "GE=PS GF=NM GG=UC GH=LF GI=ZN GJ=UI GL=ZM GN=GS GR=IQ GT=RM GU=YJ GV=LY GW=RE GX=WC " +
        "GY=OT HA=LH HC=NG HD=XX HE=TF HF=MI HG=YY HH=QJ HI=VN HL=TS HM=VL HN=JA HO=JY HP=ZK " +
        "HR=TC HS=JJ HT=NI HU=XQ HV=MN HW=TD HY=OD HZ=KP IA=RO IB=QK IC=SG ID=JG IE=WT II=LB " +
        "IK=KL IL=TX IM=PL IO=WY IR=UL IS=JZ IT=UG IV=OI IW=WM IX=YV IY=TZ IZ=LM JC=SQ JD=XL " +
        "JE=RK JF=OU JH=OB JK=NH JL=XY JM=UD JN=LQ JO=VM JQ=LI JR=UQ JS=WN JT=PH JU=LL JV=VQ " +
        "KA=WQ KB=XO KC=MB KD=MO KE=PJ KF=TA KG=OC KI=PN KJ=KW KK=PV KN=YD KO=PX KQ=KS KR=OZ " +
        "KU=QC KV=ZI KX=XI LA=LO LC=NC LD=RY LE=UJ LG=NZ LJ=MH LK=MY LN=PE LP=SP LR=SF LS=PT " +
        "LV=PR LX=NT LZ=OK MD=UM ME=QL MF=XR MJ=YR MK=RT ML=TJ MM=XM MP=SS MQ=UX MS=YP MT=WZ " +
        "MU=TU MV=PK MX=ZX MZ=NS NB=PZ ND=WI NE=VK NF=VS NL=UE NN=YO NO=QM NQ=NY NR=NX NU=UO " +
        "NV=RB NW=YE OF=RV OH=ZZ OJ=OX ON=SU OO=RW OP=YS OQ=TN OR=PW OS=UY OV=TI OW=QT OY=RI " +
        "PA=WE PB=ZP PC=WR PD=UB PI=RR PM=YN PQ=VJ PU=QD PY=SN QA=SR QE=VW QG=VI QH=WP QO=SC " +
        "QP=VX QQ=VU QR=SA QS=WW QV=TH QY=ZU RN=YZ RP=VA RS=ZO RU=SX RX=ZC RZ=ZV SB=SL SD=SW " +
        "SH=WD SK=YQ SO=XC SV=UV TE=XP TG=US TK=WB TL=VG TR=YK TT=XS TV=YA TW=YU TY=YG UK=VV " +
        "UR=YH UZ=WV VD=YC VE=ZA VH=ZG VO=ZD VZ=YW WA=ZS WG=ZQ WK=ZE WO=XD WX=ZR XA=XN XF=XH " +
        "XG=ZL XT=YX";
}
