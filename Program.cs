using System;

namespace cpu {
	class MainClass {
		static Cpu cpu = new Cpu32bit();
		public static void Main(string[] args) {
      if (!cpu.LoadFromFile("E:\\OneDrive\\lisp\\bin\\Debug\\asm.s")) {
        Console.WriteLine(cpu.Error);
      }
      else {
        Console.WriteLine("exec start");
        cpu.Exec();
        Console.WriteLine("exec end");
      }
     
      Console.WriteLine("end");
      //Test.TestAll();
     // Console.ReadLine();
    }
	}
}
