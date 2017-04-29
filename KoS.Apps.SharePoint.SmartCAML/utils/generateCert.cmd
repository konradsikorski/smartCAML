makecert.exe -sv smartCaml.pvk -n "cn=KonradSikorski-SmartCaml" smartCaml.cer -b 01/01/2017 -e 30/12/2027 -r
pvk2pfx.exe -pvk smartCaml.pvk -spc smartCaml.cer -pfx smartCaml.pfx