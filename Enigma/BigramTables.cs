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
}
