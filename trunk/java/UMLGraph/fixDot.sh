#!/bin/sh

sed -e 's/laquo/lt/g' -e 's/raquo/gt/g' ViewCoreDataModel_.dot | cat > ViewCoreDataModel__.dot
sed -e 's/laquo/lt/g' -e 's/raquo/gt/g' ViewExceptions_.dot | cat > ViewExceptions__.dot
sed -e 's/laquo/lt/g' -e 's/raquo/gt/g' ViewMedia_.dot | cat > ViewMedia__.dot
sed -e 's/laquo/lt/g' -e 's/raquo/gt/g' ViewFullUML_.dot | cat > ViewFullUML__.dot
sed -e 's/laquo/lt/g' -e 's/raquo/gt/g' ViewCommands_.dot | cat > ViewCommands__.dot