DirectShow.dll : DirectShow.tlb
	tlbimp DirectShow.tlb
	echo ildasm DirectShowLib.dll /out:source.il
	echo patch -i modification.patch
	echo ilasm /DLL /output=DirectShowLib.dll source.il
	
DirectShow.tlb : DirectShow.idl
	midl DirectShow.idl
	rm *.c
	rm *.h
