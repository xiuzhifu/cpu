_start:
	movb $100, %al
	movb $200, %bl
	movb $1, %dl
	int $_test_register_equ
	addb $101, %al
	int $_test_register_equ
	decb %al
	int $_test_register_equ
	incb %al
	int $_test_register_equ
	decb %al
	int $_test_register_equ