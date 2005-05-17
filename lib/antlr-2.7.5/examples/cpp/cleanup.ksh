#!/bin/ksh
for ex in *
do
if [ -d $ex ]
then
	echo "Cleaning up" $ex
	if [ -f $ex/shiplist ]
	then
		cp $ex/shiplist /tmp
		for q in `cat $ex/shiplist`
		do
			#echo "saving" $ex/$q
			mv $ex/$q /tmp
		done
		#echo "caterizing dir"
		rm $ex/*
		mv /tmp/shiplist $ex
		for i in `cat $ex/shiplist`
		do
			#echo "restoring" $i
			mv /tmp/$i $ex
		done
	else
		echo $ex "has no shiplist; ignoring..."
	fi
fi
done
