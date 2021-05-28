#include "stdafx.h"
#include "WinMain.h"
#include "sha256.h"
using namespace std;

constexpr float FOV_NORMAL = 81.0f;
constexpr float FOV_ADS = 70.0f;
const vector<int> FOV_OFFSETS = { 0x1F8, 0x68, 0x78, 0x128, 0x20, 0x20, 0x50 };
unsigned long FOV_BASE_ADDR;

// Really this is (void*, void* wchar_t*, int) >.>
int WINAPI wWinMain(_In_ HINSTANCE hInstance, _In_opt_ HINSTANCE hPrevInstance, _In_ LPWSTR lpCmdLine, _In_ int nShowCmd)
{
    unsigned long pid = GetProcessIdByName(L"re8.exe");
    if (pid == 0)
        return 2; // Process not found, error.

    std::vector<unsigned char> hash = GetHashByProcessId(pid);
    if (hash == std::vector<unsigned char> { 0x03, 0x62, 0x8b, 0x43, 0x59, 0xb1, 0x08, 0x61, 0xb3, 0x27, 0xa8, 0x24, 0x5f, 0xe2, 0x4e, 0x5b, 0x97, 0xb1, 0xc0, 0xbe, 0x8c, 0xa8, 0x20, 0x8e, 0xa7, 0x92, 0x17, 0x01, 0x07, 0xbd, 0x5b, 0xe1 })
    { // WW RTM
        OutputDebugStringW(L"WW RTM detected!\r\n");
        FOV_BASE_ADDR = 0x0A1A74F0UL;
    }
    else if (hash == std::vector<unsigned char> { 248, 18, 212, 128, 184, 219, 12, 144, 5, 248, 18, 119, 67, 167, 153, 209, 150, 26, 72, 170, 14, 208, 57, 29, 8, 208, 168, 231, 124, 196, 77, 49 })
    { // Cero D RTM
        OutputDebugStringW(L"Cero D RTM detected!\r\n");
        FOV_BASE_ADDR = 0x0A1A74F0UL + 0x2000UL;
    }
    else if (hash == std::vector<unsigned char> { 0x8A, 0x92, 0x8C, 0x97, 0xEC, 0xF0, 0x40, 0x88, 0xE8, 0x2A, 0x24, 0x19, 0x93, 0x1E, 0x59, 0x26, 0x9A, 0x8B, 0x56, 0xD6, 0x2C, 0x72, 0x22, 0xE9, 0xE2, 0xB4, 0x04, 0xF7, 0x68, 0x96, 0x72, 0x2F })
    { // Cero Z RTM
        OutputDebugStringW(L"Cero Z RTM detected!\r\n");
        FOV_BASE_ADDR = 0x0A1A74F0UL + 0x1000UL;
    }
    else if (hash == std::vector<unsigned char> { 0xEF, 0x08, 0x39, 0xEF, 0xBB, 0x3D, 0x59, 0x92, 0x5D, 0xB0, 0xB0, 0xA5, 0x3D, 0xD6, 0x63, 0xD1, 0x08, 0x7F, 0xCD, 0xDE, 0xE1, 0x6A, 0x3C, 0xAC, 0x37, 0x50, 0xB4, 0x37, 0xCE, 0x5D, 0x07, 0x9F })
    { // promo_01 RTM
        OutputDebugStringW(L"promo_01 RTM detected!\r\n");
        FOV_BASE_ADDR = 0x0A1A74F0UL + 0x1030UL;
    }
    else
    {
        OutputDebugStringW(L"Unknown version detected!\r\n");
        return 4; // Unknown version.
    }

    float normalFov = FOV_NORMAL; // Default normal FOV value.
    float adsFov = FOV_ADS; // Default ADS FOV value.

    int argc = 0;
    wchar_t** lpArgs = nullptr;
    try
    {
        // Do not process command-line arguments if there are none.
        if (lstrcmpW(lpCmdLine, L""))
        {
            lpArgs = CommandLineToArgvW(lpCmdLine, &argc);

            // Check if CommandLineToArgvW failed.
            if (lpArgs == NULL)
            {
                OutputDebugStringW(L"CommandLineToArgvW failed\r\n");
                return 3; // CommandLineToArgvW failed.
            }
            else
            {
                // Statically process each argument.
                for (int i = 0; i < argc; i++)
                {
                    OutputDebugStringW(wstring_format(L"%d: %ws\r\n", i, lpArgs[i]).c_str());
                    if (i == 0)
                        normalFov = wcstof(lpArgs[i], NULL);
                    else if (i == 1)
                        adsFov = wcstof(lpArgs[i], NULL);
                }
            }
        }
    }
    catch (const exception& ex)
    {
        OutputDebugStringA(ex.what());
        DebugBreak();
        return 1; // general exception.
    }
    if (lpArgs != nullptr)
        LocalFree(lpArgs);

    try
    {
        unique_handle procHandle(OpenProcess(PROCESS_QUERY_INFORMATION | PROCESS_VM_READ | PROCESS_VM_WRITE, false, pid));
        unsigned long long fovBaseAddress = GetDestinationAddress(procHandle, 0x140000000UL + FOV_BASE_ADDR, FOV_OFFSETS);
        unsigned long long bytesWritten;
        unsigned long lastError;

        if (!WriteProcessMemory(procHandle.get(), (void*)(fovBaseAddress + 0x38UL), &normalFov, sizeof(normalFov), &bytesWritten))
        {
            lastError = GetLastError();
        }

        if (!WriteProcessMemory(procHandle.get(), (void*)(fovBaseAddress + 0x3CUL), &adsFov, sizeof(adsFov), &bytesWritten))
        {
            lastError = GetLastError();
        }
    }
    catch (const exception& ex)
    {
        OutputDebugStringA(ex.what());
        DebugBreak();
        return 1; // general exception.
    }

    return 0;
}

std::vector<unsigned char> GetHashByProcessId(unsigned long id)
{
    std::vector<unsigned char> hashBinary(32, 0);
    FILE* file;
    _wfopen_s(&file, GetProcessFullNameById(id).c_str(), L"rb");

    if (file != nullptr) {
        char hash[65] = { 0 };
        char buffer[1024];
        size_t size;
        struct sha256_buff buff;
        sha256_init(&buff);
        while (!feof(file)) {
            // Hash file by 1kb chunks, instead of loading into RAM at once
            size = fread(buffer, 1, 1024, file);
            sha256_update(&buff, buffer, size);
        }
        sha256_finalize(&buff);
        sha256_read(&buff, hashBinary.data());
    }

    return hashBinary;
}

unsigned long long GetDestinationAddress(unique_handle& procHandle, const unsigned long long baseAddress, const vector<int> offsets)
{
    unsigned long long buffer;
    unsigned long long bytesRead;

    if (!ReadProcessMemory(procHandle.get(), (const void*)baseAddress, &buffer, sizeof(buffer), &bytesRead))
    {
        unsigned long lastError = GetLastError();
        return 0;
    }

    for (unsigned int i = 0; i < offsets.size(); ++i)
    {
        if (!ReadProcessMemory(procHandle.get(), (const void*)(buffer + offsets[i]), &buffer, sizeof(buffer), &bytesRead))
        {
            unsigned long lastError = GetLastError();
            return 0;
        }
    }

    return buffer;
}

unsigned long GetProcessIdByName(const std::wstring& processName)
{
    DWORD aProcesses[2048], cbNeeded, cProcesses;
    if (!EnumProcesses(aProcesses, sizeof(aProcesses), &cbNeeded))
    {
        return 0;
    }

    cProcesses = cbNeeded / sizeof(DWORD);
    unsigned long i;
    for (i = 0; i < cProcesses; i++)
    {
        std::wstring iProcName = GetProcessNameById(aProcesses[i]);
        if (iProcName == processName)
        {
            return aProcesses[i];
        }
    }

    return 0UL;
}

std::wstring GetProcessNameById(unsigned long id)
{
    WCHAR szProcessName[MAX_PATH] = L"<Unknown>";
    HANDLE hProcess;

    try
    {
        hProcess = OpenProcess(PROCESS_QUERY_INFORMATION | PROCESS_VM_READ, FALSE, id);

        // Get the process name.
        if (hProcess != NULL)
        {
            HMODULE hMod;
            DWORD cbNeeded;
            if (EnumProcessModulesEx(hProcess, &hMod, sizeof(hMod), &cbNeeded, LIST_MODULES_DEFAULT))
            {
                GetModuleBaseName(hProcess, hMod, szProcessName, sizeof(szProcessName) / sizeof(WCHAR));
            }
            CloseHandle(hProcess);
        }
    }
    catch (const exception& ex)
    {
        if (hProcess != NULL)
        {
            CloseHandle(hProcess);
        }

        OutputDebugStringA(ex.what());
        DebugBreak();
    }

    return szProcessName;
}

std::wstring GetProcessFullNameById(unsigned long id)
{
    WCHAR szProcessFullName[4096] = L"<Unknown>";
    HANDLE hProcess;

    try
    {
        hProcess = OpenProcess(PROCESS_QUERY_INFORMATION | PROCESS_VM_READ, FALSE, id);

        // Get the process name.
        if (hProcess != NULL)
        {
            HMODULE hMod;
            DWORD cbNeeded;
            if (EnumProcessModulesEx(hProcess, &hMod, sizeof(hMod), &cbNeeded, LIST_MODULES_DEFAULT))
            {
                GetModuleFileNameExW(hProcess, hMod, szProcessFullName, sizeof(szProcessFullName) / sizeof(WCHAR));
            }
            CloseHandle(hProcess);
        }
    }
    catch (const exception& ex)
    {
        if (hProcess != NULL)
        {
            CloseHandle(hProcess);
        }

        OutputDebugStringA(ex.what());
        DebugBreak();
    }

    return szProcessFullName;
}

template<typename ... Args>
std::wstring wstring_format(const std::wstring& format, Args ... args)
{
    unsigned long long size = static_cast<unsigned long long>(_scwprintf(format.c_str(), args ...)) + 1; // Add 1 for the null terminator.
    if (size <= 0)
    {
        throw std::runtime_error("Error during string formatting.\r\n");
    }

    std::unique_ptr<wchar_t[]> wsbuf = make_unique<wchar_t[]>(size);
    _snwprintf_s(wsbuf.get(), size, _TRUNCATE, format.c_str(), args ...);
    return std::wstring(wsbuf.get(), wsbuf.get() + size);
}
