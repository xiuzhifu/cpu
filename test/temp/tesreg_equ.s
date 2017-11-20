_start:
	movb 100, %al
	movb 200, %bl
	movb $1, %dl
	movb $11, %cl
	int $_test_register_equ