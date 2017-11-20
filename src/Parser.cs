using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;

public class Parser {
  private Lex lex;
  private List<Tuple<int, string, int, int>> BackFillAddrList = new List<Tuple<int, string, int, int>>();
  private List<string> ErrorList = new List<string>();
  private List<string> NeedAddrLabel = new List<string>();
  private Dictionary<string, int> LabelAddr = new Dictionary<string, int>();
  private Dictionary<string, int> EquValue = new Dictionary<string, int>();
  private byte[] Data = new byte[1024 * 1024];
  private int DataIndex = 0;
  private string EntryAddressLabel;
  private string FileName;
  private int CpuWidth;
  private OpCode opcode = new OpCode();
  public Parser(int cpuwidth) {
    CpuWidth = cpuwidth;
    lex = new Lex();
  }
  public byte[] GetInsts() {
    if (ErrorList.Count > 0) return null;
    byte[] buf = new byte[DataIndex + 1];
    Array.Copy(Data, buf, DataIndex + 1);
    return buf;
  }

  public void AddNeedAddrLabel(string s) {
    NeedAddrLabel.Add(s);
  }

  public void SetNeedAddrLabel(int addr) {
    if (NeedAddrLabel.Count > 0) {
      for (int i = 0; i < NeedAddrLabel.Count; i++) {
        AddLabelAddr(NeedAddrLabel[i], addr);
        SetEquValue(NeedAddrLabel[i], addr);
      }
    }
    NeedAddrLabel.Clear();
  }
  public string PreParseing(string s) {
    StringBuilder sb = new StringBuilder();
    int c = 0;
    int l = s.Length;
    while (c < l) {
      switch (s[c]) {
        case '\r':
          c++;
          break;
        case '@': case '#':
          while (c < l && s[c] != '\n') c++;
          break;
        default:
          sb.Append(s[c]);
          c++;
          break;
      }
    }
    //rept count endr

    return sb.ToString();
  }
  public void WriteByte(byte b) {
    WriteByte(DataIndex, b);
    DataIndex++;
  }

  public void WriteBytes(byte[] b) {
    for (int i = 0; i < b.Length; i++) {
      if (DataIndex >= Data.Length) return;
      WriteByte(DataIndex, b[i]);
      DataIndex++;
    }
  }

  public void WriteWord(short w) {
    WriteWord(DataIndex, w);
    DataIndex = DataIndex + 2;
  }

  public void WriteInt(Int32 i) {
    WriteInt(DataIndex, i);
    DataIndex = DataIndex + 4;
  }

  public void WriteByte(int addr, byte b) {
    if (addr >= Data.Length) throw new Exception("code size beyound code buffer");
    Data[addr] = b;
  }

  public void WriteWord(int addr, Int16 w) {
    if (addr >= Data.Length) throw new Exception("code size beyound code buffer");
    Data[addr] = (byte)w;
    Data[addr + 1] = (byte)(w >> 8);
  }

  public void WriteInt(int addr, Int32 i) {
    if (addr >= Data.Length) throw new Exception("code size beyound code buffer");
    Data[addr] = (byte)i;
    Data[addr + 1] = (byte)(i >> 8);
    Data[addr + 2] = (byte)(i >> 16);
    Data[addr + 3] = (byte)(i >> 24);
  }

  public byte ReadByte(int addr) {
    if (addr >= Data.Length) return 0;
    return Data[addr];
  }

  public Int16 ReadWord(int addr) {
    if (addr >= Data.Length) return 0;
    return (Int16)((Data[addr + 1] << 8) | Data[addr]);
  }

  public Int32 ReadInt(int addr) {
    if (addr >= Data.Length) return 0;
    return (Int32)((Data[addr + 3] << 24) | (Data[addr + 2] << 16) | (Data[addr + 1] << 8) | Data[addr]);
  }

  public void Load(string s) {
    LabelAddr.Clear();
    ErrorList.Clear();
    NeedAddrLabel.Clear();
    BackFillAddrList.Clear();
    EquValue.Clear();
    DataIndex = 0;
    WriteInt(4);//init entry point;
    s = PreParseing(s);
    lex.Load(s);
  }

  public void LoadFromFile(string filename) {
    if (File.Exists(filename)) {
      StreamReader sr = File.OpenText(filename);
      string s = sr.ReadToEnd();
      sr.Dispose();
      if (s != "") Load(s);
    }
  }

    public void Error(string s, int line = 0, int row = 0) {
        if (line > 0) {
            Console.WriteLine("(" + line.ToString() + "," + row.ToString() + "):" + s);
            ErrorList.Add(lex.GetLineNo() + " : " + s);
        }
        else {
            Console.WriteLine("(" + lex.GetLineNo().ToString() + "," + lex.GetRowNo().ToString() + "):" + s);
            ErrorList.Add(lex.GetLineNo() + " : " + s);
        }

  }

  public void Warning(string s) {
    Console.WriteLine("warning: (" + lex.GetLineNo().ToString() + "," + lex.GetRowNo().ToString() + "):" + s);
  }


  public bool CheckRegisterWidth(Addressing addressing, int width) {
    if (opcode.GetRegisterWidth(addressing) != width) {
      Error("inst expect register width is " + width.ToString() + " but " + opcode.GetRegisterWidth(addressing).ToString() + " found");
      lex.SkipLine();
      return false;
    }
    else
      return true;
  }

  public void AddLabelAddr(string name, int addr) {
    if (LabelAddr.ContainsKey(name)) {
      Error("duplicate definition label addr" + name);
      return;
    }
    LabelAddr.Add(name, addr);
  }

  public int GetLabelAddr(string name) {
    if (LabelAddr.ContainsKey(name))
      return LabelAddr[name];
    else
      return -1;
  }

  public void SetEquValue(string name, int i) {
    if (EquValue.ContainsKey(name))
      EquValue[name] = i;
    else
      EquValue.Add(name, i);
  }

  public bool GetEquValue(string name, ref int i) {
    if (EquValue.ContainsKey(name)) {
      i = EquValue[name];
      return true;
    } else {
      return false;
    }
  }

  public int StrToInt(string s) {
    if (s == null || s == "") return 0;
    if (s[0] == 0)
      return Convert.ToInt32(s, 8);
    else if (s.Length > 2 && (s[1] == 'x' || s[1] == 'X'))
      return Convert.ToInt32(s, 16);
    else
      return Convert.ToInt32(s, 10);
  }

  public int GetDot() {
    return DataIndex;
  }

  public void ParsingLabel(string name) {
    SetEquValue(name, ParsingExp());
  }

  public void Parsing() {
    for (;;) {
      Token tk = lex.NextToken();
      if (tk == Token.end)
        break;
      //dummy inst
      else if (tk == Token.dot) {
        SetNeedAddrLabel(DataIndex);
        ParsingDummyInst(tk);
      }
      else if (tk == Token.Ident) {
        //is addr lable?
        string name = lex.GetTokenString();
        lex.Match(Token.Ident);
        if (lex.NextToken() == Token.colon) {
          lex.Match(Token.colon);
          AddNeedAddrLabel(name);
        }
        //is =?
        else if (lex.NextToken() == Token.equ){
          lex.Match(Token.equ);
          AddLabelAddr(name, lex.GetLineNo());
          ParsingLabel(name);
        }
        else {
        SetNeedAddrLabel(DataIndex);
        ParsingText(name);
        }
      }
      else {
        Error("inst expect but " + lex.GetTokenString() + " found");
        lex.SkipLine();
      }
    }
    BackFillAddr();
    ParsingEnd();
  }

  public void ParsingEnd() {
    int addr = GetLabelAddr("_start");
    if (addr == -1) {
      Warning("can't found entry point label: start, the entry point well be set to first line");
      addr = 4;
    }
     
    WriteInt(0, addr);
  }

  public void BackFillAddr() {
    for (int i = 0; i < BackFillAddrList.Count; i++) {
      Tuple<int, string, int, int> bfr = BackFillAddrList[i];
      int addr = 0;
      if (GetEquValue(bfr.Item2, ref addr)) {
        WriteInt(bfr.Item1, addr - ReadInt(bfr.Item1));
      }
      else {
        Error("unknown identifier '" + bfr.Item2 + "'", bfr.Item3, bfr.Item4);
      }
    }
  }

  public void WriteInteger(int i, int width) {
    switch (width) {
      case 1:
        WriteByte((byte)i);
        break;
      case 2:
        WriteWord((short)i);
        break;
      case 4:
        WriteInt(i);
        break;
    }
  }
  //exp-> term (+|-) term|term
  public int ParsingExp() {
    int i = ParsingTerm();
    for (;;) {
      Token tk = lex.NextToken();
      if (tk == Token.plus) {
        lex.Match(Token.plus);
        i += ParsingTerm();
      }
      else if (tk == Token.minus) {
        lex.Match(Token.minus);
        i -= ParsingTerm();
      }
      else
        return i;
    }
  }
  //term -> factor (*|/|%) factor|factor
  public int ParsingTerm() {
    int i = ParsingFactor();
    for (;;) {
      Token tk = lex.NextToken();
      if (tk == Token.mul) {
        lex.Match(Token.mul);
        i *= ParsingFactor();
      }
      else if (tk == Token.div) {
        lex.Match(Token.div);
        int t = ParsingFactor();
        if (t == 0) {
          Error("dividend can not be zero");
          lex.SkipLine();
          return 0;
        }
        else
          i /= t;
      }
      else if (tk == Token.percent) {
        lex.Match(Token.percent);
        i %= ParsingFactor();
      }
      else
        return i;
    }
  }
  //factor-> (exp)|num|identifier|.
  public int ParsingFactor() {
    Token tk = lex.NextToken();
    int i;
    int sign = 1;
    if (tk == Token.plus || tk == Token.minus) {
      lex.Match(tk);
      sign = sign * StrToInt(lex.GetTokenString());
      tk = lex.NextToken();
    }
    tk = lex.NextToken();
    if (tk == Token.Number) {
      lex.Match(Token.Number);
      return StrToInt(lex.GetTokenString());
    }
    else if (tk == Token.Ident) {
      string s = lex.GetTokenString();
      lex.Match(Token.Ident);
      i = GetLabelAddr(s);
      if (i == -1) {
        Error("can't found identifier '" + s + "'");
        lex.SkipLine();
        return 0;
      }
      else {
        return i;
      }
    }
    else if (tk == Token.dot) {
      lex.Match(Token.dot);
      return GetDot();
    }
    else if (tk == Token.leftbracket) {
      lex.Match(Token.leftbracket);
      i = ParsingExp();
      lex.Match(Token.rightbracket);
      return i;
    }
    else {
      Error("unknown identifier: '" + lex.GetTokenString() + "'");
      lex.SkipLine();
      return 0;
    }
  }

  public void ParsingDataDefineInteger(int width) {
    for (;;) {
      Token tk = lex.NextToken();
      WriteInteger(ParsingExp(), width);
      tk = lex.NextToken();
      if (tk == Token.comma) {
        lex.Match(Token.comma);
      }
      else { 
        break;
      }
    }
  }

  public void ParsingDataDefineString() {
    while (lex.NextToken() == Token.String) {
      lex.Match(Token.String);
      string s = lex.GetTokenString();
      s = s.Substring(1, s.Length - 2);
      byte[] b = System.Text.Encoding.UTF8.GetBytes(s);
      WriteBytes(b);
      if (lex.NextToken() == Token.comma)
        lex.Match(Token.comma);
      else
        return;
    }
    Error("string expect but" + lex.GetTokenString() + " found");
    lex.SkipLine();
  }

  public void ParsingDataDefineSet() {

  }

  public void ParsingDummyInst(Token tk) {
    if (tk == Token.dot) {
      lex.Match(Token.dot);
      tk = lex.NextToken();
      if (tk == Token.equ) {
        lex.Match(Token.equ);
        DataIndex = ParsingExp();
      }
      else if (tk == Token.Ident) {
        string s = lex.GetTokenString();
        s = s.ToLower();
        if (s == "cfi_startproc" || s == "cfi_endproc" || s == "cfi_def_cfa" ||
          s == "cfi_def_cfa_register" || s == "cfi_def_cfa_offset" || s == "cfi_adjust_cfa_offset" || s == "cfi_offset" || s == "cfi_restore") {
          lex.SkipLine();
        }
        else if (s == "byte") {
          lex.Match(Token.Ident);
          ParsingDataDefineInteger(1);
        }
        else if (s == "word") {
          lex.Match(Token.Ident);
          ParsingDataDefineInteger(2);
        }
        else if (s == "long") {
          lex.Match(Token.Ident);
          ParsingDataDefineInteger(4);
        }
        else if (s == "ascii" || s == "string" || s == "utf8") {
          lex.Match(Token.Ident);
          ParsingDataDefineString();

        }
        else if (s == "equ") {
          lex.Match(Token.Ident);
          lex.SkipLine();
          //.equ factor,3
          //.equ LINUX_SYS_CALL, 0x80
          //; 引用静态数据元素
          //movl $LINUX_SYS_CALL, % eax
        }
        else if (s == "set") {
          lex.Match(Token.Ident);
          ParsingDataDefineSet();
        }
        else if (s == "space") {
          lex.Match(Token.Ident);

        }
        else if (s == "fill") {
          lex.Match(Token.Ident);

        }
        else if (s == "text") {
          lex.Match(Token.Ident);

        }
        else if (s == "data") {
          lex.Match(Token.Ident);

        }
        else if (s == "bss") {
          lex.Match(Token.Ident);

        }
        else if (s == "fill") {
          lex.Match(Token.Ident);

        }
        else if (s == "text") {
          lex.Match(Token.Ident);
          lex.SkipLine();

        }
        else if (s == "data") {
          lex.Match(Token.Ident);
          lex.SkipLine();
        }
        else if (s == "section") {
          lex.Match(Token.Ident);
          lex.SkipLine();
        }
        else if (s == "align") {
          lex.Match(Token.Ident);
          lex.SkipLine();
        }
        else if (s == "def") {
          lex.Match(Token.Ident);
          lex.SkipLine();
        }
        else if (s == "ident") {
          lex.Match(Token.Ident);
          lex.SkipLine();
        }
        else if (s == "file") {
          lex.Match(Token.Ident);
          if (lex.Match(Token.String)) {
            FileName = lex.GetTokenString();
          }
          else {
            Error("file string expect but" + lex.GetTokenString() + " found");
            lex.SkipLine();
            return;
          }
        }
        else if (s == "globl") {
          lex.Match(Token.Ident);
          if (lex.Match(Token.Ident)) {
            EntryAddressLabel = lex.GetTokenString();
          }
          else {
            Error("identifier expect but" + lex.GetTokenString() + " found");
            lex.SkipLine();
            return;
          }
        }
        else {
          Error("unknown dummy inst '" + lex.GetTokenString() + "'");
          lex.SkipLine();
          return;
        }
      }
      else {
        Error("dummy inst expect but" + lex.GetTokenString() + " found");
        lex.SkipLine();
        return;
      }
    }
  }

	public Addressing ParsingRegister() {
		lex.Match(Token.percent);
		Token tk = lex.NextToken();
		if (tk != Token.Ident) {
			Error("register name expect but " + lex.GetTokenString() + " found");
			lex.SkipLine();
			return Addressing.none;
		}

		Addressing addressing = opcode.isRegister(lex.GetTokenString());
        if (addressing == Addressing.none) {
            Error("register name expect but " + lex.GetTokenString() + " found");
            lex.SkipLine();
            return Addressing.none;
        }
    lex.Match(Token.Ident);
		return addressing;
	}

  public bool CheckIndexedAddressingRegisterWidth(Addressing addressing, int width) {
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
    return true;
  }

	public void ParsingIndexedAddressingMode(int index) {
    int width = opcode.GetInstWidth(opcode.inst);
    opcode.scale = 1;
    opcode.BaseReg = Addressing.none;
    opcode.IndexReg = Addressing.none;
    opcode.MemImm = opcode.TempImm;
    opcode.TempImm = 0;
    opcode.Register[index] = Addressing.mem;
    lex.Match(Token.leftbracket);
		Token tk = lex.NextToken();
		if (tk == Token.percent) {
			Addressing addressing = ParsingRegister();
			if (addressing == Addressing.none) return;
      if (width == 1) {
        if (addressing != Addressing.bp) {
          Error("in 8bit inst base register only can assigned bx or bp");
          lex.SkipLine();
          return;
        }
      }
      else if (width == 2) {
        if (addressing != Addressing.bx && addressing != Addressing.bp) {
          Error("in 16bit inst base register only can assigned bx, bp");
          lex.SkipLine();
          return;
        }
      }
      else if (width == 4) {
        //理论上基址寄存器就是这几个，但是，看gcc生成的汇编似乎所有的寄存器都可以的，先去掉
        //if (addressing != Addressing.ebx||addressing != Addressing.ebp||addressing != Addressing.bx||addressing != Addressing.bp) {
        //  Error("in 32bit inst base register only can assigned ebx, ebp, bx, bp");
        //  lex.SkipLine();
        //  return;
        //}
      }
      if (opcode.SegReg == Addressing.none) {
        if (addressing ==Addressing.ebp || addressing == Addressing.bp || addressing ==Addressing.esp || addressing == Addressing.sp) opcode.SegReg = Addressing.ss; else opcode.SegReg = Addressing.ds;
      }
      opcode.BaseReg = addressing;
      tk = lex.NextToken();
      if (tk == Token.comma) {
        lex.Match(Token.comma);
        tk = lex.NextToken();
        if (tk == Token.percent) {
          addressing = ParsingRegister();
          if (addressing == Addressing.none) return;
          if (width == 1) {
            if (addressing != Addressing.si && addressing != Addressing.di && addressing != Addressing.sp) {
            Error("in 8bit inst index register only can assigned si, di or sp");
            lex.SkipLine();
            return;
          }
          }
          else if (width == 2) {
            if (addressing != Addressing.ax && addressing != Addressing.cx && addressing != Addressing.dx 
            || addressing != Addressing.si && addressing != Addressing.di && addressing != Addressing.sp) {
              Error("in 16bit inst index register only can assigned ax, cx, dx, si, di, sp");
              lex.SkipLine();
              return;
            }
          }
          else if (width == 4) {
            if (addressing != Addressing.eax && addressing != Addressing.ecx && addressing != Addressing.edx&&
                addressing != Addressing.esi && addressing != Addressing.edi && addressing != Addressing.esp&&
                addressing != Addressing.ax && addressing != Addressing.cx && addressing != Addressing.dx&& 
                addressing != Addressing.si && addressing != Addressing.di && addressing != Addressing.sp) {
              Error("in 32bit inst index register only can assigned eax, ecx, edx, esi, edi, esp, ax, cx, dx, si, di, sp");
              lex.SkipLine();
              return;
            }
          }
          opcode.IndexReg = addressing;
          tk = lex.NextToken();
          if (tk == Token.comma) {
            lex.Match(Token.comma);
            tk = lex.NextToken();
            if (tk == Token.Number) {
              int scale = Int32.Parse(lex.GetTokenString());
              lex.Match(Token.Number);
              if (scale == 1) {             
                opcode.scale = 1;
              }
              else if (scale == 2) {
                opcode.scale = 2;
              }
              else if (scale == 4) {
                opcode.scale = 4;
              }
              else if (scale == 8) {
                opcode.scale = 8;
              }
              else {
                Error("scale value is only 1, 2, 4, 8");
                lex.SkipLine();
                return;
              }
            }
            else {
              Error("need scale value");
              lex.SkipLine();
              return;
            }
            lex.Match(Token.rightbracket);
          }
          else if (tk == Token.rightbracket) {
            lex.Match(Token.rightbracket);
            opcode.scale = 1;
          }
          else {
            Error("‘）’ expect but" + lex.GetTokenString() + " found");
            lex.SkipLine();
            return;
          }
        }
        else {
          Error("register expect but" + lex.GetTokenString() + " found");
          lex.SkipLine();
          return;
        }
      }
      else if (tk == Token.rightbracket) {
        lex.Match(Token.rightbracket);
      }
      else {
        Error("right bracket expect but" + lex.GetTokenString() + " found");
        lex.SkipLine();
        return;
      }
		}
		else if (tk == Token.comma) {
			lex.Match(Token.comma);
      tk = lex.NextToken();
      if (tk == Token.percent) {
        Addressing addressing = ParsingRegister();
        if (addressing == Addressing.none) return;
        if (width == 1) {
          if (addressing != Addressing.si||addressing != Addressing.di||addressing != Addressing.sp) {
          Error("in 8bit inst index register only can assigned si, di or sp");
          lex.SkipLine();
          return;
        }
        }
        else if (width == 2) {
          if (addressing != Addressing.ax || addressing != Addressing.cx || addressing != Addressing.dx 
          || addressing != Addressing.si || addressing != Addressing.di || addressing != Addressing.sp) {
            Error("in 16bit inst index register only can assigned ax, cx, dx, si, di, sp");
            lex.SkipLine();
            return;
          }
        }
        else if (width == 4) {
          /*
          if (addressing != Addressing.eax || addressing != Addressing.ecx || addressing != Addressing.edx||
              addressing != Addressing.esi || addressing != Addressing.edi || addressing != Addressing.esp ||
              addressing != Addressing.ax || addressing != Addressing.cx || addressing != Addressing.dx 
          || addressing != Addressing.si || addressing != Addressing.di || addressing != Addressing.sp) {
            Error("in 32bit inst index register only can assigned eax, ecx, edx, esi, edi, esp, ax, cx, dx, si, di, sp");
            lex.SkipLine();
            return;
          }
          */
        }
        if (opcode.SegReg == Addressing.none) {
          if (addressing ==Addressing.ebp || addressing == Addressing.bp || addressing ==Addressing.esp || addressing == Addressing.sp) opcode.SegReg = Addressing.ss; else opcode.SegReg = Addressing.ds;
        }
        opcode.IndexReg = addressing;
        tk = lex.NextToken();
        if (tk == Token.comma) {
          lex.Match(Token.comma);
          tk = lex.NextToken();
          if (tk == Token.Number) {
            int scale = Int32.Parse(lex.GetTokenString());
            lex.Match(Token.Number);
            if (scale == 1) {
              opcode.scale = 1;
            }
            else if (scale == 2) {
              opcode.scale = 1;
            }
            else if (scale == 4) {
              opcode.scale = 1;
            }
            else if (scale == 8) {
              opcode.scale = 1;
            }
            else {
              Error("scale value is only 1, 2, 4, 8");
              lex.SkipLine();
              return;
            }
          }
          else {
            Error("need scale value");
            lex.SkipLine();
            return;
          }
          lex.Match(Token.rightbracket);
        }
        else if (tk == Token.rightbracket) {
          opcode.scale = 1;
          lex.Match(Token.rightbracket);
        }
        else {
          Error("')' expect but " + lex.GetTokenString() + " found");
          lex.SkipLine();
        }
      }
		}
	}
		
	public bool ParsingImmVariable(Token tk, int index) {
		string s = lex.GetTokenString();
		int i = 0;
        if (GetEquValue(s, ref i)) {
            lex.Match(Token.Ident);
            opcode.Imm = i;
            opcode.Register[index] = Addressing.imm;
            return true;
        } else if (opcode.IsCanBackFillInst()) {
            opcode.BackFillLabel = s;
            opcode.Imm = DataIndex;
            opcode.LineNo = lex.GetLineNo();
            opcode.RowNo = lex.GetRowNo();
            lex.Match(Token.Ident);
            opcode.Register[index] = Addressing.backfill;
            return true;
        }
        else {
            Error("unknown identifier: " + s);
            lex.SkipLine();
            return false;
        }	
	}

	public bool ParsingImmValue(Token tk) {
		int imm = 1;
		if (tk == Token.plus || tk == Token.minus) {
			if (tk == Token.minus) imm = -1;
			lex.Match(tk);
			tk = lex.NextToken();
		}
		if (tk != Token.Number) {
			Error("number expect but" + lex.GetTokenString() + " found");
			lex.SkipLine();
			return false;
		}

		imm = imm * StrToInt(lex.GetTokenString());
		opcode.TempImm = imm;
    lex.Match(Token.Number);
    return true;
	}

	public void ParsingText(string s) {
    Token tk;
    int instwidth = 0;

    if (s != null) {
      //is pre inst?
      opcode.Clear();
			Inst inst = OpCode.IsPreOpCode(s);
			if (inst != Inst.none) {
				opcode.preinst = inst;
				tk = lex.NextToken();
        if (tk != Token.Ident) {
				  Error("inst expect but " + s + " found");
          lex.SkipLine();
          return;
        }
        s = lex.GetTokenString();
        lex.Match(Token.Ident);
			};
			// is inst?
			inst = opcode.IsOpCode(s);
			if (inst == Inst.none) {
				Error("unknow inst: " + s);
				lex.SkipLine();
				return;
			} else {
				opcode.inst = inst;
        instwidth = opcode.GetInstWidth(opcode.inst);
      }
	  }
		int count = opcode.ParamCount;
		int index = 0;
    while (count > 0) {
      tk = lex.NextToken();
      // a indexed addressing mode like -1(,,)
      if (tk == Token.plus || tk == Token.minus || tk == Token.Number) {
        if (!ParsingImmValue(tk)) return;
        tk = lex.NextToken();
        if (tk == Token.leftbracket) {
          ParsingIndexedAddressingMode(index);
        }
        else {
          Error("'(' expect but" + lex.GetTokenString() + " found");
          lex.SkipLine();
          return;
        }
      }
      // (,,)
      else if (tk == Token.leftbracket) {
        ParsingIndexedAddressingMode(index);
      }
      //immVariable or indexed addressing value(,,)
      else if (tk == Token.Ident) {
        if (!ParsingImmVariable(tk, index)) return;
        tk = lex.NextToken();
        if (tk == Token.leftbracket) ParsingIndexedAddressingMode(index);
      }
      //register addressing like %eax , %cs or indexed addressing %cs(,,) %cs:-10(,,) %cs:value(,,)
      else if (tk == Token.percent) {
        Addressing addressing = ParsingRegister();
        if (addressing == Addressing.none) return;

        if (opcode.IsSegRegister(addressing)) {
          opcode.SegReg = addressing;
          tk = lex.NextToken();
          if (tk == Token.leftbracket) ParsingIndexedAddressingMode(index);
          else if (tk == Token.Ident) {
            if (!ParsingImmVariable(tk, index)) return;
            tk = lex.NextToken();
            if (tk == Token.leftbracket) ParsingIndexedAddressingMode(index);
          }
          else if (tk == Token.plus || tk == Token.minus || tk == Token.Number) {
            if (!ParsingImmValue(tk)) return;
            tk = lex.NextToken();
            opcode.Imm = opcode.TempImm;
            if (tk == Token.leftbracket) ParsingIndexedAddressingMode(index);
          }
          else
            opcode.Register[index] = addressing;
        }
        else {
            if (CheckRegisterWidth(addressing, instwidth))
                opcode.Register[index] = addressing;
            else
                return;   
       }
      }
      //imm addr $10 or $addr
      else if (tk == Token.dollar) {
        lex.Match(Token.dollar);
        tk = lex.NextToken();
        if (tk == Token.Ident) {
          s = lex.GetTokenString();
          int addr = GetLabelAddr(s);
          if (addr == -1) {
            Error("unknown identifier: " + s);
            lex.SkipLine();
            return;
          }
          lex.Match(Token.Ident);
          opcode.Imm = addr;
          opcode.Register[index] = Addressing.imm;
        }
        else if (tk == Token.plus || tk == Token.minus || tk == Token.Number) {
          if (!ParsingImmValue(tk)) return;
          opcode.Imm = opcode.TempImm;
          opcode.Register[index] = Addressing.imm;
        }
      }

      count--;
      if (count > 0) {
        if (!lex.Match(Token.comma)) {
          Error("',' expect but " + lex.GetTokenString() + " found");
          lex.SkipLine();
        }
      }
       
      index++;
    }
    EmitCode();
	}

  public void EmitRegister(int index) {
    Addressing addressing = opcode.Register[index];
    switch (addressing) {
    case Addressing.eax:
    case Addressing.ebx:
    case Addressing.ecx:
    case Addressing.edx:
    case Addressing.ebp:
    case Addressing.esp:
    case Addressing.esi:
    case Addressing.edi:
    case Addressing.ax:
    case Addressing.bx:
    case Addressing.cx:
    case Addressing.dx:
    case Addressing.ah:
    case Addressing.bh:
    case Addressing.ch:
    case Addressing.dh:
    case Addressing.al:
    case Addressing.bl:
    case Addressing.cl:
    case Addressing.dl:
    case Addressing.bp:
    case Addressing.sp:
    case Addressing.si:
    case Addressing.di:
    case Addressing.cs:
    case Addressing.ds:
    case Addressing.es:
    case Addressing.fs:
    case Addressing.ss:
      WriteByte((byte)addressing);
      Console.Write("%" + addressing.ToString());
      break;
    case Addressing.imm:
      WriteByte((byte)Addressing.imm);
      if (CpuWidth == 2)
        WriteWord((short)opcode.Imm);
      else if (CpuWidth == 4)
        WriteInt(opcode.Imm);
      else
        WriteWord((short)opcode.Imm);    
      Console.Write("$" + opcode.Imm);
      break;
    case Addressing.mem:
      WriteByte((byte)Addressing.mem);
      WriteByte((byte)opcode.SegReg);
      WriteByte((byte)opcode.BaseReg);
      WriteByte((byte)opcode.IndexReg);
      WriteByte(opcode.scale);
      string segreg = "";
      if (opcode.SegReg != Addressing.ds) segreg = "%" + opcode.SegReg.ToString() + ":";
      string basereg = "";
      if (opcode.BaseReg != Addressing.none) basereg = "%" + opcode.BaseReg.ToString();
      string indexreg = "";
      if (opcode.IndexReg != Addressing.none) indexreg = ",%" + opcode.IndexReg.ToString();
      string scalestring = "";
      if (opcode.scale > 0)  scalestring = "," + opcode.scale.ToString();

      if (CpuWidth == 2)
        WriteWord((short)opcode.MemImm);
      else if (CpuWidth == 4)
        WriteInt(opcode.MemImm);
      else
        WriteWord((short)opcode.MemImm);
      if (opcode.MemImm > 0)
          Console.Write(segreg + opcode.MemImm.ToString() + "(" + basereg + indexreg + scalestring + ")");
      else
          Console.Write(segreg + "(" + basereg + indexreg + scalestring + ")");
      break;
    case Addressing.backfill:
      BackFillAddrList.Add(new Tuple<int, string, int, int>(DataIndex, opcode.BackFillLabel, opcode.LineNo, opcode.RowNo));
      WriteInt(opcode.Imm);
      Console.Write(opcode.Imm);
      break;
    default:
      break;
    }
  }

  public void EmitCode() {
    if (opcode.inst != Inst.none) {
      if (opcode.preinst != Inst.none) {
        WriteByte((byte)opcode.preinst);      
        Console.Write(opcode.preinst.ToString());
      }
      WriteByte((byte)opcode.inst);
      Console.Write(opcode.inst.ToString() + " ");
      if (opcode.ParamCount > 0) {
        for (int i = 0; i < opcode.ParamCount; i++) {
          EmitRegister(i);
          if (i < opcode.ParamCount - 1 && opcode.ParamCount != 0) Console.Write(", ");
        }
      }
      Console.Write("\n");
    }
  }
}

