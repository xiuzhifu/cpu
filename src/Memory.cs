using System;

public class Memory {
  private byte[] Mem;
  private int MemSize;
  public Memory(int memsize = 1024 * 1024) {
    MemSize = memsize;
    Mem = new byte[MemSize];
  }

  public int GetMemSize() {
    return MemSize;
  }

  public void WriteByte(int addr, byte b) {
    if (addr >= MemSize) return;
    Mem[addr] = b;
  }

  public bool WriteBytes(int addr, byte[] b) {
    if (addr + b.Length >= MemSize) return false;
    b.CopyTo(Mem, addr);
    return true;
  }

  public void WriteWord(int addr, ushort w) {
    if (addr >= MemSize) return;
    Mem[addr] = (byte)w;
    Mem[addr + 1] = (byte)(w >> 8);
  }

  public void WriteInt(int addr, Int32 i) {
    if (addr >= MemSize) return;
    Mem[addr] = (byte)i;
    Mem[addr + 1] = (byte)(i >> 8);
    Mem[addr + 2] = (byte)(i >> 16);
    Mem[addr + 3] = (byte)(i >> 24);
  }

  public byte ReadByte(int addr) {
    if (addr >= MemSize) return 0;
    return Mem[addr];
  }

  public byte[] ReadBytes(int addr, int len) {
    if (addr + len >= MemSize) return null;
    byte[] b = new byte[len];
    Array.Copy(Mem, addr, b, 0, len);
    return b;
  }

  public ushort ReadWord(int addr) {
    if (addr >= MemSize) return 0;
    return (ushort)((Mem[addr + 1] << 8) | Mem[addr]);
  }

  public Int32 ReadInt(int addr) {
    if (addr >= MemSize) return 0;
    return (Int32)((Mem[addr + 3] << 24) | (Mem[addr + 2] << 16) | (Mem[addr + 1] << 8) | Mem[addr]);
  }
}

