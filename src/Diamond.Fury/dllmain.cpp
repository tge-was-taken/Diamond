#pragma pack(1)

// dllmain.cpp : Defines the entry point for the DLL application.
#include "pch.h"
#include <cstdio>

namespace config {
	namespace debug {
		bool showConsole = false;
		bool enableDebugMode = false;
		bool enableTestMenu = false;
		bool enableFileLog = false;
	}
	namespace autologin {
		bool enable = false;
		std::string username = "";
		std::string password = "";
		std::string pin = "";
	}
	namespace rendering {
		bool disableLods = false;
		bool increaseRenderDistance = false;
		float charRenderDistance = 90000; 
		float terrainRenderDistance = 18000;
		bool increaseShadowResolution = false;
		bool forcePhongShading = false;
	}

	static void loadIni() {
		inipp::Ini<char> ini;

		// --- Load main INI ---
		{
			std::ifstream mainFile("Diamond.Fury.ini");
			if (mainFile.is_open()) {
				ini.parse(mainFile);
				mainFile.close();
			}
			else {
				std::cerr << "[Config] Failed to open Diamond.Fury.ini\n";
			}
		}

		// --- Load local user override INI ---
		{
			std::ifstream userFile("Diamond.Fury.user.ini");
			if (userFile.is_open()) {
				inipp::Ini<char> userIni;
				userIni.parse(userFile);
				userFile.close();

				// Merge overrides
				for (auto& sectionPair : userIni.sections) {
					const std::string& sectionName = sectionPair.first;
					auto& section = sectionPair.second;
					for (auto& keyPair : section) {
						ini.sections[sectionName][keyPair.first] = keyPair.second;
					}
				}

				std::cout << "[Config] Loaded local user overrides from Diamond.Fury.user.ini\n";
			}
		}

		// --- Load %USERPROFILE% override INI ---
		{
			const char* userProfile = std::getenv("USERPROFILE");
			if (userProfile) {
				std::string userIniPath = std::string(userProfile) + "\\Diamond.Fury.user.ini";
				std::ifstream userProfileFile(userIniPath);
				if (userProfileFile.is_open()) {
					inipp::Ini<char> userIni;
					userIni.parse(userProfileFile);
					userProfileFile.close();

					// Merge overrides
					for (auto& sectionPair : userIni.sections) {
						const std::string& sectionName = sectionPair.first;
						auto& section = sectionPair.second;
						for (auto& keyPair : section) {
							ini.sections[sectionName][keyPair.first] = keyPair.second;
						}
					}

					std::cout << "[Config] Loaded user-profile overrides from " << userIniPath << "\n";
				}
			}
		}

		ini.strip_trailing_comments();
		ini.default_section(ini.sections[""]);

		// --- Helper lambdas ---
		auto getBool = [&](const std::string& section, const std::string& key, bool def) -> bool {
			std::string val = ini.sections[section][key];
			if (val.empty()) return def;
			std::transform(val.begin(), val.end(), val.begin(), ::tolower);
			return (val == "true" || val == "1" || val == "yes" || val == "on");
			};

		auto getFloat = [&](const std::string& section, const std::string& key, float def) -> float {
			std::string val = ini.sections[section][key];
			return val.empty() ? def : std::stof(val);
			};

		auto getString = [&](const std::string& section, const std::string& key, const char* def) -> std::string {
			std::string val = ini.sections[section][key];
			static std::string temp;
			return val.empty() ? def : val;
			};

		// --- Load values into config ---
		debug::showConsole = getBool("debug", "showConsole", false);
		debug::enableDebugMode = getBool("debug", "enableDebugMode", false);
		debug::enableTestMenu = getBool("debug", "enableTestMenu", false);
		debug::enableFileLog = getBool("debug", "enableFileLog", false);

		autologin::enable = getBool("autologin", "enable", false);
		autologin::username = getString("autologin", "username", "");
		autologin::password = getString("autologin", "password", "");
		autologin::pin = getString("autologin", "pin", "");

		rendering::disableLods = getBool("rendering", "disableLods", false);
		rendering::increaseRenderDistance = getBool("rendering", "increaseRenderDistance", false);
		rendering::charRenderDistance = getFloat("rendering", "charRenderDistance", 90000.f);
		rendering::terrainRenderDistance = getFloat("rendering", "terrainRenderDistance", 1500.f);
		rendering::increaseShadowResolution = getBool("rendering", "increaseShadowResolution", false);
		rendering::forcePhongShading = getBool("rendering", "forcePhongShading", false);

		std::cout << "[Config] Config loaded successfully.\n";
	}
}

namespace xstd {
	class string {
	public:
		int dword0 = {};
		union {
			char buffer[20] = {};
			char* p;
		};
		int length = {};

		inline string(const char* Str) {
			fpCtor(this, 0, Str);
		};
		inline ~string() {
			fpTidy(this, 0, 1, 0);
		}

		FUNCTION_PTR(string*, __fastcall, fpCtor, ASLR(0x403918), string* _this, void* _, const char* Str);
		FUNCTION_PTR(string*, __fastcall, fpTidy, ASLR(0x403234), string* _this, void* _, char a2, size_t SourceSize);
	};
}

namespace Diamond {
	class GUTextbox {
	public:
		void setValue(xstd::string* value) {
			FUNCTION_PTR(void, __fastcall, fpSetValue, ASLR(0x0061B40F), GUTextbox * _this, void* _, xstd::string * value);
			fpSetValue(this, 0, value);
		}
	};
	class EventHandler_48 {
	public:
		INSERT_PADDING(0x1A4);
		Diamond::GUTextbox* userNameTextBox;
		Diamond::GUTextbox* passwordTextBox;
	};
	ASSERT_OFFSETOF(EventHandler_48, passwordTextBox, 0x1A8);

	class EventHandler {
	public:
		INSERT_PADDING(0x48);
		EventHandler_48 _48;
	};
	ASSERT_OFFSETOF(EventHandler, _48, 0x48);

	class GUWindow {
	public:
		INSERT_PADDING(0xBC);
		EventHandler eventHandler;
	};
	ASSERT_OFFSETOF(GUWindow, eventHandler, 0xBC);
}

class class_736210 : public Diamond::EventHandler {

};

static int debugPrint(int a1, const char* fmt, ...)
{
	int ret;

	/* Declare a va_list type variable */
	va_list myargs;

	/* Initialise the va_list variable with the ... after fmt */

	va_start(myargs, fmt);

	/* Forward the '...' to vprintf */
	ret = vprintf(fmt, myargs);

	/* Clean up the va_list */
	va_end(myargs);

	return ret;
}

void setupStdOut()
{
	// Allocate a console if none exists
	AllocConsole();

	// Redirect stdout
	FILE* fp;
	freopen_s(&fp, "CONOUT$", "w", stdout);

	// Redirect stderr (optional)
	freopen_s(&fp, "CONOUT$", "w", stderr);

	// Redirect stdin (optional)
	freopen_s(&fp, "CONIN$", "r", stdin);

	// Make the CRT flush after each write
	setvbuf(stdout, nullptr, _IONBF, 0);
	setvbuf(stderr, nullptr, _IONBF, 0);

	// Optional: attach standard handles
	HANDLE hConsole = GetStdHandle(STD_OUTPUT_HANDLE);
	if (hConsole != INVALID_HANDLE_VALUE)
		SetStdHandle(STD_OUTPUT_HANDLE, hConsole);
}

static void enableDebugStuff() {
	int* g_debugMode = (int*)(ASLR(0x7B235E));
	int* g_vfsMode = (int*)(ASLR(0x008CC171));

	WRITE_JUMP(ASLR(0x00401B2C), debugPrint);
	*g_debugMode = 1;
}

static void enableTestMenu() {
	// Overwrite LoginWindow vtables with TestWindow
	const char buf2[8] = {};
	memcpy((void*)buf2, (const void*)ASLR(0x00730B50), 8);
	WRITE_MEMORY_BUFFER(ASLR(0x00733914), buf2);

	const char buf[0x3C] = {};
	memcpy((void*)buf, (const void*)ASLR(0x00730B5C), 0x3C);
	WRITE_MEMORY_BUFFER(ASLR(0x00733924), buf);
}

static void disableLODs() {
	// Disable setting of LOD model index
	WRITE_NOP(ASLR(0x00411AAD), 10);
	WRITE_NOP(ASLR(0x00411AB9), 10);
}

HOOK(void, __stdcall, setRenderDistance, ASLR(0x00424476))
{
	float* g_renderDistance = (float*)(ASLR(0x792D84));
	*g_renderDistance = config::rendering::charRenderDistance /*1000000.0f * 5*/;
}

HOOK(int, __fastcall, TerrainManager__setRenderDistance, ASLR(0x442D8F), void* pThis, void* _, float dist)
{
	// overwrite max render distance
	WRITE_MEMORY(ASLR(0x0072532C), float, dist);
	return orig_TerrainManager__setRenderDistance(pThis, _, config::rendering::terrainRenderDistance);
}

static void increaseRenderDistance() {
	INSTALL_HOOK(setRenderDistance);
	WRITE_MEMORY(ASLR(0x0072532C), float, FLT_MAX); // Set default max terrain render distance
	WRITE_NOP(ASLR(0x00442DB8), 8); // disable terrain distance clamp
	INSTALL_HOOK(TerrainManager__setRenderDistance);
}

static void increaseShadowResolution() {
	WRITE_NOP(ASLR(0x004A0040), 3); // disable clamping shadow resolution to 4096
}

HOOK(void*, __fastcall, Diamond__GRSShadeModel__ctor, ASLR(0x6211F2), void* _this, void* _, int  shadeModel)
{
	return orig_Diamond__GRSShadeModel__ctor(_this, _, 3);
}

static void forcePhongShading() {
	INSTALL_HOOK(Diamond__GRSShadeModel__ctor);
}

HOOK(int, __fastcall, createLoginWindow, 0x5FF8D3, Diamond::GUWindow* _this, void* _)
{
	auto res = orig_createLoginWindow(_this, _);
	auto& passwordTextBox = _this->eventHandler._48.passwordTextBox;
	xstd::string pwd = config::autologin::password.c_str();
	passwordTextBox->setValue(&pwd);
	return res;
};

class class_436704 {
public:
	INSERT_PADDING(0x48);
	char secondPassword[5];

	static class_436704* getInstance() {
		FUNCTION_PTR(class_436704*, __stdcall, fpGetInstance, ASLR(0x43677A));
		return fpGetInstance();
	}
};
ASSERT_OFFSETOF(class_436704, secondPassword, 0x48);

HOOK(int, __fastcall, sub_5FEC6F, 0x5FEC6F, void* _this, void* _)
{
	auto res = orig_sub_5FEC6F(_this, _);
	strncpy_s(class_436704::getInstance()->secondPassword, 5, config::autologin::pin.c_str(), 4);
	return res;
}

static void autoLogin() {
	INSTALL_HOOK(createLoginWindow);
	INSTALL_HOOK(sub_5FEC6F);
}

HOOK(HANDLE, __stdcall, CreateFileAHook, CreateFileA, LPCSTR lpFileName, DWORD dwDesiredAccess, DWORD dwShareMode, LPSECURITY_ATTRIBUTES lpSecurityAttributes, DWORD dwCreationDisposition, DWORD dwFlagsAndAttributes, HANDLE hTemplateFile)
{
	printf("[FILE] %s\n", lpFileName);
	return orig_CreateFileAHook(lpFileName, dwDesiredAccess, dwShareMode, lpSecurityAttributes, dwCreationDisposition, dwFlagsAndAttributes, hTemplateFile);
}

HOOK(DWORD, __stdcall, SetFilePointerHook, SetFilePointer, HANDLE hFile, LONG lDistanceToMove, PLONG lpDistanceToMoveHigh, DWORD dwMoveMethod)
{
	return orig_SetFilePointerHook(hFile, lDistanceToMove, lpDistanceToMoveHigh, dwMoveMethod);
}

HOOK(BOOL, __stdcall, SetFilePointerExHook, SetFilePointerEx, HANDLE hFile, LARGE_INTEGER liDistanceToMove, PLARGE_INTEGER lpNewFilePointer, DWORD dwMoveMethod)
{
	return orig_SetFilePointerExHook(hFile, liDistanceToMove, lpNewFilePointer, dwMoveMethod);
}

HOOK(BOOL, __stdcall, ReadFileHook, ReadFile, HANDLE hFile, LPVOID lpBuffer, DWORD nNumberOfBytesToRead, LPDWORD lpNumberOfBytesRead, LPOVERLAPPED lpOverlapped)
{
	return orig_ReadFileHook(hFile, lpBuffer, nNumberOfBytesToRead, lpNumberOfBytesRead, lpOverlapped);
}

HOOK(DWORD, __stdcall, GetFileSizeHook, GetFileSize, HANDLE hFile, LPDWORD lpFileSizeHigh)
{
	return orig_GetFileSizeHook(hFile, lpFileSizeHigh);
}

static void fileHooks() {
	INSTALL_HOOK(CreateFileAHook);
	INSTALL_HOOK(SetFilePointerHook);
	INSTALL_HOOK(SetFilePointerExHook);
	INSTALL_HOOK(ReadFileHook);
	INSTALL_HOOK(GetFileSizeHook);
}

static void onDllAttached() {
	config::loadIni();
	if (config::debug::showConsole) {
		setupStdOut();
		printf("Hello world from Fury!\n");
	}
	if (config::debug::enableDebugMode) {
		enableDebugStuff();
	}
	if (config::debug::enableTestMenu) {
		enableTestMenu();
	}
	if (config::debug::enableFileLog) {
		fileHooks();
	}
	if (config::autologin::enable) {
		autoLogin();
	}
	if (config::rendering::increaseRenderDistance) {
		increaseRenderDistance();
	}
	if (config::rendering::increaseShadowResolution) {
		increaseShadowResolution();
	}
	if (config::rendering::forcePhongShading) {
		forcePhongShading();
	}
	if (config::rendering::disableLods) {
		disableLODs();
	}
}

BOOL APIENTRY DllMain(HMODULE hModule,
	DWORD  ul_reason_for_call,
	LPVOID lpReserved
)
{
	switch (ul_reason_for_call)
	{
	case DLL_PROCESS_ATTACH:
		onDllAttached();
		break;
	}
	return TRUE;
}