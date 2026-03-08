# About
This repository is an attempt to add a modding framework to the game "Holder of Place". 
It consists of two parts: a DLLEditor that injects entry points into the source code and code that uses these entry points to establish a DLL reading framework.
The setup will require a bit of technical know-how, but the installation below should smooth out the rough patches.

# Credits
Creating this repository truly makes me feel like I am standing on the shoulders of giants.
This would not have been possible without if the groundwork hasn't been already laid out.
I would like to thank everyone who contributed to MonoMod, Harmony, and the Unity Explorer.
I would also like to thank the creators of Holder of Place for making the game (and not using IL2CPP).

# Downloads (v0.1.0)
## Latest release version: 0.1.0 (Pre-Release)
- [See all releases](https://github.com/mhcdc9/HolderOfPlace-ModdingBase/releases)
- [Dependencies (updated 03/07/26)](https://github.com/mhcdc9/HolderOfPlace-ModdingBase/releases/download/v0.1.0/Dependencies.zip)
- [StreamingAssets Folder (updated 03/07/26)](https://github.com/mhcdc9/HolderOfPlace-ModdingBase/releases/download/v0.1.0/StreamingAssets.zip)
- [ModBootstrap.dll (updated 03/07/26)](https://github.com/mhcdc9/HolderOfPlace-ModdingBase/releases/download/v0.1.0/ModBootstrap.dll)
- [Source Code (updated 03/07/26)](https://github.com/mhcdc9/HolderOfPlace-ModdingBase/archive/refs/tags/v0.1.0.zip)
- Example mods are located in [another repository](https://github.com/mhcdc9/HolderOfPlace-Mods/tree/master)


# Installation

> [!Note]
> This section explains in detail how to set everything with essentially zero coding know-how. For those more well-versed, click the drop-down below for a very condensed summary. 
> <details>
> <summary>Summary of Installation</summary>
>
> 1. Locate the `...\HolderOfPlace_Data` folder on your computer ("Browse local files" on Steam).
> 2. Place the StreamingAssets folder inside of the "HolderOfPlace_Data" folder (`...\HolderOfPlace_Data\StreamingAssets`).
> 3. Locate `...\HolderOfPlace_Data\Managed` folder and copy its filepath for later.
> 4. Place ModBootstrap.dll into this `...\Managed` folder (`...\Managed\ModBootstrap.dll`).
> 5. Place all of the dlls in the Dependencies folder into this `...\HolderOfPlace_Data\Managed` (something like `...\Managed\DLL1.dll`, `...\Managed\DLL2.dll`, etc.)
> 6. Inside of `...\Managed`, make a copy of `...\Managed\Assembly-CSharp.dll` and place it somewhere else. Save the filepath for later.
> 7. Open the source code (obtained via .zip or forking/cloning the repository) in Visual Studios or similar IDE.
> 8. For the DLLEditor project, add references to both `...\Managed\ModBootstrap.dll` and `...\Managed\Assembly-CSharp.dll` on your computer.
> 9. They are likely some reference errors still. Clear those out and any other erros. Feel free to unload the ModBootstrap project if you don't want to fix those errors.
> 10. Replace `outputDirectory` with the filepath you saved from step 3. Replace `inputPath` with the filepath you saved from step 6.
> 11. Run DLLEditor.
> 12. If there were no runtime errors or unsuccessful messages, you are finished! Run the game and see the results.
> </details>

## Step 0: Prerequisite Downloads
There are a few files and programs that you need to install first. They come from pretty standard websites.

---

### Visual Studios (or similar C# IDE)
1. For maximum simplicity, download and install visual studios (VS): https://visualstudio.microsoft.com. The default free version is Visual Studios 2026 Community. This project was made in an older version of VS, but as long as it can run .Net Framework 4.8.1, it should work fine.

<p align=center>
<img width="523" height="297" alt="image" src="https://github.com/user-attachments/assets/4384a5a5-b428-4847-95f7-52c6a9fd4192" />
</p>

2. Run the installer. It will ask you what workload you want to install. You must install "ASP.Net and Web Development" or whatever looks the most similar to that. If you want to work on mods, installing "Game Development with Unity" would be nice but not required.

<p align=center>
<img width="425" height="138" alt="image" src="https://github.com/user-attachments/assets/e4e9d6d0-5445-427f-8f77-f9836434dccf" />
<img width="424" height="144" alt="image" src="https://github.com/user-attachments/assets/438fabae-cdd9-4ab9-926f-b7473f4b2264" />
</p>
  
4. Start the installation. You can finish the rest of steps 0-1 as you wait for the installation to finish.

---

### ModBootstrap and Harmony (w/Dependencies)

We will require a few C# libraries that will not be packaged in with Visual Studios. See the Downloads section above. The Dependencies folder holds external dlls that you need to run this. In addition, there is one more dll download named ModBootstrap.dll. If this is your first time trying to do something like this, this may seem a bit sketchy. Here is a summary of all of the dlls and where you can read up on or cross-validate them.
- `ModBootstrap.dll`: This will be the central library for modding Holder of Place. It will start Harmony, interface with the source code, and load mods. It is created by me, and the code is accessible in this very repository.
  - Official github: You are here.
- `0Harmony.dll`: This is the harmony library, created by Andreas Pardeike. In the words of its GitHub, it is "a library for patching, replacing and decorating .NET and Mono methods during reuntime". This is the library that will allow modders to modify the source code in a reversible way.
  - Official Github: https://github.com/pardeike/Harmony
  - API documentation: https://harmony.pardeike.net/articles/patching.html
  - This dll was obtained on the Unity Explorer repository (Standalone version): https://github.com/sinai-dev/UnityExplorer
- `MonoMod.*`:  An open-source library to help with C# modding. Harmony uses it as a dependency.
  - Official Github: https://github.com/MonoMod
  - These dlls were obtained on the Unity Explorer repository (Standalone): https://github.com/sinai-dev/UnityExplorer
- `Mono.Cecil.*`: Mono.Cecil are libraries with the purpose of modifying dlls. They are used by MonoMod.
  - Official website: https://www.mono-project.com/docs/tools+libraries/libraries/Mono.Cecil
  - These dlls were obtained from the Unity Explorer repository (Standalone): https://github.com/sinai-dev/UnityExplorer

Download these dlls either from this repository or another source.

---

### StreamingAssets

In this repository, there is a folder named `StreamingAssets`. You can browse its contents and find only a single 100x100 image. This will be the folder that the modding framework will check for mods (among other things) and save configs, but right now, it's essentially empty.

Once you have that downloaded, you are ready for the next step.

---

## Step 1: Locating and Moving Files
First, find where the game's source code is saved on your computer. 
- The easiest way to do this is right-click the game in steam and select "Manage > Browse Local Files".

<p align=center>
<img width="337" height="359" alt="BrowseLocalFiles" src="https://github.com/user-attachments/assets/487272e7-7eb5-4f21-878d-55ed7c2c2e8f" />
</p>

- For the folder that just popped up, click "HolderOfPlace_Data".
- Place the downloaded StreamingAssets inside this data folder (shown below).

<p align=center>
<img width="500" height="500" alt="Screenshot 2026-03-04 234852" src="https://github.com/user-attachments/assets/cddc4089-88ee-430c-96a8-333c45d51210" />
</p>
 
- Now, head to the "Managed" folder. You will see a bunch of dll files.
- Place the downloaded ModBootstrap.dll inside. In addition, place all of the dlls from the downloaded Dependencies folder inside this folder as well (there were 7 of them, making a total of 8 new dlls inside of Managed).

<p align=center>
<img width="491" height="320" alt="image" src="https://github.com/user-attachments/assets/747eebb6-771f-4af0-8f59-9cd1698b5003" />
</p>

- Save the full file address for the managed folder somewhere, like Notepad, for later use (the filepath will almost certainly be `C:\Program Files (x86)\Steam\steamapps\common\Holder of Place\HolderOfPlace_Data\Managed`).
- In addition, find the the `AssemblyCSharp.dll` in the managed folder and make a copy of it (I copied mine onto a new folder on the desktop). Save the file path for later use (results vary but it might look similar to `C:\Users\[USER]\Desktop\HolderOfPlace\Assembly-CSharp.dll`).

And with that, all of the files are in the right place. Hopefully, visual studios has finished installing so we can move on to that.

## Step 2: Cleaning Code in Visual Studios
This repository has the code to add entry points to the game's source code. Distributing the modified source code would put me in a legal bind, but running the code on your copy of the source code should be fine. This is why you had to download Visual Studios. 
- Download the repository code. You can download it through the link in the Downloads section. If you are familar with Github, you can clone/fork the repository and download a local copy instead.
- Open the .sln file in Visual Studios (Open Visual Studios and click on whatever button says "open solution"). Navigate to the .sln file inside the downloaded source code folder.

<p align=center>
<img width="372" height="394" alt="image" src="https://github.com/user-attachments/assets/a55ffa16-daa3-4aaf-afff-1f73af849bc2" />
</p>

- Once the solution is open, there is usually a solution explorer to the right. Open it, click the drop-down on "DLLEditor" and then double-click "Program.cs".

<p align=center>
<img width="320" height="433" alt="image" src="https://github.com/user-attachments/assets/a845c875-c268-4f06-992c-aec963f6bcec" />
</p>

- There are two file addresses at the top of the DLLEditor.cls file. A couple instructions ago, we saved two filepaths for later use. We will use them here. Replace everthing inside the double-quotes ("") of `outputDirectory` with the path to your managed folder. Do something similar with the `inputPath` and the path to your AssemblyCSharp.dll copy.

```C#
//Change these paths
//You may notice that the backslash's doubled (\ -> \\). This is normal.
static string inputPath = "C:\\Users\\Michael\\Desktop\\HolderOfPlace\\Assembly-CSharp.dll";
static string outputDirectory = "C:\\Program Files (x86)\\Steam\\steamapps\\common\\Holder of Place\\HolderOfPlace_Data\\Managed";  
```

Now we simply "fix all the errors". I am going to list as much possible errors that could have formed from this process. If I miss any, let me know so I can add to this document (pictures also help!). 

### Missing references to dlls
The project relies on references to some of the dlls in the managed folder. Moving the code to another computer likely broke these, so we need to reset them. What you want to do is in the solution explorer on the right side, go to DLLEditor > Dependencies > References and remove the 3-5 references you see there (ctrl+click/shift+click to select multiple, right-click, and select remove). Then, right-click This opens up a new window. Click browse, navigate all the way to the managed folder (I hope you still have that filepath saved) and add the following dlls: `Assembly_CSharp.dll`, `ModBootstrap.dll`, `UnityEngine.dll`, `UnityEngine.CoreMdule.dll`, `UnityEngine.GridModule.dll` (the third and last one were once used in an earlier build. They may be optional now).

<p align=center>
<img width="353" height="393" alt="image" src="https://github.com/user-attachments/assets/d39b1085-45fc-456f-8ab9-d94e512df214" />
</p>

### Unload the other project
The other project in this solution is named `ModBootstrap`. This is source code for `ModBootstrap.dll`. If this project is throwing errors at you, simply right-click the project and click "Unload project". It can be reloaded later. 

---

If you see a red squiggly line under any line of code, you can right-click the line and view possible fixes. These are usually correct, but not always (if the "fix" makes more errors, just undo it).


## Step 3: Run the Program. Run the Game
> [!Warning]
> If it has not happened already, your computer's antivirus should be screaming at you to not be so trusting of files downloaded through the web (and rightfully so). The next step will run the program and, if malicious, possibly harm your computer. The good news is that the code that it runs is right in front of you (the `Program.cs`). It's a relatively small file that consists of the same cycle: find method, find line in that method, inject a line calling method from ModBootstrap (which you also have the code for). If you see anything suspicious, ask about it and I'll give you an explanation.

- Click the play button (green triangle, usually with the word "DLLEditor" nearby) at the top to run the code.
- A command window will pop and tell you the results. If it worked perfectly, the output will look like the picture below:

<p align=center>
<img width="555" height="313" alt="image" src="https://github.com/user-attachments/assets/ff5c837c-08ce-459c-a33d-a8e9fd47222f" />
</p>

- If you got those messages, you are essentially finished! You can safely close out of that command window and Visual Studio.
- Run the game and you will be greeted by a new debug window and some extra words on the title screen:

<p align=center>
<img width="350" height="275" alt="image" src="https://github.com/user-attachments/assets/fa1fd85b-1b11-442a-b533-248b2a99e365" />
</p>

Congratulations! You have successfully enabled modding for Holder of Place. Here is a quick tour:

### Debug Log
That extra window that opened up with `Hello World` shows you the debug messages that occur as you play the game. Useful for developing mods and, umm, debugging.
### Command Line
Press ~ and a prompt appears for you to type commands. I'll leave it to you to figure out how these commands work. These are made for the purpose of testing things out.
### Mod Menu
If you go to settings, you will also find a gray rectangle on the right side of the screen. Once you installed mods, you can cycle through mods and turn toggle them on/off. How do you add mods? If you place a mod's `.dll` file in a folder (maybe with an icon), and place that folder in `...\StreamingAssets\Mods`, it will show up in the game the next time you play (don't forget to turn on the mod!). See the [other repository](https://github.com/mhcdc9/HolderOfPlace-Mods/tree/master) for some example mods.
