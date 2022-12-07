@echo off
cmake -B "vs2022_uwp" -S "./../" -G "Visual Studio 17 2022" -DCMAKE_SYSTEM_NAME=WindowsStore -DCMAKE_SYSTEM_VERSION=10.0 %*
echo If cmake failed then be sure to check the "Universal Windows Platform development" checkbox in the Visual Studio Installer
echo Open vs2022_uwp\Alimer.Native.SDL.sln to build the project.