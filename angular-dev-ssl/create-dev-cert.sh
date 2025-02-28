#!/bin/bash

# check if openssl is installed
if ! [ -x "$(command -v openssl)" ]; then
  echo 'Error: openssl is not installed.' >&2
  echo 'To install using brew, run: brew install openssl@3' >&2
  echo 'Please install openssl and try again.' >&2
  exit 1
else
    echo "openssl found."    
fi

# check if del-ssl-cert-info.cnf exists
if [ ! -f "dev-ssl-cert-info.cnf" ]; then
  echo "Error: dev-ssl-cert-info.cnf does not exist." >&2
  exit 1
else
  echo "dev-ssl-cert-info.cnf found."
  echo "Creating dev-ssl-cert-key.key and dev-ssl-cert.crt..."
  openssl req -new -x509 -newkey rsa:2048 -sha256 -nodes -keyout dev-ssl-cert-key.key -days 3560 -out dev-ssl-cert.crt -config dev-ssl-cert-info.cnf
  echo "Done."
fi

