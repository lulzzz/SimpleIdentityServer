makecert -n "CN=Lokit CA,O=AdvICT,OU=Dev" -pe -ss Root -sr LocalMachine LokitCa.cer -sky exchange -m 120 -a sha1 -len 2048 -r

makecert -n "CN=localhost" -pe -ss My -sr LocalMachine -sky exchange -m 120 -in "Lokit CA" -is Root -ir LocalMachine SimpleIdServer.cer -a sha1 -eku 1.3.6.1.5.5.7.3.1