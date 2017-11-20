using System;
using System.IO;
public class Cpu {
  protected SystemCall FSysCall;
  protected Memory FMem;
  protected Register FReg;
  protected Inst FPrefixInst;
  protected Inst FInst;
  protected int[] Addr = new int[3];
  private Parser FParser;
  protected int FCpuWidth;
  public string Error;
  public void AddSysCall(string name, int i) {
     FParser.SetEquValue(name, i);
    FParser.AddLabelAddr(name, i);
  }
  public void InitSysCall() { 
    AddSysCall("_printf", FSysCall.CSYSCALL_PRINT);
    AddSysCall("_test_string_equ", FSysCall.CSYSCALL_TESTEQUSTRING);
    AddSysCall("_test_string_unequ", FSysCall.CSYSCALL_TESTUNEQUSTRING);
    AddSysCall("_test_register_equ", FSysCall.CSYSCALL_TESTEQUREGISTER);
    AddSysCall("_test_register_unequ", FSysCall.CSYSCALL_TESTUNEQUREGISTER);
    AddSysCall("_printeax", FSysCall.CSYSCALL_PRINTEAX);
  }
	public Cpu(int cpuwidth, int memsize = 1024 * 1024) {
    FMem = new Memory(memsize);
    FReg = new Register();
    FSysCall = new SystemCall(FReg, FMem);
    FCpuWidth = cpuwidth;
    FParser = new Parser(FCpuWidth);    
  }

	public bool Load(string filename) {
		if (!File.Exists(filename)) return false;
		BinaryReader reader = new BinaryReader(new FileStream(filename, FileMode.Open));
		byte[] bin = reader.ReadBytes((int)reader.BaseStream.Length);
		return Load(bin);
	}
  public bool LoadFromFile(string filename) {
    if (!File.Exists(filename)) {
      Error = "source file not exists";
      return false;
    }
    FParser.LoadFromFile(filename);
    InitSysCall();
    FParser.Parsing();
    byte[] insts = FParser.GetInsts();
    if (insts != null) {
      return Load(insts);
    }
    return false;
  }
  public bool Load(byte[] code) {
    FMem.WriteBytes(0, code);
    int ea = FMem.ReadInt(0);
    FReg.EIP = ea;
    return true;
  }

  public void WriteByte(int addr, byte b) {
    if (addr < 0)
      FReg.WriteByte(addr, b);
    else
      FMem.WriteByte(addr, b);
  }

  public void WriteWord(int addr, ushort w) {
    if (addr < 0)
      FReg.WriteWord(addr, w);
    else
      FMem.WriteWord(addr, w);
  }

  public void WriteInt(int addr, Int32 i) {
    if (addr < 0)
      FReg.WriteInt(addr, i);
    else
      FMem.WriteInt(addr, i);
  }

  public byte ReadByte(int addr) {
    if (addr < 0)
      return FReg.ReadByte(addr);
    else
      return FMem.ReadByte(addr);
  }

  public ushort ReadWord(int addr) {
    if (addr < 0)
      return FReg.ReadWord(addr);
    else
      return FMem.ReadWord(addr);
  }

  public Int32 ReadInt(int addr) {
    if (addr < 0)
      return FReg.ReadInt(addr);
    else
      return FMem.ReadInt(addr);
  }

  protected virtual bool IsSupportInst(Inst inst) {
    return false;
  }

  protected virtual bool IsSupportAddressing(Addressing addressing) {
    return false;
  }

	private Inst GetInst() {
    Inst inst = (Inst)FMem.ReadByte(FReg.EIP);
    FReg.EIP++;
		return inst;
	}

	public bool Next() {
    FPrefixInst = Inst.none;
		Inst inst = GetInst();
    if (OpCode.IsPreOpCode(inst)) {
      FPrefixInst = inst;
      inst = GetInst();
    }
    if (IsSupportInst(inst)) {
      FInst = inst;
      return ExecInst();
    }
    else {
      Error = "unknown instruction: " + inst.ToString();
    }
    return false;
	}

	public void Exec()
	{
	    for(;;) if (!Next()) break;
	}
  protected bool GetOperandAddr(int count) {
    int eip = FReg.EIP;
    for (int i = 0; i < count; i++) {
      Addressing addressing = (Addressing)FMem.ReadByte(eip);
      if (!IsSupportAddressing(addressing)) {
        Error = "unsupport register" + addressing.ToString();
        return false;
      }
      eip++;
      if (addressing == Addressing.imm) {
        if (FCpuWidth == 2) {
          ushort imm = FMem.ReadWord(eip);
          eip += 2;
          Addr[i] = FReg.GetRegisterAddr(addressing);
          FReg.WriteWord(Addr[i], imm);
        }
        else if (FCpuWidth == 4) {
          Int32 imm = FMem.ReadInt(eip);
          eip += 4;
          Addr[i] = FReg.GetRegisterAddr(addressing);
          FReg.WriteInt(Addr[i], imm);
        }
        else {
          ushort imm = FMem.ReadWord(eip);
          eip += 2;
          Addr[i] = FReg.GetRegisterAddr(addressing);
          FReg.WriteWord(Addr[i], imm);
        }
      }
      else if (addressing == Addressing.mem) {
        /*
         if 8bit 
           base register is only bp
           index register can si or di sp
           scale can be 1, 2, 4, 8
           imm is 16bit
         if 16bit
           base register can bp or bx 
           index register can ax, cx, dx, si, di, sp
           scale can be 1, 2, 4, 8
           imm is 16bit
         if 32bit
           base register can ebx, bp or bx 
           index register can eax, ecx, edx, esi, edi, esp, ax, cx, dx, si, di, sp
           scale can be 1, 2, 4, 8
           imm is 32bit       
         */
        int addr = 0;
        //seg reg
        addressing = (Addressing)FMem.ReadByte(eip);
        eip++;
        addr = (ushort)FReg.ReadWord(addressing) << 4;
        //base reg
        addressing = (Addressing)FMem.ReadByte(eip);
        eip++;
        if (addressing != Addressing.none)
          addr = addr + FReg.ReadWord(Addressing.bp);
        //index reg
        addressing = (Addressing)FMem.ReadByte(eip);
        eip++;
        //scale
        int scale = FMem.ReadByte(eip);
        eip++;
        if (addressing != Addressing.none)
          addr = addr + FReg.ReadWord(addressing) * scale;

        //imm
        if (FCpuWidth == 2) {
          addr = addr + FMem.ReadWord(eip);
          eip = eip + 2;
        }
        else if (FCpuWidth == 4) {
          addr = addr + FMem.ReadInt(eip);
          eip += 4;
        }
        else {
          addr = addr + FMem.ReadWord(eip);
          eip = eip + 2;
        }

        Addr[i] = addr;
      }
      else {
        Addr[i] = FReg.GetRegisterAddr(addressing);
      }
      FReg.EIP = eip;
    }
    return true;
  }

  private void SetSFZFRegister(byte s) {
    if (s == 0) FReg.ZF = 1; else FReg.ZF = 0;
    if ((s & 0x80) == 0) FReg.SF = 0; else FReg.SF = 1;
  }

  private void SetSFZFRegisterW(ushort w) {
    if (w == 0) FReg.ZF = 1; else FReg.ZF = 0;
    if ((w & 0x8000) == 0) FReg.SF = 0; else FReg.SF = 1;
  }

  private void SetSFZFRegisterL(int i) {
    if (i == 0) FReg.ZF = 1; else FReg.ZF = 0;
    if ((i & 0x80000000) == 0) FReg.SF = 0; else FReg.SF = 1;
  }

  private void SetOFRegister(byte op1, byte op2, byte s) {
    sbyte b = (sbyte)s;
    sbyte o1 = (sbyte)op1;
    sbyte o2 = (sbyte)op2;
    if (o1 > 0 && o2 > 0) {
      if (b > 0) FReg.OF = 0; else FReg.OF = 1;
    }
    else if (o1 < 0 && o2 < 0) {
      if (b < 0) FReg.OF = 0; else FReg.OF = 1;
    }
  }

  private void SetOFRegisterW(ushort op1, ushort op2, ushort s) {
    short b = (short)s;
    short o1 = (short)op1;
    short o2 = (short)op2;
    if (o1 > 0 && o2 > 0) {
      if (b > 0) FReg.OF = 0; else FReg.OF = 1;
    }
    else if (o1 < 0 && o2 < 0) {
      if (b < 0) FReg.OF = 0; else FReg.OF = 1;
    }
  }

  private void SetOFRegisterL(int op1, int op2, int s) {
    int b = (int)s;
    int o1 = (int)op1;
    int o2 = (int)op2;
    if (o1 > 0 && o2 > 0) {
      if (b > 0) FReg.OF = 0; else FReg.OF = 1;
    }
    else if (o1 < 0 && o2 < 0) {
      if (b < 0) FReg.OF = 0; else FReg.OF = 1;
    }
  }

  private void SetCFRegisterAdd(short w) {
    if (w > 0xff) FReg.CF = 1; else FReg.CF = 0;
  }

  private void SetCFRegisterSub(byte op1, byte op2) {
    if (op1 < op2) FReg.CF = 1; else FReg.CF = 0;
  }

  private void PushB(byte b) {
    FMem.WriteByte(FReg.SS + FReg.SP, b);
    FReg.SP++;
  }

  private void PushW(ushort s) {
    FMem.WriteWord(FReg.SS + FReg.SP, s);
    FReg.SP += 2;
  }

  private void PushL(Int32 i) {
    FMem.WriteInt(FReg.SS + FReg.SP, i);
    FReg.SP += 4;
  }

  private byte PopB() {
    byte b = FMem.ReadByte(FReg.SS + FReg.SP);
    FReg.SP--;
    return b;
  }

  private ushort PopW() {
    ushort s = (ushort)FMem.ReadWord(FReg.SS + FReg.SP);
    FReg.SP -= 2;
    return s;
  }

  private Int32 PopL() {
    Int32 i = FMem.ReadInt(FReg.SS + FReg.SP);
    FReg.SP -= 4;
    return i;
  }

  private bool JMP() {
    if(!GetOperandAddr(1)) return false;
    short ip = (short)ReadWord(Addr[0]);
    FReg.EIP += ip;
    return true;
  }

  private bool LJMP() {
    if(!GetOperandAddr(1)) return false;
    int ip = ReadInt(Addr[0]);
    FReg.EIP = ip;
    return true;
  }

  private bool ExecInst() {
    byte op1, op2, operand;
    sbyte sop1, sop2, soperand;
    short sw, sw1, sw2, swr;
    ushort w, w1, w2, wr;
    Int32 ii1, ii2, iir;
    switch (FInst) {
      case Inst.movb:
        if (!GetOperandAddr(2)) return false;
        operand = ReadByte(Addr[0]);
        WriteByte(Addr[1], operand);
        break;
      case Inst.movw:
        if(!GetOperandAddr(2)) return false;
        w = ReadWord(Addr[0]);
        WriteWord(Addr[1], w);
        break;
      case Inst.movl:
        if(!GetOperandAddr(2)) return false;
        ii1 = ReadInt(Addr[0]);
        WriteInt(Addr[1], ii1);
        break;
      case Inst.movsb:
        int src = FReg.DS << 4 + FReg.SI;
        int des = FReg.ES << 4 + FReg.DI;
        if (FPrefixInst == Inst.REP) {
          int c = FReg.CL;
          if (FReg.DF == 0)
            for (int i = 0; i < c; i++) {
              FMem.WriteByte(des, FMem.ReadByte(src));
              src++;
              des++;
            }
          else
            for (int i = 0; i < c; i++) {
              FMem.WriteByte(des, FMem.ReadByte(src));
              src--;
              des--;
            }
        }
        else
          FMem.WriteByte(des, FMem.ReadByte(src));
        break;
      case Inst.movsw:
        int srcw = FReg.DS << 4 + FReg.SI;
        int desw = FReg.ES << 4 + FReg.DI;
        if (FPrefixInst == Inst.REP) {
          int c = FReg.CL;
          if (FReg.DF == 0)
            for (int i = 0; i < c; i++) {
              FMem.WriteWord(desw, FMem.ReadWord(srcw));
              srcw++;
              desw++;
            }
          else
            for (int i = 0; i < c; i++) {
              FMem.WriteWord(desw, FMem.ReadWord(srcw));
              srcw--;
              desw--;
            }
        }
        else
          FMem.WriteWord(desw, FMem.ReadWord(srcw));
        break;
      case Inst.movsl:
        int srcl = FReg.DS + FReg.ESI;
        int desl = FReg.ES + FReg.EDI;
        if (FPrefixInst == Inst.REP) {
          int c = FReg.CL;
          if (FReg.DF == 0)
            for (int i = 0; i < c; i++) {
              FMem.WriteInt(desl, FMem.ReadInt(srcl));
              srcl++;
              desl++;
            }
          else
            for (int i = 0; i < c; i++) {
              FMem.WriteInt(desl, FMem.ReadInt(srcl));
              srcl--;
              desl--;
            }
        }
        else
          FMem.WriteInt(desl, FMem.ReadInt(srcl));
        break;
      case Inst.cmpb:
        if(!GetOperandAddr(2)) return false;
        op1 = ReadByte(Addr[0]);
        op2 = ReadByte(Addr[1]);
        operand = (byte)(op1 - op2);
        SetSFZFRegister(operand);
        SetOFRegister(op1, op2, operand);
        break;
      case Inst.cmpw:
        if(!GetOperandAddr(2)) return false;
        w = ReadWord(Addr[0]);
        w2 = ReadWord(Addr[1]);
        wr = (ushort)(w - w2);
        SetSFZFRegisterW(wr);
        SetOFRegisterW(w, w2, wr);
        break;
      case Inst.cmpl:
        if(!GetOperandAddr(2)) return false;
        ii1 = ReadInt(Addr[0]);
        ii2 = ReadInt(Addr[1]);
        iir = ii1 - ii2;
        SetSFZFRegisterL(iir);
        SetOFRegisterL(ii1, ii2, iir);
        break;
      case Inst.testb:
        if(!GetOperandAddr(2)) return false;
        op1 = ReadByte(Addr[0]);
        op2 = ReadByte(Addr[1]);
        operand = (byte)(op1 & op2);
        SetSFZFRegister(operand);
        FReg.OF = 0;
        FReg.CF = 0;
        break;
      case Inst.testw:
        if(!GetOperandAddr(2)) return false;
        w1 = ReadWord(Addr[0]);
        w2 = ReadWord(Addr[1]);
        wr = (ushort)(w1 & w2);
        SetSFZFRegisterW(wr);
        FReg.OF = 0;
        FReg.CF = 0;
        break;
      case Inst.testl:
        if(!GetOperandAddr(2)) return false;
        ii1 = ReadInt(Addr[0]);
        ii2 = ReadInt(Addr[1]);
        iir = (Int32)(ii1 & ii2);
        SetSFZFRegisterL(iir);
        FReg.OF = 0;
        FReg.CF = 0;
        break;
      case Inst.incb:
        if(!GetOperandAddr(1)) return false;
        op1 = ReadByte(Addr[0]);
        op2 = (byte)(op1 + 1);
        WriteByte(Addr[0], op2);
        SetSFZFRegister(op2);
        if (op2 > op1) {
          FReg.OF = 0;
          FReg.CF = 0;
        }
        else {
          FReg.OF = 1;
          FReg.CF = 1;
        }
        break;
      case Inst.incw:
        if(!GetOperandAddr(1)) return false;
        w1 = ReadWord(Addr[0]);
        w2 = (ushort)(w1 + 1);
        WriteWord(Addr[0], w2);
        SetSFZFRegisterW(w2);
        if (w2 > w1) {
          FReg.OF = 0;
          FReg.CF = 0;
        }
        else {
          FReg.OF = 1;
          FReg.CF = 1;
        }
        break;
      case Inst.incl:
        if(!GetOperandAddr(1)) return false;
        ii1 = ReadInt(Addr[0]);
        ii2 = ii1 + 1;
        WriteInt(Addr[0], ii2);
        SetSFZFRegisterL(ii2);
        if (ii2 > ii1) {
          FReg.OF = 0;
          FReg.CF = 0;
        }
        else {
          FReg.OF = 1;
          FReg.CF = 1;
        }
        break;
      case Inst.decb:
        if(!GetOperandAddr(1)) return false;
        op1 = ReadByte(Addr[0]);
        op2 = (byte)(op1 - 1);
        WriteByte(Addr[0], op2);
        SetSFZFRegister(op2);
        if (op1 > op2) {
          FReg.OF = 0;
          FReg.CF = 0;
        }
        else {
          FReg.OF = 1;
          FReg.CF = 1;
        }
        break;
      case Inst.decw:
        if(!GetOperandAddr(1)) return false;
        w1 = ReadWord(Addr[0]);
        w2 = (ushort)(w1 - 1);
        WriteWord(Addr[0], w2);
        SetSFZFRegisterW(w2);
        if (w1 > w2) {
          FReg.OF = 0;
          FReg.CF = 0;
        }
        else {
          FReg.OF = 1;
          FReg.CF = 1;
        }
        break;
      case Inst.decl:
        if(!GetOperandAddr(1)) return false;
        ii1 = ReadInt(Addr[0]);
        ii2 = ii1 - 1;
        WriteInt(Addr[0], ii2);
        SetSFZFRegisterL(ii2);
        if (ii1 > ii2) {
          FReg.OF = 0;
          FReg.CF = 0;
        }
        else {
          FReg.OF = 1;
          FReg.CF = 1;
        }
        break;
      case Inst.negb:
        if(!GetOperandAddr(1)) return false;
        sbyte b = (sbyte)ReadByte(Addr[0]);
        b = (sbyte)(-b);
        WriteByte(Addr[0], (byte)b);
        SetSFZFRegister((byte)b);
        break;
      case Inst.negw:
        if(!GetOperandAddr(1)) return false;
        sw = (short)ReadWord(Addr[0]);
        sw = (short)(-sw);
        WriteWord(Addr[0], (ushort)sw);
        SetSFZFRegisterW((ushort)sw);
        break;
      case Inst.negl:
        if(!GetOperandAddr(1)) return false;
        ii1 = ReadInt(Addr[0]);
        ii1 = -ii1;
        WriteInt(Addr[0], ii1);
        SetSFZFRegisterL(ii1);
        break;
      case Inst.notb:
        if(!GetOperandAddr(1)) return false;
        operand = ReadByte(Addr[0]);
        operand = (byte)~operand;
        WriteByte(Addr[0], operand);
        SetSFZFRegister(operand);
        break;
      case Inst.notw:
        if(!GetOperandAddr(1)) return false;
        w = ReadWord(Addr[0]);
        w = (ushort)~w;
        WriteWord(Addr[0], w);
        SetSFZFRegisterW(w);
        break;
      case Inst.notl:
        if(!GetOperandAddr(1)) return false;
        ii1 = ReadInt(Addr[0]);
        ii1 = ~ii1;
        WriteInt(Addr[0], ii1);
        SetSFZFRegisterL(ii1);
        break;
      case Inst.salb: case Inst.shlb: 
        if(!GetOperandAddr(2)) return false;
        op1 = ReadByte(Addr[0]);
        op2 = ReadByte(Addr[1]);
        op2 <<= op1;
        WriteByte(Addr[1], op2);
        SetSFZFRegister(op2);
        break;
      case Inst.salw: case Inst.shlw: 
        if(!GetOperandAddr(2)) return false;
        w1 = ReadWord(Addr[0]);
        w2 = ReadWord(Addr[1]);
        w2 <<= w1;
        WriteWord(Addr[1], w2);
        SetSFZFRegisterW(w2);
        break;
      case Inst.sall: case Inst.shll: 
        if(!GetOperandAddr(2)) return false;
        ii1 = ReadInt(Addr[0]);
        ii2 = ReadInt(Addr[1]);
        ii2 <<= ii1;
        WriteInt(Addr[1], ii2);
        SetSFZFRegisterL(ii2);
        break;
      case Inst.sarb: case Inst.shrb:    
        if(!GetOperandAddr(2)) return false;
        op1 = ReadByte(Addr[0]);
        op2 = ReadByte(Addr[1]);
        op2 >>= op1;
        WriteByte(Addr[1], op2);
        SetSFZFRegister(op2);
        break;
      case Inst.sarw: case Inst.shrw:    
        if(!GetOperandAddr(2)) return false;
        w1 = ReadWord(Addr[0]);
        w2 = ReadWord(Addr[1]);
        w2 >>= w1;
        WriteWord(Addr[1], w2);
        SetSFZFRegisterW(w2);
        break;
      case Inst.sarl: case Inst.shrl:    
        if(!GetOperandAddr(2)) return false;
        ii1 = ReadInt(Addr[0]);
        ii2 = ReadInt(Addr[1]);
        ii2 >>= ii1;
        WriteInt(Addr[1], ii2);
        SetSFZFRegisterL(ii2);
        break;
      case Inst.cltd:
        break;

      case Inst.andb:
        if(!GetOperandAddr(2)) return false;
        op1 = ReadByte(Addr[0]);
        op2 = ReadByte(Addr[1]);
        operand = (byte)(op1 & op2);
        WriteByte(Addr[1], operand);
        SetSFZFRegister(operand);
        SetOFRegister(op1, op2, operand);
        break;
      case Inst.andw:
        if(!GetOperandAddr(2)) return false;
        w1 = ReadWord(Addr[0]);
        w2 = ReadWord(Addr[1]);
        wr = (ushort)(w1 & w2);
        WriteWord(Addr[1], wr);
        SetSFZFRegisterW(wr);
        SetOFRegisterW(w1, w2, wr);
        break;
      case Inst.andl:
        if(!GetOperandAddr(2)) return false;
        ii1 = ReadInt(Addr[0]);
        ii2 = ReadInt(Addr[1]);
        iir = ii1 & ii2;
        WriteInt(Addr[1], iir);
        SetSFZFRegisterL(iir);
        SetOFRegisterL(ii1, ii2, iir);
        break;
      case Inst.orb:
        if(!GetOperandAddr(2)) return false;
        op1 = ReadByte(Addr[0]);
        op2 = ReadByte(Addr[1]);
        operand = (byte)(op1 | op2);
        WriteByte(Addr[1], operand);
        SetSFZFRegister(operand);
        SetOFRegister(op1, op2, operand);
        break;
      case Inst.orw:
        if(!GetOperandAddr(2)) return false;
        w1 = ReadWord(Addr[0]);
        w2 = ReadWord(Addr[1]);
        wr = (ushort)(w1 | w2);
        WriteWord(Addr[1], wr);
        SetSFZFRegisterW(wr);
        SetOFRegisterW(w1, w2, wr);
        break;
      case Inst.orl:
        if(!GetOperandAddr(2)) return false;
        ii1 = ReadInt(Addr[0]);
        ii2 = ReadInt(Addr[1]);
        iir = ii1 | ii2;
        WriteInt(Addr[1], iir);
        SetSFZFRegisterL(iir);
        SetOFRegisterL(ii1, ii2, iir);
        break;
      case Inst.xorb:
        if(!GetOperandAddr(2)) return false;
        op1 = ReadByte(Addr[0]);
        op2 = ReadByte(Addr[1]);
        operand = (byte)(op1 ^ op2);
        WriteByte(Addr[1], operand);
        SetSFZFRegister(operand);
        SetOFRegister(op1, op2, operand);
        break;
      case Inst.xorw:
        if(!GetOperandAddr(2)) return false;
        w1 = ReadWord(Addr[0]);
        w2 = ReadWord(Addr[1]);
        wr = (ushort)(w1 ^ w2);
        WriteWord(Addr[1], wr);
        SetSFZFRegisterW(wr);
        SetOFRegisterW(w1, w2, wr);
        break;
      case Inst.xorl:
        if(!GetOperandAddr(2)) return false;
        ii1 = ReadInt(Addr[0]);
        ii2 = ReadInt(Addr[1]);
        iir = ii1 ^ ii2;
        WriteInt(Addr[1], iir);
        SetSFZFRegisterL(iir);
        SetOFRegisterL(ii1, ii2, iir);
        break;
      case Inst.addb:
        if(!GetOperandAddr(2)) return false;
        op1 = ReadByte(Addr[0]);
        op2 = ReadByte(Addr[1]);
        operand = (byte)(op1 + op2);
        WriteByte(Addr[1], operand);
        SetSFZFRegister(operand);
        SetOFRegister(op1, op2, operand);
        break;
      case Inst.addw:
        if(!GetOperandAddr(2)) return false;
        w1 = ReadWord(Addr[0]);
        w2 = ReadWord(Addr[1]);
        wr = (ushort)(w1 + w2);
        WriteWord(Addr[1], wr);
        SetSFZFRegisterW(wr);
        SetOFRegisterW(w1, w2, wr);
        break;
      case Inst.addl:
        if(!GetOperandAddr(2)) return false;
        ii1 = ReadInt(Addr[0]);
        ii2 = ReadInt(Addr[1]);
        iir = ii1 + ii2;
        WriteInt(Addr[1], iir);
        SetSFZFRegisterL(iir);
        SetOFRegisterL(ii1, ii2, iir);
        break;
      case Inst.subb:
        if(!GetOperandAddr(2)) return false;
        op1 = ReadByte(Addr[0]);
        op2 = ReadByte(Addr[1]);
        operand = (byte)(op2 - op1);
        WriteByte(Addr[1], operand);
        SetSFZFRegister(operand);
        SetOFRegister(op1, op2, operand);
        break;
      case Inst.subw:
        if(!GetOperandAddr(2)) return false;
        w1 = ReadWord(Addr[0]);
        w2 = ReadWord(Addr[1]);
        wr = (ushort)(w2 - w1);
        WriteWord(Addr[1], wr);
        SetSFZFRegisterW(wr);
        SetOFRegisterW(w1, w2, wr);
        break;
      case Inst.subl:
        if(!GetOperandAddr(2)) return false;
        ii1 = ReadInt(Addr[0]);
        ii2 = ReadInt(Addr[1]);
        iir = ii2 - ii1;
        WriteInt(Addr[1], iir);
        SetSFZFRegisterL(iir);
        SetOFRegisterL(ii1, ii2, iir);
        break;
      case Inst.mulb:
        if(!GetOperandAddr(2)) return false;
        op1 = ReadByte(Addr[0]);
        op2 = ReadByte(Addr[1]);
        operand = (byte)(op1 * op2);
        WriteByte(Addr[1], operand);
        SetSFZFRegister(operand);
        SetOFRegister(op1, op2, operand);
        break;
      case Inst.mulw:
        if(!GetOperandAddr(2)) return false;
        w1 = ReadWord(Addr[0]);
        w2 = ReadWord(Addr[1]);
        wr = (ushort)(w1 * w2);
        WriteWord(Addr[1], wr);
        SetSFZFRegisterW(wr);
        SetOFRegisterW(w1, w2, wr);
        break;
      case Inst.mull:
        if(!GetOperandAddr(2)) return false;
        ii1 = ReadInt(Addr[0]);
        ii2 = ReadInt(Addr[1]);
        iir = ii1 * ii2;
        WriteInt(Addr[1], iir);
        SetSFZFRegisterL(iir);
        SetOFRegisterL(ii1, ii2, iir);
        break;
      case Inst.imulb:
        if(!GetOperandAddr(2)) return false;
        sop1 = (sbyte)ReadByte(Addr[0]);
        sop2 = (sbyte)ReadByte(Addr[1]);
        soperand = (sbyte)(sop1 * sop2);
        WriteByte(Addr[1], (byte)soperand);
        SetSFZFRegister((byte)soperand);
        SetOFRegister((byte)sop1, (byte)sop2, (byte)soperand);
        break;
      case Inst.imulw:
        if(!GetOperandAddr(2)) return false;
        sw1 = (short)ReadWord(Addr[0]);
        sw2 = (short)ReadWord(Addr[1]);
        swr = (short)(sw1 * sw2);
        WriteWord(Addr[1], (ushort)swr);
        SetSFZFRegisterW((ushort)swr);
        SetOFRegisterW((ushort)sw1, (ushort)sw2, (ushort)swr);
        break;
      case Inst.imull:
        if(!GetOperandAddr(2)) return false;
        ii1 = ReadInt(Addr[0]);
        ii2 = ReadInt(Addr[1]);
        iir = ii1 * ii2;
        WriteInt(Addr[1], iir);
        SetSFZFRegisterL(iir);
        SetOFRegisterL(ii1, ii2, iir);
        break;
      case Inst.divb:
        if(!GetOperandAddr(1)) return false;
        w = FReg.AX;
        op1 = ReadByte(Addr[0]);
        op2 = (byte)(w / op1);
        FReg.AL = op2;
        SetSFZFRegister((byte)op2);
        op2 = (byte)(w % op1);
        FReg.AH = op2;
        break;
      case Inst.divw:
        if(!GetOperandAddr(1)) return false;
        uint i1 = (uint)((FReg.DX << 16) + FReg.AX);
        w1 = ReadWord(Addr[0]);
        w2 = (ushort)(i1 / w1);
        FReg.AX = w2;
        SetSFZFRegisterW(w2);
        w2 = (ushort)(i1 % w1);
        FReg.DX = w2;
        break;
      case Inst.divl:
        if(!GetOperandAddr(1)) return false;
        UInt64 i64 = (UInt64)(FReg.EDX << 32);
        i64 = (UInt64)(i64 + (UInt64)FReg.EAX);
        uint ui = (uint)ReadInt(Addr[0]);
        ui = (byte)(i64 / ui);
        FReg.EAX = (int)ui;
        SetSFZFRegisterL((int)ui);
        ui = (uint)(i64 % ui);
        FReg.EDX = (int)ui;
        break;
      case Inst.idivb:
        if(!GetOperandAddr(1)) return false;
        sw = (sbyte)FReg.AX;
        op1 = ReadByte(Addr[0]);
        op2 = (byte)(sw / op1);
        FReg.AL = op2;
        SetSFZFRegister((byte)op2);
        op2 = (byte)(sw % op1);
        FReg.AH = op2;
        break;
      case Inst.idivw:
        if(!GetOperandAddr(1)) return false;
        ii1 = (FReg.DX << 16) + FReg.AX;
        sw1 = (short)ReadWord(Addr[0]);
        sw2 = (short)(ii1 / sw1);
        FReg.AX = (ushort)sw2;
        SetSFZFRegisterW((ushort)sw2);
        sw2 = (short)(ii1 % sw1);
        FReg.DX = (ushort)sw2;
        break;
      case Inst.idivl:
        if(!GetOperandAddr(1)) return false;
        Int64 i642 = (Int64)(FReg.EDX << 32);
        i642 = (Int64)(i642 + (Int64)FReg.EAX);
        ii1 = ReadInt(Addr[0]);
        ii1 = (byte)(i642 / ii1);
        FReg.EAX = ii1;
        SetSFZFRegisterL(ii1);
        ii1 = (int)(i642 % ii1);
        FReg.EDX = ii1;
        break;
      case Inst.callb:
        if(!GetOperandAddr(1)) return false;
        FMem.WriteWord(FReg.SS + FReg.ESP, (ushort)(FReg.IP + 1));
        sop1 = (sbyte)ReadByte(Addr[0]);
        FReg.IP = (ushort)(FReg.IP + sop1);
        break;
      case Inst.callw:
      case Inst.call:
        if(!GetOperandAddr(1)) return false;
        PushW((ushort)(FReg.IP + 1));
        sw = (short)ReadWord(Addr[0]);
        FReg.IP = (ushort)(FReg.IP + sw);
        break;
      case Inst.calll:
        if(!GetOperandAddr(1)) return false;
        PushL(FReg.EIP + 1);
        ii1 = ReadInt(Addr[0]);
        FReg.EIP = ii1;
        break;
      case Inst.ret:
        FReg.IP = PopW();
        break;
      case Inst.retl:
        FReg.EIP = PopL();
        break;
      case Inst.leavew:
        FReg.SP = FReg.BP;
        FReg.BP = PopW();
        break;
      case Inst.leavel: case Inst.leave:
        FReg.ESP = FReg.EBP;
        FReg.EBP = PopL();
        break;
      case Inst.pushb:
        if(!GetOperandAddr(1)) return false;
        operand = ReadByte(Addr[0]);
        PushB(operand);
        break;
      case Inst.pushw:
        if(!GetOperandAddr(1)) return false;
        w = ReadWord(Addr[0]);
        PushW(w);
        break;
      case Inst.pushl:
        if(!GetOperandAddr(1)) return false;
        iir = ReadInt(Addr[0]);
        PushL(iir);
        break;
      case Inst.popb:
        if(!GetOperandAddr(1)) return false;
        operand = PopB();
        WriteByte(Addr[0], operand);
        break;
      case Inst.popw:
        if(!GetOperandAddr(1)) return false;
        w = PopW();
        WriteWord(Addr[0], w);
        break;
      case Inst.popl:
        if(!GetOperandAddr(1)) return false;
        iir = PopL();
        WriteInt(Addr[0], iir);
        break;
      case Inst.jmp:
        return JMP();
      case Inst.ljmp:
        return LJMP();
      case Inst.jc:
        if (FReg.CF == 1) return JMP();
        break;
      case Inst.ljc:
        if (FReg.CF == 1) return LJMP();
        break;
      case Inst.jcxz:
        if (FReg.CX == 0) return JMP();
        break;
      case Inst.ljcxz:
        if (FReg.CX == 0) return LJMP();
        break;
      case Inst.jnc:
        if (FReg.CF == 0) return JMP();
        break;
      case Inst.ljnc:
        if (FReg.CF == 0) return LJMP();
        break;
      case Inst.je:
      case Inst.jz:
        if (FReg.ZF == 1) return JMP();
        break;
      case Inst.lje:
      case Inst.ljz:
        if (FReg.ZF == 1) return LJMP();
        break;
      case Inst.jne:
      case Inst.jnz:
        if (FReg.ZF == 0) return JMP();
        break;
      case Inst.ljne:
      case Inst.ljnz:
        if (FReg.ZF == 0) return LJMP();
        break;
      case Inst.js:
        if (FReg.SF == 1) return JMP();
        break;
      case Inst.ljs:
        if (FReg.SF == 1) return LJMP();
        break;
      case Inst.jns:
        if (FReg.SF == 0) return JMP();
        break;
      case Inst.ljns:
        if (FReg.SF == 0) return LJMP();
        break;
      case Inst.jg:
      case Inst.jnle:
        if (FReg.SF == FReg.OF && FReg.ZF == 0) return JMP();
        break;
      case Inst.ljg:
      case Inst.ljnle:
        if (FReg.SF == FReg.OF && FReg.ZF == 0) return LJMP();
        break;
      case Inst.jge:
      case Inst.jnl:
        if (FReg.SF == FReg.OF || FReg.ZF == 1) return JMP();
        break;
      case Inst.ljge:
      case Inst.ljnl:
        if (FReg.SF == FReg.OF || FReg.ZF == 1) return LJMP();
        break;
      case Inst.jl:
      case Inst.jnge:
        if (FReg.SF != FReg.OF && FReg.ZF == 0) return JMP();
        break;
      case Inst.ljl:
      case Inst.ljnge:
        if (FReg.SF != FReg.OF && FReg.ZF == 0) return LJMP();
        break;
      case Inst.jle:
        if (FReg.SF != FReg.OF || FReg.ZF == 1) return JMP();
        break;
      case Inst.ljle:
        if (FReg.SF != FReg.OF || FReg.ZF == 1) return LJMP();
        break;
      case Inst.ja:
      case Inst.jnbe:
        if (FReg.CF == 0 && FReg.ZF == 0) return JMP();
        break;
      case Inst.lja:
      case Inst.ljnbe:
        if (FReg.CF == 0 && FReg.ZF == 0) return LJMP();
        break;
      case Inst.jae:
      case Inst.jnb:
        if (FReg.CF == 0 || FReg.ZF == 1) return JMP();
        break;
      case Inst.ljae:
      case Inst.ljnb:
        if (FReg.CF == 0 || FReg.ZF == 1) return LJMP();
        break;
      case Inst.jb:
        if (FReg.CF == 1 && FReg.ZF == 0) return JMP();
        break;
      case Inst.ljb:
        if (FReg.CF == 1 && FReg.ZF == 0) return LJMP();
        break;
      case Inst.jbe:
        if (FReg.CF == 1 || FReg.ZF == 1) return JMP();
        break;
      case Inst.ljbe:
        if (FReg.CF == 1 || FReg.ZF == 1) return LJMP();
        break;

      case Inst.jo:
        if (FReg.OF == 1) return JMP();
        break;
      case Inst.ljo:
        if (FReg.OF == 1) return LJMP();
        break;
      case Inst.jno:
        if (FReg.OF == 0) return JMP();
        break;
      case Inst.ljno:
        if (FReg.OF == 0) return LJMP();
        break;

      case Inst.jp:
      case Inst.jpe:
        if (FReg.PF == 1) return JMP();
        break;
      case Inst.ljp:
      case Inst.ljpe:
        if (FReg.PF == 1) return LJMP();
        break;
      case Inst.jnp:
      case Inst.jpo:
        if (FReg.PF == 0) return JMP();
        break;
      case Inst.ljnp:
      case Inst.ljpo:
        if (FReg.PF == 0) return LJMP();
        break;
      case Inst.INT:
        if (!GetOperandAddr(1)) return false;
        w = ReadWord(Addr[0]);
        FSysCall.call(w);
        break;
      default:
        return false;
    }
    return true;
  }

}

