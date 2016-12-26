#conding: utf-8
$stdout = File.open("./select_used.pe", "w")

print "codepoints = "
p File.read("used_letters.txt", encoding:"utf-8").codepoints
print <<EOM
i = 0
len = SizeOf(codepoints)
while (i < len)
    SelectMore(codepoints[i], codepoints[i])
    ++i
endloop

SelectInvert()
Clear()
EOM
