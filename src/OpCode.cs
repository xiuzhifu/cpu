using System;
public enum Inst:byte {
	none,

	movb,
	movw,
	movl,
  movsb,
  movsw,
  movsl,

  pushb,
  pushw,
  pushl,

  popb,
  popw,
  popl,

  cmpb,
  cmpw,
  cmpl,
  testb,
  testw,
  testl,

  leab,
  leaw,
  leal,

  incb,
  incw,
  incl,

  decb,
  decw,
  decl,

  negb,
  negw,
  negl,

  notb,
  notw,
  notl,

  salb,
  salw,
  sall,

  shlb,
  shlw,
  shll,

  sarb,
  sarw,
  sarl,

  shrb,
  shrw,
  shrl,

  cltd,

  andb,
  andw,
  andl,

  orb,
  orw,
  orl,

  xorb,
  xorw,
  xorl,

  addb,
  addw,
  addl,

  subb,
  subw,
  subl,

  mulb,
  mulw,
  mull,
  imulb,
  imulw,
  imull,

  divb,
  divw,
  divl,
  idivb,
  idivw,
  idivl,

  call,
  callb,
  callw,
  calll,
  lcallb,
  lcall,
  lcallw,
  lcalll,

  ret,
  retw,
  retl,
  lret,
  lretw,
  lretl,

  leave,
  leavew,
  leavel,

  jmp,
  ljmp,

  jc,
  ljc,
  jcxz,
  ljcxz,
  jnb,
  ljnb,
  jnbe,
  ljnbe,
  jnc,
  ljnc,
  jnge,
  ljnge,
  jnl,
  ljnl,
  jnle,
  ljnle,
  jo,
  ljo,
  jno,
  ljno,
  jp,
  ljp,
  jnp,
  ljnp,
  jpe,
  ljpe,
  jpo,
  ljpo,
  je,
  lje,
  jz,
  ljz,
  jne,
  ljne,
  jnz,
  ljnz,
  js,
  ljs,
  jns,
  ljns,
  jg,
  ljg,
  jge,
  ljge,
  jl,
  ljl,
  jle,
  ljle,
  ja,
  lja,
  jae,
  ljae,
  jb,
  ljb,
  jbe,
  ljbe,

  INT,

  inb,
  inw,
  inl,

  outb,
  outw,
  outl,

	REP,
	REPZ,
	REPE,
	REPNZ,
	REPNE,
	LOCK,
}
	
public enum Addressing: byte {
	none,
	eax,
	ebx,
	ecx,
	edx,
	ebp,
	esp,
	esi,
	edi,
	ax,
	bx,
	cx,
	dx,
	ah,
	bh,
	ch,
	dh,
	al,
	bl,
	cl,
	dl,
	bp,
	sp,
	si,
	di,

	cs,
	ds,
	es,
	fs,
	ss,
	eip,
	ip,
	mem,
	imm,
  backfill,
}

public struct InstDef {
	public string name;
	public Inst inst;
	public byte paramcount;
  public byte width;
}

public struct AddressingDef {
    public string name;
    public Addressing addressing;
    public int width;
}

public class OpCode {
    public Inst preinst;
    public Inst inst;
    public Addressing[] Register;
    public Addressing SegReg;
    public Addressing BaseReg;
    public Addressing IndexReg;
    public byte scale;
    public Int32 Imm;
    public Int32 MemImm;
    public Int32 TempImm;
    public string BackFillLabel;
    public byte ParamCount;
    public int LineNo;
    public int RowNo;

	public OpCode() {
    Clear();
  }

	private static readonly string[] CPreInstString = {
		"rep",
		"repz",
		"repe",
		"repnz",
		"repne",
		"lock",
	};

	private static readonly Inst[] CPreInst = {
		Inst.REP,
		Inst.REPZ,
		Inst.REPE,
		Inst.REPNZ,
		Inst.REPNE,
		Inst.LOCK,
	};

    private readonly InstDef[] CInst = {
    new InstDef() { name = "movb", inst = Inst.movb, paramcount = 2, width = 1},
    new InstDef() { name = "movw", inst = Inst.movw, paramcount = 2, width = 2},
    new InstDef() { name = "movl", inst = Inst.movl, paramcount = 2, width = 4},
    new InstDef() { name = "movsb", inst = Inst.movsb, paramcount = 2, width = 1},
    new InstDef() { name = "movsw", inst = Inst.movsw, paramcount = 2, width = 2},
    new InstDef() { name = "movsl", inst = Inst.movsl, paramcount = 2, width = 4},
    new InstDef() { name = "pushb", inst = Inst.pushb, paramcount = 1, width = 1},
    new InstDef() { name = "pushw", inst = Inst.pushw, paramcount = 1, width = 2},
    new InstDef() { name = "pushl", inst = Inst.pushl, paramcount = 1, width = 4},
    new InstDef() { name = "popb", inst = Inst.popb, paramcount = 1, width = 1},
    new InstDef() { name = "popw", inst = Inst.popw, paramcount = 1, width = 2},
    new InstDef() { name = "popl", inst = Inst.popl, paramcount = 1, width = 4},
    new InstDef() { name = "cmpb", inst = Inst.cmpb, paramcount = 2, width = 1},
    new InstDef() { name = "cmpw", inst = Inst.cmpw, paramcount = 2, width = 2},
    new InstDef() { name = "cmpl", inst = Inst.cmpl, paramcount = 2, width = 4},
    new InstDef() { name = "testb", inst = Inst.testb, paramcount = 2, width = 1},
    new InstDef() { name = "testw", inst = Inst.testw, paramcount = 2, width = 2},
    new InstDef() { name = "testl", inst = Inst.testl, paramcount = 2, width = 4},
    new InstDef() { name = "leab", inst = Inst.leab, paramcount = 2, width = 1},
    new InstDef() { name = "leaw", inst = Inst.leaw, paramcount = 2, width = 2},
    new InstDef() { name = "leal", inst = Inst.leal, paramcount = 2, width = 4},
    new InstDef() { name = "incb", inst = Inst.incb, paramcount = 1, width = 1},
    new InstDef() { name = "incw", inst = Inst.incw, paramcount = 1, width = 2},
    new InstDef() { name = "incl", inst = Inst.incl, paramcount = 1, width = 4},
    new InstDef() { name = "decb", inst = Inst.decb, paramcount = 1, width = 1},
    new InstDef() { name = "decw", inst = Inst.decw, paramcount = 1, width = 2},
    new InstDef() { name = "decl", inst = Inst.decl, paramcount = 1, width = 4},
    new InstDef() { name = "negb", inst = Inst.negb, paramcount = 2, width = 1},
    new InstDef() { name = "negw", inst = Inst.negw, paramcount = 2, width = 2},
    new InstDef() { name = "negl", inst = Inst.negl, paramcount = 2, width = 4},
    new InstDef() { name = "notb", inst = Inst.notb, paramcount = 2, width = 1},
    new InstDef() { name = "notw", inst = Inst.notw, paramcount = 2, width = 2},
    new InstDef() { name = "notl", inst = Inst.notl, paramcount = 2, width = 4},
    new InstDef() { name = "salb", inst = Inst.salb, paramcount = 2, width = 1},
    new InstDef() { name = "salw", inst = Inst.salw, paramcount = 2, width = 2},
    new InstDef() { name = "sall", inst = Inst.sall, paramcount = 2, width = 4},
    new InstDef() { name = "shlb", inst = Inst.shlb, paramcount = 2, width = 1},
    new InstDef() { name = "shlw", inst = Inst.shlw, paramcount = 2, width = 2},
    new InstDef() { name = "shll", inst = Inst.shll, paramcount = 2, width = 4},
    new InstDef() { name = "sarb", inst = Inst.sarb, paramcount = 2, width = 1},
    new InstDef() { name = "sarw", inst = Inst.sarw, paramcount = 2, width = 2},
    new InstDef() { name = "sarl", inst = Inst.sarl, paramcount = 2, width = 4},
    new InstDef() { name = "shrb", inst = Inst.shrb, paramcount = 2, width = 1},
    new InstDef() { name = "shrw", inst = Inst.shrw, paramcount = 2, width = 2},
    new InstDef() { name = "shrl", inst = Inst.shrl, paramcount = 2, width = 4},
    new InstDef() { name = "cltd", inst = Inst.cltd, paramcount = 0, width = 0},
    new InstDef() { name = "andb", inst = Inst.andb, paramcount = 2, width = 1},
    new InstDef() { name = "andw", inst = Inst.andw, paramcount = 2, width = 2},
    new InstDef() { name = "andl", inst = Inst.andl, paramcount = 2, width = 4},
    new InstDef() { name = "orb", inst = Inst.orb, paramcount = 2, width = 1},
    new InstDef() { name = "orw", inst = Inst.orw, paramcount = 2, width = 2},
    new InstDef() { name = "orl", inst = Inst.orl, paramcount = 2, width = 4},
    new InstDef() { name = "xorb", inst = Inst.xorb, paramcount = 2, width = 1},
    new InstDef() { name = "xorw", inst = Inst.xorw, paramcount = 2, width = 2},
    new InstDef() { name = "xorl", inst = Inst.xorl, paramcount = 2, width = 4},
    new InstDef() { name = "addb", inst = Inst.addb, paramcount = 2, width = 1},
    new InstDef() { name = "addw", inst = Inst.addw, paramcount = 2, width = 2},
    new InstDef() { name = "addl", inst = Inst.addl, paramcount = 2, width = 4},
    new InstDef() { name = "subb", inst = Inst.subb, paramcount = 2, width = 1},
    new InstDef() { name = "subw", inst = Inst.subw, paramcount = 2, width = 2},
    new InstDef() { name = "subl", inst = Inst.subl, paramcount = 2, width = 4},
    new InstDef() { name = "mulb", inst = Inst.mulb, paramcount = 2, width = 1},
    new InstDef() { name = "mulw", inst = Inst.mulw, paramcount = 2, width = 2},
    new InstDef() { name = "mull", inst = Inst.mull, paramcount = 2, width = 4},
    new InstDef() { name = "imulb", inst = Inst.imulb, paramcount = 2, width = 1},
    new InstDef() { name = "imulw", inst = Inst.imulw, paramcount = 2, width = 2},
    new InstDef() { name = "imull", inst = Inst.imull, paramcount = 2, width = 4},
    new InstDef() { name = "divb", inst = Inst.divb, paramcount = 1, width = 1},
    new InstDef() { name = "divw", inst = Inst.divw, paramcount = 1, width = 2},
    new InstDef() { name = "divl", inst = Inst.divl, paramcount = 1, width = 4},
    new InstDef() { name = "idivb", inst = Inst.idivb, paramcount = 1, width = 1},
    new InstDef() { name = "idivw", inst = Inst.idivw, paramcount = 1, width = 2},
    new InstDef() { name = "idivl", inst = Inst.idivl, paramcount = 1, width = 4},

    new InstDef() { name = "callb", inst = Inst.callb, paramcount = 1, width = 1},
    new InstDef() { name = "callw", inst = Inst.callw, paramcount = 1, width = 2},
    new InstDef() { name = "calll", inst = Inst.calll, paramcount = 1, width = 4},
    new InstDef() { name = "lcallb", inst = Inst.lcallb, paramcount = 1, width = 1},
    new InstDef() { name = "lcallw", inst = Inst.lcallw, paramcount = 1, width = 2},
    new InstDef() { name = "lcalll", inst = Inst.lcalll, paramcount = 1, width = 4},
    new InstDef() { name = "call", inst = Inst.call, paramcount = 1, width = 2},
    new InstDef() { name = "lcall", inst = Inst.lcall, paramcount = 1, width = 2},

    new InstDef() { name = "retw", inst = Inst.retw, paramcount = 0, width = 0},
    new InstDef() { name = "retl", inst = Inst.retl, paramcount = 0, width = 0},
    new InstDef() { name = "lretw", inst = Inst.lretw, paramcount = 0, width = 0},
    new InstDef() { name = "lretl", inst = Inst.lretl, paramcount = 0, width = 0},

    new InstDef() { name = "ret", inst = Inst.ret, paramcount = 0, width = 0},
    new InstDef() { name = "lret", inst = Inst.lret, paramcount = 0, width = 0},

    new InstDef() { name = "inb", inst = Inst.inb, paramcount = 2, width = 1},
    new InstDef() { name = "inw", inst = Inst.inw, paramcount = 2, width = 2},
    new InstDef() { name = "inl", inst = Inst.inl, paramcount = 2, width = 4},

    new InstDef() { name = "outb", inst = Inst.outb, paramcount = 2, width = 1},
    new InstDef() { name = "outw", inst = Inst.outw, paramcount = 2, width = 2},
    new InstDef() { name = "outl", inst = Inst.outl, paramcount = 2, width = 4},

    new InstDef() { name = "int", inst = Inst.INT, paramcount = 1, width = 2},
    new InstDef() { name = "leave", inst = Inst.leave, paramcount = 0, width = 0},
    new InstDef() { name = "leavew", inst = Inst.leavew, paramcount = 0, width = 0},
    new InstDef() { name = "leavel", inst = Inst.leavel, paramcount = 0, width = 0},
    new InstDef() { name = "jmp", inst = Inst.jmp, paramcount = 1, width = 2},
    new InstDef() { name = "ljmp", inst = Inst.ljmp, paramcount = 1, width = 4},
    new InstDef() { name = "jc", inst = Inst.jc, paramcount = 1, width = 2},
    new InstDef() { name = "ljc", inst = Inst.ljc, paramcount = 1, width = 4},
    new InstDef() { name = "jcxz", inst = Inst.jcxz, paramcount = 1, width = 2},
    new InstDef() { name = "ljcxz", inst = Inst.ljcxz, paramcount = 1, width = 4},
    new InstDef() { name = "jnb", inst = Inst.jnb, paramcount = 1, width = 2},
    new InstDef() { name = "ljnb", inst = Inst.ljnb, paramcount = 1, width = 4},
    new InstDef() { name = "jnbe", inst = Inst.jnbe, paramcount = 1, width = 2},
    new InstDef() { name = "ljnbe", inst = Inst.ljnbe, paramcount = 1, width = 4},
    new InstDef() { name = "jnc", inst = Inst.jnc, paramcount = 1, width = 2},
    new InstDef() { name = "ljnc", inst = Inst.ljnc, paramcount = 1, width = 4},
    new InstDef() { name = "jnge", inst = Inst.jnge, paramcount = 1, width = 2},
    new InstDef() { name = "ljnge", inst = Inst.ljnge, paramcount = 1, width = 4},
    new InstDef() { name = "jnl", inst = Inst.jnl, paramcount = 1, width = 2},
    new InstDef() { name = "ljnl", inst = Inst.ljnl, paramcount = 1, width = 4},
    new InstDef() { name = "jnle", inst = Inst.jnle, paramcount = 1, width = 2},
    new InstDef() { name = "ljnle", inst = Inst.ljnle, paramcount = 1, width = 4},
    new InstDef() { name = "jno", inst = Inst.jno, paramcount = 1, width = 2},
    new InstDef() { name = "ljno", inst = Inst.ljno, paramcount = 1, width = 4},
    new InstDef() { name = "jnp", inst = Inst.jnp, paramcount = 1, width = 2},
    new InstDef() { name = "ljnp", inst = Inst.ljnp, paramcount = 1, width = 4},
    new InstDef() { name = "jo", inst = Inst.jo, paramcount = 1, width = 2},
    new InstDef() { name = "ljo", inst = Inst.ljo, paramcount = 1, width = 4},
    new InstDef() { name = "jp", inst = Inst.jp, paramcount = 1, width = 2},
    new InstDef() { name = "ljp", inst = Inst.ljp, paramcount = 1, width = 4},
    new InstDef() { name = "jpe", inst = Inst.jpe, paramcount = 1, width = 2},
    new InstDef() { name = "ljpe", inst = Inst.ljpe, paramcount = 1, width = 4},
    new InstDef() { name = "jpo", inst = Inst.jpo, paramcount = 1, width = 2},
    new InstDef() { name = "ljpo", inst = Inst.ljpo, paramcount = 1, width = 4},
    new InstDef() { name = "je", inst = Inst.je, paramcount = 1, width = 2},
    new InstDef() { name = "lje", inst = Inst.lje, paramcount = 1, width = 4},
    new InstDef() { name = "jz", inst = Inst.jz, paramcount = 1, width = 2},
    new InstDef() { name = "ljz", inst = Inst.ljz, paramcount = 1, width = 4},
    new InstDef() { name = "jne", inst = Inst.jne, paramcount = 1, width = 2},
    new InstDef() { name = "ljne", inst = Inst.ljne, paramcount = 1, width = 4},
    new InstDef() { name = "jnz", inst = Inst.jnz, paramcount = 1, width = 2},
    new InstDef() { name = "ljnz", inst = Inst.ljnz, paramcount = 1, width = 4},
    new InstDef() { name = "js", inst = Inst.js, paramcount = 1, width = 2},
    new InstDef() { name = "ljs", inst = Inst.ljs, paramcount = 1, width = 4},
    new InstDef() { name = "jns", inst = Inst.jns, paramcount = 1, width = 2},
    new InstDef() { name = "ljns", inst = Inst.ljns, paramcount = 1, width = 4},
    new InstDef() { name = "jg", inst = Inst.jg, paramcount = 1, width = 2},
    new InstDef() { name = "ljg", inst = Inst.ljg, paramcount = 1, width = 4},
    new InstDef() { name = "jge", inst = Inst.jge, paramcount = 1, width = 2},
    new InstDef() { name = "ljge", inst = Inst.ljge, paramcount = 1, width = 4},
    new InstDef() { name = "jl", inst = Inst.jl, paramcount = 1, width = 2},
    new InstDef() { name = "ljl", inst = Inst.ljl, paramcount = 1, width = 4},
    new InstDef() { name = "jle", inst = Inst.jle, paramcount = 1, width = 2},
    new InstDef() { name = "ljle", inst = Inst.ljle, paramcount = 1, width = 4},
    new InstDef() { name = "ja", inst = Inst.ja, paramcount = 1, width = 2},
    new InstDef() { name = "lja", inst = Inst.lja, paramcount = 1, width = 4},
    new InstDef() { name = "jae", inst = Inst.jae, paramcount = 1, width = 2},
    new InstDef() { name = "ljae", inst = Inst.ljae, paramcount = 1, width = 4},
    new InstDef() { name = "jb", inst = Inst.jb, paramcount = 1, width = 2},
    new InstDef() { name = "ljb", inst = Inst.ljb, paramcount = 1, width = 4},
    new InstDef() { name = "jbe", inst = Inst.jbe, paramcount = 1, width = 2},
    new InstDef() { name = "ljbe", inst = Inst.ljbe, paramcount = 1, width = 4},

    new InstDef() { name = "int", inst = Inst.INT, paramcount = 1, width = 4},
};

	private readonly AddressingDef[] CAddressing = {
		new AddressingDef() { name = "eax", addressing = Addressing.eax, width = 4},
		new AddressingDef() { name = "ebx", addressing = Addressing.ebx, width = 4},
		new AddressingDef() { name = "ecx", addressing = Addressing.ecx, width = 4},
		new AddressingDef() { name = "edx", addressing = Addressing.edx, width = 4},
		new AddressingDef() { name = "ebp", addressing = Addressing.ebp, width = 4},
		new AddressingDef() { name = "esp", addressing = Addressing.esp, width = 4},
		new AddressingDef() { name = "esi", addressing = Addressing.esi, width = 4},
		new AddressingDef() { name = "edi", addressing = Addressing.edi, width = 4},
		new AddressingDef() { name = "ax", addressing = Addressing.ax, width = 2},
		new AddressingDef() { name = "bx", addressing = Addressing.bx, width = 2},
		new AddressingDef() { name = "cx", addressing = Addressing.cx, width = 2},
		new AddressingDef() { name = "dx", addressing = Addressing.dx, width = 2},
		new AddressingDef() { name = "bp", addressing = Addressing.bp, width = 2},
		new AddressingDef() { name = "sp", addressing = Addressing.sp, width = 2},
		new AddressingDef() { name = "si", addressing = Addressing.si, width = 2},
		new AddressingDef() { name = "di", addressing = Addressing.di, width = 2},
		new AddressingDef() { name = "ah", addressing = Addressing.ah, width = 1},
		new AddressingDef() { name = "bh", addressing = Addressing.bh, width = 1},
		new AddressingDef() { name = "ch", addressing = Addressing.ch, width = 1},
		new AddressingDef() { name = "dh", addressing = Addressing.dh, width = 1},
		new AddressingDef() { name = "al", addressing = Addressing.al, width = 1},
		new AddressingDef() { name = "bl", addressing = Addressing.bl, width = 1},
		new AddressingDef() { name = "cl", addressing = Addressing.cl, width = 1},
		new AddressingDef() { name = "dl", addressing = Addressing.dl, width = 1},
		new AddressingDef() { name = "cs", addressing = Addressing.cs, width = 4},
		new AddressingDef() { name = "ds", addressing = Addressing.ds, width = 4},
		new AddressingDef() { name = "es", addressing = Addressing.es, width = 4},
		new AddressingDef() { name = "fs", addressing = Addressing.fs, width = 4},
		new AddressingDef() { name = "ss", addressing = Addressing.ss, width = 4},
	};

    private readonly Inst[] CBackFillInst = {
        Inst.jmp,
        Inst.ljmp,
        Inst.je,
        Inst.lje,
        Inst.jne,
        Inst.ljne,
        Inst.js,
        Inst.ljs,
        Inst.jnz,
        Inst.ljnz,
        Inst.jg,
        Inst.ljg,
        Inst.jge,
        Inst.ljge,
        Inst.jl,
        Inst.ljl,
        Inst.jle,
        Inst.ljle,
        Inst.ja,
        Inst.lja,
        Inst.jae,
        Inst.ljae,
        Inst.jb,
        Inst.ljb,
        Inst.jbe,
        Inst.ljbe,
    };
	public static Inst IsPreOpCode(string s) {
		s = s.ToUpper();
		for (int i = 0; i < CPreInstString.Length; i++) {
			if (CPreInstString[i] == s) 
				return CPreInst[i];
		}
		return Inst.none;	
	}

	public static bool IsPreOpCode(Inst inst) {
		for (int i = 0; i < CPreInst.Length; i++)
			if (CPreInst[i] == inst) return true;
		return false;	
	}

  public Inst IsOpCode(string s) {
    s = s.ToLower();
    for (int i = 0; i < CInst.Length; i++) {
      if (CInst[i].name == s) {
        ParamCount = CInst[i].paramcount;
        Register = new Addressing[ParamCount];
        return CInst[i].inst;
      }
    }
    return Inst.none;
  }

  public string PreOpcodeToString(Inst inst) {
    for (int i = 0; i < CPreInst.Length; i++) {
      if (CPreInst[i] == inst)
        return CPreInstString[i].ToLower();
    }
    return null;
  }

  public string OpcodeToString(Inst inst) {
    for (int i = 0; i < CInst.Length; i++) {
      if (CInst[i].inst == inst) {
        return CInst[i].name;
      }
    }
    return null;
  }

  public string AddressingToString(Addressing addressing) {
    for (int i = 0; i < CAddressing.Length; i++) {
      if (CAddressing[i].addressing == addressing) {
        return CAddressing[i].name;
      }
    }
    return null;
  }

	public Addressing isRegister(string s) {
		s = s.ToLower();
		for (int i = 0; i < CAddressing.Length; i++) {
			if (CAddressing[i].name == s) {
				return CAddressing[i].addressing;
			}
		}
		return Addressing.none;
	}

	public bool IsSegRegister(Addressing addressing) {
		if (addressing == Addressing.cs || addressing == Addressing.ds ||
		    addressing == Addressing.es || addressing == Addressing.fs || addressing == Addressing.ss) 
			return true;
		else 
			return false;
	}

    public bool IsCanBackFillInst(Inst inst) {
        for (int i = 0; i < CBackFillInst.Length; i++) {
            if (CBackFillInst[i] == inst)
                return true;
        }
        return false;
    }

    public bool IsCanBackFillInst() {
        return IsCanBackFillInst(inst);
    }

    public int GetInstWidth(Inst inst) {
        for (int i = 0; i < CInst.Length; i++) {
            if (CInst[i].inst == inst) {
                return CInst[i].width;
            }
        }
        return 0;
    }

    public int GetRegisterWidth(Addressing addressing) {
        for (int i = 0; i < CAddressing.Length; i++) {
            if (CAddressing[i].addressing == addressing) {
                return CAddressing[i].width;
            }
        }
        return 0;
    }
  public void Clear() {
    preinst = Inst.none;
    inst = Inst.none;
    Register = null;
    SegReg = Addressing.none;
    BaseReg = Addressing.none;
    IndexReg = Addressing.none;
    Imm = 0;
    ParamCount = 0;
  }

}

