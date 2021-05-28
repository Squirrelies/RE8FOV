#pragma once

struct HandleDeleter
{
	typedef HANDLE pointer;
	void operator()(HANDLE h)
	{
		::CloseHandle(h);
	}
};
typedef std::unique_ptr<HANDLE, HandleDeleter> unique_handle;

std::vector<unsigned char> GetHashByProcessId(unsigned long id);
unsigned long long GetDestinationAddress(unique_handle& procHandle, const unsigned long long baseAddress, const std::vector<int> offsets);
unsigned long GetProcessIdByName(const std::wstring& processName);
std::wstring GetProcessNameById(unsigned long id);
std::wstring GetProcessFullNameById(unsigned long id);

template<typename ... Args>
std::wstring wstring_format(const std::wstring& format, Args ... args);