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
    /// The tables of "Meer" by their letter. The edition runs to nine, A to J
    /// without I, and unlike "Quelle" the scan holds all of them.
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
}
