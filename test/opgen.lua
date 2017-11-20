local f = io.open('inst.txt', "r")
local o = io.open('insto.txt', "w")
local i = 0
for s in f:lines()  do
	if s and s ~= "" then 
	s = string.sub(s, 1, -2);
	s = 'inst == Inst.'..s.. ' ||\n'
	--s = 'new InstDef() { name = "'..s..'", inst = Inst.'..s..', paramcount = 1, width = '..tostring((i % 2 + 1) * 2)..'},\n'
	i = i + 1
	print(s)
	o:write(s)
	end
end
o:close()