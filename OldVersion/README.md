This repo consists of several projects
d3dPresentHook
================
This project hooks d3d in order to write on the game's screen. it should be game independent
if you remove the define LEAGUEOFLEGENDS

Dependencies: DirectX SDK (June 2010)

MusicForm
================
This project is basicly 100fm player, but while lol active it will write the current song name and artist
on the screen, and let you switch songs, and change volume with the keyboard.

Dependencies: Awesomium

LolThingies
================
This is the main project, it uses all of the included projects (except of MusicForm)
It contains high level control of the game and bot creating by using ObjReader.

Dependencies: AutoIt for the auto smite, and Awesomium for Divisions (currently not used)

ObjReader
================
This project is the the one that actually reads LoL memory.
It contains a nice api (not yet documented) for people wants to use it,
for example it allows looping threw all Units (minions, turrets, heros), and see their stats.

lolcomjector
================
This project consists of two modules:
Communicator - Allows you to communicate with the d3d hook dll (sending it text to show on screen),
Injector - Allows you to easily inject any dll into any process


**Note: there are 2 files in this project that are patch dependent -**   
\ObjReader\ObjReader\Offsets.cs - all the offsets used by ObjReader  
\d3dPresentHook\d3dPresentHook\lol.h - The offset of League's FloatingText and MoveTo functions
