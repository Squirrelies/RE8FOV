#include "stdafx.h"
#include "WinMain.h"

constexpr float FOV_NORMAL = 81.0f;
constexpr float FOV_ADS = 70.0f;

using namespace std;

// Really this is (void*, void* wchar_t*, int) >.>
int WINAPI wWinMain(_In_ HINSTANCE hInstance, _In_opt_ HINSTANCE hPrevInstance, _In_ LPWSTR lpCmdLine, _In_ int nShowCmd)
{
    unsigned long pid = GetProcessIdByName(L"re8.exe");
    if (pid == 0)
        return 1; // Process not found, error.

    float normalFov = FOV_NORMAL; // Default normal FOV value.
    float adsFov = FOV_ADS; // Default ADS FOV value.

    int argc = 0;
    wchar_t** lpArgs = nullptr;
    try
    {
        lpArgs = CommandLineToArgvW(lpCmdLine, &argc);

        if (lpArgs == NULL)
        {
            OutputDebugStringW(L"CommandLineToArgvW failed\r\n");
        }
        else
        {
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
    catch (const exception& ex)
    {
        OutputDebugStringA(ex.what());
        DebugBreak();
    }
    if (lpArgs != nullptr)
        LocalFree(lpArgs);

    try
    {
        unique_handle procHandle(OpenProcess(PROCESS_QUERY_INFORMATION | PROCESS_VM_READ | PROCESS_VM_WRITE, false, pid));
        unsigned long long fovBaseAddress = GetDestinationAddress(procHandle, 0x140000000UL + 0x0A1A8520UL, { 0x1F8, 0x68, 0x78, 0x128, 0x20, 0x20, 0x50 });
        if (normalFov != FOV_NORMAL)
        {
            unsigned long long bytesWritten;
            if (!WriteProcessMemory(procHandle.get(), (void*)(fovBaseAddress + 0x38UL), &normalFov, sizeof(normalFov), &bytesWritten))
            {
                unsigned long lastError = GetLastError();
            }
        }
        if (adsFov != FOV_ADS)
        {
            unsigned long long bytesWritten;
            if (!WriteProcessMemory(procHandle.get(), (void*)(fovBaseAddress + 0x3CUL), &adsFov, sizeof(adsFov), &bytesWritten))
            {
                unsigned long lastError = GetLastError();
            }
        }
    }
    catch (const exception& ex)
    {
        OutputDebugStringA(ex.what());
        DebugBreak();
    }

    return 0;
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
}

unsigned long long GetDestinationAddress(unique_handle& procHandle, const unsigned long long baseAddress, vector<int> offsets)
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
                GetModuleBaseName(hProcess, hMod, szProcessName, sizeof(szProcessName) / sizeof(TCHAR));
            }
            CloseHandle(hProcess);
        }

        return szProcessName;
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
