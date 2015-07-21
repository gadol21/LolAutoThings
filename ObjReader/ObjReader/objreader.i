%module objreader

%include "stl.i"
%include "windows.i"
  
%{  
    #include "engine.h"  
%}  
  
%include "engine.h"

%extend Engine {
	%template(read_byte)  read<char>;
	%template(read_short) read<short>;
	%template(read_int)   read<int>;
	%template(read_float) read<float>;
};