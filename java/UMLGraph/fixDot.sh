#!/bin/sh

sed -e 's/laquo/lt/g' -e 's/raquo/gt/g' ViewCoreDataModel.dot | cat > ViewCoreDataModel_.dot
sed -e 's/laquo/lt/g' -e 's/raquo/gt/g' ViewExceptions.dot | cat > ViewExceptions_.dot
sed -e 's/laquo/lt/g' -e 's/raquo/gt/g' ViewMediaObject.dot | cat > ViewMediaObject_.dot
sed -e 's/laquo/lt/g' -e 's/raquo/gt/g' ViewFullUML.dot | cat > ViewFullUML_.dot