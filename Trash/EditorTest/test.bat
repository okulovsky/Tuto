REM ====cleanup====
del /s *.exe
rmdir 00 /s /q

REM ====prepare====
mkdir 00
copy log.txt 00\
call pull

REM ====showtime!====
Montager 00
cd 00
call MakeChunksHigh
cd ..
call Clear 00
Assembler 00
REM cd 00
REM call AssemblyHigh
