# Xiaomi_Band_Tools
Tools from @m0tral (shamelessly decompiled) used to compile/decompile Xiaomi watchfaces

The project does build, but probably won't run correctly.
For instance, I see some hard references to DLL in the program.cs

But, that's a start.

The goal is to be able to port everything to .NET 9 so we can have a linux build.
The Xiaomi.Coomon is already OK in NET Standard 2.1 so it will build fine in .NET 9.
The two other projects though, the compiler and unpacker, require some changes to migrate them to some Core version of .NET
The nuget package used, the Magick lib, is obsolete with version 7.1
Using a more recent version, like version 14, would make the projects buildable in .NET 9... probably.
Haven't tried.

But, there needs to be code change because version 14 break stuff.

So the overall plan to have these tools in Linux is:
- Make it work in framework 4.7 first, change the two Program.cs
- Then when all works, upgrade Magick lib to 14
- Make it work again
- Then when all works, upgrade both compiler and unpacker to NET 9
- Make sure it works
- Change target to NET Standard 2.1 for the Xiaomi common
- Make sure it works
- Publish as a self contained binary for linux
- Enjoy
