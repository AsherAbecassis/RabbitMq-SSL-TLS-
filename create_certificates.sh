#!/bin/bash
mkdir certificates3

cd tls-gen/basic
make PASSWORD=random_pw
make verify
make info
cp result/* -R ../../certificates3/
ls -l ../../certificates3
