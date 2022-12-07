cmake_minimum_required(VERSION 3.16 FATAL_ERROR)

project(Alimer.Native.SDL)

if (DEFINED SDL2_INCLUDE_DIRS AND DEFINED SDL2_LIBRARIES)
else()
	include(FetchContent)
	set(FETCHCONTENT_QUIET FALSE)

    set (SDL_SHARED ON CACHE BOOL "Build shared library")
    set (SDL_STATIC OFF CACHE BOOL "")

	FetchContent_Declare(
		SDL2
		GIT_REPOSITORY https://github.com/libsdl-org/SDL
		GIT_TAG release-2.26.1 # grab latest stable release
		GIT_PROGRESS TRUE
	)
	FetchContent_MakeAvailable(SDL2)
	set(BLAH_SDL2_LIBS SDL2main SDL2-static)
	set(BLAH_SDL2_INCLUDE ${sdl2_SOURCE_DIRS}/include)
endif()
