Montager %1
cd %1
call MakeChunksLow
cd ..
call Clear %1
Assembler %1
cd %1
call AssemblyLow
