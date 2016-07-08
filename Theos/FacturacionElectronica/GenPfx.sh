#!/bin/bash

# Script para generar un archivo .Pfx partiendo de:
#   - Un certificado (.cer)
#   - Una archivo de llave pública (.key)
#   - Contraseña del archivo .key

# Version: 0.1 (se piensa mejorar en versiones posteriores)

# Ejemplo de uso:
# bash GenPfx.sh Archivo.cer Archivo.key ContraseñaKey Archivo.pfx ContraseñaPfx


RutaCer=$1
RutaKey=$2
ContKey=$3
RutaPfx=$4
ContPfx=$5

CertPem=/tmp/Cert.pem
LlavePem=/tmp/Llave.pem

# Se genera el .pfx
openssl x509 -inform DER -in "$RutaCer" -out "$CertPem"
openssl pkcs8 -inform DER -in "$RutaKey" -passin pass:$ContKey -out "$LlavePem"
openssl pkcs12 -export -out "$RutaPfx" -inkey "$LlavePem" -in "$CertPem" -passout pass:$ContPfx

# Se borran archivos usados
rm "$CertPem"
rm "$LlavePem"
