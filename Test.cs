using System;
using System.Collections.Generic;
using System.IO;
class Test {
  public static void TestAll() {
    Test16bitcpu();
    //Test32bitcpu();
  }

  public static void Test16bitcpu() {
    string[] files = Directory.GetFiles(System.AppDomain.CurrentDomain.BaseDirectory + "\\..\\..\\test\\16bit");
    for (int i = 0; i < files.Length; i++) {
      Cpu cpu = new Cpu16bit();
      cpu.LoadFromFile(files[i]);
      cpu.Exec();
    }
  }

  public static void Test32bitcpu() {
    string[] files = Directory.GetFiles(System.AppDomain.CurrentDomain.BaseDirectory + "\\..\\..\\test\\32bit");
    for (int i = 0; i < files.Length; i++) {
      Cpu cpu = new Cpu32bit();
      cpu.LoadFromFile(files[i]);
      cpu.Exec();
    }
  }
}
