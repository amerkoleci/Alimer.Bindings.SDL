@echo off
cmake -B "vs2022_x64" -S "./../" -G "Visual Studio 17 2022" -A x64 %*
echo Open vs2022_x64\SDL.sln to build the project.