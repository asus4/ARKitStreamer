#!/bin/bash -xe

if ! patch -R -s -f --dry-run -u $1 < $2; then
    patch -u $1 < $2
    echo patch ${2} - applyed to ${1}
fi

