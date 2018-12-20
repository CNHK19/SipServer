:: Copy this .bat to Debug folder
copy ..\..\..\Tools\LocBaml.exe
LocBaml /parse en-US\messenger.resources.dll /out:all.csv
move all.csv all.csv.txt