#!/bin/bash

# Requires Linux, bash, curl, wget, and ImageMagick
# chmod +x sifsheet.sh
# then run it with ./sifsheet.sh N/R/SR/UR

# I is for idolized
# U is for unidolized
# For cards with only one unidolized/idolized state, use I
owned_cards_N=( # NORMALS
              # 0001I 0002I 0003I 0004I 0005I 0006I 0007I 0008I 0009I \
              # 0010I 0011I 0012I 0013I 0014I 0015I 0016I 0017I 0018I \
              # 0019I 0020I 0021I 0022I 0023I 0024I 0025I 0026I 0027I \
              # 0069I 0070I 0074I 0076I 0080I 0084I 0085I 0089I 0091I \
              # 0097I 0098I 0102I 0106I 0120I 0121I 0128I 0130I 0135I \
              # 0140I 0149I 0151I 0156I 0160I 0166I 0168I 0173I 0175I \
              # 0181I 0183I
              );
owned_cards_R=( # RARES
              # 0028I 0029I 0030I 0031I 0032I 0033I 0034I 0035I 0036I \
              # 0037I 0038I 0039I 0040I 0041I 0042I 0043I 0044I 0045I \
              # 0046I 0047I 0048I 0049I 0050I 0051I 0052I 0053I 0054I \
              # 0421I 0494I 0304I 0305I \
              # ALPACA & OTHERS
              # 0083I 0146I 0147I 0148I
              );
owned_cards_SR=( # SUPER RARES
               );
owned_cards_UR=( # ULTRA RARES
               );
               
if [ $# -ne 1 ]; then
	echo "Invalid number of arguments: ./sifsheet N/R/SR/UR"
	exit 1
fi
if [ $1 != "N" ] && [ $1 != "R" ] && [ $1 != "SR" ] && [ $1 != "UR" ]; then
	echo "Invalid argument: must be N/R/SR/UR"
	exit 1
fi

mkdir -p cache
mkdir -p temp

if [ ! -f "html" ]; then
    html=$(curl "web.ruliweb.daum.net/etcs/lovelive/lovelive_card.htm" | grep "lovelive_grade")
    IFS=$'\n' html=($html)
else
    IFS=$'\n' html=($(cat html))
fi

j=0
for i in ${html[*]}; do
	if [[ "$i" =~ '<span class="lovelive_grade">'$1'</span>' ]]; then
		num=$(echo $i | grep -oh 'id=[0-9]\+')
		card_num[$j]=$(printf "%04d" ${num:3})
		j=$((j+1))
	fi
done

# Download all the normie cards
url="http://img.ruliweb.com/family/lovelive/face/"
for i in ${card_num[*]}; do
    name1="face_$i.png"
    name2="face_$i"_"horo.png"
	url1=$url$name1
	url2=$url$name2
    if [ ! -f "cache/$name1" ]; then
        wget -q -P cache "$url1" &
    fi
    if [ ! -f "cache/$name2" ]; then
        wget -q -P cache "$url2"  &
    fi
done

# Get the no_img picture
no_img="face_no.png"
if [ ! -f "cache/$no_img" ]; then
    wget -q -P cache "$url$no_img"
fi

# Wait until bg downloads are done
wait

# copy no image for cards with only one state
for i in ${card_num[*]}; do
    if [ ! -f "cache/face_$i.png" ]; then
        cp "cache/$no_img" "cache/face_$i.png"
    fi
    if [ ! -f "cache/face_$i"_"horo.png" ]; then
        cp "cache/$no_img" "cache/face_$i"_"horo.png"
    fi
done

# copy files to our temp directory
for i in ${card_num[*]}; do
    cp "cache/face_$i.png" "temp/face_$i.png"
    cp "cache/face_$i"_"horo.png" "temp/face_$i"_"horo.png"
done

# gray out owned_cards
owned_list=owned_cards_$1[*]
for i in ${!owned_list}; do
	j=${i%?}
	if [ ${i:4} == U ]; then
		convert "temp/face_$j.png" -fill white -colorize 80% "temp/face_$j.png" 2> /dev/null
	elif [ ${i:4} == I ]; then
		convert "temp/face_$j.png" -fill white -colorize 80% "temp/face_$j.png" 2> /dev/null
		convert "temp/face_$j"_"horo.png" -fill white -colorize 80% "temp/face_$j"_"horo.png" 2> /dev/null
	fi
done

# Resize and gattai images
append=0
for i in ${card_num[*]}; do
	name1="temp/face_$i.png"
	name2="temp/face_$i"_"horo.png"
	convert $name1 $name2 -set colorspace RGB -bordercolor white -border 5x0 -gravity south -splice 0x30 +append \
	-pointsize 20 -draw "gravity south fill gray text 0,0 '$i'" -append "temp/$(printf '%03d' $append)".png
	append=$((append+1))
    rm $name1 $name2
done

montage -tile 9x -geometry +15+15 "temp/*.png" "temp/$1.jpg"
convert "temp/$1.jpg" -resize 60% "temp/$1.jpg"
rm temp/*.png

exit 0
