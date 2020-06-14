#!/bin/sh

cd public
sed -e "s|%BLOB_URL%|"$BLOB_URL"|g" -e "s|%SHARE_URL_BASE%|"$SHARE_URL_BASE"|g" runtimeConfigTemplate.js > runtimeConfig.js
