cmake_minimum_required(VERSION 3.16 FATAL_ERROR)

project(SDL3)

# Configure CMake global variables
set(CMAKE_RUNTIME_OUTPUT_DIRECTORY ${CMAKE_BINARY_DIR}/bin)
set(CMAKE_LIBRARY_OUTPUT_DIRECTORY ${CMAKE_BINARY_DIR}/lib)
set(CMAKE_ARCHIVE_OUTPUT_DIRECTORY ${CMAKE_BINARY_DIR}/lib)

set(CMAKE_OSX_DEPLOYMENT_TARGET "10.15" CACHE STRING "Minimum OS X deployment version")
set(CMAKE_OSX_ARCHITECTURES "x86_64;arm64" CACHE STRING "Build architectures for OS X")

set (SDL_SHARED ON CACHE BOOL "Build shared library")
set (SDL_STATIC OFF CACHE BOOL "")
set (SDL_TESTS OFF CACHE BOOL "")

add_subdirectory (${CMAKE_SOURCE_DIR}/sdl)
