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

unsigned long GetProcessIdByName(const std::wstring& processName);
unsigned long long GetDestinationAddress(unique_handle& procHandle, const unsigned long long baseAddress, std::vector<int> offsets);
std::wstring GetProcessNameById(unsigned long id);

template<typename ... Args>
std::wstring wstring_format(const std::wstring& format, Args ... args);