#include "stdafx.h"
#include <D3dx9core.h>
#include <list>

#ifndef FONTS_H
#define FONTS_H

typedef struct{
	int fontSize;
	ID3DXFont *font;
} font;

class Fonts{
private:
	static font *FindFont(int fontSize);
	static std::list<font> fonts;
public:
	static ID3DXFont *GetFont(int fontSize,IDirect3DDevice9 *dev);
	static void Reset();
};

#endif