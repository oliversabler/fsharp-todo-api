#!/bin/bash
set -e

/etc/init.d/postgresql start
psql -f create-todo-table.sql    
/etc/init.d/postgresql stop