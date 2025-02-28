if (!(Get-Command -ErrorAction SilentlyContinue -name openssl)) {
    Write-Host "OpenSSL not found. To install, run 'winget install -e --id ShiningLight.OpenSSL.Light' and try again."
    Write-Host "Exiting..."
    exit
}
else {
    Write-Host "OpenSSL found."
}

if (!(Test-Path dev-ssl-cert-info.cnf)) {
    Write-Host "The certificate config file is not found. Expected to find 'dev-ssl-cert-info.cnf'. Exiting..."
    exit
}
else 
{
    Write-Host "Certificate config file found."
    Write-Host "Creating certificate..."
    openssl req -new -x509 -newkey rsa:2048 -sha256 -nodes -keyout dev-ssl-cert-key.key -days 3560 -out dev-ssl-cert.crt -config dev-ssl-cert-info.cnf
    Write-Host "Certificate created."
}


