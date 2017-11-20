using System;
using System.Collections.Generic;

class Cpu32bit: Cpu {
  public Cpu32bit(int memsize = 1024 * 1024): base(4, memsize) {
    FReg.SS = memsize - 1024 - 1;
    FReg.EBP = FReg.SS;
    FReg.ESP = memsize - 1;
  }
  protected override bool IsSupportInst(Inst inst) {
    return
      inst == Inst.movb ||
      inst == Inst.movw ||
      inst == Inst.movl ||
      inst == Inst.movsb ||
      inst == Inst.movsw ||
      inst == Inst.movsl ||
      inst == Inst.pushb ||
      inst == Inst.pushw ||
      inst == Inst.pushl ||
      inst == Inst.popb ||
      inst == Inst.popw ||
      inst == Inst.popl ||
      inst == Inst.cmpb ||
      inst == Inst.cmpw ||
      inst == Inst.cmpl ||
      inst == Inst.testb ||
      inst == Inst.testw ||
      inst == Inst.testl ||
      inst == Inst.leab ||
      inst == Inst.leaw ||
      inst == Inst.leal ||
      inst == Inst.incb ||
      inst == Inst.incw ||
      inst == Inst.incl ||
      inst == Inst.decb ||
      inst == Inst.decw ||
      inst == Inst.decl ||
      inst == Inst.negb ||
      inst == Inst.negw ||
      inst == Inst.negl ||
      inst == Inst.notb ||
      inst == Inst.notw ||
      inst == Inst.notl ||
      inst == Inst.salb ||
      inst == Inst.salw ||
      inst == Inst.sall ||
      inst == Inst.shlb ||
      inst == Inst.shlw ||
      inst == Inst.shll ||
      inst == Inst.sarb ||
      inst == Inst.sarw ||
      inst == Inst.sarl ||
      inst == Inst.shrb ||
      inst == Inst.shrw ||
      inst == Inst.shrl ||
      inst == Inst.cltd ||
      inst == Inst.andb ||
      inst == Inst.andw ||
      inst == Inst.andl ||
      inst == Inst.orb ||
      inst == Inst.orw ||
      inst == Inst.orl ||
      inst == Inst.xorb ||
      inst == Inst.xorw ||
      inst == Inst.xorl ||
      inst == Inst.addb ||
      inst == Inst.addw ||
      inst == Inst.addl ||
      inst == Inst.subb ||
      inst == Inst.subw ||
      inst == Inst.subl ||
      inst == Inst.mulb ||
      inst == Inst.mulw ||
      inst == Inst.mull ||
      inst == Inst.imulb ||
      inst == Inst.imulw ||
      inst == Inst.imull ||
      inst == Inst.divb ||
      inst == Inst.divw ||
      inst == Inst.divl ||
      inst == Inst.idivb ||
      inst == Inst.idivw ||
      inst == Inst.idivl ||
      inst == Inst.call ||
      inst == Inst.callb ||
      inst == Inst.callw ||
      inst == Inst.calll ||
      inst == Inst.lcallb ||
      inst == Inst.lcall ||
      inst == Inst.lcallw ||
      inst == Inst.lcalll ||
      inst == Inst.ret ||
      inst == Inst.retw ||
      inst == Inst.retl ||
      inst == Inst.lret ||
      inst == Inst.lretw ||
      inst == Inst.lretl ||
      inst == Inst.leave ||
      inst == Inst.leavew ||
      inst == Inst.leavel ||

      inst == Inst.jmp ||
      inst == Inst.ljmp ||
      inst == Inst.jc ||
      inst == Inst.ljc ||
      inst == Inst.jcxz ||
      inst == Inst.ljcxz ||
      inst == Inst.jnb ||
      inst == Inst.ljnb ||
      inst == Inst.jnbe ||
      inst == Inst.ljnbe ||
      inst == Inst.jnc ||
      inst == Inst.ljnc ||
      inst == Inst.jnge ||
      inst == Inst.ljnge ||
      inst == Inst.jnl ||
      inst == Inst.ljnl ||
      inst == Inst.jnle ||
      inst == Inst.ljnle ||
      inst == Inst.jo ||
      inst == Inst.ljo ||
      inst == Inst.jno ||
      inst == Inst.ljno ||
      inst == Inst.jp ||
      inst == Inst.ljp ||
      inst == Inst.jnp ||
      inst == Inst.ljnp ||
      inst == Inst.jpe ||
      inst == Inst.ljpe ||
      inst == Inst.jpo ||
      inst == Inst.ljpo ||
      inst == Inst.je ||
      inst == Inst.lje ||
      inst == Inst.jz ||
      inst == Inst.ljz ||
      inst == Inst.jne ||
      inst == Inst.ljne ||
      inst == Inst.jnz ||
      inst == Inst.ljnz ||
      inst == Inst.js ||
      inst == Inst.ljs ||
      inst == Inst.jns ||
      inst == Inst.ljns ||
      inst == Inst.jg ||
      inst == Inst.ljg ||
      inst == Inst.jge ||
      inst == Inst.ljge ||
      inst == Inst.jl ||
      inst == Inst.ljl ||
      inst == Inst.jle ||
      inst == Inst.ljle ||
      inst == Inst.ja ||
      inst == Inst.lja ||
      inst == Inst.jae ||
      inst == Inst.ljae ||
      inst == Inst.jb ||
      inst == Inst.ljb ||
      inst == Inst.jbe ||
      inst == Inst.ljbe ||
      inst == Inst.INT;
  }

  protected override bool IsSupportAddressing(Addressing addressing) {
    return
      addressing == Addressing.eax ||
      addressing == Addressing.ebx ||
      addressing == Addressing.ecx ||
      addressing == Addressing.edx ||
      addressing == Addressing.ebp ||
      addressing == Addressing.esp ||
      addressing == Addressing.esi ||
      addressing == Addressing.edi ||
      addressing == Addressing.ax ||
      addressing == Addressing.bx ||
      addressing == Addressing.cx ||
      addressing == Addressing.dx ||
      addressing == Addressing.ah ||
      addressing == Addressing.bh ||
      addressing == Addressing.ch ||
      addressing == Addressing.dh ||
      addressing == Addressing.al ||
      addressing == Addressing.bl ||
      addressing == Addressing.cl ||
      addressing == Addressing.dl ||
      addressing == Addressing.bp ||
      addressing == Addressing.sp ||
      addressing == Addressing.si ||
      addressing == Addressing.di ||
      addressing == Addressing.cs ||
      addressing == Addressing.ds ||
      addressing == Addressing.es ||
      addressing == Addressing.fs ||
      addressing == Addressing.ss ||
      addressing == Addressing.imm ||
      addressing == Addressing.mem;
  }
}
