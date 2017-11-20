using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


class Cpu16bit: Cpu {
  public Cpu16bit(int memsize = 1024 * 1024) : base(2, memsize) {
    FReg.SS = memsize - 1024 - 1;
    FReg.EBP = FReg.SS;
    FReg.ESP = memsize - 1;
  }

  protected override bool IsSupportInst(Inst inst) {
    return
      inst == Inst.movb ||
      inst == Inst.movw ||
      inst == Inst.movsb ||
      inst == Inst.movsw ||
      inst == Inst.pushb ||
      inst == Inst.pushw ||
      inst == Inst.popb ||
      inst == Inst.popw ||
      inst == Inst.cmpb ||
      inst == Inst.cmpw ||
      inst == Inst.testb ||
      inst == Inst.testw ||
      inst == Inst.leab ||
      inst == Inst.leaw ||
      inst == Inst.incb ||
      inst == Inst.incw ||
      inst == Inst.decb ||
      inst == Inst.decw ||
      inst == Inst.negb ||
      inst == Inst.negw ||
      inst == Inst.notb ||
      inst == Inst.notw ||
      inst == Inst.salb ||
      inst == Inst.salw ||
      inst == Inst.shlb ||
      inst == Inst.shlw ||
      inst == Inst.sarb ||
      inst == Inst.sarw ||
      inst == Inst.shrb ||
      inst == Inst.shrw ||
      inst == Inst.cltd ||
      inst == Inst.andb ||
      inst == Inst.andw ||
      inst == Inst.orb ||
      inst == Inst.orw ||
      inst == Inst.xorb ||
      inst == Inst.xorw ||
      inst == Inst.addb ||
      inst == Inst.addw ||
      inst == Inst.subb ||
      inst == Inst.subw ||
      inst == Inst.mulb ||
      inst == Inst.mulw ||
      inst == Inst.imulb ||
      inst == Inst.imulw ||
      inst == Inst.divb ||
      inst == Inst.divw ||
      inst == Inst.idivb ||
      inst == Inst.idivw ||
      inst == Inst.call ||
      inst == Inst.callb ||
      inst == Inst.callw ||
      inst == Inst.lcallb ||
      inst == Inst.lcall ||
      inst == Inst.lcallw ||
      inst == Inst.ret ||
      inst == Inst.retw ||
      inst == Inst.lret ||
      inst == Inst.lretw ||
      inst == Inst.leavew ||
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

