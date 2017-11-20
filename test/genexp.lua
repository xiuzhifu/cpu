o = io.open('test\\8bit\\test_exp.s', "w")
math.randomseed(os.time())
local op ={
'+','-','*','/'
}
local opl ={
'+','-','*','//'
}
local t = 'local tt = {'
for i = 1, 1000 do
	while true do
		a = math.random(4) + 1
		b = math.random(4) + 1
		c = math.random(4) + 1
		d = math.random(4) + 1
		e = math.random(4) + 1
		f = math.random(4) + 1
		op1 = math.random(4)
		op2 = math.random(4)
		op3 = math.random(4)
		op4 = math.random(4)
		op5 = math.random(4)
		
		s = tostring(a)..op[op1]
		..tostring(b)..op[op2].."("
		..tostring(c)..op[op3]
		..tostring(d)..op[op4]
		..tostring(e)..")"..op[op5]
		..tostring(f)
		s1 = tostring(a)..opl[op1]
	..tostring(b)..opl[op2].."("
	..tostring(c)..opl[op3]
	..tostring(d)..opl[op4]
	..tostring(e)..")"..opl[op5]
	..tostring(f)	
		local func = assert(load("return ".. s1))
		if pcall(func) then break end
	
	end
	o:write("_math"..tostring(i).. '=' ..s.."\n")
	print(s)
	s = tostring(a)..opl[op1]
	..tostring(b)..opl[op2].."("
	..tostring(c)..opl[op3]
	..tostring(d)..opl[op4]
	..tostring(e)..")"..opl[op5]
	..tostring(f)	
	t = t .. s..',\n'
end

t = t.. '} \n o:write("_start:\\n")\n for i = 1, 1000 do \n local s1 = "movb _math"..tostring(i)..", %al\\n".."movb $"..tostring(tt[i])..", %bl\\nmovb $"..tostring(i)..", %dl\\nint _test_register_equ\\n" o:write(s1) end'
print(t)

f = assert(load(t))
f = f()
o:close()
