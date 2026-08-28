namespace Enigma;

/// <summary>
/// The bigram tables this library ships with, under the names their booklets used.
///
/// A set was named by a codeword and held nine tables lettered A to H and J, with a
/// Tauschtafelplan saying which applied on which day. Only tables that have been
/// transcribed from a published scan of the original appear here.
/// </summary>
public static class BigramTables
{
    /// <summary>
    /// Tafel A of the set "Quelle", from the booklet Prüf-Nr. 2499 reproduced by the
    /// Crypto Museum. This is the table U-534 was using on 1 May 1945, and the four
    /// entries its message P1030690 needed — FN, HC, GV and ET — are exactly those
    /// published with the working of that message.
    ///
    /// Written once through: each entry implies its reverse, and
    /// <see cref="BigramTable"/> builds the other direction itself.
    /// </summary>
    public static BigramTable QuelleA { get; } = BigramTable.Parse(QuelleAEntries);

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
}
