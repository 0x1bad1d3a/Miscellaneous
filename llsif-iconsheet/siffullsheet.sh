#!/bin/bash

name="llsif.jpg"
rm -f temp/*.*

html=$(curl "web.ruliweb.daum.net/etcs/lovelive/lovelive_card.htm" | grep "lovelive_grade")
IFS=$'\n' html=($html)

printf "%s\n" ${html[*]} > html

./sifsheet.sh N
echo Finished Normals
./sifsheet.sh R
echo Finished Rares
./sifsheet.sh SR
echo Finished Super Rares
./sifsheet.sh UR
echo Finished Ultra Rares
convert temp/*.jpg -bordercolor white -border 20x20 -append llsif.jpg

find temp -name "*.jpg" ! -name $name -exec rm {} \;
rm -f html