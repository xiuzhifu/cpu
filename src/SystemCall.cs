using System;
using System.Collections.Generic;

public delegate void SysCall();
public class SystemCall {
  private Memory _mem;
  private Register _reg;
  private Dictionary<int, SysCall> _systemCall = new Dictionary<int, SysCall>();
  public int CSYSCALL_PRINT = 1;
  public int CSYSCALL_TESTEQUSTRING = 2;
  public int CSYSCALL_TESTUNEQUSTRING = 3;
  public int CSYSCALL_TESTEQUREGISTER = 4;
  public int CSYSCALL_TESTUNEQUREGISTER = 5;
  public int CSYSCALL_PRINTEAX = 6;
  //public const int CSYSCALL_
  //public const int CSYSCALL_

  public SystemCall(Register reg, Memory mem) {
    _reg = reg;
    _mem = mem;
    initSysCall();
  }

  private void initSysCall() {
    addSysCall(CSYSCALL_PRINT, syscall_Print);
    addSysCall(CSYSCALL_TESTEQUSTRING, syscall_TestEquString);
    addSysCall(CSYSCALL_TESTUNEQUSTRING, syscall_TestUnEquString);
    addSysCall(CSYSCALL_TESTEQUREGISTER, syscall_TestEquInt);
    addSysCall(CSYSCALL_TESTUNEQUREGISTER, syscall_TestUnEquInt);
    addSysCall(CSYSCALL_PRINTEAX, syscall_PrintEax);

  }

  private void addSysCall(int id, SysCall syscall) {
    _systemCall.Add(id, syscall);
  }

  public void call(int id) {
    if (_systemCall.ContainsKey(id)) {
      SysCall syscall = _systemCall[id];
      syscall();
    }
  }

  private void syscall_Print() {
    byte[] b = _mem.ReadBytes(_reg.EAX, _reg.ECX);
    string s = System.Text.Encoding.UTF8.GetString(b);
    Console.WriteLine(s);
  }

  private void syscall_PrintEax() {
    Console.WriteLine(_reg.EAX.ToString());
  }

  public void syscall_TestEquString() {
    byte[] b1 = _mem.ReadBytes(_reg.EAX, _reg.ECX);
    byte[] b2 = _mem.ReadBytes(_reg.EBX, _reg.ECX);
    string s1 = System.Text.Encoding.UTF8.GetString(b1);
    string s2 = System.Text.Encoding.UTF8.GetString(b2);
    if (s1 == s2) {
      Console.WriteLine("test string: " + _reg.EDX.ToString() + " test passed.");
    }
    else {
      Console.WriteLine("test string: " + _reg.EDX.ToString() + " test failed.");
    }
  }

  public void syscall_TestUnEquString() {
    byte[] b1 = _mem.ReadBytes(_reg.EAX, _reg.ECX);
    byte[] b2 = _mem.ReadBytes(_reg.EBX, _reg.ECX);
    string s1 = System.Text.Encoding.UTF8.GetString(b1);
    string s2 = System.Text.Encoding.UTF8.GetString(b2);
    if (s1 != s2) {
      Console.WriteLine("test string: " + _reg.EDX.ToString() + " test passed.");
    }
    else {
      Console.WriteLine("test string: " + _reg.EDX.ToString() + " test failed.");
    }
  }

  public void syscall_TestEquInt() {
    if (_reg.EAX == _reg.EBX) {
      Console.WriteLine("test integer: " + _reg.EDX.ToString() + " test passed.");
    }
    else {
      Console.WriteLine("test integer: " + _reg.EDX.ToString() + " test failed." + _reg.EAX.ToString() + " != "+ _reg.EBX.ToString());
    }
  }

  public void syscall_TestUnEquInt() {
    if (_reg.EAX != _reg.EBX) {
      Console.WriteLine("test integer: " + _reg.EDX.ToString() + " test passed.");
    }
    else {
      Console.WriteLine("test integer: " + _reg.EDX.ToString() + " test failed." + _reg.EAX.ToString() + " != " + _reg.EBX.ToString());
    }
  }

}

