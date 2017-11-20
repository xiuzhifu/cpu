_hello:.utf8 "hello world"
_hello1: .utf8 "hello worlD"
_start:
	movb _hello, %al
	movb _hello1, %bl
	movb $2, %dl
	movb $11, %cl
	int $_test_string_unequ