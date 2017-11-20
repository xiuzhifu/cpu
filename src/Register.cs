using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
public struct AddressingAddr {
    public Addressing addressing;
    public int addr;
    public int width;
}
public class Register {
  private const int MemSize = 15 * 4 + 1;
  private byte[] Mem = new byte[MemSize];

	private const int CAXADDR = 0 + 1;
	private const int CCXADDR = 1 * 4 + 1;
	private const int CDXADDR = 2 * 4 + 1;
	private const int CBXADDR = 3 * 4 + 1;
	private const int CSPADDR = 4 * 4 + 1;
	private const int CBPADDR = 5 * 4 + 1;
	private const int CSIADDR = 6 * 4 + 1;
	private const int CDIADDR = 7 * 4 + 1;
  private const int cCSADDR = 8 * 4 + 1;
  private const int CDSADDR = 9 * 4 + 1;
  private const int CESADDR = 10 * 4 + 1;
  private const int CFSADDR = 11 * 4 + 1;
  private const int CSSADDR = 12 * 4 + 1;
  private const int CIMMADDR = 13 * 4 + 1;
  private const int CMEMIMMADDR = 14 * 4 + 1;

	private const int CALADDR = 0 + 1;
	private const int CCLADDR = 1 * 4 + 1;
	private const int CDLADDR = 2 * 4 + 1;
	private const int CBLADDR = 3 * 4 + 1;
	private const int CAHADDR = 1 * 4 + 1 + 1;
	private const int CCHADDR = 2 * 4 + 1 + 1;
	private const int CDHADDR = 3 * 4 + 1 + 1;
	private const int CBHADDR = 4 * 4 + 1 + 1;

  public Int32 EIP;
  public byte CF;
  public byte DF;
  public byte IF;
  public byte TF;
  public byte OF;
  public byte SF;
  public byte PF;
  public byte ZF;
  public ushort IP {
    get {
      return (ushort)EIP;
    }
    set {
      EIP = (Int32)value;
    }
  }
	private readonly AddressingAddr[] CAddressingAddr = {
		new AddressingAddr() {addressing = Addressing.eax, addr = CAXADDR, width = 4},
		new AddressingAddr() {addressing = Addressing.ebx, addr = CBXADDR, width = 4},
		new AddressingAddr() {addressing = Addressing.ecx, addr = CCXADDR, width = 4},
		new AddressingAddr() {addressing = Addressing.edx, addr = CDXADDR, width = 4},
		new AddressingAddr() {addressing = Addressing.ebp, addr = CBPADDR, width = 4},
		new AddressingAddr() {addressing = Addressing.esp, addr = CSPADDR, width = 4},
		new AddressingAddr() {addressing = Addressing.esi, addr = CSIADDR, width = 4},
		new AddressingAddr() {addressing = Addressing.edi, addr = CDIADDR, width = 4},
		new AddressingAddr() {addressing = Addressing.ax, addr = CAXADDR, width = 2},
		new AddressingAddr() {addressing = Addressing.bx, addr = CBXADDR, width = 2},
		new AddressingAddr() {addressing = Addressing.cx, addr = CCXADDR, width = 2},
		new AddressingAddr() {addressing = Addressing.dx, addr = CDXADDR, width = 2},
		new AddressingAddr() {addressing = Addressing.bp, addr = CBPADDR, width = 2},
		new AddressingAddr() {addressing = Addressing.sp, addr = CSPADDR, width = 2},
		new AddressingAddr() {addressing = Addressing.si, addr = CSIADDR, width = 2},
		new AddressingAddr() {addressing = Addressing.di, addr = CDIADDR, width = 2},
		new AddressingAddr() {addressing = Addressing.ah, addr = CAHADDR, width = 1},
		new AddressingAddr() {addressing = Addressing.bh, addr = CBHADDR, width = 1},
		new AddressingAddr() {addressing = Addressing.ch, addr = CBHADDR, width = 1},
		new AddressingAddr() {addressing = Addressing.dh, addr = CBHADDR, width = 1},
		new AddressingAddr() {addressing = Addressing.al, addr = CALADDR, width = 1},
		new AddressingAddr() {addressing = Addressing.bl, addr = CBLADDR, width = 1},
		new AddressingAddr() {addressing = Addressing.cl, addr = CCLADDR, width = 1},
		new AddressingAddr() {addressing = Addressing.dl, addr = CDLADDR, width = 1},
		new AddressingAddr() {addressing = Addressing.cs, addr = cCSADDR, width = 4},
		new AddressingAddr() {addressing = Addressing.ds, addr = CDSADDR, width = 4},
		new AddressingAddr() {addressing = Addressing.es, addr = CESADDR, width = 4},
		new AddressingAddr() {addressing = Addressing.fs, addr = CFSADDR, width = 4},
		new AddressingAddr() {addressing = Addressing.ss, addr = CSSADDR, width = 4},
		new AddressingAddr() {addressing = Addressing.imm, addr = CIMMADDR, width = 4},
		new AddressingAddr() {addressing = Addressing.mem, addr = CMEMIMMADDR, width = 4},
	};

  public int GetRegisterAddr(Addressing addressing) {
    for (int i = 0; i < CAddressingAddr.Length; i++) {
      if (CAddressingAddr[i].addressing == addressing) 
        return - CAddressingAddr[i].addr;
    }
    return -1;
  }

  private bool GetRegisterAddr(Addressing addressing, ref int addr, ref int width) {
    return false;
  }

  public void WriteByte(Addressing addressing, byte b) {
    int addr = - GetRegisterAddr(addressing);
    if (addr >= MemSize) return;
    Mem[addr] = b;
  }

  public void WriteWord(Addressing addressing, Int16 w) {
    int addr = - GetRegisterAddr(addressing);
    if (addr >= MemSize) return;
    Mem[addr] = (byte)w;
    Mem[addr + 1] = (byte)(w >> 8);
  }

  public void WriteInt(Addressing addressing, Int32 i) {
    int addr = - GetRegisterAddr(addressing);
    if (addr >= MemSize) return;
    Mem[addr] = (byte)i;
    Mem[addr + 1] = (byte)(i >> 8);
    Mem[addr + 2] = (byte)(i >> 16);
    Mem[addr + 3] = (byte)(i >> 24);
  }

  public byte ReadByte(Addressing addressing) {
    int addr = - GetRegisterAddr(addressing);
    if (addr >= MemSize) return 0;
    return Mem[addr];
  }

  public Int16 ReadWord(Addressing addressing) {
    int addr = - GetRegisterAddr(addressing);
    if (addr >= MemSize) return 0;
    return (Int16)((Mem[addr + 1] << 8) | Mem[addr]);
  }

  public Int32 ReadInt(Addressing addressing) {
    int addr = - GetRegisterAddr(addressing);
    if (addr >= MemSize) return 0;
    return (Int32)((Mem[addr + 3] << 24) | (Mem[addr + 2] << 16) | (Mem[addr + 1] << 8) | Mem[addr]);
  }

  public void WriteByte(int addr, byte b) {
    addr = -addr;
    if (addr >= MemSize) return;
    Mem[addr] = b;
  }

  public void WriteWord(int addr, ushort w) {
    addr = -addr;
    if (addr >= MemSize) return;
    Mem[addr] = (byte)w;
    Mem[addr + 1] = (byte)(w >> 8);
  }

  public void WriteInt(int addr, Int32 i) {
    addr = -addr;
    if (addr >= MemSize) return;
    Mem[addr] = (byte)i;
    Mem[addr + 1] = (byte)(i >> 8);
    Mem[addr + 2] = (byte)(i >> 16);
    Mem[addr + 3] = (byte)(i >> 24);
  }

  public byte ReadByte(int addr) {
    addr = -addr;
    if (addr >= MemSize) return 0;
    return Mem[addr];
  }

  public ushort ReadWord(int addr) {
    addr = -addr;
    if (addr >= MemSize) return 0;
    return (ushort)((Mem[addr + 1] << 8) | Mem[addr]);
  }

  public Int32 ReadInt(int addr) {
    addr = -addr;
    if (addr >= MemSize) return 0;
    return (Int32)((Mem[addr + 3] << 24) | (Mem[addr + 2] << 16) | (Mem[addr + 1] << 8) | Mem[addr]);
  }

	public byte AL {
		get { 
			return Mem[CALADDR];
		}
		set { 
			Mem[CALADDR] = value;
		}
	}


	public byte AH {
		get { 
			return Mem[CAHADDR];
		}
		set { 
			Mem[CAHADDR] = value;
		}
	}

	public ushort AX {
		get { 
			return (ushort)(Mem[CAXADDR + 1] << 8 | Mem[CAXADDR]);
		}

		set { 
      Mem[CAXADDR + 1] = (byte)(value >> 8);
      Mem[CAXADDR] = (byte)value;
		}
	}

	public Int32 EAX {
		get { 
			return (Int32)(Mem[CAXADDR + 3] << 24 | Mem[CAXADDR + 2] << 16 | Mem[CAXADDR + 1] << 8 | Mem[CAXADDR]);
		}

		set { 
      Mem[CAXADDR + 3] = (byte)(value >> 24);
      Mem[CAXADDR + 2] = (byte)(value >> 16);
      Mem[CAXADDR + 1] = (byte)(value >> 8);
      Mem[CAXADDR] = (byte)value;
		}
	}

	public byte BL {
		get { 
			return Mem[CBLADDR];
		}
		set { 
			Mem[CBLADDR] = value;
		}
	}


	public byte BH {
		get { 
			return Mem[CBHADDR];
		}
		set { 
			Mem[CBHADDR] = value;
		}
	}

	public ushort BX {
		get { 
			return (ushort)(Mem[CBXADDR + 1] << 8 | Mem[CBXADDR]);
		}

		set { 
      Mem[CBXADDR + 1] = (byte)(value >> 8);
      Mem[CBXADDR] = (byte)value;
		}
	}

	public Int32 EBX {
		get { 
			return (Int32)(Mem[CBXADDR + 3] << 24 | Mem[CBXADDR + 2] << 16 | Mem[CBXADDR + 1] << 8 | Mem[CBXADDR]);
		}

		set { 
      Mem[CBXADDR + 3] = (byte)(value >> 24);
      Mem[CBXADDR + 2] = (byte)(value >> 16);
      Mem[CBXADDR + 1] = (byte)(value >> 8);
      Mem[CBXADDR] = (byte)value;
		}
	}

	public byte CL {
		get { 
			return Mem[CCLADDR];
		}
		set { 
			Mem[CCLADDR] = value;
		}
	}


	public byte CH {
		get { 
			return Mem[CCHADDR];
		}
		set { 
			Mem[CCHADDR] = value;
		}
	}

	public ushort CX {
		get { 
			return (ushort)(Mem[CCXADDR + 1] << 8 | Mem[CCXADDR]);
		}

		set { 
      Mem[CCXADDR + 1] = (byte)(value >> 8);
      Mem[CCXADDR] = (byte)value;
		}
	}

	public Int32 ECX {
		get { 
			return (Int32)(Mem[CCXADDR + 3] << 24 | Mem[CCXADDR + 2] << 16 | Mem[CCXADDR + 1] << 8 | Mem[CCXADDR]);
		}

		set { 
      Mem[CCXADDR + 3] = (byte)(value >> 24);
      Mem[CCXADDR + 2] = (byte)(value >> 16);
      Mem[CCXADDR + 1] = (byte)(value >> 8);
      Mem[CCXADDR] = (byte)value;
		}
	}

	public byte DL {
		get { 
			return Mem[CDLADDR];
		}
		set { 
			Mem[CDLADDR] = value;
		}
	}


	public byte DH {
		get { 
			return Mem[CDHADDR];
		}
		set { 
			Mem[CDHADDR] = value;
		}
	}

	public ushort DX {
		get { 
			return (ushort)(Mem[CDXADDR + 1] << 8 | Mem[CDXADDR]);
		}

		set { 
      Mem[CDXADDR + 1] = (byte)(value >> 8);
      Mem[CDXADDR] = (byte)value;
		}
	}

	public Int32 EDX {
		get { 
			return (Int32)(Mem[CDXADDR + 3] << 24 | Mem[CDXADDR + 2] << 16 | Mem[CDXADDR + 1] << 8 | Mem[CDXADDR]);
		}

		set { 
      Mem[CDXADDR + 3] = (byte)(value >> 24);
      Mem[CDXADDR + 2] = (byte)(value >> 16);
      Mem[CDXADDR + 1] = (byte)(value >> 8);
      Mem[CDXADDR] = (byte)value;
		}
	}


	public ushort SI {
		get { 
			return (ushort)(((ushort)Mem[CSIADDR] << 8) | Mem[CSIADDR + 1]);
		}

		set { 
			Mem[CSIADDR] = (byte)(value >> 8);
			Mem[CSIADDR + 1] = (byte)value;
		}
	}

	public ushort DI {
		get { 
			return (ushort)(((ushort)Mem[CDIADDR] << 8) | Mem[CDIADDR + 1]);
		}

		set { 
			Mem[CDIADDR] = (byte)(value >> 8);
			Mem[CDIADDR + 1] = (byte)value;
		}
	}

	public ushort BP {
		get { 
			return (ushort)(((ushort)Mem[CBPADDR] << 8) | Mem[CBPADDR + 1]);
		}

		set { 
			Mem[CBPADDR] = (byte)(value >> 8);
			Mem[CBPADDR + 1] = (byte)value;
		}
	}

	public ushort SP {
		get { 
			return (ushort)(((ushort)Mem[CSPADDR] << 8) | Mem[CSPADDR + 1]);
		}

		set { 
			Mem[CSPADDR] = (byte)(value >> 8);
			Mem[CSPADDR + 1] = (byte)value;
		}
	}

	public Int32 ESI {
		get { 
			return (Int32)(Mem[CSIADDR + 3] << 24 | Mem[CSIADDR + 2] << 16 | Mem[CSIADDR + 1] << 8 | Mem[CSIADDR]);
		}

		set { 
      Mem[CSIADDR + 3] = (byte)(value >> 24);
      Mem[CSIADDR + 2] = (byte)(value >> 16);
      Mem[CSIADDR + 1] = (byte)(value >> 8);
      Mem[CSIADDR] = (byte)value;
		}
	}

	public Int32 EDI {
		get { 
			return (Int32)(Mem[CDIADDR + 3] << 24 | Mem[CDIADDR + 2] << 16 | Mem[CDIADDR + 1] << 8 | Mem[CDIADDR]);
		}

		set { 
      Mem[CDIADDR + 3] = (byte)(value >> 24);
      Mem[CDIADDR + 2] = (byte)(value >> 16);
      Mem[CDIADDR + 1] = (byte)(value >> 8);
      Mem[CDIADDR] = (byte)value;
		}
	}

	public Int32 EBP {
		get { 
			return (Int32)(Mem[CBPADDR + 3] << 24 | Mem[CBPADDR + 2] << 16 | Mem[CBPADDR + 1] << 8 | Mem[CBPADDR]);
		}

		set { 
      Mem[CBPADDR + 3] = (byte)(value >> 24);
      Mem[CBPADDR + 2] = (byte)(value >> 16);
      Mem[CBPADDR + 1] = (byte)(value >> 8);
      Mem[CBPADDR] = (byte)value;
		}
	}

	public Int32 ESP {
		get { 
			return (Int32)(Mem[CSPADDR + 3] << 24 | Mem[CSPADDR + 2] << 16 | Mem[CSPADDR + 1] << 8 | Mem[CSPADDR]);
		}

		set { 
      Mem[CSPADDR + 3] = (byte)(value >> 24);
      Mem[CSPADDR + 2] = (byte)(value >> 16);
      Mem[CSPADDR + 1] = (byte)(value >> 8);
      Mem[CSPADDR] = (byte)value;
		}
	}

	public Int32  CS {
		get { 
			return (Int32)(Mem[cCSADDR + 3] << 24 | Mem[cCSADDR + 2] << 16 | Mem[cCSADDR + 1] << 8 | Mem[cCSADDR]);
		}

		set { 
      Mem[cCSADDR + 3] = (byte)(value >> 24);
      Mem[cCSADDR + 2] = (byte)(value >> 16);
      Mem[cCSADDR + 1] = (byte)(value >> 8);
      Mem[cCSADDR] = (byte)value;
		}
	}

	public Int32 DS {
		get { 
			return (Int32)(Mem[CDSADDR + 3] << 24 | Mem[CDSADDR + 2] << 16 | Mem[CDSADDR + 1] << 8 | Mem[CDSADDR]);
		}

		set { 
      Mem[CDSADDR + 3] = (byte)(value >> 24);
      Mem[CDSADDR + 2] = (byte)(value >> 16);
      Mem[CDSADDR + 1] = (byte)(value >> 8);
      Mem[CDSADDR] = (byte)value;
		}
	}

	public Int32 ES {
		get { 
			return (Int32)(Mem[CESADDR + 3] << 24 | Mem[CESADDR + 2] << 16 | Mem[CESADDR + 1] << 8 | Mem[CESADDR]);
		}

		set { 
      Mem[CESADDR + 3] = (byte)(value >> 24);
      Mem[CESADDR + 2] = (byte)(value >> 16);
      Mem[CESADDR + 1] = (byte)(value >> 8);
      Mem[CESADDR] = (byte)value;
		}
	}

	public Int32 FS {
		get { 
			return (Int32)(Mem[CFSADDR + 3] << 24 | Mem[CFSADDR + 2] << 16 | Mem[CFSADDR + 1] << 8 | Mem[CFSADDR]);
		}

		set { 
      Mem[CFSADDR + 3] = (byte)(value >> 24);
      Mem[CFSADDR + 2] = (byte)(value >> 16);
      Mem[CFSADDR + 1] = (byte)(value >> 8);
      Mem[CFSADDR] = (byte)value;
		}
	}

	public Int32 SS {
		get { 
			return (Int32)(Mem[CSSADDR + 3] << 24 | Mem[CSSADDR + 2] << 16 | Mem[CSSADDR + 1] << 8 | Mem[CSSADDR]);
		}

		set { 
      Mem[CSSADDR + 3] = (byte)(value >> 24);
      Mem[CSSADDR + 2] = (byte)(value >> 16);
      Mem[CSSADDR + 1] = (byte)(value >> 8);
      Mem[CSSADDR] = (byte)value;
		}
	}

	public Int32 IMM {
		get { 
			return (Int32)(Mem[CIMMADDR + 3] << 24 | Mem[CIMMADDR + 2] << 16 | Mem[CIMMADDR + 1] << 8 | Mem[CIMMADDR]);
		}

		set { 
      Mem[CIMMADDR + 3] = (byte)(value >> 24);
      Mem[CIMMADDR + 2] = (byte)(value >> 16);
      Mem[CIMMADDR + 1] = (byte)(value >> 8);
      Mem[CIMMADDR] = (byte)value;
		}
	}

	public Int32 MEMIMM {
		get { 
			return (Int32)(Mem[CMEMIMMADDR + 3] << 24 | Mem[CMEMIMMADDR + 2] << 16 | Mem[CMEMIMMADDR + 1] << 8 | Mem[CMEMIMMADDR]);
		}

		set { 
      Mem[CMEMIMMADDR + 3] = (byte)(value >> 24);
      Mem[CMEMIMMADDR + 2] = (byte)(value >> 16);
      Mem[CMEMIMMADDR + 1] = (byte)(value >> 8);
      Mem[CMEMIMMADDR] = (byte)value;
		}
	}
}

