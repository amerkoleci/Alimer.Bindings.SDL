@echo off
cmake -B "vs2022_arm64" -S "./../" -G "Visual Studio 17 2022" -A ARM64 %*
echo Open vs2022_arm64\Alimer.Native.SDL.sln to build the project.