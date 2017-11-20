	.file	"asm.c"
	.text
	.globl	_start
	.def	_start;	.scl	2;	.type	32;	.endef
_start:
LFB0:
	.cfi_startproc
	pushl	%ebp
	.cfi_def_cfa_offset 8
	.cfi_offset 5, -8
	movl	%esp, %ebp
	.cfi_def_cfa_register 5
	subl	$48, %esp
	movl	$115, -4(%ebp)
	movl	$83, -8(%ebp)
	movl	$27654342, -12(%ebp)
	movl	$94, -16(%ebp)
	movl	$115, -20(%ebp)
	movl	$904, -24(%ebp)
	movl	$5148, -28(%ebp)
	movl	$539378, -32(%ebp)
	movl	$108319, -36(%ebp)
	movl	$-2938, -40(%ebp)
	movl	$0, %eax
	leave
	.cfi_restore 5
	.cfi_def_cfa 4, 4
	ret
	.cfi_endproc
LFE0:
	.ident	"GCC: (GNU) 4.9.3"
