@ECHO OFF

"C:\Program Files (x86)\Windows Kits\10\bin\10.0.19041.0\x64\signtool.exe" sign /tr "http://timestamp.digicert.com" /td SHA1 /sha1 "33982512a318ef75eeb82a9db6d637c041a18974" /fd SHA1 "RE8FOV\bin\x64\Release\netcoreapp3.1\publish\RE8FOV.exe"
"C:\Program Files (x86)\Windows Kits\10\bin\10.0.19041.0\x64\signtool.exe" sign /tr "http://timestamp.digicert.com" /td SHA256 /sha1 "33982512a318ef75eeb82a9db6d637c041a18974" /fd SHA256 /as "RE8FOV\bin\x64\Release\netcoreapp3.1\publish\RE8FOV.exe"
"C:\Program Files (x86)\Windows Kits\10\bin\10.0.19041.0\x64\signtool.exe" sign /tr "http://timestamp.digicert.com" /td SHA512 /sha1 "33982512a318ef75eeb82a9db6d637c041a18974" /fd SHA512 /as "RE8FOV\bin\x64\Release\netcoreapp3.1\publish\RE8FOV.exe"

PAUSE
