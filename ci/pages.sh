#!/bin/sh

mv ./src/Builds/WebGL public
mkdir zipping
cd zipping
mv ../src/Builds/StandaloneWindows64 RRCS
zip -r9 --move ../public/RRCS_win64.zip RRCS
