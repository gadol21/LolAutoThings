#include "stdafx.h"
#include "fonts.h"

std::list<font> Fonts::fonts;

font *Fonts::FindFont(int fontSize){
	for(std::list<font>::iterator it = fonts.begin(); it!=fonts.end();it++){
		if((*it).fontSize == fontSize)
			return &(*it);
	}
	return NULL;
}

ID3DXFont *Fonts::GetFont(int fontSize,IDirect3DDevice9 *dev){
	font *f = FindFont(fontSize);
	if(!f){
		font newFont = {0};
		newFont.fontSize = fontSize;
		if(FAILED(D3DXCreateFontA(dev,fontSize,0,FW_NORMAL,0,FALSE,DEFAULT_CHARSET,OUT_DEFAULT_PRECIS,ANTIALIASED_QUALITY,DEFAULT_PITCH | FF_DONTCARE,"Arial",&newFont.font)))
			return NULL;
		fonts.push_back(newFont);
		return newFont.font;
	}
	return f->font;
}

void Fonts::Reset(){ //on reset release all fonts and clear the list
	for(std::list<font>::iterator it = fonts.begin(); it!=fonts.end();it++){
		(*it).font->Release();
	}
	fonts.clear();
}