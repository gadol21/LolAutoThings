%module objreader

%include "stl.i"
%include "windows.i"

%exception {
   try {
      $action
   } catch (std::runtime_error &e) {
      PyErr_SetString(PyExc_RuntimeError, const_cast<char*>(e.what()));
      return NULL;
   }
}
  
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