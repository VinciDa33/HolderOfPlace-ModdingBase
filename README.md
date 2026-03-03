# About
This repository is an attempt to add a modding framework to the game "Holder of Place". 
It consists of two parts: a DLLeditor that injects entry points into the source code and code that uses these entry points to establish a DLL reading framework.
The setup will require a bit of technical know-how, but the installation below should smooth out the rough patches.

# Installation [WORK IN PROGRESS]
## Step 0: Prerequisite Downloads
You need the following files/programs
- Visual Studios 2022 or later (as long as you have an IDE that can read .Net v4.8.1, you should be good)
- Harmony and dependencies (go to the Unity Explorer repository and download its dlls. You need all the dlls involving "0Harmony", "Mono.Cecil", and "MonoMod".

## Step 1: Locating and Moving Files
First, find where the game's source code is saved on your computer. 
- The easiest way to do this is right-click the game in steam and select "Manage > Browse Local Files".
- For the folder that just popped up, click "HolderOfPlace_Data" and then "Managed".
- inside the "Managed" folder is a bunch of dll files.
- You need to (1) make a copy of "AssemblyCSharp.dll" somewhere else (I copied it onto my desktop) and (2) save the file address of the new dll location and the address of the managed folder.
- Place the Harmony dll and its dependency dlls into the managed folder.

## Step 2: Running the Code
The repository has the code to add entry points to the game's source code. Distributing the modified source code would put me in a legal bind, but running the code on your copy of the source code should be fine.
- Download the repository as a zip folder and unpack it.
- Open the .sln file in Visual Studios
- There are three file addresses at the top of the DLLEditor.cls file. Change them to your version of the three.
- Run the DLLEditor file (there should be a green play button somewhere at the top). This wil be the step where your computer and antivirus is warning you (if not before).
You have the code in front of you, you can read through to see if there is anything questionable with it.
- You should get an output that looks like this. If you did, good job!

## Step 3: Run the Game
Run the game and you should see a debug log pop-up. That means everything is running smoothly.
