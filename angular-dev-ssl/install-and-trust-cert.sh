#!/bin/bash

sudo security add-trusted-cert -d -r trustRoot -k /Library/Keychains/System.keychain dev-ssl-cert.crt