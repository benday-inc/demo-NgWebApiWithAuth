# Replace these values as needed
$certPath = ".\dev-ssl-cert.crt"
$store = "Cert:\LocalMachine\Root"
Import-Certificate -FilePath $certPath -CertStoreLocation $store